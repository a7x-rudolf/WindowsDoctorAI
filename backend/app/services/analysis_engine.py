from app.collector.disk_collector import (
    get_disk_info
)

from app.services.system_validator import (
    validate_disk
)

from app.services.diagnosis_engine import (
    generate_diagnosis
)


def analyze_system(grouped_events):

    diagnoses = generate_diagnosis(
        grouped_events
    )

    disk_info = get_disk_info()

    disk_result = validate_disk(
        disk_info
    )

    critical_issues = []

    for item in diagnoses:

        if (
            item["severity"] == "high"
            and item["service_health"] == "Still Failing"
        ):

            critical_issues.append({
                "type": "service",
                "name": item["service_name"]
            })

    if not disk_result["healthy"]:

        critical_issues.append({
            "type": "disk",
            "name": disk_result["message"]
        })

    if critical_issues:

        overall_health = "Poor"

    elif len(diagnoses) >= 5:

        overall_health = "Warning"

    else:

        overall_health = "Good"

    return {
        "overall_health": overall_health,
        "diagnoses": diagnoses,
        "disk": disk_result,
        "critical_issues": critical_issues
    }