import csv
import re
from collections import Counter, defaultdict
from pathlib import Path


MOD = Path(r"D:\AtelieProd\MOD")
AUTH_LEGACY = MOD / "docs" / "04-autenticacao-permissoes"
AUTH = MOD / "docs" / "08-auth"
NEXT = MOD / "docs" / "15-nextgen"
SEC = MOD / "docs" / "17-seguranca"
PAY = MOD / "docs" / "13-cloud" / "pagamentos"

for d in [AUTH, NEXT, SEC, PAY]:
    d.mkdir(parents=True, exist_ok=True)


def read_csv(path):
    if not path.exists() or path.stat().st_size == 0:
        return []
    with path.open("r", encoding="utf-8-sig", newline="") as f:
        return list(csv.DictReader(f))


def write_csv(path, rows, fields):
    with path.open("w", encoding="utf-8-sig", newline="") as f:
        w = csv.DictWriter(f, fieldnames=fields)
        w.writeheader()
        w.writerows(rows)


def write_md(path, text):
    path.write_text(text, encoding="utf-8")


def clean(v):
    return re.sub(r"\s+", " ", (v or "").strip())


def classify_permission(op):
    s = op.lower()
    if any(x in s for x in ["usuario", "senha", "permiss", "param", "filial", "matriz", "licen", "bloq"]):
        return "administrativa", "critica"
    if any(x in s for x in ["caixa", "pagamento", "fatur", "duplic", "credito", "desconto", "fechamento", "cobranca", "financeiro"]):
        return "financeira", "critica" if any(x in s for x in ["cancel", "desconto", "fechamento", "baixa"]) else "alta"
    if any(x in s for x in ["nfe", "nota", "sat", "fiscal", "cf"]):
        return "fiscal", "critica"
    if any(x in s for x in ["estoque", "produto", "baixa", "entrada"]):
        return "estoque", "alta"
    if any(x in s for x in ["cancel", "exclui", "desbloq", "bloq"]):
        return "operacional", "critica"
    if any(x in s for x in ["relat", "listagem", "extrato", "resumo", "movimento"]):
        return "relatorio", "media"
    if any(x in s for x in ["rol", "entrega", "passadoria", "terceir", "localizacao", "lavagem"]):
        return "operacional", "alta"
    if any(x in s for x in ["cliente", "cadastro", "marca", "cor", "servico", "tabela"]):
        return "operacional", "media"
    return "operacional", "baixa"


def profile_from_legacy_group(group, user):
    g = (group or "").upper()
    u = (user or "").upper()
    if g.startswith("MASTE") or u == "GABRIELA":
        return "Administrador Geral"
    if g.startswith("OPERA"):
        return "Operador"
    return "Somente leitura"


users = read_csv(AUTH_LEGACY / "amostra-legado-Usuarios.csv")
legacy_perms = read_csv(AUTH_LEGACY / "mapa-permissoes-legado-nivel.csv")
ui_actions = read_csv(MOD / "docs" / "11-modulos" / "matriz-tela-acao-tabela.csv")

user_rows = []
for u in users:
    login = clean(u.get("CodUsuario"))
    group = clean(u.get("GruUsuario"))
    typ = clean(u.get("TipUsuario"))
    profile = profile_from_legacy_group(group, login)
    status = "nao confirmado"
    if clean(u.get("Cancelado")):
        status = "inativo confirmado por schema"
    elif typ:
        status = f"ativo/inativo nao confirmado; TipUsuario={typ}"
    user_rows.append(
        {
            "Login": login,
            "Nome": clean(u.get("NomUsuario")),
            "GrupoLegado": group,
            "TipUsuario": typ,
            "PerfilNovoSugerido": profile,
            "Status": status,
            "UltimoAcesso": "nao confirmado",
            "VinculoVendas": "por CodUsuario em MovCab/Notas, a validar",
            "VinculoCaixa": "por CodUsuario em FecCaixa/MovIniCaixa, a validar",
            "VinculoFinanceiro": "por CodUsuario em Duplicat/Titulos/Notas, a validar",
            "VinculoEstoque": "por CodUsuario em MovEst, a validar",
            "VinculoOS": "por CodUsuario em MovCab, a validar",
            "Evidencia": "confirmado por schema: Usuarios.DB",
        }
    )

write_csv(
    AUTH / "matriz-usuarios-legado.csv",
    user_rows,
    [
        "Login",
        "Nome",
        "GrupoLegado",
        "TipUsuario",
        "PerfilNovoSugerido",
        "Status",
        "UltimoAcesso",
        "VinculoVendas",
        "VinculoCaixa",
        "VinculoFinanceiro",
        "VinculoEstoque",
        "VinculoOS",
        "Evidencia",
    ],
)

perm_rows = []
for p in legacy_perms:
    op = clean(p.get("Op"))
    if not op:
        continue
    area, crit = classify_permission(op)
    perm_rows.append(
        {
            "CodUsuarioOuPerfilLegado": clean(p.get("CodUsuario")),
            "CodFilial": clean(p.get("CodFil")),
            "CodSistema": clean(p.get("CodSistema")),
            "OperacaoLegado": op,
            "NivelI": clean(p.get("NivelI")),
            "NivelA": clean(p.get("NivelA")),
            "NivelE": clean(p.get("NivelE")),
            "NivelT": clean(p.get("NivelT")),
            "Area": area,
            "Criticidade": crit,
            "PermissaoNovaSugerida": "",
            "StatusEvidencia": "confirmado por schema; semantica I/A/E/T pendente",
        }
    )

write_csv(
    AUTH / "matriz-permissoes-legado-classificada.csv",
    perm_rows,
    [
        "CodUsuarioOuPerfilLegado",
        "CodFilial",
        "CodSistema",
        "OperacaoLegado",
        "NivelI",
        "NivelA",
        "NivelE",
        "NivelT",
        "Area",
        "Criticidade",
        "PermissaoNovaSugerida",
        "StatusEvidencia",
    ],
)

action_matrix = []
for a in ui_actions:
    op = clean(a.get("PermissaoExigida"))
    area, crit = classify_permission(op + " " + clean(a.get("TelaOuMenu")))
    action_matrix.append(
        {
            "UsuarioOuPerfil": "a validar por Nivel/usuario",
            "PerfilNovoSugerido": "",
            "Tela": clean(a.get("TelaOuMenu")),
            "BotaoOuMenu": clean(a.get("TelaOuMenu")),
            "Acao": clean(a.get("Acao")),
            "Tabela": clean(a.get("TabelaConsulta") or a.get("TabelaAltera") or a.get("TabelaInsere")),
            "PermissaoLegado": op,
            "Area": area,
            "Criticidade": crit,
            "StatusEvidencia": clean(a.get("Status")),
        }
    )

write_csv(
    AUTH / "matriz-usuario-perfil-tela-botao-acao-tabela.csv",
    action_matrix,
    ["UsuarioOuPerfil", "PerfilNovoSugerido", "Tela", "BotaoOuMenu", "Acao", "Tabela", "PermissaoLegado", "Area", "Criticidade", "StatusEvidencia"],
)

profiles = {
    "Administrador Geral": ["*.*", "usuarios.gerenciar", "licencas.gerenciar", "devices.gerenciar", "pagamentos.configurar", "auditoria.visualizar"],
    "Administrador da Loja": ["clientes.*", "ordens.*", "vendas.*", "caixa.*", "estoque.visualizar", "relatorios.*", "usuarios.loja"],
    "Operador": ["clientes.visualizar", "clientes.criar", "ordens.criar", "ordens.editar", "ordens.entregar", "produtos.visualizar"],
    "Caixa": ["vendas.criar", "vendas.receber", "caixa.abrir", "caixa.fechar", "pagamentos.pix", "pagamentos.cartao", "pagamentos.dinheiro"],
    "Financeiro": ["financeiro.visualizar", "financeiro.editar", "financeiro.baixar_pagamento", "financeiro.estornar", "relatorios.financeiro"],
    "Estoque": ["produtos.visualizar", "produtos.editar", "estoque.visualizar", "estoque.ajustar", "estoque.movimentar"],
    "Atendimento": ["clientes.visualizar", "clientes.criar", "ordens.criar", "ordens.consultar", "relatorios.operacionais"],
    "Técnico": ["ordens.visualizar", "ordens.status", "ordens.observacao", "produtos.visualizar"],
    "Auditor": ["*.visualizar", "auditoria.visualizar", "relatorios.exportar"],
    "Somente leitura": ["clientes.visualizar", "ordens.visualizar", "produtos.visualizar", "relatorios.visualizar"],
}

profile_rows = []
for profile, perms in profiles.items():
    for perm in perms:
        module = perm.split(".")[0]
        profile_rows.append(
            {
                "Perfil": profile,
                "Modulo": module,
                "Permissao": perm,
                "PodeCancelar": "sim" if "cancel" in perm or profile in {"Administrador Geral", "Administrador da Loja"} else "nao",
                "PodeDarDesconto": "sim" if profile in {"Administrador Geral", "Administrador da Loja", "Financeiro", "Caixa"} else "nao",
                "PodeEditar": "sim" if any(x in perm for x in ["editar", "*", "gerenciar", "ajustar"]) else "nao",
                "PodeExportarRelatorio": "sim" if profile in {"Administrador Geral", "Administrador da Loja", "Financeiro", "Auditor"} else "nao",
                "Criticidade": "administrativa" if profile == "Administrador Geral" else "alta" if profile in {"Administrador da Loja", "Financeiro", "Caixa"} else "media",
            }
        )

write_csv(
    AUTH / "matriz-perfis-novo-sistema.csv",
    profile_rows,
    ["Perfil", "Modulo", "Permissao", "PodeCancelar", "PodeDarDesconto", "PodeEditar", "PodeExportarRelatorio", "Criticidade"],
)

summary = f"""# Analise de Usuarios e Permissoes

Data: 2026-05-23

## Artefatos

- `matriz-usuarios-legado.csv`
- `matriz-permissoes-legado-classificada.csv`
- `matriz-usuario-perfil-tela-botao-acao-tabela.csv`
- `matriz-perfis-novo-sistema.csv`

## Usuarios legados identificados

Total: {len(user_rows)}

Perfis/grupos observados:

{chr(10).join(f"- {k}: {v}" for k, v in Counter(r['GrupoLegado'] for r in user_rows).items())}

## Observacoes

- `GABRIELA` foi classificada como Administrador Geral por grupo legado `MASTE` e regra administrativa ja definida no projeto.
- Usuarios em grupo `OPERA` foram classificados inicialmente como Operador.
- `TipUsuario` possui valores como `S` e `U`, mas o significado exato ainda nao foi confirmado por UI/runtime.
- Permissoes `NivelI`, `NivelA`, `NivelE`, `NivelT` existem no schema, mas sua semantica precisa de validacao.

## Regra de evidencia

- Usuario: confirmado por schema em `Usuarios.DB`.
- Permissao: confirmado por schema em `Nivel.DB`.
- Tela/acao: confirmado por UI quando vindo de menu/string/permissao; runtime pendente.
"""
write_md(AUTH / "analise-usuarios-permissoes.md", summary)

print("Usuarios:", len(user_rows))
print("Permissoes legado:", len(perm_rows))
print("Acoes matriz:", len(action_matrix))
print("Permissoes perfis novos:", len(profile_rows))
