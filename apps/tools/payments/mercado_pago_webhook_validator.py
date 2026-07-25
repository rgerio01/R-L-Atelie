from __future__ import annotations

import hashlib
import hmac
import os
from dataclasses import dataclass


@dataclass
class WebhookValidationResult:
    valid: bool
    reason: str


def validate_x_signature(x_signature: str, x_request_id: str, data_id: str, secret_env: str) -> WebhookValidationResult:
    """Validate Mercado Pago webhook signature without logging secrets.

    The secret must be stored in the environment variable passed in secret_env.
    The exact manifest string must be reviewed against the configured Mercado Pago webhook
    documentation before production because products/topics can vary.
    """
    secret = os.environ.get(secret_env)
    if not secret:
        return WebhookValidationResult(False, "missing_secret_env")
    if not x_signature or not x_request_id or not data_id:
        return WebhookValidationResult(False, "missing_headers_or_data_id")

    parts = {}
    for item in x_signature.split(","):
        if "=" in item:
            key, value = item.split("=", 1)
            parts[key.strip()] = value.strip()
    ts = parts.get("ts")
    received_hash = parts.get("v1")
    if not ts or not received_hash:
        return WebhookValidationResult(False, "invalid_signature_header")

    manifest = f"id:{data_id};request-id:{x_request_id};ts:{ts};"
    expected = hmac.new(secret.encode("utf-8"), manifest.encode("utf-8"), hashlib.sha256).hexdigest()
    return WebhookValidationResult(hmac.compare_digest(expected, received_hash), "ok")
