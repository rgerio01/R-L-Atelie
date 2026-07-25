import csv
import hashlib
import os
import re
from datetime import datetime
from pathlib import Path


MOD_ROOT = Path(r"D:\AtelieProd\MOD")
LEGACY_ROOT = Path(r"D:\AtelieProd\Equipexe")
RUNTIME_ROOT = MOD_ROOT / "apps" / "legacy-runtime" / "Equipexe"
OUT_DIR = MOD_ROOT / "docs" / "09-licensing"
SCHEMA_COLUMNS = MOD_ROOT / "docs" / "03-banco-de-dados" / "dicionario-paradox-colunas.csv"

OUT_DIR.mkdir(parents=True, exist_ok=True)

TERMS = re.compile(
    r"licen[cç]|license|licenciamento|serial|registro|registrar|register|novoreg|"
    r"ativa[cç][aã]o|activation|valid|valida[cç][aã]o|chave|senha|password|usuario|usu[aá]rio|"
    r"permiss|nivel|grupo|hardware|macaddress|codmaq|maquina|m[aá]quina|computador|"
    r"estacao|esta[cç][aã]o|vencimento|vencer|bloq|bloque|bloqueia|controla|"
    r"vers[aã]o|version|update|atualiza|wininet|url|http|https|191\.6\.218\.152|kinghost",
    re.IGNORECASE,
)


def classify(text: str) -> str:
    checks = [
        (r"NovoReg|Licen[cç]a|License|Licenciamento|Register|Registrar|Serial|Ativa[cç][aã]o|Activation|Valida[cç][aã]o|Vencimento|Bloq", "licenciamento"),
        (r"Senha|Password|Usuario|Usu[aá]rio|Permiss|Nivel|Grupo|Sess[aã]o|Session", "autenticacao-permissao"),
        (r"MacAddress|CodMaq|Maquina|M[aá]quina|Hardware|CPU|Mother|Disco|Volume|Computador|Estacao|Esta[cç][aã]o", "hardware-binding"),
        (r"http|https|ftp|WinINet|InternetOpen|URLDownload|Socket|Connect|191\.6\.218\.152|kinghost", "comunicacao-remota"),
        (r"Update|Atualiza|LiveUpdate|Vers[aã]o|Version", "atualizacao"),
    ]
    for pattern, name in checks:
        if re.search(pattern, text, re.IGNORECASE):
            return name
    return "sinal-relacionado"


def file_sha256(path: Path) -> str:
    h = hashlib.sha256()
    with path.open("rb") as f:
        for chunk in iter(lambda: f.read(1024 * 1024), b""):
            h.update(chunk)
    return h.hexdigest().upper()


def ascii_strings(path: Path, min_len: int = 5):
    buf = bytearray()
    with path.open("rb") as f:
        while True:
            chunk = f.read(1024 * 1024)
            if not chunk:
                break
            for b in chunk:
                if 32 <= b <= 126:
                    buf.append(b)
                    if len(buf) > 512:
                        try:
                            yield bytes(buf).decode("latin-1")
                        except UnicodeDecodeError:
                            pass
                        buf.clear()
                else:
                    if len(buf) >= min_len:
                        try:
                            yield bytes(buf).decode("latin-1")
                        except UnicodeDecodeError:
                            pass
                    buf.clear()
    if len(buf) >= min_len:
        try:
            yield bytes(buf).decode("latin-1")
        except UnicodeDecodeError:
            pass


def write_csv(path: Path, rows, fieldnames):
    with path.open("w", newline="", encoding="utf-8-sig") as f:
        writer = csv.DictWriter(f, fieldnames=fieldnames)
        writer.writeheader()
        writer.writerows(rows)


def main():
    exe_names = [
        "LavSoft.exe",
        "LavFacilLan.exe",
        "Gerenciador.exe",
        "Financeiro.exe",
        "Estoque.exe",
        "NFE.exe",
        "SAT.exe",
        "Senhas.exe",
        "EquConfig.exe",
        "EquEstruAtu.exe",
    ]
    executable_rows = []
    seen = set()
    for exe_name in exe_names:
        candidates = [RUNTIME_ROOT / "Exe" / exe_name, LEGACY_ROOT / "Exe" / exe_name]
        exe_path = next((p for p in candidates if p.exists()), None)
        if not exe_path:
            continue

        stat = exe_path.stat()
        exe_hash = ''
        count = 0
        for s in ascii_strings(exe_path):
            if len(s) > 240 or not TERMS.search(s):
                continue
            key = (exe_name, s)
            if key in seen:
                continue
            seen.add(key)
            executable_rows.append(
                {
                    "Executavel": exe_name,
                    "Categoria": classify(s),
                    "Texto": s,
                    "Origem": str(exe_path),
                    "TamanhoArquivo": stat.st_size,
                    "SHA256Arquivo": exe_hash,
                    "UltimaAlteracao": datetime.fromtimestamp(stat.st_mtime).isoformat(timespec="seconds"),
                }
            )
            count += 1
            if count >= 250:
                break

    write_csv(
        OUT_DIR / "sinais-licenciamento-executaveis.csv",
        executable_rows,
        ["Executavel", "Categoria", "Texto", "Origem", "TamanhoArquivo", "SHA256Arquivo", "UltimaAlteracao"],
    )

    schema_rows = []
    if SCHEMA_COLUMNS.exists():
        with SCHEMA_COLUMNS.open("r", encoding="utf-8-sig", newline="") as f:
            for row in csv.DictReader(f):
                table = row.get("TableName", "")
                column = row.get("ColumnName", "")
                text = f"{table}.{column}"
                if not TERMS.search(text):
                    continue
                obs = "requer validacao funcional"
                if re.search(r"NovoReg", table, re.IGNORECASE):
                    obs = "forte candidato a registro/licenciamento"
                elif re.search(r"Licen[cç]a|NovoReg|Serial|Ativa|Venc|Bloq", column, re.IGNORECASE):
                    obs = "coluna candidata a regra de licenca/ativacao"
                schema_rows.append(
                    {
                        "Categoria": classify(text),
                        "TableName": table,
                        "ColumnName": column,
                        "RelativePath": row.get("RelativePath", ""),
                        "DataType": row.get("DataType", ""),
                        "ColumnSize": row.get("ColumnSize", ""),
                        "Observacao": obs,
                    }
                )

    write_csv(
        OUT_DIR / "mapa-tabelas-licenciamento.csv",
        sorted(schema_rows, key=lambda r: (r["Categoria"], r["TableName"], r["ColumnName"])),
        ["Categoria", "TableName", "ColumnName", "RelativePath", "DataType", "ColumnSize", "Observacao"],
    )

    config_rows = []
    config_names = {
        "equnet.ini",
        "equsenha.ini",
        "registrar.xml",
        "update-policy.json",
        "liveupdate.runtimeconfig.json",
    }
    config_scan_roots = [
        LEGACY_ROOT,
        LEGACY_ROOT / "Exe",
        RUNTIME_ROOT,
        RUNTIME_ROOT / "Exe",
    ]
    for root in config_scan_roots:
        if not root.exists():
            continue
        for path in root.glob("*"):
            if not path.is_file() or path.name.lower() not in config_names:
                continue
            if path.stat().st_size > 1024 * 1024:
                continue
            try:
                lines = path.read_text(encoding="utf-8", errors="ignore").splitlines()
            except OSError as exc:
                config_rows.append(
                    {
                        "Arquivo": str(path),
                        "Linha": "",
                        "Categoria": "erro",
                        "Texto": str(exc),
                        "TamanhoArquivo": path.stat().st_size,
                        "UltimaAlteracao": datetime.fromtimestamp(path.stat().st_mtime).isoformat(timespec="seconds"),
                    }
                )
                continue
            for idx, line in enumerate(lines, 1):
                if TERMS.search(line):
                    config_rows.append(
                        {
                            "Arquivo": str(path),
                            "Linha": idx,
                            "Categoria": classify(line),
                            "Texto": line,
                            "TamanhoArquivo": path.stat().st_size,
                            "UltimaAlteracao": datetime.fromtimestamp(path.stat().st_mtime).isoformat(timespec="seconds"),
                        }
                    )

    write_csv(
        OUT_DIR / "sinais-licenciamento-configs.csv",
        sorted(config_rows, key=lambda r: (r["Arquivo"], int(r["Linha"] or 0))),
        ["Arquivo", "Linha", "Categoria", "Texto", "TamanhoArquivo", "UltimaAlteracao"],
    )

    summary = [
        {
            "GeradoEm": datetime.now().isoformat(timespec="seconds"),
            "SinaisExecutaveis": len(executable_rows),
            "SinaisSchema": len(schema_rows),
            "SinaisConfigs": len(config_rows),
            "ExecutaveisComSinais": "; ".join(sorted({r["Executavel"] for r in executable_rows})),
            "TabelasFortes": "; ".join(sorted({r["TableName"] for r in schema_rows if "forte candidato" in r["Observacao"]})),
        }
    ]
    write_csv(
        OUT_DIR / "resumo-licenciamento-profundo.csv",
        summary,
        ["GeradoEm", "SinaisExecutaveis", "SinaisSchema", "SinaisConfigs", "ExecutaveisComSinais", "TabelasFortes"],
    )

    print(f"Sinais executaveis: {len(executable_rows)}")
    print(f"Sinais schema: {len(schema_rows)}")
    print(f"Sinais configs: {len(config_rows)}")
    print(f"Saida: {OUT_DIR}")


if __name__ == "__main__":
    main()
