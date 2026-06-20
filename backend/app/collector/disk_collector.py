import psutil


def get_disk_info():

    usage = psutil.disk_usage("C:\\")

    total_gb = round(
        usage.total / (1024 ** 3),
        2
    )

    used_gb = round(
        usage.used / (1024 ** 3),
        2
    )

    free_gb = round(
        usage.free / (1024 ** 3),
        2
    )

    free_percent = round(
        (usage.free / usage.total) * 100,
        2
    )

    return {
        "drive": "C:",
        "total_gb": total_gb,
        "used_gb": used_gb,
        "free_gb": free_gb,
        "free_percent": free_percent
    }