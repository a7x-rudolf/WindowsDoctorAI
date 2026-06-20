def validate_disk(disk_info):

    free_percent = disk_info[
        "free_percent"
    ]

    if free_percent < 10:

        return {
            "healthy": False,
            "severity": "high",
            "message": "Disk almost full"
        }

    if free_percent < 20:

        return {
            "healthy": False,
            "severity": "medium",
            "message": "Disk space low"
        }

    return {
        "healthy": True,
        "severity": "ok",
        "message": "Disk healthy"
    }