"""
Process Collector
-------------------
Daftar proses dengan penggunaan CPU & RAM tertinggi, menggunakan psutil
(lebih cepat & stabil dibanding PowerShell Get-Process untuk data ini).
"""

import time
import psutil


def get_top_processes(limit=10, sort_by="cpu"):
    procs = []

    # Warm-up: psutil butuh 2 sample untuk cpu_percent yang akurat
    for p in psutil.process_iter():
        try:
            p.cpu_percent(None)
        except (psutil.NoSuchProcess, psutil.AccessDenied):
            continue
    time.sleep(0.3)

    for p in psutil.process_iter(
        ["pid", "name", "cpu_percent", "memory_percent", "memory_info"]
    ):
        try:
            info = p.info
            procs.append({
                "pid": info["pid"],
                "name": info["name"],
                "cpu_percent": round(info["cpu_percent"] or 0, 1),
                "memory_percent": round(info["memory_percent"] or 0, 1),
                "memory_mb": round(
                    (info["memory_info"].rss / (1024 ** 2))
                    if info["memory_info"] else 0, 1
                ),
            })
        except (psutil.NoSuchProcess, psutil.AccessDenied):
            continue

    key = "cpu_percent" if sort_by == "cpu" else "memory_percent"
    procs.sort(key=lambda x: x[key], reverse=True)

    return {
        "sort_by": sort_by,
        "total_processes": len(procs),
        "top_processes": procs[:limit],
    }
