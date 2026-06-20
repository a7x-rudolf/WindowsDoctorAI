def service_status_lookup(
    service_name,
    services
):

    for svc in services:

        if (
            svc["DisplayName"]
            == service_name
        ):
            return svc

    return None


def translate_status(status):

    mapping = {
        1: "Stopped",
        4: "Running"
    }

    return mapping.get(
        status,
        "Unknown"
    )