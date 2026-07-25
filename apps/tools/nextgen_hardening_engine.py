#!/usr/bin/env python3
"""
NextGen operational hardening scaffold.

Creates local-only backup/audit/event/readiness artifacts under D:\\AtelieProd\\MOD\\hardening.
It does not touch the legacy system.
"""

from __future__ import annotations

import argparse
import hashlib
import json
import shutil
import uuid
from datetime import datetime, timezone
from pathlib import Path


ROOT = Path(r"D:\AtelieProd")
BASE = ROOT / "MOD" / "hardening"


def now() -> str:
    return datetime.now(timezone.utc).isoformat()


def sha256(path: Path) -> str:
    h = hashlib.sha256()
    with path.open("rb") as f:
        for chunk in iter(lambda: f.read(1024 * 1024), b""):
            h.update(chunk)
    return h.hexdigest()


def append_jsonl(path: Path, event: dict) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    with path.open("a", encoding="utf-8") as f:
        f.write(json.dumps(event, ensure_ascii=False) + "\n")


def event(event_type: str, payload: dict) -> dict:
    return {
        "event_id": str(uuid.uuid4()),
        "timestamp": now(),
        "event_type": event_type,
        "correlation_id": payload.get("correlation_id", str(uuid.uuid4())),
        "payload": payload,
    }


def init() -> None:
    for d in ["backups", "journals", "readiness", "restore-tests", "chaos", "rollback"]:
        (BASE / d).mkdir(parents=True, exist_ok=True)
    append_jsonl(BASE / "journals" / "event-journal.jsonl", event("hardening.init", {"status": "ok"}))
    print(f"Initialized hardening workspace at {BASE}")


def backup(target: str | None) -> None:
    target_path = Path(target) if target else ROOT / "MOD" / "validation"
    if not target_path.exists():
        raise SystemExit(f"Target not found: {target_path}")
    backup_id = datetime.now().strftime("%Y%m%d-%H%M%S") + "-" + str(uuid.uuid4())[:8]
    dest = BASE / "backups" / backup_id
    if target_path.is_dir():
        shutil.copytree(target_path, dest)
        files = [p for p in dest.rglob("*") if p.is_file()]
    else:
        dest.mkdir(parents=True, exist_ok=True)
        shutil.copy2(target_path, dest / target_path.name)
        files = [dest / target_path.name]
    manifest = {
        "backup_id": backup_id,
        "timestamp": now(),
        "source": str(target_path),
        "destination": str(dest),
        "files": [{"path": str(p), "sha256": sha256(p), "size": p.stat().st_size} for p in files],
    }
    (dest / "backup-manifest.json").write_text(json.dumps(manifest, indent=2, ensure_ascii=False), encoding="utf-8")
    append_jsonl(BASE / "journals" / "audit-journal.jsonl", event("backup.created", manifest))
    print(json.dumps(manifest, indent=2, ensure_ascii=False))


def readiness() -> None:
    blockers = [
        "restore_not_validated",
        "rollback_not_tested",
        "recovery_not_tested",
        "critical_divergence_gate_active",
        "hardware_gates_pending",
    ]
    result = {
        "timestamp": now(),
        "status": "NO-GO",
        "blockers": blockers,
        "scores": {
            "backup": 10 if any((BASE / "backups").iterdir()) else 0,
            "restore": 0,
            "rollback": 0,
            "recovery": 0,
            "audit": 10 if (BASE / "journals" / "audit-journal.jsonl").exists() else 0,
            "appliance": 0,
        },
    }
    out = BASE / "readiness" / "hardening-readiness.json"
    out.write_text(json.dumps(result, indent=2, ensure_ascii=False), encoding="utf-8")
    append_jsonl(BASE / "journals" / "event-journal.jsonl", event("readiness.evaluated", result))
    print(json.dumps(result, indent=2, ensure_ascii=False))


def main() -> None:
    parser = argparse.ArgumentParser()
    sub = parser.add_subparsers(dest="command", required=True)
    sub.add_parser("init")
    b = sub.add_parser("backup")
    b.add_argument("--target")
    sub.add_parser("readiness")
    args = parser.parse_args()
    if args.command == "init":
        init()
    elif args.command == "backup":
        backup(args.target)
    elif args.command == "readiness":
        readiness()


if __name__ == "__main__":
    main()
