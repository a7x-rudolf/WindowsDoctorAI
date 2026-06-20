import subprocess
import json


def get_services():

    command = """
    Get-Service |
    Select-Object Name,DisplayName,Status |
    ConvertTo-Json -Depth 3
    """

    result = subprocess.run(
        ["powershell", "-Command", command],
        capture_output=True,
        text=True
    )

    try:
        data = json.loads(result.stdout)

        return {
            "success": True,
            "services": data
        }

    except Exception as e:
        return {
            "success": False,
            "error": str(e)
        }