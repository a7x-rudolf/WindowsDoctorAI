import uvicorn
import sys
import os
import logging
import traceback
import socket
from pathlib import Path

def is_port_free(port):
    with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
        return s.connect_ex(('127.0.0.1', port)) != 0

if __name__ == "__main__":
    try:
        if not is_port_free(8000):
            logging.error("Port 8000 sudah dipakai! Backend tidak bisa start.")
            raise RuntimeError("Port 8000 in use")
        uvicorn.run(
            "app.main:app",
            host="127.0.0.1",
            port=8000,
            log_level="error",
            log_config=None
        )
    except Exception:
        logging.error("Backend gagal start:\n%s", traceback.format_exc())
        raise

# ── Lokasi log file yang pasti writable, sama seperti history.db ───────────
if getattr(sys, 'frozen', False):
    _log_dir = Path(os.environ.get("LOCALAPPDATA") or os.path.expanduser("~")) / "WindowsDoctorAI"
else:
    _log_dir = Path(__file__).parent / "logs"
_log_dir.mkdir(parents=True, exist_ok=True)
_log_file = _log_dir / "backend_error.log"

logging.basicConfig(
    level=logging.ERROR,
    filename=str(_log_file),
    filemode="a",
    format="%(asctime)s %(levelname)s %(message)s",
)

if getattr(sys, 'frozen', False):
    base_dir = sys._MEIPASS
    sys.path.insert(0, base_dir)

if __name__ == "__main__":
    try:
        uvicorn.run(
            "app.main:app",
            host="127.0.0.1",
            port=8000,
            log_level="error",
            log_config=None
        )
    except Exception:
        # Kalau backend gagal start (port dipakai, import error, dll),
        # catat ke file supaya bisa didiagnosa dari PC mana pun —
        # tanpa ini, .exe yang console=False akan gagal diam-diam.
        logging.error("Backend gagal start:\n%s", traceback.format_exc())
        raise