from __future__ import annotations

import json
import os
import sys
import urllib.request
from datetime import datetime, timezone
from pathlib import Path


OUT = Path(r"D:\AtelieProd\MOD\final-execution-parity\evidence\mercado-pago-accounts-validation.json")
API = "https://api.mercadopago.com/users/me"


def call_me(env_name: str) -> dict:
    token = os.environ.get(env_name)
    if not token:
        return {"env": env_name, "status": "NO-GO", "reason": "missing_env"}

    req = urllib.request.Request(API, headers={"Authorization": f"Bearer {token}"})
    try:
        with urllib.request.urlopen(req, timeout=30) as res:
            data = json.loads(res.read().decode("utf-8"))
    except Exception as exc:  # noqa: BLE001
        return {"env": env_name, "status": "NO-GO", "reason": type(exc).__name__}

    return {
        "env": env_name,
        "status": "VALIDATED",
        "account_id": data.get("id"),
        "nickname": data.get("nickname"),
        "site_id": data.get("site_id"),
        "email_masked": mask_email(data.get("email")),
        "raw_saved": False,
    }


def mask_email(value: str | None) -> str | None:
    if not value or "@" not in value:
        return None
    prefix, domain = value.split("@", 1)
    return f"{prefix[:2]}***@{domain}"


def main() -> int:
    result = {
        "timestamp": datetime.now(timezone.utc).isoformat(),
        "tokens_logged": False,
        "rogerio": call_me("MERCADOPAGO_ROGERIO_TOKEN"),
        "luci": call_me("MERCADOPAGO_LUCI_TOKEN"),
        "required_manual_check": [
            "confirmar se account_id/nickname correspondem a Rogerio",
            "confirmar se account_id/nickname correspondem a Luci",
            "rotacionar tokens expostos em imagem/chat antes de producao",
        ],
    }
    OUT.parent.mkdir(parents=True, exist_ok=True)
    OUT.write_text(json.dumps(result, indent=2, ensure_ascii=False), encoding="utf-8")
    print(json.dumps(result, indent=2, ensure_ascii=False))
    if result["rogerio"]["status"] == "VALIDATED" and result["luci"]["status"] == "VALIDATED":
        return 0
    return 2


if __name__ == "__main__":
    sys.exit(main())
