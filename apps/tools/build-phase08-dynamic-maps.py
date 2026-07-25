import csv
import json
from collections import defaultdict
from pathlib import Path


ROOT = Path(r"D:\AtelieProd\MOD")
OBS = ROOT / "logs" / "observability"
OUT_DEP = ROOT / "docs" / "06-dependencias"
OUT_TEL = ROOT / "docs" / "08-telemetria-protocolos"
OUT_DEP.mkdir(parents=True, exist_ok=True)
OUT_TEL.mkdir(parents=True, exist_ok=True)


def read_csv(path):
    if not path.exists():
        return []
    with path.open("r", encoding="utf-8-sig", newline="") as f:
        return list(csv.DictReader(f))


def read_json(path):
    with path.open("r", encoding="utf-8-sig") as f:
        return json.load(f)


def write_csv(path, rows, fields):
    with path.open("w", encoding="utf-8-sig", newline="") as f:
        writer = csv.DictWriter(f, fieldnames=fields)
        writer.writeheader()
        writer.writerows(rows)


def mb(value):
    try:
        return round(int(value) / 1024 / 1024, 2)
    except Exception:
        return 0


def classify_module(module_name, file_name):
    text = f"{module_name} {file_name}".lower()
    if any(x in text for x in ["idapi32", "bde", "tutil32", "dbsrv32"]):
        return ("critica", "banco/BDE")
    if any(x in text for x in ["wininet", "wsock", "urlmon", "mswsock", "iertutil"]):
        return ("alta", "rede/protocolo")
    if any(x in text for x in ["bemafi", "mp20", "daruma", "bemasat", "sat", "general32"]):
        return ("critica", "fiscal/hardware")
    if any(x in text for x in ["winspool", "splwow64", "spoolss", "print"]):
        return ("alta", "impressao")
    if any(x in text for x in ["qtintf", "borlndmm", "midas", "olepro32"]):
        return ("alta", "runtime legado")
    if file_name.lower().startswith(str(ROOT).lower()):
        return ("media", "mod/runtime MOD")
    if "windows" in text:
        return ("media", "windows")
    return ("baixa", "outro")


summaries = []
for path in sorted(OBS.glob("*-summary-*.json")):
    data = read_json(path)
    exe = Path(data.get("Executable", "")).name
    if exe:
        data["_exe"] = exe
        data["_stem"] = Path(exe).stem
        data["_summary_path"] = str(path)
        summaries.append(data)

network_rows = []
dependency_rows = []
startup_rows = []
memory_rows = []

for summary in summaries:
    stem = summary["_stem"]
    exe = summary["_exe"]
    stamp = Path(summary["_summary_path"]).name.replace(f"{stem}-summary-", "").replace(".json", "")
    network_path = OBS / f"{stem}-network-{stamp}.csv"
    modules_path = OBS / f"{stem}-modules-{stamp}.csv"
    samples_path = OBS / f"{stem}-samples-{stamp}.csv"
    children_path = OBS / f"{stem}-children-{stamp}.csv"

    for row in read_csv(network_path):
        if row.get("RemoteAddress") and row.get("RemoteAddress") != "0.0.0.0":
            network_rows.append({
                "Executavel": exe,
                "ProcessId": row.get("ProcessId", ""),
                "ProcessName": row.get("ProcessName", ""),
                "LocalAddress": row.get("LocalAddress", ""),
                "LocalPort": row.get("LocalPort", ""),
                "RemoteAddress": row.get("RemoteAddress", ""),
                "RemotePort": row.get("RemotePort", ""),
                "State": row.get("State", ""),
                "Classificacao": "externa_confirmada" if row.get("RemoteAddress") == "191.6.218.152" else "avaliar",
                "Hipotese": "update/sync/licenca/telemetria_desconhecido" if row.get("RemoteAddress") == "191.6.218.152" else "desconhecido",
            })

    module_index = 0
    for row in read_csv(modules_path):
        module_index += 1
        crit, category = classify_module(row.get("ModuleName", ""), row.get("FileName", ""))
        dependency_rows.append({
            "Executavel": exe,
            "ProcessName": row.get("ProcessName", ""),
            "ModuleName": row.get("ModuleName", ""),
            "FileName": row.get("FileName", ""),
            "Criticidade": crit,
            "Categoria": category,
            "Observacao": "observado_em_runtime_MOD",
        })
        startup_rows.append({
            "Executavel": exe,
            "OrdemAproximada": module_index,
            "Evento": "modulo_carregado_observado",
            "Modulo": row.get("ModuleName", ""),
            "Arquivo": row.get("FileName", ""),
            "Limite": "ordem de enumeracao final; nao substitui trace ETW/ProcMon",
        })

    samples = read_csv(samples_path)
    if samples:
        first = samples[0]
        last = samples[-1]
        memory_rows.append({
            "Executavel": exe,
            "Amostras": len(samples),
            "WorkingSetInicialMB": mb(first.get("WorkingSetBytes", 0)),
            "WorkingSetFinalMB": mb(last.get("WorkingSetBytes", 0)),
            "WorkingSetPicoMB": mb(summary.get("PeakWorkingSetBytes", 0)),
            "PrivateMemoryPicoMB": mb(summary.get("PeakPrivateMemoryBytes", 0)),
            "ThreadsPico": summary.get("PeakThreadCount", 0),
            "HandlesPico": summary.get("PeakHandleCount", 0),
            "DuracaoSegundos": round(summary.get("DurationSeconds", 0), 2),
            "SaiuSozinho": summary.get("Exited", False),
            "CodigoSaida": summary.get("ExitCode", ""),
            "ObservacoesRede": summary.get("NetworkObservationCount", 0),
            "ObservacoesFilhos": summary.get("ChildObservationCount", 0),
        })

write_csv(
    OUT_TEL / "mapa-real-comunicacao.csv",
    network_rows,
    ["Executavel", "ProcessId", "ProcessName", "LocalAddress", "LocalPort", "RemoteAddress", "RemotePort", "State", "Classificacao", "Hipotese"],
)
write_csv(
    OUT_DEP / "mapa-dependencias-runtime.csv",
    dependency_rows,
    ["Executavel", "ProcessName", "ModuleName", "FileName", "Criticidade", "Categoria", "Observacao"],
)
write_csv(
    OUT_TEL / "mapa-inicializacao-runtime.csv",
    startup_rows,
    ["Executavel", "OrdemAproximada", "Evento", "Modulo", "Arquivo", "Limite"],
)
write_csv(
    OUT_TEL / "baseline-memoria-runtime.csv",
    memory_rows,
    ["Executavel", "Amostras", "WorkingSetInicialMB", "WorkingSetFinalMB", "WorkingSetPicoMB", "PrivateMemoryPicoMB", "ThreadsPico", "HandlesPico", "DuracaoSegundos", "SaiuSozinho", "CodigoSaida", "ObservacoesRede", "ObservacoesFilhos"],
)

by_exe_network = defaultdict(list)
for row in network_rows:
    by_exe_network[row["Executavel"]].append(row)

by_crit = defaultdict(int)
by_cat = defaultdict(int)
for row in dependency_rows:
    by_crit[row["Criticidade"]] += 1
    by_cat[row["Categoria"]] += 1

lines = [
    "# Fase 08 - Telemetria, Protocolos e Dependencias Reais",
    "",
    "Data: 2026-05-23",
    "",
    "## Escopo",
    "",
    "Consolidacao das evidencias dinamicas existentes no runtime MOD.",
    "",
    "## Arquivos gerados",
    "",
    f"- `{OUT_TEL / 'mapa-real-comunicacao.csv'}`",
    f"- `{OUT_DEP / 'mapa-dependencias-runtime.csv'}`",
    f"- `{OUT_TEL / 'mapa-inicializacao-runtime.csv'}`",
    f"- `{OUT_TEL / 'baseline-memoria-runtime.csv'}`",
    "",
    "## Comunicacao real",
    "",
]

if network_rows:
    for exe, rows in sorted(by_exe_network.items()):
        lines.append(f"### {exe}")
        lines.append("")
        for row in rows:
            lines.append(f"- `{row['LocalAddress']}:{row['LocalPort']} -> {row['RemoteAddress']}:{row['RemotePort']}` estado `{row['State']}`")
        lines.append("")
else:
    lines.append("Nenhuma comunicacao externa foi consolidada a partir dos logs atuais.")
    lines.append("")

lines.extend([
    "## Endpoint externo confirmado",
    "",
    "- IP: `191.6.218.152`",
    "- Porta: `80`",
    "- Reverse DNS observado: `web22f62.kinghost.net`",
    "- Teste manual HTTP HEAD: resposta `403 Forbidden`",
    "- Classificacao: dependencia externa real ainda sem finalidade identificada",
    "",
    "## Dependencias por criticidade",
    "",
])

for key, count in sorted(by_crit.items()):
    lines.append(f"- {key}: {count}")

lines.extend(["", "## Dependencias por categoria", ""])
for key, count in sorted(by_cat.items()):
    lines.append(f"- {key}: {count}")

lines.extend([
    "",
    "## Hipotese de Core Engine",
    "",
    "Com base nos baselines atuais:",
    "",
    "- `LavFacilLan.exe` aparenta ser um nucleo operacional forte: carrega BDE, fiscal/hardware, WinINet/Winsock e realiza comunicacao externa.",
    "- `LavSoft.exe` e nucleo operacional classico, com impressao e dependencias fiscais, mas nesta janela nao abriu rede.",
    "- `Estoque.exe` possui dependencia real de rede e BDE, devendo ser tratado como modulo com integracao externa.",
    "- `Gerenciador.exe` aparenta componente administrativo/.NET, mas ainda precisa de analise ILSpy/dnSpy.",
    "- `Financeiro.exe` iniciou leve e sem rede observada, mas precisa de fluxo funcional com telas.",
    "",
    "## Limitacoes",
    "",
    "- O mapa de inicializacao usa enumeracao de modulos ao final da coleta, nao a ordem exata de LoadLibrary.",
    "- Para ordem exata sera necessario ProcMon, ETW ou instrumentacao API Monitor em sessao controlada.",
    "- O firewall de isolamento MOD nao foi aplicado por falta de elevacao administrativa.",
    "- Payload HTTP ainda nao foi capturado.",
    "",
    "## Proximas acoes",
    "",
    "1. Aplicar firewall MOD com elevacao administrativa e repetir baselines.",
    "2. Capturar payload/protocolo HTTP de `LavFacilLan` e `Estoque` em laboratorio.",
    "3. Analisar `Gerenciador.exe` como .NET com ILSpy/dnSpy.",
    "4. Executar ProcMon/ETW para ordem real de inicializacao.",
    "5. Executar fase fiscal separada para `NFE` e `SAT`.",
])

(OUT_TEL / "relatorio-fase08-telemetria-protocolos-dependencias.md").write_text("\n".join(lines), encoding="utf-8")
print(OUT_TEL / "relatorio-fase08-telemetria-protocolos-dependencias.md")
