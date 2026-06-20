"""
System Info Collector
-----------------------
Info umum sistem: OS version, uptime, motherboard, BIOS.
"""

import subprocess
import json
import time
import psutil


def get_system_info():
    os_info = _get_os_info() or {}
    board_info = _get_board_info() or {}
    boot_time = psutil.boot_time()
    uptime_seconds = int(time.time() - boot_time)

    return {
        "os_name": os_info.get("WindowsProductName", "Windows"),
        "os_version": os_info.get("WindowsVersion"),
        "os_build": os_info.get("OsHardwareAbstractionLayer"),
        "computer_name": os_info.get("CsName"),
        "manufacturer": board_info.get("Manufacturer"),
        "model": board_info.get("Model"),
        "bios_version": board_info.get("BiosVersion"),
        "uptime_seconds": uptime_seconds,
        "uptime_readable": _format_uptime(uptime_seconds),
        "boot_time": boot_time,
    }


def _run_ps(command: str):
    try:
        result = subprocess.run(
            ["powershell", "-NoProfile", "-Command", command],
            capture_output=True,
            text=True,
            timeout=15,
        )
        if result.returncode != 0 or not result.stdout.strip():
            return None
        data = json.loads(result.stdout)
        return data[0] if isinstance(data, list) else data
    except Exception:
        return None


def _get_os_info():
    return _run_ps(
        "Get-ComputerInfo | Select-Object WindowsProductName, WindowsVersion, "
        "OsHardwareAbstractionLayer, CsName | ConvertTo-Json -Depth 3"
    )


def _get_board_info():
    return _run_ps(
        "$b = Get-CimInstance Win32_BaseBoard; "
        "$bios = Get-CimInstance Win32_BIOS; "
        "[PSCustomObject]@{ Manufacturer=$b.Manufacturer; Model=$b.Product; "
        "BiosVersion=$bios.SMBIOSBIOSVersion } | ConvertTo-Json -Depth 3"
    )


def _format_uptime(seconds: int) -> str:
    days, rem = divmod(seconds, 86400)
    hours, rem = divmod(rem, 3600)
    minutes, _ = divmod(rem, 60)

    parts = []
    if days:
        parts.append(f"{days}d")
    if hours:
        parts.append(f"{hours}h")
    parts.append(f"{minutes}m")

    return " ".join(parts)
