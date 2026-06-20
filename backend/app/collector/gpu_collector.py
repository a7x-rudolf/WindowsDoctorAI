"""
GPU Collector
-------------
Mencoba beberapa metode untuk mendapatkan info GPU:
1. nvidia-smi (jika GPU NVIDIA tersedia) -> utilization, memory, temp real-time.
2. Fallback ke Win32_VideoController (WMI) untuk nama adapter & VRAM,
   tanpa metrik real-time (AMD/Intel non-NVIDIA umumnya tidak expose
   utilization tanpa vendor tool resmi).
"""

import subprocess
import json
import shutil


def get_gpu_info():
    nvidia = _get_nvidia_gpu()
    if nvidia:
        return {
            "source": "nvidia-smi",
            "gpus": nvidia,
        }

    fallback = _get_wmi_gpu()
    return {
        "source": "wmi" if fallback else "unavailable",
        "gpus": fallback or [],
    }


def _get_nvidia_gpu():
    if not shutil.which("nvidia-smi"):
        return None

    query = (
        "name,utilization.gpu,memory.total,memory.used,"
        "temperature.gpu,fan.speed"
    )
    try:
        result = subprocess.run(
            [
                "nvidia-smi",
                f"--query-gpu={query}",
                "--format=csv,noheader,nounits",
            ],
            capture_output=True,
            text=True,
            timeout=8,
        )
        if result.returncode != 0:
            return None

        gpus = []
        for line in result.stdout.strip().splitlines():
            parts = [p.strip() for p in line.split(",")]
            if len(parts) < 6:
                continue
            name, util, mem_total, mem_used, temp, fan = parts
            gpus.append({
                "name": name,
                "utilization_percent": _to_num(util),
                "memory_total_mb": _to_num(mem_total),
                "memory_used_mb": _to_num(mem_used),
                "memory_percent": (
                    round(_to_num(mem_used) / _to_num(mem_total) * 100, 1)
                    if _to_num(mem_total) else None
                ),
                "temperature_c": _to_num(temp),
                "fan_speed_percent": _to_num(fan),
            })
        return gpus if gpus else None
    except Exception:
        return None


def _get_wmi_gpu():
    command = (
        "Get-CimInstance Win32_VideoController | "
        "Select-Object Name, AdapterRAM, DriverVersion, "
        "VideoProcessor, Status | ConvertTo-Json -Depth 3"
    )
    try:
        result = subprocess.run(
            ["powershell", "-NoProfile", "-Command", command],
            capture_output=True,
            text=True,
            timeout=10,
        )
        if result.returncode != 0 or not result.stdout.strip():
            return None

        data = json.loads(result.stdout)
        if isinstance(data, dict):
            data = [data]

        gpus = []
        for item in data:
            ram = item.get("AdapterRAM")
            gpus.append({
                "name": item.get("Name", "Unknown GPU"),
                "driver_version": item.get("DriverVersion"),
                "video_processor": item.get("VideoProcessor"),
                "status": item.get("Status"),
                "memory_total_mb": (
                    round(ram / (1024 ** 2), 0) if isinstance(ram, (int, float)) and ram > 0 else None
                ),
                "utilization_percent": None,
                "temperature_c": None,
            })
        return gpus if gpus else None
    except Exception:
        return None


def _to_num(value):
    try:
        return float(value)
    except (ValueError, TypeError):
        return None
