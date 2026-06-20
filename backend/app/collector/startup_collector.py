"""
Startup Impact Collector
--------------------------
Mengambil daftar program yang berjalan saat startup beserta
estimasi dampaknya, menggunakan Win32_StartupCommand (registry/folder
startup) dan data StartupApps dari registry performa Windows jika tersedia.
"""

import subprocess
import json


def get_startup_programs():
    items = _get_startup_commands()
    if items is None:
        return {
            "success": False,
            "error": "Failed to query startup programs",
            "programs": [],
        }

    programs = []
    for item in items:
        programs.append({
            "name": item.get("Name", "Unknown"),
            "command": item.get("Command", ""),
            "location": item.get("Location", ""),
            "user": item.get("User", ""),
            # Estimasi impact sederhana berdasarkan lokasi & nama proses umum
            "estimated_impact": _estimate_impact(item.get("Name", "")),
        })

    # Urutkan: impact tinggi dulu
    impact_rank = {"High": 0, "Medium": 1, "Low": 2}
    programs.sort(key=lambda p: impact_rank.get(p["estimated_impact"], 3))

    return {
        "success": True,
        "total": len(programs),
        "programs": programs,
    }


_HIGH_IMPACT_HINTS = [
    "adobe", "creative cloud", "onedrive", "dropbox", "steam",
    "epic", "discord", "skype", "spotify", "teams", "zoom",
]

_LOW_IMPACT_HINTS = [
    "realtek", "audio", "touchpad", "synaptics", "security health",
]


def _estimate_impact(name: str) -> str:
    n = (name or "").lower()
    if any(h in n for h in _HIGH_IMPACT_HINTS):
        return "High"
    if any(h in n for h in _LOW_IMPACT_HINTS):
        return "Low"
    return "Medium"


def _get_startup_commands():
    command = (
        "Get-CimInstance Win32_StartupCommand | "
        "Select-Object Name, Command, Location, User | "
        "ConvertTo-Json -Depth 3"
    )
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
        return data if isinstance(data, list) else [data]
    except Exception:
        return None
