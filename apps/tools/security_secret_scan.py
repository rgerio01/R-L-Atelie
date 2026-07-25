from __future__ import annotations

import json
import os
import re
import sys
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
ROOTS = [
    MOD,
    ROOT / "docs",
    ROOT / "Atelie_Windows",
    ROOT / "Atelie_Linux",
]

SKIP_DIRS = {
    "__pycache__",
    "total-validation",
    "backups",
    ".git",
}

PATTERNS = {
    "mercado_pago_token": re.compile(r"APP_USR-[A-Za-z0-9._-]{20,}"),
    "supabase_access_token": re.compile(r"sbp_[A-Za-z0-9]{20,}"),
    "github_pat": re.compile(r"(ghp|github_pat)_[A-Za-z0-9_]{20,}"),
    "supabase_service_role": re.compile(r"eyJ[A-Za-z0-9_-]{20,}\.[A-Za-z0-9_-]{20,}\.[A-Za-z0-9_-]{20,}"),
    "postgres_password_inline": re.compile(r"postgresql://postgres:[^@\s]+@"),
    "known_db_password_like": re.compile(r"D4EBo7KXS145gCTy"),
}


def should_skip(path: Path) -> bool:
    if path.name == "security_secret_scan.py":
        return True
    if path.name == ".env.example":
        return True
    if path.name == ".env":
        return True
    return any(part in SKIP_DIRS for part in path.parts)


def scan() -> dict:
    findings = []
    for root in ROOTS:
        if not root.exists():
            continue
        for path in root.rglob("*"):
            if should_skip(path) or not path.is_file():
                continue
            if path.stat().st_size > 5_000_000:
                continue
            try:
                text = path.read_text(encoding="utf-8", errors="ignore")
            except Exception:  # noqa: BLE001
                continue
            for name, pattern in PATTERNS.items():
                for match in pattern.finditer(text):
                    findings.append({
                        "file": str(path),
                        "pattern": name,
                        "line": text[:match.start()].count("\n") + 1,
                        "masked": mask(match.group(0)),
                    })
    return {
        "timestamp": datetime.now(timezone.utc).isoformat(),
        "status": "PASS" if not findings else "FAIL",
        "findings": findings,
    }


def mask(value: str) -> str:
    if len(value) <= 12:
        return "***"
    return f"{value[:6]}...{value[-4:]}"


def main() -> int:
    result = scan()
    out = MOD / "final-execution-parity" / "reports" / "secret-scan.json"
    out.parent.mkdir(parents=True, exist_ok=True)
    out.write_text(json.dumps(result, indent=2, ensure_ascii=False), encoding="utf-8")
    print(json.dumps(result, indent=2, ensure_ascii=False))
    return 0 if result["status"] == "PASS" else 2


if __name__ == "__main__":
    sys.exit(main())
