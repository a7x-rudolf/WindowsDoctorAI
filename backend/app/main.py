from fastapi import FastAPI
from fastapi.middleware.cors import CORSMiddleware
from fastapi.responses import StreamingResponse
from pydantic import BaseModel

# ── Windows / software collectors & services (existing, refined) ───────────
from app.collector.event_collector import get_system_errors
from app.collector.service_collector import get_services
from app.collector.disk_collector import get_disk_info

from app.services.event_grouper import group_events
from app.services.severity_engine import enrich_events, calculate_health_score
from app.services.diagnosis_engine import generate_diagnosis
from app.services.analysis_engine import analyze_system
from app.services.action_runner import run_action_stream

# ── Hardware collectors (new) ───────────────────────────────────────────────
from app.collector.cpu_memory_collector import get_cpu_info, get_memory_info
from app.collector.gpu_collector import get_gpu_info
from app.collector.disk_health_collector import get_disk_health
from app.collector.driver_collector import get_driver_health
from app.collector.startup_collector import get_startup_programs
from app.collector.process_collector import get_top_processes
from app.collector.system_collector import get_system_info

from app.services.hardware_diagnosis_engine import diagnose_hardware

# ── Ai Groq) ───────────────────────────────────────────────
from app.services.ai_engine import analyze_with_ai

# ── History database ─────────────────────────────────────────────────────────
from app.db import history_db

app = FastAPI(
    title="Windows Doctor AI",
    version="2.0.0",
    description=(
        "Diagnostic & repair assistant for Windows. "
        "Software/Windows issues -> automated actions. "
        "Hardware issues -> advisory recommendations only."
    ),
)

app.add_middleware(
    CORSMiddleware,
    # "*" aman dipakai di sini karena backend hanya listen di 127.0.0.1
    # (tidak exposed ke network) dan tidak memakai cookie/credential auth.
    # Frontend production di-load via file:// (origin "null") oleh Electron,
    # sedangkan dev server pakai http://localhost:5173 — keduanya perlu lolos.
    allow_origins=["*"],
    allow_methods=["*"],
    allow_headers=["*"],
)

history_db.init_db()


class ActionRequest(BaseModel):
    action: str
    service_name: str


# ── Root ──────────────────────────────────────────────────────────────────────

@app.get("/")
def root():
    return {
        "status": "ok",
        "message": "Windows Doctor AI Backend Running",
        "version": "2.0.0",
    }


# ── Windows / Software diagnostics (existing, action-capable) ──────────────

@app.get("/scan")
def scan_system():
    return get_system_errors()


@app.get("/scan/grouped")
def scan_grouped():
    scan_result = get_system_errors()
    if not scan_result["success"]:
        return scan_result

    grouped = group_events(scan_result["events"])
    return {
        "success": True,
        "total_groups": len(grouped),
        "groups": grouped,
    }


@app.get("/health")
def health_check():
    scan_result = get_system_errors()
    if not scan_result["success"]:
        return scan_result

    grouped = group_events(scan_result["events"])
    enriched, penalty = enrich_events(grouped)
    score = calculate_health_score(penalty)

    return {
        "health_score": score,
        "total_penalty": penalty,
        "issues": enriched,
    }


@app.get("/diagnosis")
def diagnosis():
    scan_result = get_system_errors()
    if not scan_result["success"]:
        return scan_result

    grouped = group_events(scan_result["events"])
    diagnoses = generate_diagnosis(grouped)

    return {
        "total_diagnoses": len(diagnoses),
        "diagnoses": diagnoses,
    }


@app.get("/services")
def services():
    return get_services()


@app.get("/analysis")
def analysis():
    scan_result = get_system_errors()
    if not scan_result["success"]:
        return scan_result

    grouped = group_events(scan_result["events"])
    return analyze_system(grouped)


@app.get("/disk")
def disk():
    return get_disk_info()


@app.post("/action/run")
def action_run(req: ActionRequest):
    return StreamingResponse(
        run_action_stream(req.action, req.service_name),
        media_type="text/event-stream",
        headers={
            "Cache-Control": "no-cache",
            "X-Accel-Buffering": "no",
        },
    )


# ── Hardware diagnostics (new, advisory only — no auto-fix) ────────────────

@app.get("/hardware/cpu")
def hardware_cpu():
    return get_cpu_info()


@app.get("/hardware/memory")
def hardware_memory():
    return get_memory_info()


@app.get("/hardware/gpu")
def hardware_gpu():
    return get_gpu_info()


@app.get("/hardware/disk")
def hardware_disk():
    return get_disk_health()


@app.get("/hardware/drivers")
def hardware_drivers():
    return get_driver_health()


@app.get("/hardware/startup")
def hardware_startup():
    return get_startup_programs()


@app.get("/hardware/processes")
def hardware_processes(sort_by: str = "cpu", limit: int = 10):
    return get_top_processes(limit=limit, sort_by=sort_by)


@app.get("/hardware/diagnosis")
def hardware_diagnosis():
    """
    Menjalankan semua hardware collector lalu memberikan daftar
    temuan + REKOMENDASI (bukan tombol Run/Action otomatis).
    """
    cpu_info = get_cpu_info()
    memory_info = get_memory_info()
    disk_health = get_disk_health()
    gpu_info = get_gpu_info()
    driver_health = get_driver_health()

    result = diagnose_hardware(
        cpu_info=cpu_info,
        memory_info=memory_info,
        disk_health=disk_health,
        gpu_info=gpu_info,
        driver_health=driver_health,
    )

    result["raw"] = {
        "cpu": cpu_info,
        "memory": memory_info,
        "disk": disk_health,
        "gpu": gpu_info,
        "drivers": driver_health,
    }

    return result


@app.get("/system/info")
def system_info():
    return get_system_info()


# ── Combined overview (single call for dashboard) ───────────────────────────

@app.get("/overview")
def overview():
    """
    Endpoint gabungan untuk halaman utama dashboard:
    - Windows health score & critical issues (software/event-based)
    - Hardware health & advisory findings
    - System info ringkas
    Hasil scan juga otomatis disimpan ke history database.
    """
    scan_result = get_system_errors()

    windows_part = {"health_score": None, "overall_health": "Unknown", "critical_issues": []}
    if scan_result["success"]:
        grouped = group_events(scan_result["events"])
        enriched, penalty = enrich_events(grouped)
        score = calculate_health_score(penalty)
        windows_analysis = analyze_system(grouped)

        windows_part = {
            "health_score": score,
            "overall_health": windows_analysis["overall_health"],
            "critical_issues": windows_analysis["critical_issues"],
            "diagnoses_count": len(windows_analysis["diagnoses"]),
            "disk": windows_analysis["disk"],
        }

    cpu_info = get_cpu_info()
    memory_info = get_memory_info()
    disk_health = get_disk_health()
    gpu_info = get_gpu_info()
    driver_health = get_driver_health()

    hardware_diag = diagnose_hardware(
        cpu_info=cpu_info,
        memory_info=memory_info,
        disk_health=disk_health,
        gpu_info=gpu_info,
        driver_health=driver_health,
    )

    sys_info = get_system_info()

    # Simpan ke history (best-effort, jangan sampai gagalkan response)
    try:
        history_db.save_scan_result(
            windows_score=windows_part.get("health_score"),
            overall_health=windows_part.get("overall_health"),
            hardware_health=hardware_diag.get("overall_hardware_health"),
            critical_issues_count=len(windows_part.get("critical_issues", [])),
            hardware_findings_count=hardware_diag.get("total_findings", 0),
            summary={
                "windows": windows_part,
                "hardware_overall": hardware_diag.get("overall_hardware_health"),
            },
        )
    except Exception:
        pass

    return {
        "system": sys_info,
        "windows": windows_part,
        "hardware": {
            "overall_hardware_health": hardware_diag["overall_hardware_health"],
            "total_findings": hardware_diag["total_findings"],
            "findings": hardware_diag["findings"],
            "cpu": cpu_info,
            "memory": memory_info,
            "gpu": gpu_info,
            "disk": disk_health,
        },
    }

# ── AI Analysis ───────────────────────────────────────────────────────────────

@app.get("/ai/analyze")
def ai_analyze():
    """
    Endpoint AI Analysis — kumpulkan semua data sistem lalu
    kirim ke Groq AI untuk diagnosis mendalam.
    """
    # Kumpulkan semua data
    scan_result = get_system_errors()
    windows_part = {"health_score": None, "overall_health": "Unknown", "critical_issues": []}

    if scan_result["success"]:
        grouped = group_events(scan_result["events"])
        enriched, penalty = enrich_events(grouped)
        score = calculate_health_score(penalty)
        windows_analysis = analyze_system(grouped)
        windows_part = {
            "health_score": score,
            "overall_health": windows_analysis["overall_health"],
            "critical_issues": windows_analysis["critical_issues"],
        }

    cpu_info = get_cpu_info()
    memory_info = get_memory_info()
    disk_health = get_disk_health()
    gpu_info = get_gpu_info()
    driver_health = get_driver_health()

    hardware_diag = diagnose_hardware(
        cpu_info=cpu_info,
        memory_info=memory_info,
        disk_health=disk_health,
        gpu_info=gpu_info,
        driver_health=driver_health,
    )

    # Kirim ke AI
    scan_data = {
        "windows": windows_part,
        "hardware": {
            "cpu": cpu_info,
            "memory": memory_info,
            "disk": disk_health,
            "findings": hardware_diag.get("findings", []),
        }
    }

    ai_result = analyze_with_ai(scan_data)
    return ai_result

# ── History ───────────────────────────────────────────────────────────────────

@app.get("/history/scans")
def history_scans(limit: int = 30):
    return {"history": history_db.get_scan_history(limit=limit)}


@app.get("/history/actions")
def history_actions(limit: int = 50):
    return {"history": history_db.get_action_history(limit=limit)}