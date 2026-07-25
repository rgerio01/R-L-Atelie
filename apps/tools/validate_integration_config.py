from __future__ import annotations

import json
import os
from datetime import datetime, timezone
from pathlib import Path


REQUIRED_ENV = {
    "supabase": [
        "SUPABASE_URL",
        "SUPABASE_PUBLISHABLE_KEY",
        "SUPABASE_DB_URL",
    ],
    "github": [
        "GITHUB_TOKEN",
    ],
    "mercado_pago": [
        "MERCADOPAGO_ROGERIO_TOKEN",
        "MERCADOPAGO_LUCI_TOKEN",
        "MERCADOPAGO_WEBHOOK_SECRET",
    ],
    "app": [
        "APP_ENCRYPTION_KEY",
    ],
}


def masked_present(name: str) -> dict:
    value = os.environ.get(name)
    return {
        "name": name,
        "present": bool(value),
        "length": len(value) if value else 0,
        "logged_value": False,
    }


def main() -> int:
    result = {
        "timestamp": datetime.now(timezone.utc).isoformat(),
        "status": "NO-GO",
        "groups": {},
        "tokens_logged": False,
    }
    missing = []
    for group, names in REQUIRED_ENV.items():
        result["groups"][group] = [masked_present(name) for name in names]
        missing.extend(name for name in names if not os.environ.get(name))
    result["missing"] = missing
    if not missing:
        result["status"] = "CONFIG_PRESENT_PENDING_RUNTIME_VALIDATION"
    out = Path(r"D:\AtelieProd\MOD\final-execution-parity\reports\integration-config-validation.json")
    out.parent.mkdir(parents=True, exist_ok=True)
    out.write_text(json.dumps(result, indent=2, ensure_ascii=False), encoding="utf-8")
    print(json.dumps(result, indent=2, ensure_ascii=False))
    return 0 if not missing else 2


if __name__ == "__main__":
    raise SystemExit(main())
