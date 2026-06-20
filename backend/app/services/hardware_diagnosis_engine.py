"""
Hardware Diagnosis Engine
----------------------------
Berbeda dari Windows/software issues yang BISA di-fix otomatis (action),
masalah hardware HANYA menghasilkan REKOMENDASI untuk user — karena
sistem tidak boleh/tidak bisa "memperbaiki" fisik hardware secara otomatis.

Engine ini membaca data dari seluruh hardware collector, membandingkan
dengan threshold di hardware_knowledge.json, dan menghasilkan daftar
temuan dengan severity + rekomendasi.
"""

import json
from pathlib import Path

KNOWLEDGE_FILE = (
    Path(__file__).parent.parent / "data" / "hardware_knowledge.json"
)

with open(KNOWLEDGE_FILE, "r", encoding="utf-8") as f:
    KNOWLEDGE = json.load(f)

# ── Threshold konfigurasi ──────────────────────────────────────────────────
THRESHOLDS = {
    "cpu_usage_percent": 85,
    "cpu_temp_c": 85,
    "ram_usage_percent": 90,
    "disk_wear_percent": 80,
    "disk_temp_c": 55,
    "disk_free_percent_low": 10,
    "gpu_temp_c": 85,
    "gpu_usage_idle_percent": 50,
}


def _finding(key, extra_detail=None, override_severity=None):
    info = KNOWLEDGE.get(key)
    if not info:
        return None

    finding = {
        "key": key,
        "title": info["title"],
        "category": info["category"],
        "severity": override_severity or info["severity"],
        "description": info["description"],
        "recommended_actions": info["recommended_actions"],
        "type": "hardware_advisory",  # marker: ini rekomendasi, bukan action
    }
    if extra_detail:
        finding["detail"] = extra_detail
    return finding


def diagnose_hardware(cpu_info, memory_info, disk_health, gpu_info, driver_health):
    findings = []

    # ── CPU ──────────────────────────────────────────────────────────────
    if cpu_info:
        usage = cpu_info.get("usage_percent") or 0
        temp = cpu_info.get("temperature_c")

        if usage >= THRESHOLDS["cpu_usage_percent"]:
            findings.append(_finding(
                "cpu_high_usage",
                f"CPU usage saat ini {usage}%"
            ))

        if temp is not None and temp >= THRESHOLDS["cpu_temp_c"]:
            findings.append(_finding(
                "cpu_high_temperature",
                f"Suhu CPU saat ini {temp}°C"
            ))

    # ── Memory ───────────────────────────────────────────────────────────
    if memory_info:
        mem_usage = memory_info.get("usage_percent") or 0
        if mem_usage >= THRESHOLDS["ram_usage_percent"]:
            findings.append(_finding(
                "ram_high_usage",
                f"RAM usage saat ini {mem_usage}%"
            ))

    # ── Disk ─────────────────────────────────────────────────────────────
    if disk_health:
        for disk in disk_health.get("disks", []):
            if not disk.get("is_healthy", True):
                findings.append(_finding(
                    "disk_unhealthy",
                    f"{disk.get('friendly_name')} status: {disk.get('health_status')}"
                ))

            wear = disk.get("wear_percent")
            if wear is not None and wear >= THRESHOLDS["disk_wear_percent"]:
                findings.append(_finding(
                    "disk_high_wear",
                    f"{disk.get('friendly_name')} wear level {wear}%"
                ))

            temp = disk.get("temperature_c")
            if temp is not None and temp >= THRESHOLDS["disk_temp_c"]:
                findings.append(_finding(
                    "disk_high_temperature",
                    f"{disk.get('friendly_name')} suhu {temp}°C"
                ))

        for vol in disk_health.get("volumes", []):
            if vol.get("free_percent", 100) < THRESHOLDS["disk_free_percent_low"]:
                findings.append(_finding(
                    "disk_low_space",
                    f"Drive {vol.get('drive')} sisa {vol.get('free_percent')}% "
                    f"({vol.get('free_gb')} GB)"
                ))

    # ── GPU ──────────────────────────────────────────────────────────────
    if gpu_info:
        for gpu in gpu_info.get("gpus", []):
            temp = gpu.get("temperature_c")
            usage = gpu.get("utilization_percent")

            if temp is not None and temp >= THRESHOLDS["gpu_temp_c"]:
                findings.append(_finding(
                    "gpu_high_temperature",
                    f"{gpu.get('name')} suhu {temp}°C"
                ))

            if usage is not None and usage >= THRESHOLDS["gpu_usage_idle_percent"]:
                findings.append(_finding(
                    "gpu_high_usage",
                    f"{gpu.get('name')} usage {usage}%"
                ))

    # ── Driver / Device problems ─────────────────────────────────────────
    if driver_health and driver_health.get("success"):
        for prob in driver_health.get("problem_devices", []):
            code = prob.get("error_code")
            key = "hardware_failure_suspected" if code == 43 else "driver_problem"
            findings.append(_finding(
                key,
                f"{prob.get('name')} — {prob.get('error_meaning')} "
                f"(code {code})"
            ))

    findings = [f for f in findings if f is not None]

    severity_rank = {"high": 0, "medium": 1, "low": 2}
    findings.sort(key=lambda f: severity_rank.get(f["severity"], 3))

    high_count = sum(1 for f in findings if f["severity"] == "high")
    if high_count > 0:
        overall = "Poor"
    elif findings:
        overall = "Warning"
    else:
        overall = "Good"

    return {
        "overall_hardware_health": overall,
        "total_findings": len(findings),
        "findings": findings,
    }
