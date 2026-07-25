#!/usr/bin/env python3
"""
Physical validation readiness scaffold.

Creates NO-GO reports for physical gates until real Dell/printer/Bluetooth/appliance
evidence is attached. Does not touch the legacy system.
"""

from __future__ import annotations

import json
from datetime import datetime, timezone
from pathlib import Path

ROOT = Path(r"D:\AtelieProd")
OUT = ROOT / "MOD" / "physical-validation"


def now() -> str:
    return datetime.now(timezone.utc).isoformat()


def main() -> None:
    OUT.mkdir(parents=True, exist_ok=True)
    gates = {
        "hardware": "NO-GO",
        "printer": "NO-GO",
        "bluetooth": "NO-GO",
        "appliance": "NO-GO",
        "restore": "NO-GO",
        "recovery": "NO-GO",
        "rollback": "NO-GO",
        "runtime_replay": "NO-GO",
        "shadow_execution": "NO-GO",
        "chaos": "NO-GO",
    }
    report = {
        "timestamp": now(),
        "overall": "NO-GO",
        "gates": gates,
        "reason": "Physical evidence is required before approval.",
        "required_evidence": [
            "Dell hardware inventory",
            "printer physical test",
            "Bluetooth physical test",
            "VM appliance boot",
            "Dell appliance boot",
            "restore execution",
            "rollback execution",
            "recovery execution",
        ],
    }
    (OUT / "physical-readiness.json").write_text(json.dumps(report, indent=2, ensure_ascii=False), encoding="utf-8")
    print(json.dumps(report, indent=2, ensure_ascii=False))


if __name__ == "__main__":
    main()
