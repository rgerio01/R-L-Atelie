from __future__ import annotations

import json
import os
from datetime import datetime, timezone
from pathlib import Path


def project_root() -> Path:
    configured = os.environ.get("ATELIE_ROOT")
    if configured:
        return Path(configured)
    windows_root = Path(r"D:\AtelieProd")
    if windows_root.exists():
        return windows_root
    return Path.cwd().resolve()


ROOT = project_root()
MOD = ROOT / "MOD" if (ROOT / "MOD").exists() else ROOT
BASE = MOD / "final-execution-parity"
EVIDENCE = BASE / "evidence"
REPORTS = BASE / "reports"
ABSOLUTE = MOD / "absolute-parity" / "absolute-parity-readiness.json"
PHYSICAL = MOD / "physical-validation" / "physical-readiness.json"
HARDENING = MOD / "hardening" / "readiness" / "hardening-readiness.json"


GATES = {
    "replay": "replay-evidence.json",
    "runtime": "runtime-parity-evidence.json",
    "print": "print-parity-evidence.json",
    "ui": "ui-parity-evidence.json",
    "reports": "report-diff-evidence.json",
    "permissions": "permission-replay-evidence.json",
    "dell_hardware": "dell-hardware-validation.json",
    "bluetooth": "bluetooth-validation-real.json",
    "appliance": "appliance-validation-real.json",
    "restore": "restore-validation-real.json",
    "recovery": "recovery-validation-real.json",
    "rollback": "rollback-validation-real.json",
    "shadow_execution": "shadow-execution-definitivo.json",
    "divergence_elimination": "divergence-elimination.json",
}


def read_json(path: Path) -> dict:
    if not path.exists():
        return {}
    return json.loads(path.read_text(encoding="utf-8"))


def write_json(path: Path, data: dict) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(json.dumps(data, indent=2, ensure_ascii=False), encoding="utf-8")


def evidence_template(name: str) -> dict:
    return {
        "gate": name,
        "status": "NO-GO",
        "validated": False,
        "critical_divergences": None,
        "evidence_files": [],
        "operator": None,
        "validated_at": None,
        "notes": "Preencher somente apos execucao real. Nao marcar validated=true sem evidencia anexada.",
    }


def init() -> None:
    EVIDENCE.mkdir(parents=True, exist_ok=True)
    REPORTS.mkdir(parents=True, exist_ok=True)
    for gate, filename in GATES.items():
        path = EVIDENCE / filename
        if not path.exists():
            write_json(path, evidence_template(gate))


def gate_status(doc: dict) -> str:
    if doc.get("validated") is True and doc.get("critical_divergences") == 0 and doc.get("status") in {"GO", "CONDITIONAL-GO", "SHADOW-GO"}:
        return "GO"
    return "NO-GO"


def evaluate() -> dict:
    init()
    absolute = read_json(ABSOLUTE)
    physical = read_json(PHYSICAL)
    hardening = read_json(HARDENING)

    gate_results = {}
    blockers = []
    for gate, filename in GATES.items():
        doc = read_json(EVIDENCE / filename)
        status = gate_status(doc)
        gate_results[gate] = status
        if status != "GO":
            blockers.append(f"{gate}_not_validated")

    upstream = {
        "absolute_legacy_parity": absolute.get("overall", "MISSING"),
        "physical_readiness": physical.get("overall", "MISSING"),
        "hardening": hardening.get("status", "MISSING"),
    }
    for name, status in upstream.items():
        if status != "GO":
            blockers.append(f"{name}_not_go")

    critical_clear = gate_results.get("divergence_elimination") == "GO"
    all_core = all(status == "GO" for status in gate_results.values())
    upstream_go = all(status == "GO" for status in upstream.values())

    if all_core and upstream_go and critical_clear:
        level = "CONDITIONAL-GO"
    elif any(status == "GO" for status in gate_results.values()):
        level = "LIMITED-NOGO"
    else:
        level = "NO-GO"

    result = {
        "timestamp": datetime.now(timezone.utc).isoformat(),
        "level": level,
        "ownership_allowed": False,
        "shadow_go_allowed": level in {"CONDITIONAL-GO", "SHADOW-GO", "PARTIAL-GO", "CONTROLLED-GO"},
        "policy": "Ownership permanece bloqueado; esta fase busca no maximo CONDITIONAL-GO/SHADOW-GO baseado em evidencia real.",
        "gates": gate_results,
        "upstream": upstream,
        "blockers": blockers,
        "required_next_actions": [
            "executar replay real e anexar evidencia",
            "executar tracing runtime real",
            "executar impressao fisica real",
            "executar UI replay real",
            "executar diff real de relatorios",
            "executar replay real de permissoes",
            "validar Dell, Bluetooth e appliance reais",
            "validar restore, recovery e rollback reais",
            "executar shadow execution definitiva",
            "zerar divergencias criticas",
        ],
    }
    write_json(REPORTS / "final-execution-readiness.json", result)
    return result


def main() -> None:
    init()
    result = evaluate()
    print(json.dumps(result, indent=2, ensure_ascii=False))


if __name__ == "__main__":
    main()
