from __future__ import annotations

import argparse
import json
import sys
from pathlib import Path


ROOT = Path(__file__).resolve().parents[2]
REPORTS = ROOT / "final-execution-parity" / "reports"

# Canais de teste (homolog/appliance/beta) usam o release para validar o
# mecanismo de atualizacao/appliance em si, nao para ir a producao real.
# So o canal 'stable' exige as evidencias completas de prontidao fisica
# (hardware Dell, Bluetooth, recovery, rollback, shadow execution etc.);
# nos demais canais so o scan de segredos precisa passar.
STRICT_CHANNELS = {"stable"}


def read_json(path: Path) -> dict:
    if not path.exists():
        return {}
    return json.loads(path.read_text(encoding="utf-8"))


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--channel", default="stable")
    args = parser.parse_args()

    final_readiness = read_json(REPORTS / "final-execution-readiness.json")
    auto_management = read_json(REPORTS / "auto-management-report.json")
    secret_scan = read_json(REPORTS / "secret-scan.json")

    blockers = []
    if secret_scan.get("status") != "PASS":
        blockers.append("secret_scan_not_pass")
    if args.channel in STRICT_CHANNELS:
        if final_readiness.get("level") not in {"CONDITIONAL-GO", "SHADOW-GO", "PARTIAL-GO", "CONTROLLED-GO"}:
            blockers.append("release_readiness_not_met")
        if auto_management.get("status") == "NO-GO":
            blockers.append("auto_management_no_go")

    result = {
        "channel": args.channel,
        "release_allowed": not blockers,
        "blockers": blockers,
        "policy": "Canal 'stable' exige tests, secret scan, readiness, checksums, signatures e manifest gates completos. Canais de teste (homolog/appliance/beta) exigem apenas o secret scan.",
    }
    print(json.dumps(result, indent=2))
    if blockers:
        return 10
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
