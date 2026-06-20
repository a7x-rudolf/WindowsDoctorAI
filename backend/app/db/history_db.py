"""
Scan History Database
------------------------
SQLite ringan untuk menyimpan riwayat hasil scan (Windows health score +
hardware health) supaya user bisa melihat tren dari waktu ke waktu.
"""

import sqlite3
import json
import sys
import os
from pathlib import Path
from datetime import datetime
from contextlib import contextmanager


def _resolve_db_path() -> Path:
    """
    Tentukan lokasi database yang aman untuk ditulis di semua kondisi:
    - Saat development (python langsung): backend/database/history.db
    - Saat dibundle PyInstaller (.exe): %LOCALAPPDATA%\\WindowsDoctorAI\\history.db
      (folder instalasi/_MEIPASS bisa read-only atau terhapus, jadi JANGAN
      simpan data di situ — selalu pakai folder data user yang writable)
    """
    if getattr(sys, "frozen", False):
        # Berjalan sebagai .exe hasil PyInstaller
        base = os.environ.get("LOCALAPPDATA") or os.path.expanduser("~")
        db_dir = Path(base) / "WindowsDoctorAI"
    else:
        # Berjalan sebagai script Python biasa (development)
        db_dir = Path(__file__).parent.parent.parent / "database"

    db_dir.mkdir(parents=True, exist_ok=True)
    return db_dir / "history.db"


DB_PATH = _resolve_db_path()


@contextmanager
def get_connection():
    conn = sqlite3.connect(str(DB_PATH))
    conn.row_factory = sqlite3.Row
    try:
        yield conn
        conn.commit()
    finally:
        conn.close()


def init_db():
    with get_connection() as conn:
        conn.execute("""
            CREATE TABLE IF NOT EXISTS scan_history (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                timestamp TEXT NOT NULL,
                windows_health_score INTEGER,
                overall_health TEXT,
                hardware_health TEXT,
                critical_issues_count INTEGER,
                hardware_findings_count INTEGER,
                summary_json TEXT
            )
        """)
        conn.execute("""
            CREATE TABLE IF NOT EXISTS action_history (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                timestamp TEXT NOT NULL,
                action TEXT NOT NULL,
                service_name TEXT,
                status TEXT,
                output_summary TEXT
            )
        """)


def save_scan_result(windows_score, overall_health, hardware_health,
                      critical_issues_count, hardware_findings_count, summary):
    with get_connection() as conn:
        conn.execute(
            """
            INSERT INTO scan_history (
                timestamp, windows_health_score, overall_health,
                hardware_health, critical_issues_count,
                hardware_findings_count, summary_json
            ) VALUES (?, ?, ?, ?, ?, ?, ?)
            """,
            (
                datetime.now().isoformat(),
                windows_score,
                overall_health,
                hardware_health,
                critical_issues_count,
                hardware_findings_count,
                json.dumps(summary, ensure_ascii=False),
            ),
        )


def get_scan_history(limit=30):
    with get_connection() as conn:
        rows = conn.execute(
            "SELECT * FROM scan_history ORDER BY id DESC LIMIT ?",
            (limit,),
        ).fetchall()
        return [dict(r) for r in rows]


def save_action_result(action, service_name, status, output_summary=""):
    with get_connection() as conn:
        conn.execute(
            """
            INSERT INTO action_history (
                timestamp, action, service_name, status, output_summary
            ) VALUES (?, ?, ?, ?, ?)
            """,
            (
                datetime.now().isoformat(),
                action,
                service_name,
                status,
                output_summary[:2000] if output_summary else "",
            ),
        )


def get_action_history(limit=50):
    with get_connection() as conn:
        rows = conn.execute(
            "SELECT * FROM action_history ORDER BY id DESC LIMIT ?",
            (limit,),
        ).fetchall()
        return [dict(r) for r in rows]