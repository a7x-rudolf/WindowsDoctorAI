import subprocess
import threading

from app.db import history_db


# ── Service name → Windows internal service name ─────────────────────────────

SERVICE_NAME_MAP = {
    # Original 4
    "Windows Search": "WSearch",
    "Virtual Disk": "vds",
    "Remote Procedure Call (RPC) Locator": "RpcLocator",
    "Microsoft Office Click-to-Run Service": "ClickToRunSvc",
    # System Update
    "Windows Update": "wuauserv",
    "Background Intelligent Transfer Service": "BITS",
    # Security
    "Cryptographic Services": "CryptSvc",
    "Windows Defender Antivirus Service": "WinDefend",
    "Windows Firewall": "MpsSvc",
    "Security Center": "wscsvc",
    # Network
    "DHCP Client": "Dhcp",
    "DNS Client": "Dnscache",
    "Network Location Awareness": "NlaSvc",
    "Server": "LanmanServer",
    "Workstation": "LanmanWorkstation",
    "Network Store Interface Service": "nsi",
    # Core Services
    "Windows Event Log": "EventLog",
    "Task Scheduler": "Schedule",
    "Windows Time": "W32Time",
    "Remote Procedure Call (RPC)": "RpcSs",
    "DCOM Server Process Launcher": "DcomLaunch",
    "COM+ Event System": "EventSystem",
    "Plug and Play": "PlugPlay",
    "Windows Management Instrumentation": "Winmgmt",
    # Performance
    "Superfetch": "SysMain",
    "SysMain": "SysMain",
    "Application Experience": "AeLookupSvc",
    # Audio
    "Windows Audio": "Audiosrv",
    "Windows Audio Endpoint Builder": "AudioEndpointBuilder",
    # Print
    "Print Spooler": "Spooler",
    # Installation & User
    "Windows Installer": "msiserver",
    "User Profile Service": "ProfSvc",
    "Group Policy Client": "gpsvc",
    "Shell Hardware Detection": "ShellHWDetection",
    "Windows Push Notifications System Service": "WpnService",
}


# ── Action → command builder ──────────────────────────────────────────────────

def resolve_command(action: str, service_name: str) -> list[str] | None:

    a = action.lower().strip()

    # Restart service
    if "restart" in a and "service" in a:
        svc = SERVICE_NAME_MAP.get(service_name)
        if not svc:
            return None
        # We return a special marker; runner handles stop+start sequence
        return ["__restart__", svc]

    # SFC
    if "sfc" in a:
        return ["sfc", "/scannow"]

    # DISM
    if "dism" in a:
        return [
            "DISM",
            "/Online",
            "/Cleanup-Image",
            "/RestoreHealth"
        ]

    # CHKDSK
    if "chkdsk" in a:
        return ["chkdsk", "C:", "/f", "/scan"]

    # Rebuild Search Index
    if "rebuild search index" in a:
        return [
            "powershell", "-Command",
            (
                "Stop-Service WSearch -Force; "
                "Remove-Item -Path 'C:\\ProgramData\\Microsoft\\Search\\Data\\Applications\\Windows\\Windows.edb' "
                "-Force -ErrorAction SilentlyContinue; "
                "Start-Service WSearch; "
                "Write-Output 'Search Index rebuild initiated.'"
            )
        ]

    # Verify RPC Service
    if "verify rpc" in a:
        return ["sc", "query", "RpcSs"]

    # Check Disk Management
    if "check disk management" in a:
        return ["powershell", "-Command",
                "Get-Disk | Format-List Number, FriendlyName, HealthStatus, OperationalStatus, Size"]

    # Repair Microsoft Office
    if "repair" in a and "office" in a:
        return [
            "powershell", "-Command",
            "Get-WmiObject -Class Win32_Product | Where-Object { $_.Name -like '*Office*' } | "
            "ForEach-Object { Write-Output \"Found: $($_.Name)\" }"
        ]

    return None


# ── Stream runner ─────────────────────────────────────────────────────────────

def run_action_stream(action: str, service_name: str):
    """
    Generator: yields SSE-formatted strings.
    Caller should set Content-Type: text/event-stream.
    Setiap eksekusi otomatis dicatat ke history database (best-effort).
    """

    def sse(text: str, event: str = "log") -> str:
        # Escape newlines inside the data value
        safe = text.replace("\n", "↵").replace("\r", "")
        return f"event: {event}\ndata: {safe}\n\n"

    collected_lines = []
    status = "success"

    def log_history():
        try:
            history_db.save_action_result(
                action=action,
                service_name=service_name,
                status=status,
                output_summary="\n".join(collected_lines),
            )
        except Exception:
            pass

    cmd = resolve_command(action, service_name)

    if cmd is None:
        msg = f"[ERROR] Action not recognized: '{action}'"
        collected_lines.append(msg)
        status = "failed"
        yield sse(msg, "error")
        yield sse("", "done")
        log_history()
        return

    # ── Special: restart (stop then start) ───────────────────────────────────
    if cmd[0] == "__restart__":
        svc = cmd[1]
        line = f"[INFO] Stopping service: {svc}"
        collected_lines.append(line)
        yield sse(line)

        for out in _run_proc(["sc", "stop", svc]):
            collected_lines.append(out)
            if out.startswith("[ERROR]"):
                status = "failed"
            yield sse(out)

        line = f"[INFO] Starting service: {svc}"
        collected_lines.append(line)
        yield sse(line)

        for out in _run_proc(["sc", "start", svc]):
            collected_lines.append(out)
            if out.startswith("[ERROR]"):
                status = "failed"
            yield sse(out)

        line = "[DONE] Restart sequence complete."
        collected_lines.append(line)
        yield sse(line, "done")
        log_history()
        return

    # ── Normal command ────────────────────────────────────────────────────────
    cmd_str = " ".join(cmd)
    line = f"[RUN] {cmd_str}"
    collected_lines.append(line)
    yield sse(line)

    for out in _run_proc(cmd):
        collected_lines.append(out)
        if out.startswith("[ERROR]"):
            status = "failed"
        yield sse(out)

    line = "[DONE] Command finished."
    collected_lines.append(line)
    yield sse(line, "done")
    log_history()


def _run_proc(cmd: list[str]):
    """Run a subprocess and yield output lines as they arrive."""
    try:
        proc = subprocess.Popen(
            cmd,
            stdout=subprocess.PIPE,
            stderr=subprocess.STDOUT,
            text=True,
            encoding="utf-8",
            errors="replace",
            shell=False,
        )

        for line in proc.stdout:
            stripped = line.rstrip()
            if stripped:
                yield stripped

        proc.wait()
        yield f"[EXIT] Return code: {proc.returncode}"

    except FileNotFoundError:
        yield f"[ERROR] Command not found: {cmd[0]}"
    except PermissionError:
        yield "[ERROR] Permission denied. Make sure backend runs as Administrator."
    except Exception as e:
        yield f"[ERROR] Unexpected error: {str(e)}"