#!/usr/bin/env python3
"""
NextGen validation engines scaffold.

This tool does not touch the legacy system. It creates/validates JSONL scenario
files for operation replay, ownership simulation, cutover simulation, chaos
testing and readiness scoring.
"""

from __future__ import annotations

import argparse
import json
import uuid
from datetime import datetime, timezone
from pathlib import Path


ROOT = Path(r"D:\AtelieProd")
OUT = ROOT / "MOD" / "validation"


def now() -> str:
    return datetime.now(timezone.utc).isoformat()


def write_json(path: Path, data: object) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(json.dumps(data, indent=2, ensure_ascii=False), encoding="utf-8")


def init() -> None:
    scenarios = {
        "operation_replay": [
            {
                "id": str(uuid.uuid4()),
                "timestamp": now(),
                "module": "clientes",
                "operation": "list",
                "entity": "cliente",
                "source": "synthetic",
                "evidence": "schema+ui",
                "payload": {"filter": "all"},
                "expected": {"must_not_error": True},
            },
            {
                "id": str(uuid.uuid4()),
                "timestamp": now(),
                "module": "clientes",
                "operation": "create_test",
                "entity": "cliente",
                "source": "synthetic",
                "evidence": "planned",
                "payload": {"nome": "CLIENTE TESTE NEXTGEN", "documento": "TESTE"},
                "expected": {"audit_event": True, "rollback_possible": True},
            },
        ],
        "ownership_simulation": [
            {
                "module": "clientes",
                "from_owner": "legacy",
                "to_owner": "nextgen",
                "required_gates": [
                    "backup",
                    "snapshot",
                    "etl",
                    "comparator",
                    "reconciler",
                    "rollback",
                    "recovery",
                    "audit",
                ],
            }
        ],
        "chaos": [
            {"id": "sqlite_unavailable", "severity": "high", "allowed_env": "homologation"},
            {"id": "network_down", "severity": "medium", "allowed_env": "homologation"},
            {"id": "printer_unavailable", "severity": "high", "allowed_env": "homologation"},
        ],
    }
    for name, data in scenarios.items():
        write_json(OUT / f"{name}.json", data)
    write_json(
        OUT / "readiness_criteria.json",
        {
            "blockers": [
                "critical_divergence",
                "rollback_not_tested",
                "audit_disabled",
                "backup_missing",
                "recovery_not_tested",
            ],
            "scores": {
                "operational": 0,
                "consistency": 0,
                "print": 0,
                "runtime": 0,
                "sqlite": 0,
                "ownership": 0,
                "rollback": 0,
                "recovery": 0,
            },
        },
    )
    print(f"Initialized validation scaffolds in {OUT}")


def score() -> None:
    criteria = json.loads((OUT / "readiness_criteria.json").read_text(encoding="utf-8"))
    scores = criteria["scores"]
    blockers = criteria["blockers"]
    status = "NO-GO" if blockers else "GO"
    result = {
        "timestamp": now(),
        "status": status,
        "scores": scores,
        "blockers": blockers,
        "note": "Initial scaffold score. Replace blockers after real validations.",
    }
    write_json(OUT / "readiness_result.json", result)
    print(json.dumps(result, indent=2, ensure_ascii=False))


def main() -> None:
    parser = argparse.ArgumentParser()
    parser.add_argument("command", choices=["init", "score"])
    args = parser.parse_args()
    if args.command == "init":
        init()
    elif args.command == "score":
        score()


if __name__ == "__main__":
    main()
