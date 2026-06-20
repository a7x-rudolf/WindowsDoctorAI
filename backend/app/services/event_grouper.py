from collections import defaultdict

from app.services.fingerprint_engine import (
    extract_service_name
)


def group_events(events):

    grouped = defaultdict(int)

    details = {}

    for event in events:

        service_name = extract_service_name(
            event.get("Message", "")
        )

        key = (
            event["Id"],
            event["ProviderName"],
            service_name
        )

        grouped[key] += 1

        details[key] = {
            "event_id": event["Id"],
            "provider": event["ProviderName"],
            "level": event["LevelDisplayName"],
            "service_name": service_name
        }

    result = []

    for key, count in grouped.items():

        item = details[key]

        item["count"] = count

        result.append(item)

    result.sort(
        key=lambda x: x["count"],
        reverse=True
    )

    return result