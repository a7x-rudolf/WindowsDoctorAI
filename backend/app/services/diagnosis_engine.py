import json
from pathlib import Path

from app.collector.service_collector import get_services
from app.services.service_resolver import (
    service_status_lookup,
    translate_status
)

root_file = (
    Path(__file__).parent.parent
    / "data"
    / "root_causes.json"
)

with open(root_file, "r", encoding="utf-8") as f:
    ROOT_CAUSES = json.load(f)


SEVERITY_WEIGHT = {
    "high": 5,
    "medium": 3,
    "low": 1
}


def generate_diagnosis(grouped_events):

    diagnoses = []

    services_result = get_services()
    services = services_result["services"]

    for event in grouped_events:

        service_name = event.get(
            "service_name"
        )

        if not service_name:
            continue

        info = ROOT_CAUSES.get(
            service_name
        )

        if not info:
            continue

        svc = service_status_lookup(
            service_name,
            services
        )

        weight = SEVERITY_WEIGHT.get(
            info["severity"],
            1
        )

        priority_score = (
            event["count"] * weight
        )

        current_status = None

        if svc:
            current_status = translate_status(
                svc["Status"]
            )

        service_health = "Unknown"

        if current_status == "Running":
            service_health = "Recovered"

        elif current_status == "Stopped":
            service_health = "Still Failing"

        diagnosis = {
            **event,
            "category": info["category"],
            "severity": info["severity"],
            "possible_causes": info["possible_causes"],
            "recommended_actions": info["recommended_actions"],
            "current_status": current_status,
            "service_health": service_health,
            "priority_score": priority_score
        }

        diagnoses.append(
            diagnosis
        )

    diagnoses.sort(
        key=lambda x: x["priority_score"],
        reverse=True
    )

    return diagnoses