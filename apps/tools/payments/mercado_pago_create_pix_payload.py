from __future__ import annotations

import json
from decimal import Decimal, ROUND_HALF_UP


def licensing_plan(code: str) -> dict:
    plans = {
        "30_DIAS": {"months": 1, "discount": Decimal("0")},
        "3_MESES": {"months": 3, "discount": Decimal("0.05")},
        "6_MESES": {"months": 6, "discount": Decimal("0.08")},
        "12_MESES": {"months": 12, "discount": Decimal("0.10")},
    }
    if code not in plans:
        raise ValueError("plano invalido")
    monthly = Decimal("350.00")
    gross = monthly * plans[code]["months"]
    discount = (gross * plans[code]["discount"]).quantize(Decimal("0.01"), rounding=ROUND_HALF_UP)
    final = (gross - discount).quantize(Decimal("0.01"), rounding=ROUND_HALF_UP)
    return {
        "code": code,
        "months": plans[code]["months"],
        "gross": str(gross.quantize(Decimal("0.01"))),
        "discount": str(discount),
        "final": str(final),
    }


def pix_payment_payload(amount: str, description: str, payer_email: str, external_reference: str, notification_url: str) -> dict:
    return {
        "transaction_amount": float(amount),
        "description": description,
        "payment_method_id": "pix",
        "payer": {"email": payer_email},
        "external_reference": external_reference,
        "notification_url": notification_url,
    }


if __name__ == "__main__":
    plan = licensing_plan("30_DIAS")
    print(json.dumps(plan, indent=2, ensure_ascii=False))
