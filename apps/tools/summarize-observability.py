import csv
import json
from pathlib import Path


ROOT = Path(r"D:\AtelieProd\MOD\logs\observability")
OUT = Path(r"D:\AtelieProd\MOD\docs\07-observabilidade")
OUT.mkdir(parents=True, exist_ok=True)


def read_json(path: Path):
    with path.open("r", encoding="utf-8-sig") as f:
        return json.load(f)


def mb(value):
    return round((value or 0) / 1024 / 1024, 2)


summaries = []
for path in sorted(ROOT.glob("*-summary-*.json")):
    data = read_json(path)
    summaries.append(data)

lines = [
    "# Relatorio de Observabilidade e Profiling",
    "",
    "Data: 2026-05-23",
    "",
    "Escopo: coletas dinamicas e snapshots gravados em `D:\\AtelieProd\\MOD\\logs\\observability`.",
    "",
]

if not summaries:
    lines.extend([
        "## Estado",
        "",
        "Ainda nao ha execucoes dinamicas de executaveis legados registradas.",
        "Existe snapshot de ambiente quando arquivos `snapshot-*.json` estiverem presentes.",
        "",
    ])
else:
    lines.extend(["## Execucoes monitoradas", ""])
    for data in summaries:
        exe = Path(data.get("Executable", "")).name
        lines.extend([
            f"### {exe}",
            "",
            f"- PID inicial: `{data.get('ProcessId')}`",
            f"- Duracao: `{round(data.get('DurationSeconds', 0), 2)}s`",
            f"- Saiu sozinho: `{data.get('Exited')}`",
            f"- Codigo de saida: `{data.get('ExitCode')}`",
            f"- Amostras: `{data.get('SampleCount')}`",
            f"- Pico Working Set: `{mb(data.get('PeakWorkingSetBytes'))} MB`",
            f"- Pico Private Memory: `{mb(data.get('PeakPrivateMemoryBytes'))} MB`",
            f"- Pico Threads: `{data.get('PeakThreadCount')}`",
            f"- Pico Handles: `{data.get('PeakHandleCount')}`",
            f"- Observacoes de processos filhos: `{data.get('ChildObservationCount')}`",
            f"- Observacoes de rede: `{data.get('NetworkObservationCount')}`",
            f"- Modulos/DLLs observados: `{data.get('ModuleCount')}`",
            "",
        ])

snapshot_count = len(list(ROOT.glob("snapshot-*.json")))
process_snapshot_count = len(list(ROOT.glob("process-snapshot-*.csv")))
network_snapshot_count = len(list(ROOT.glob("network-snapshot-*.csv")))

lines.extend([
    "## Snapshots de ambiente",
    "",
    f"- Snapshots JSON: `{snapshot_count}`",
    f"- Snapshots de processos: `{process_snapshot_count}`",
    f"- Snapshots de rede: `{network_snapshot_count}`",
    "",
    "## Limitacoes",
    "",
    "- A coleta dinamica deve ser feita somente no runtime MOD.",
    "- A abertura de telas exige acompanhamento visual para associar amostras a menus e acoes.",
    "- Dumps de memoria, ProcMon, Wireshark, Fiddler e debuggers devem ser tratados como etapas controladas separadas, com evidencia e rollback documentados.",
])

(OUT / "relatorio-observabilidade-profiling.md").write_text("\n".join(lines), encoding="utf-8")
print(OUT / "relatorio-observabilidade-profiling.md")
