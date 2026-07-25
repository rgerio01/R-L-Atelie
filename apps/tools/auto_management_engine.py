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
REPORTS = MOD / "final-execution-parity" / "reports"
EVIDENCE = MOD / "final-execution-parity" / "evidence"


def read_json(path: Path) -> dict:
    if not path.exists():
        return {}
    return json.loads(path.read_text(encoding="utf-8"))


def status_from_evidence(path: Path) -> str:
    data = read_json(path)
    return data.get("status", "MISSING")


def main() -> int:
    secret_scan = read_json(REPORTS / "secret-scan.json")
    mp = read_json(EVIDENCE / "mercado-pago-accounts-validation.json")
    final = read_json(REPORTS / "final-execution-readiness.json")

    actions = []
    blockers = []

    if secret_scan.get("status") != "PASS":
        blockers.append("secret_scan_failed")
        actions.append("bloquear pipeline e rotacionar credenciais expostas")

    if mp.get("rogerio", {}).get("status") != "VALIDATED":
        blockers.append("mercado_pago_rogerio_not_validated")
        actions.append("validar MERCADOPAGO_ROGERIO_TOKEN")

    if mp.get("luci", {}).get("status") != "VALIDATED":
        blockers.append("mercado_pago_luci_not_validated")
        actions.append("validar MERCADOPAGO_LUCI_TOKEN")

    if final.get("level") == "NO-GO":
        blockers.append("final_readiness_no_go")
        actions.append("manter ownership/shadow-go bloqueados")

    report = {
        "timestamp": datetime.now(timezone.utc).isoformat(),
        "status": "NO-GO" if blockers else "OFFLINE-APPLIANCE-GO",
        "ownership_allowed": False,
        "shadow_go_allowed": False,
        "auto_management": {
            "auto_diagnostics": True,
            "auto_observability": True,
            "auto_reconciliation": "prepared",
            "auto_healing": "prepared",
            "auto_readiness": True,
            "auto_rollback": "requires recovery evidence",
            "auto_recovery": "requires physical/appliance evidence",
        },
        "inputs": {
            "secret_scan_status": secret_scan.get("status"),
            "mercado_pago": {
                "rogerio_status": mp.get("rogerio", {}).get("status"),
                "luci_status": mp.get("luci", {}).get("status"),
                "tokens_logged": mp.get("tokens_logged"),
            },
            "final_readiness": final.get("level"),
        },
        "blockers": blockers,
        "recommended_actions": actions,
    }
    out = REPORTS / "auto-management-report.json"
    out.write_text(json.dumps(report, indent=2, ensure_ascii=False), encoding="utf-8")
    print(json.dumps(report, indent=2, ensure_ascii=False))
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
