"""
CPU & Memory Collector
-----------------------
Mengumpulkan metrik real-time CPU dan RAM menggunakan psutil,
serta info tambahan (model CPU, core count, suhu jika tersedia)
melalui WMI/PowerShell sebagai fallback di Windows.
"""

import subprocess
import json
import psutil


def get_cpu_info():
    """Info statis + real-time CPU."""
    try:
        freq = psutil.cpu_freq()
    except Exception:
        freq = None

    per_core = psutil.cpu_percent(interval=0.3, percpu=True)
    overall = round(sum(per_core) / len(per_core), 1) if per_core else 0.0

    cpu_name = _get_cpu_name()
    temp = _get_cpu_temperature()

    return {
        "name": cpu_name,
        "usage_percent": overall,
        "per_core_percent": per_core,
        "physical_cores": psutil.cpu_count(logical=False),
        "logical_cores": psutil.cpu_count(logical=True),
        "current_freq_mhz": round(freq.current, 0) if freq else None,
        "max_freq_mhz": round(freq.max, 0) if freq and freq.max else None,
        "temperature_c": temp,
    }


def get_memory_info():
    """Info statis + real-time RAM."""
    vm = psutil.virtual_memory()
    swap = psutil.swap_memory()

    return {
        "total_gb": round(vm.total / (1024 ** 3), 2),
        "used_gb": round(vm.used / (1024 ** 3), 2),
        "available_gb": round(vm.available / (1024 ** 3), 2),
        "usage_percent": vm.percent,
        "swap_total_gb": round(swap.total / (1024 ** 3), 2),
        "swap_used_gb": round(swap.used / (1024 ** 3), 2),
        "swap_percent": swap.percent,
    }


def _run_ps(command: str):
    try:
        result = subprocess.run(
            ["powershell", "-NoProfile", "-Command", command],
            capture_output=True,
            text=True,
            timeout=10,
        )
        if result.returncode != 0 or not result.stdout.strip():
            return None
        return json.loads(result.stdout)
    except Exception:
        return None


def _get_cpu_name():
    data = _run_ps(
        "Get-CimInstance Win32_Processor | Select-Object Name | ConvertTo-Json"
    )
    if not data:
        return "Unknown CPU"
    if isinstance(data, list):
        data = data[0]
    return data.get("Name", "Unknown CPU").strip()


def _get_cpu_temperature():
    """
    Suhu CPU tidak tersedia langsung lewat psutil di Windows.
    Coba ambil dari MSAcpi_ThermalZoneTemperature (tidak selalu didukung
    di semua motherboard/driver). Mengembalikan None jika gagal —
    frontend harus menangani nilai None secara graceful.
    """
    data = _run_ps(
        "Get-CimInstance -Namespace 'root/wmi' -ClassName MSAcpi_ThermalZoneTemperature "
        "-ErrorAction SilentlyContinue | Select-Object CurrentTemperature | ConvertTo-Json"
    )
    if not data:
        return None
    try:
        if isinstance(data, list):
            data = data[0]
        raw = data.get("CurrentTemperature")
        if raw is None:
            return None
        # Nilai dalam satuan 1/10 Kelvin
        celsius = (raw / 10.0) - 273.15
        return round(celsius, 1)
    except Exception:
        return None
