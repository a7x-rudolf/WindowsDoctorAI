import json
from pathlib import Path

knowledge_file = (
    Path(__file__).parent.parent
    / "data"
    / "event_knowledge.json"
)

with open(knowledge_file, "r", encoding="utf-8") as f:
    KNOWLEDGE = json.load(f)


def enrich_events(grouped_events):

    total_penalty = 0

    enriched = []

    for event in grouped_events:

        event_id = str(event["event_id"])

        info = KNOWLEDGE.get(
            event_id,
            {
                "title": "Unknown Event",
                "severity": "unknown",
                "score_penalty": 2,
                "description": "Belum ada data."
            }
        )

        penalty = (
            info["score_penalty"]
            * event["count"]
        )

        total_penalty += penalty

        enriched.append({
            **event,
            "title": info["title"],
            "severity": info["severity"],
            "description": info["description"],
            "penalty": penalty
        })

    return enriched, total_penalty


def calculate_health_score(total_penalty):

    score = 100 - total_penalty

    if score < 0:
        score = 0

    return score