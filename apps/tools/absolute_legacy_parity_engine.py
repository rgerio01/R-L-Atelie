from __future__ import annotations

import json
from datetime import datetime, timezone
from pathlib import Path


ROOT = Path(r"D:\AtelieProd")
MOD = ROOT / "MOD"
PARITY_DIR = MOD / "absolute-parity"
PHYSICAL_READINESS = MOD / "physical-validation" / "physical-readiness.json"
OUTPUT = PARITY_DIR / "absolute-parity-readiness.json"


PARITY_GATES = [
    "domain_parity",
    "runtime_parity",
    "print_parity",
    "ui_parity",
    "report_parity",
    "permission_parity",
    "operation_replay",
    "shadow_execution",
    "chaos_validation",
    "appliance_parity",
    "digital_forensics",
    "divergence_detector",
    "hidden_behavior_detector",
    "implicit_rule_detector",
    "legacy_knowledge_extraction",
    "physical_readiness",
]


def read_json(path: Path) -> dict:
    if not path.exists():
        return {}
    return json.loads(path.read_text(encoding="utf-8"))


def build_readiness() -> dict:
    PARITY_DIR.mkdir(parents=True, exist_ok=True)
    physical = read_json(PHYSICAL_READINESS)
    physical_go = physical.get("overall") == "GO"

    gates = {gate: "NO-GO" for gate in PARITY_GATES}
    gates["physical_readiness"] = "GO" if physical_go else "NO-GO"

    blockers = [
        "absolute_replay_not_executed",
        "runtime_parity_not_traced",
        "print_parity_not_physically_validated",
        "ui_parity_not_replayed",
        "report_parity_not_diffed",
        "permission_parity_not_replayed",
        "critical_divergence_gate_active",
        "hardware_gates_pending",
    ]

    if not physical_go:
        blockers.append("physical_readiness_no_go")

    scores = {gate: 0 for gate in PARITY_GATES}
    if physical_go:
        scores["physical_readiness"] = 10

    return {
        "timestamp": datetime.now(timezone.utc).isoformat(),
        "overall": "NO-GO",
        "policy": "Absolute Legacy Parity blocks ownership until every gate is evidence-backed.",
        "gates": gates,
        "scores": scores,
        "blockers": blockers,
        "required_evidence": {
            "domain": [
                "full Paradox schema rerun",
                "field-to-field SQLite mapping",
                "implicit relation validation",
                "critical orphan behavior zero",
            ],
            "runtime": [
                "ProcMon/ETW/API Monitor traces",
                "startup order replay",
                "timer/thread map",
                "runtime failure replay",
            ],
            "print": [
                "physical printer test",
                "spool capture",
                "ESC/POS capture",
                "visual diff",
                "QRCode/cut validation",
            ],
            "ui": [
                "UI replay",
                "keyboard workflow validation",
                "operator productivity timing",
            ],
            "reports": [
                "legacy export",
                "NextGen export",
                "line/total/rounding diff",
            ],
            "permissions": [
                "permission graph",
                "permission replay",
                "hidden permission diff",
            ],
            "physical": physical.get("required_evidence", []),
        },
    }


def main() -> None:
    result = build_readiness()
    OUTPUT.write_text(json.dumps(result, indent=2, ensure_ascii=False), encoding="utf-8")
    print(json.dumps(result, indent=2, ensure_ascii=False))


if __name__ == "__main__":
    main()
