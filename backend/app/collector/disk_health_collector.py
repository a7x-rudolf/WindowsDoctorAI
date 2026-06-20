"""
Disk SMART / Reliability Collector
-----------------------------------
Menggunakan cmdlet bawaan Windows Storage Module:
- Get-PhysicalDisk        -> HealthStatus, MediaType (SSD/HDD), Size
- Get-StorageReliabilityCounter -> Temperature, Wear, ReadErrors, Read/WriteLatency

Tidak butuh instalasi smartctl tambahan karena modul ini sudah
built-in di Windows 10/11.
"""

import subprocess
import json

import psutil


def get_disk_health():
    physical = _get_physical_disks()
    reliability = _get_reliability_counters()
    volumes = _get_volume_usage()

    # Gabungkan berdasarkan DeviceId jika tersedia
    reliability_map = {r.get("DeviceId"): r for r in reliability} if reliability else {}

    disks = []
    for disk in physical or []:
        device_id = disk.get("DeviceId")
        rel = reliability_map.get(device_id, {})

        health_status = disk.get("HealthStatus", "Unknown")
        wear_percent = rel.get("Wear")

        disks.append({
            "device_id": device_id,
            "friendly_name": disk.get("FriendlyName", "Unknown Disk"),
            "media_type": disk.get("MediaType", "Unknown"),
            "size_gb": (
                round(disk["Size"] / (1024 ** 3), 1)
                if isinstance(disk.get("Size"), (int, float)) else None
            ),
            "health_status": health_status,
            "operational_status": disk.get("OperationalStatus"),
            "temperature_c": rel.get("Temperature"),
            "wear_percent": wear_percent,
            "read_errors_total": rel.get("ReadErrorsTotal", 0),
            "write_errors_total": rel.get("WriteErrorsTotal", 0),
            "is_healthy": health_status == "Healthy" and (
                wear_percent is None or wear_percent < 90
            ),
        })

    return {
        "disks": disks,
        "volumes": volumes,
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
        return data if isinstance(data, list) else [data]
    except Exception:
        return None


def _get_physical_disks():
    return _run_ps(
        "Get-PhysicalDisk | Select-Object DeviceId, FriendlyName, MediaType, "
        "Size, HealthStatus, OperationalStatus | ConvertTo-Json -Depth 3"
    )


def _get_reliability_counters():
    return _run_ps(
        "Get-PhysicalDisk | Get-StorageReliabilityCounter | "
        "Select-Object DeviceId, Temperature, Wear, ReadErrorsTotal, "
        "WriteErrorsTotal | ConvertTo-Json -Depth 3"
    )


def _get_volume_usage():
    """Usage per-drive (semua drive, bukan cuma C:) via psutil."""
    volumes = []
    for part in psutil.disk_partitions(all=False):
        if "cdrom" in part.opts or part.fstype == "":
            continue
        try:
            usage = psutil.disk_usage(part.mountpoint)
        except (PermissionError, OSError):
            continue

        volumes.append({
            "drive": part.device,
            "mountpoint": part.mountpoint,
            "filesystem": part.fstype,
            "total_gb": round(usage.total / (1024 ** 3), 2),
            "used_gb": round(usage.used / (1024 ** 3), 2),
            "free_gb": round(usage.free / (1024 ** 3), 2),
            "free_percent": round((usage.free / usage.total) * 100, 2) if usage.total else 0,
        })
    return volumes
