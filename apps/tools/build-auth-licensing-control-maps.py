import csv
from pathlib import Path


MOD = Path(r"D:\AtelieProd\MOD")
OUT_AUTH = MOD / "docs" / "08-auth"
OUT_LIC = MOD / "docs" / "09-licensing"
OUT_DEP = MOD / "docs" / "06-dependencias"
OUT_NEXT = MOD / "docs" / "15-nextgen"
OUT_RISK = MOD / "docs" / "18-risk"

for d in [OUT_AUTH, OUT_LIC, OUT_DEP, OUT_NEXT, OUT_RISK]:
    d.mkdir(parents=True, exist_ok=True)


def write_csv(path, rows, fields):
    with path.open("w", newline="", encoding="utf-8-sig") as f:
        writer = csv.DictWriter(f, fieldnames=fields)
        writer.writeheader()
        writer.writerows(rows)


def read_csv(path):
    if not path.exists():
        return []
    with path.open("r", encoding="utf-8-sig", newline="") as f:
        return list(csv.DictReader(f))


signals = read_csv(OUT_LIC / "sinais-licenciamento-executaveis.csv")
tables = read_csv(OUT_LIC / "mapa-tabelas-licenciamento.csv")
endpoints = read_csv(OUT_LIC / "endpoints-licenciamento-autenticacao-atualizacao.csv")
imports = read_csv(OUT_LIC / "dependencias-auth-licensing-imports.csv")
runtime_net = read_csv(MOD / "docs" / "08-telemetria-protocolos" / "mapa-real-comunicacao.csv")
memory = read_csv(MOD / "docs" / "08-telemetria-protocolos" / "baseline-memoria-runtime.csv")

by_exe = {}
for row in signals:
    exe = row.get("Executavel", "")
    by_exe.setdefault(exe, {"licenciamento": 0, "auth": 0, "device": 0, "remote": 0, "update": 0})
    cat = row.get("Categoria", "")
    if cat == "licenciamento":
        by_exe[exe]["licenciamento"] += 1
    elif cat == "autenticacao-permissao":
        by_exe[exe]["auth"] += 1
    elif cat == "hardware-binding":
        by_exe[exe]["device"] += 1
    elif cat == "comunicacao-remota":
        by_exe[exe]["remote"] += 1
    elif cat == "atualizacao":
        by_exe[exe]["update"] += 1

runtime_comm = {row.get("Executavel", ""): row for row in runtime_net}
memory_by_exe = {}
for row in memory:
    memory_by_exe[row.get("Executavel", "")] = row

imports_by_exe = {}
for row in imports:
    exe = row.get("FileName", "")
    imports_by_exe.setdefault(exe, set()).add(row.get("ImportedDll", ""))

control_rows = []
for exe, counts in sorted(by_exe.items()):
    dlls = sorted(x for x in imports_by_exe.get(exe, set()) if x)
    comm = runtime_comm.get(exe)
    mem = memory_by_exe.get(exe, {})
    role = "secundario"
    if exe == "Gerenciador.exe":
        role = "broker remoto/admin forte"
    elif exe == "Senhas.exe":
        role = "controle de usuarios/permissoes/bloqueios"
    elif exe == "LavSoft.exe":
        role = "core operacional com bloqueio/licenca"
    elif exe in {"Financeiro.exe", "SAT.exe"}:
        role = "modulo operacional com regras de bloqueio"
    elif exe in {"LavFacilLan.exe", "Estoque.exe"}:
        role = "modulo operacional com comunicacao real observada"
    elif exe in {"EquEstruAtu.exe", "EquConfig.exe"}:
        role = "infraestrutura/configuracao/atualizacao"

    criticality = "media"
    score = counts["licenciamento"] * 3 + counts["auth"] * 2 + counts["device"] * 2 + counts["remote"] * 2 + counts["update"]
    if exe in {"Gerenciador.exe", "LavSoft.exe", "Senhas.exe"} or score >= 180:
        criticality = "critica"
    elif score >= 80 or comm:
        criticality = "alta"

    control_rows.append(
        {
            "Componente": exe,
            "PapelProvavel": role,
            "Criticidade": criticality,
            "SinaisLicenciamento": counts["licenciamento"],
            "SinaisAuthPermissao": counts["auth"],
            "SinaisDeviceBinding": counts["device"],
            "SinaisComunicacaoRemota": counts["remote"],
            "SinaisAtualizacao": counts["update"],
            "DLLsRelevantes": "; ".join(dlls),
            "ComunicacaoRuntime": f"{comm.get('RemoteAddress')}:{comm.get('RemotePort')}" if comm else "",
            "WorkingSetPicoMB": mem.get("WorkingSetPicoMB", ""),
            "ThreadsPico": mem.get("ThreadsPico", ""),
            "HandlesPico": mem.get("HandlesPico", ""),
            "Evidencia": "strings executaveis; imports PE; baselines runtime; mapas de comunicacao",
        }
    )

write_csv(
    OUT_LIC / "mapa-controle-auth-licensing-componentes.csv",
    control_rows,
    [
        "Componente",
        "PapelProvavel",
        "Criticidade",
        "SinaisLicenciamento",
        "SinaisAuthPermissao",
        "SinaisDeviceBinding",
        "SinaisComunicacaoRemota",
        "SinaisAtualizacao",
        "DLLsRelevantes",
        "ComunicacaoRuntime",
        "WorkingSetPicoMB",
        "ThreadsPico",
        "HandlesPico",
        "Evidencia",
    ],
)

session_rows = [
    {
        "Area": "Persistencia de usuario",
        "Evidencia": "Tabelas Usuarios, Senhas, Nivel, GruUsuarios no dicionario Paradox e amostras mascaradas",
        "Estado": "confirmado como persistencia local de autenticacao/permissao",
        "Risco": "senhas e permissoes acopladas ao banco legado",
        "ValidacaoPendente": "mapear telas e acoes de Senhas.exe com captura dinamica",
    },
    {
        "Area": "Sessao em memoria",
        "Evidencia": "Sinais estaticos de usuario/permissao em LavSoft, Financeiro, SAT e Senhas",
        "Estado": "provavel sessao local por usuario logado, sem token moderno identificado",
        "Risco": "baixa observabilidade e ausencia de trilha centralizada",
        "ValidacaoPendente": "tracing de login/logout e ProcMon para arquivos alterados durante login",
    },
    {
        "Area": "Autenticacao remota",
        "Evidencia": "Gerenciador.exe contem AutenticaGerenciador e TestaAutentica",
        "Estado": "forte candidato a validacao remota/admin",
        "Risco": "HTTP sem TLS aparente e dependencia de dominio externo",
        "ValidacaoPendente": "captura de payload HTTP em ambiente MOD",
    },
]
write_csv(OUT_AUTH / "mapa-sessoes-autenticacao.csv", session_rows, ["Area", "Evidencia", "Estado", "Risco", "ValidacaoPendente"])

device_rows = [
    {
        "Sinal": "Registrar.xml",
        "Tipo": "arquivo local",
        "Campos": "MacAddress; Nome; Usuario; VersaoWindows; CodLojaOriginal",
        "PapelProvavel": "identidade local da estacao",
        "Criticidade": "alta",
        "ValidacaoPendente": "verificar leitura/envio em RegistraEstacao",
    },
    {
        "Sinal": "EquNet.ini",
        "Tipo": "configuracao local",
        "Campos": "CampoCC; EquipeZ; TesteW1; Tested*; COMPUTADOR.NOME",
        "PapelProvavel": "parametros codificados e associacao loja/equipe/estacao",
        "Criticidade": "alta",
        "ValidacaoPendente": "ProcMon para leitura/escrita durante inicializacao",
    },
    {
        "Sinal": "RegistraEstacao/ListarDispositivosPorFilial",
        "Tipo": "endpoint remoto",
        "Campos": "payload ainda nao capturado",
        "PapelProvavel": "registro/listagem de dispositivos autorizados",
        "Criticidade": "critica",
        "ValidacaoPendente": "captura HTTP controlada",
    },
]
write_csv(OUT_LIC / "mapa-device-binding.csv", device_rows, ["Sinal", "Tipo", "Campos", "PapelProvavel", "Criticidade", "ValidacaoPendente"])

sync_rows = []
for row in endpoints:
    url = row.get("Url", "")
    area = "sincronizacao"
    if any(x in url for x in ["Autentica", "TestaAutentica"]):
        area = "autenticacao-remota"
    elif "RegistraEstacao" in url or "Dispositivos" in url:
        area = "device-management"
    elif "Atualiz" in url or "Download" in url:
        area = "atualizacao/download"
    elif "Nuvem" in url or "/ws/" in url or "Envia" in url or "Recebe" in url:
        area = "sincronizacao-nuvem"
    sync_rows.append(
        {
            "Executavel": row.get("Executavel", ""),
            "Endpoint": url,
            "AreaProvavel": area,
            "Protocolo": "HTTP" if url.startswith("http://") else "HTTPS" if url.startswith("https://") else "",
            "Criticidade": "critica" if area in {"autenticacao-remota", "device-management", "atualizacao/download"} else "alta",
            "PayloadCapturado": "nao",
            "Observacao": "endpoint extraido estaticamente; exige captura dinamica para confirmar contrato",
        }
    )
write_csv(OUT_LIC / "mapa-endpoints-apis-classificado.csv", sync_rows, ["Executavel", "Endpoint", "AreaProvavel", "Protocolo", "Criticidade", "PayloadCapturado", "Observacao"])

risk_rows = [
    {"Risco": "Dependencia remota HTTP sem TLS aparente", "Criticidade": "critica", "Evidencia": "endpoints http://lavsoft.com.br e runtime 191.6.218.152:80", "Mitigacao": "proxy/captura MOD, nova API propria com TLS, fallback offline"},
    {"Risco": "Licenciamento distribuido em varios modulos", "Criticidade": "alta", "Evidencia": "LavSoft, Financeiro, SAT, Senhas e Gerenciador com sinais de bloqueio/licenca", "Mitigacao": "mapear regras, criar servico Auth/Licensing MOD, substituir gradualmente"},
    {"Risco": "Device binding opaco", "Criticidade": "alta", "Evidencia": "Registrar.xml, EquNet.ini e endpoints de estacao/dispositivos", "Mitigacao": "novo device binding tolerante a troca parcial de hardware"},
    {"Risco": "Persistencia Paradox/BDE 32-bit", "Criticidade": "alta", "Evidencia": "ODBC 32-bit necessario e erro 9499 em NovoRegLavFilial", "Mitigacao": "extracao controlada, migracao para SQLite/Postgres/Supabase"},
    {"Risco": "Atualizacao remota acoplada", "Criticidade": "critica", "Evidencia": "VerificaAtualizacoes/DownloadDados e LiveUpdate legado", "Mitigacao": "manter update bloqueado no MOD, criar updater assinado futuro"},
]
write_csv(OUT_RISK / "riscos-auth-licensing-operacional.csv", risk_rows, ["Risco", "Criticidade", "Evidencia", "Mitigacao"])

print("Mapas gerados:")
print(OUT_LIC / "mapa-controle-auth-licensing-componentes.csv")
print(OUT_AUTH / "mapa-sessoes-autenticacao.csv")
print(OUT_LIC / "mapa-device-binding.csv")
print(OUT_LIC / "mapa-endpoints-apis-classificado.csv")
print(OUT_RISK / "riscos-auth-licensing-operacional.csv")
