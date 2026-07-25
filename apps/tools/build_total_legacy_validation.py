from __future__ import annotations

import csv
import hashlib
import json
from datetime import datetime, timezone
from pathlib import Path


ROOT = Path(r"D:\AtelieProd")
LEGACY = ROOT / "Equipexe"
OUT = ROOT / "MOD" / "total-validation"
DOCS = ROOT / "docs" / "validacao-total"


def sha256(path: Path) -> str:
    h = hashlib.sha256()
    with path.open("rb") as f:
        for chunk in iter(lambda: f.read(1024 * 1024), b""):
            h.update(chunk)
    return h.hexdigest()


def classify(path: Path) -> str:
    ext = path.suffix.lower()
    if ext == ".db":
        return "paradox_table_candidate"
    if ext in {".px", ".xg0", ".yg0", ".val", ".tv"}:
        return "paradox_index_or_metadata"
    if ext in {".exe", ".dll", ".ocx", ".bpl"}:
        return "binary_runtime"
    if ext in {".ini", ".cfg", ".xml", ".json", ".txt"}:
        return "config_or_text"
    if ext in {".fr3", ".qrp", ".rpt"}:
        return "report_template_candidate"
    if ext in {".bmp", ".png", ".jpg", ".jpeg", ".ico"}:
        return "visual_asset"
    return "unknown"


def evidence_status(kind: str) -> str:
    if kind in {"paradox_table_candidate", "paradox_index_or_metadata"}:
        return "confirmado por schema pendente de dicionario campo-a-campo"
    if kind == "binary_runtime":
        return "confirmado por inventario pendente de tracing/runtime"
    if kind == "report_template_candidate":
        return "confirmado por arquivo pendente de diff de relatorio"
    return "confirmado por inventario pendente de classificacao funcional"


def main() -> None:
    OUT.mkdir(parents=True, exist_ok=True)
    DOCS.mkdir(parents=True, exist_ok=True)
    rows = []
    for path in LEGACY.rglob("*"):
        if path.is_file():
            rel = str(path.relative_to(LEGACY))
            kind = classify(path)
            rows.append({
                "relative_path": rel,
                "size": path.stat().st_size,
                "modified_utc": datetime.fromtimestamp(path.stat().st_mtime, timezone.utc).isoformat(),
                "kind": kind,
                "sha256": sha256(path),
                "evidence_status": evidence_status(kind),
                "nextgen_action": "mapear, migrar, arquivar ou justificar formalmente",
            })

    csv_path = OUT / "matriz-completa-equipeexe.csv"
    with csv_path.open("w", newline="", encoding="utf-8") as f:
        writer = csv.DictWriter(f, fieldnames=list(rows[0].keys()) if rows else [])
        if rows:
            writer.writeheader()
            writer.writerows(rows)

    summary = {
        "timestamp": datetime.now(timezone.utc).isoformat(),
        "legacy_path": str(LEGACY),
        "files": len(rows),
        "by_kind": {},
        "status": "NO-GO",
        "reason": "Inventario read-only criado; replay/runtime/report/permission/physical gates ainda pendentes.",
    }
    for row in rows:
        summary["by_kind"][row["kind"]] = summary["by_kind"].get(row["kind"], 0) + 1
    (OUT / "resumo-validacao-total.json").write_text(json.dumps(summary, indent=2, ensure_ascii=False), encoding="utf-8")

    report = [
        "# Relatorio Nada Ficou Para Tras",
        "",
        f"Data: {datetime.now().date().isoformat()}",
        "",
        "Status: NO-GO",
        "",
        "## Resultado",
        "",
        f"Inventario read-only executado em `{LEGACY}`.",
        f"Arquivos inventariados: {len(rows)}.",
        "",
        "## Classificacao por tipo",
        "",
    ]
    for kind, count in sorted(summary["by_kind"].items()):
        report.append(f"- {kind}: {count}")
    report.extend([
        "",
        "## Regra",
        "",
        "Nada sera considerado preservado apenas por inventario. Cada item precisa de destino NextGen: migrar, reproduzir, arquivar ou justificar formalmente.",
        "",
        "## Arquivos gerados",
        "",
        f"- `{csv_path}`",
        f"- `{OUT / 'resumo-validacao-total.json'}`",
    ])
    (DOCS / "relatorio-nada-ficou-para-tras.md").write_text("\n".join(report), encoding="utf-8")
    print(json.dumps(summary, indent=2, ensure_ascii=False))


if __name__ == "__main__":
    main()
