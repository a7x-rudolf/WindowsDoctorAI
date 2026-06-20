"""
Driver Health Collector
------------------------
Mendeteksi driver yang bermasalah menggunakan PnP device status
(Win32_PnPEntity / Get-PnpDevice). ConfigManagerErrorCode != 0
menandakan device dengan masalah (driver missing, conflict, disabled, dll).
"""

import subprocess
import json

# Referensi singkat kode error PnP Windows yang umum
PNP_ERROR_MEANINGS = {
    1: "Device is not configured correctly",
    3: "Driver may be corrupted, or system is low on memory",
    10: "Device cannot start",
    12: "Device cannot find enough free resources",
    14: "Restart computer required for device to work",
    18: "Drivers need to be reinstalled",
    19: "Registry settings for this device are corrupted",
    21: "Device is being removed",
    22: "Device has been disabled",
    24: "Device is not present, not working properly, or missing drivers",
    28: "Drivers for this device are not installed",
    31: "Device is not working properly (driver issue)",
    32: "Driver for this device has been disabled",
    37: "Driver returned a failure",
    39: "Driver is missing or corrupted",
    40: "Driver could not be accessed (registry issue)",
    41: "Driver loaded but device cannot be found",
    43: "Device has reported problems (hardware failure likely)",
    44: "Application or service has shut down this device",
    45: "Device is not connected",
    48: "Driver software is blocked from loading",
}


def get_driver_health():
    devices = _get_pnp_devices()
    if devices is None:
        return {
            "success": False,
            "error": "Failed to query PnP devices",
            "problem_devices": [],
        }

    problems = []
    for dev in devices:
        code = dev.get("ConfigManagerErrorCode", 0)
        status = dev.get("Status", "")

        if code and code != 0:
            problems.append({
                "name": dev.get("Name", "Unknown Device"),
                "device_class": dev.get("PNPClass") or dev.get("Class"),
                "manufacturer": dev.get("Manufacturer"),
                "error_code": code,
                "error_meaning": PNP_ERROR_MEANINGS.get(
                    code, "Unknown driver/device problem"
                ),
                "status": status,
            })

    return {
        "success": True,
        "total_devices_scanned": len(devices),
        "total_problems": len(problems),
        "problem_devices": problems,
    }


def _get_pnp_devices():
    command = (
        "Get-CimInstance Win32_PnPEntity | "
        "Where-Object { $_.ConfigManagerErrorCode -ne $null } | "
        "Select-Object Name, Manufacturer, PNPClass, Status, "
        "ConfigManagerErrorCode | ConvertTo-Json -Depth 3"
    )
    try:
        result = subprocess.run(
            ["powershell", "-NoProfile", "-Command", command],
            capture_output=True,
            text=True,
            timeout=20,
        )
        if result.returncode != 0 or not result.stdout.strip():
            return None

        data = json.loads(result.stdout)
        return data if isinstance(data, list) else [data]
    except Exception:
        return None
