from __future__ import annotations

import json
import os
import urllib.request
from decimal import Decimal
from typing import Any

from mercado_pago_create_pix_payload import licensing_plan, pix_payment_payload


PAYMENTS_API = "https://api.mercadopago.com/v1/payments"


class MercadoPagoConfigError(RuntimeError):
    pass


def _token_for(account_owner: str) -> str:
    env_name = {
        "rogerio": "MERCADOPAGO_ROGERIO_TOKEN",
        "luci": "MERCADOPAGO_LUCI_TOKEN",
    }.get(account_owner)
    if not env_name:
        raise MercadoPagoConfigError("account_owner invalido")
    token = os.environ.get(env_name)
    if not token:
        raise MercadoPagoConfigError(f"variavel de ambiente ausente: {env_name}")
    return token


def create_pix_payment(account_owner: str, amount: Decimal, description: str, payer_email: str, external_reference: str) -> dict[str, Any]:
    token = _token_for(account_owner)
    notification_url = os.environ.get("MERCADOPAGO_WEBHOOK_URL")
    if not notification_url:
        raise MercadoPagoConfigError("MERCADOPAGO_WEBHOOK_URL ausente")

    payload = pix_payment_payload(
        amount=str(amount),
        description=description,
        payer_email=payer_email,
        external_reference=external_reference,
        notification_url=notification_url,
    )
    body = json.dumps(payload).encode("utf-8")
    req = urllib.request.Request(
        PAYMENTS_API,
        data=body,
        headers={
            "Authorization": f"Bearer {token}",
            "Content-Type": "application/json",
            "X-Idempotency-Key": external_reference,
        },
        method="POST",
    )
    with urllib.request.urlopen(req, timeout=30) as res:
        data = json.loads(res.read().decode("utf-8"))
    return {
        "id": data.get("id"),
        "status": data.get("status"),
        "status_detail": data.get("status_detail"),
        "qr_code": data.get("point_of_interaction", {}).get("transaction_data", {}).get("qr_code"),
        "qr_code_base64": data.get("point_of_interaction", {}).get("transaction_data", {}).get("qr_code_base64"),
        "ticket_url": data.get("point_of_interaction", {}).get("transaction_data", {}).get("ticket_url"),
    }


def create_license_pix(plan_code: str, payer_email: str, external_reference: str) -> dict[str, Any]:
    plan = licensing_plan(plan_code)
    return create_pix_payment(
        account_owner="rogerio",
        amount=Decimal(plan["final"]),
        description=f"Licenca Atelie NextGen {plan_code}",
        payer_email=payer_email,
        external_reference=external_reference,
    )


def create_sale_pix(amount: Decimal, payer_email: str, external_reference: str) -> dict[str, Any]:
    return create_pix_payment(
        account_owner="luci",
        amount=amount,
        description="Venda Atelie NextGen",
        payer_email=payer_email,
        external_reference=external_reference,
    )

