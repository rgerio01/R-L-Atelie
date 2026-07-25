from __future__ import annotations

import argparse
import base64
import ctypes
import json
import os
import platform
from ctypes import wintypes
from datetime import datetime, timezone
from pathlib import Path


VAULT_PATH = Path(r"D:\AtelieProd\MOD\config\secure-runtime.secrets")
REPORT_PATH = Path(r"D:\AtelieProd\MOD\final-execution-parity\reports\credential-protection-status.json")


SECRET_ENV_NAMES = [
    "SUPABASE_ACCESS_TOKEN",
    "SUPABASE_DB_URL",
    "SUPABASE_PUBLISHABLE_KEY",
    "SUPABASE_SERVICE_ROLE_KEY",
    "MERCADOPAGO_ROGERIO_TOKEN",
    "MERCADOPAGO_LUCI_TOKEN",
    "MERCADOPAGO_WEBHOOK_SECRET",
    "GITHUB_TOKEN",
    "APP_ENCRYPTION_KEY",
]


class DataBlob(ctypes.Structure):
    _fields_ = [("cbData", wintypes.DWORD), ("pbData", ctypes.POINTER(ctypes.c_char))]


def _blob_from_bytes(data: bytes) -> DataBlob:
    buf = ctypes.create_string_buffer(data)
    return DataBlob(len(data), ctypes.cast(buf, ctypes.POINTER(ctypes.c_char)))


def _bytes_from_blob(blob: DataBlob) -> bytes:
    try:
        return ctypes.string_at(blob.pbData, blob.cbData)
    finally:
        ctypes.windll.kernel32.LocalFree(blob.pbData)


def protect_windows(data: bytes) -> str:
    crypt32 = ctypes.windll.crypt32
    in_blob = _blob_from_bytes(data)
    out_blob = DataBlob()
    entropy = _blob_from_bytes(machine_entropy())
    ok = crypt32.CryptProtectData(
        ctypes.byref(in_blob),
        "AtelieNextGenRuntime",
        ctypes.byref(entropy),
        None,
        None,
        0,
        ctypes.byref(out_blob),
    )
    if not ok:
        raise ctypes.WinError()
    return base64.b64encode(_bytes_from_blob(out_blob)).decode("ascii")


def unprotect_windows(encoded: str) -> bytes:
    crypt32 = ctypes.windll.crypt32
    raw = base64.b64decode(encoded)
    in_blob = _blob_from_bytes(raw)
    out_blob = DataBlob()
    entropy = _blob_from_bytes(machine_entropy())
    ok = crypt32.CryptUnprotectData(
        ctypes.byref(in_blob),
        None,
        ctypes.byref(entropy),
        None,
        None,
        0,
        ctypes.byref(out_blob),
    )
    if not ok:
        raise ctypes.WinError()
    return _bytes_from_blob(out_blob)


def machine_entropy() -> bytes:
    value = f"{platform.node()}|{platform.system()}|AtelieNextGen|v1"
    return value.encode("utf-8")


def collect_env() -> dict:
    secrets = {}
    for name in SECRET_ENV_NAMES:
        value = os.environ.get(name)
        if value:
            secrets[name] = value
    return secrets


def create_vault() -> dict:
    if platform.system().lower() != "windows":
        raise RuntimeError("Este engine usa DPAPI no Windows. Linux appliance deve usar o loader OpenSSL/systemd-creds.")
    secrets = collect_env()
    payload = {
        "created_at": datetime.now(timezone.utc).isoformat(),
        "provider": "windows-dpapi-current-user",
        "secrets": secrets,
    }
    VAULT_PATH.parent.mkdir(parents=True, exist_ok=True)
    encrypted = protect_windows(json.dumps(payload, ensure_ascii=False).encode("utf-8"))
    VAULT_PATH.write_text(json.dumps({
        "format": "atelie-secure-runtime-v1",
        "provider": "windows-dpapi-current-user",
        "created_at": payload["created_at"],
        "secret_names": sorted(secrets),
        "payload": encrypted,
    }, indent=2), encoding="utf-8")
    return status()


def load_vault(masked: bool = True) -> dict:
    data = json.loads(VAULT_PATH.read_text(encoding="utf-8"))
    payload = json.loads(unprotect_windows(data["payload"]).decode("utf-8"))
    if masked:
        payload["secrets"] = {k: mask(v) for k, v in payload.get("secrets", {}).items()}
    return payload


def status() -> dict:
    exists = VAULT_PATH.exists()
    secret_names = []
    provider = None
    if exists:
        data = json.loads(VAULT_PATH.read_text(encoding="utf-8"))
        secret_names = data.get("secret_names", [])
        provider = data.get("provider")
    result = {
        "timestamp": datetime.now(timezone.utc).isoformat(),
        "vault_path": str(VAULT_PATH),
        "exists": exists,
        "provider": provider,
        "secret_names": secret_names,
        "secret_values_logged": False,
        "status": "READY" if exists and secret_names else "NO-GO",
        "notes": [
            "Valores reais nao sao impressos.",
            "No Windows, o payload usa DPAPI do usuario atual com entropia adicional.",
            "O arquivo secure-runtime.secrets deve permanecer fora do Git.",
        ],
    }
    REPORT_PATH.parent.mkdir(parents=True, exist_ok=True)
    REPORT_PATH.write_text(json.dumps(result, indent=2, ensure_ascii=False), encoding="utf-8")
    return result


def mask(value: str) -> str:
    if len(value) <= 8:
        return "***"
    return f"{value[:4]}...{value[-4:]}"


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("command", choices=["create", "status", "load-masked"])
    args = parser.parse_args()
    if args.command == "create":
        result = create_vault()
    elif args.command == "load-masked":
        result = load_vault(masked=True)
    else:
        result = status()
    print(json.dumps(result, indent=2, ensure_ascii=False))
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
