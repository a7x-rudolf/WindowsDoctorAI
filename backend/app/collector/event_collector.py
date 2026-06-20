import subprocess
import json


def get_system_errors(limit=20):

    powershell_command = f"""
    Get-WinEvent -LogName System -MaxEvents {limit} |
    Where-Object {{$_.LevelDisplayName -in @('Critical','Error')}} |
    Select-Object TimeCreated, Id, ProviderName, LevelDisplayName, Message |
    ConvertTo-Json -Depth 3
    """

    result = subprocess.run(
        ["powershell", "-Command", powershell_command],
        capture_output=True,
        text=True
    )

    if result.returncode != 0:
        return {
            "success": False,
            "error": result.stderr
        }

    # PowerShell kosong (tidak ada event Critical/Error sama sekali)
    if not result.stdout.strip():
        return {
            "success": True,
            "count": 0,
            "events": []
        }

    try:
        data = json.loads(result.stdout)

        # ConvertTo-Json TIDAK membungkus hasil dalam array kalau cuma ada
        # 1 event — outputnya jadi single object, bukan list of object.
        # Normalisasi selalu jadi list di sini.
        if isinstance(data, dict):
            data = [data]

        # Filter entri yang bukan dict (mis. string mentah akibat field
        # Message yang gagal di-serialize PowerShell) supaya tidak crash
        # di event_grouper.py / fingerprint_engine.py downstream.
        events = [e for e in data if isinstance(e, dict)]

        return {
            "success": True,
            "count": len(events),
            "events": events
        }

    except Exception as e:
        return {
            "success": False,
            "error": str(e)
        }