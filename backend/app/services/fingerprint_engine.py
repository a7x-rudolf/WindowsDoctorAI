import re


# ── Pattern list — ordered most specific first ────────────────────────────────
# Each tuple: (regex_pattern, group_index_for_service_name)

PATTERNS = [
    # Event 7031 / 7034: "The X service terminated unexpectedly"
    (r"The (.+?) service terminated unexpectedly", 1),

    # Event 7023: "The X service terminated with the following error"
    (r"The (.+?) service terminated with (?:the following error|a service-specific error)", 1),

    # Event 7000: "The X service failed to start due to the following error"
    (r"The (.+?) service failed to start", 1),

    # Event 7009: "A timeout was reached (30000 milliseconds) waiting for the X service"
    (r"timeout.*?waiting for the (.+?) service", 1),

    # Event 7011: "A timeout was reached (30000 milliseconds) waiting for a transaction response from the X service"
    (r"waiting for a transaction response from the (.+?) service", 1),

    # Event 7001: "The X service depends on the Y service which failed to start"
    (r"The (.+?) service depends on the (.+?) service which failed", 1),

    # Event 7003: "The X service depends on the following nonexistent service: Y"
    (r"The (.+?) service depends on the following nonexistent service", 1),

    # Event 7024: "The X service terminated with service-specific error Y"
    (r"The (.+?) service terminated with service-specific error", 1),

    # Event 7038: "The X service was unable to log on as Y with the currently configured password"
    (r"The (.+?) service was unable to log on", 1),

    # Event 7040: "The start type of the X service was changed"
    (r"The start type of the (.+?) service was changed", 1),
]


def extract_service_name(message: str):

    if not message:
        return None

    for pattern, group_index in PATTERNS:
        match = re.search(pattern, message, re.IGNORECASE)
        if match:
            name = match.group(group_index).strip()
            # Clean up common suffixes that sometimes appear
            name = re.sub(r'\s+service$', '', name, flags=re.IGNORECASE)
            return name if name else None

    return None