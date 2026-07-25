import csv
import re
from collections import Counter, defaultdict
from pathlib import Path


MOD = Path(r"D:\AtelieProd\MOD")
DATA_ROOT = MOD / "data" / "original-readonly" / "Equipexe"
DB = MOD / "docs" / "10-database"
MODS = MOD / "docs" / "11-modulos"
NEXT = MOD / "docs" / "15-nextgen"
UI = MOD / "docs" / "11-modulos" / "ui-reverse-engineering"
LEGACY_DB = MOD / "docs" / "03-banco-de-dados"

for p in (DB, MODS, NEXT):
    p.mkdir(parents=True, exist_ok=True)


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


def clean(v, n=240):
    v = (v or "").replace("\x00", " ").strip()
    v = re.sub(r"\s+", " ", v)
    return v[:n]


def match(s, pat):
    return re.search(pat, s or "", re.I) is not None


def classify_entity(table, rel=""):
    s = f"{table} {rel}".lower()
    rules = [
        ("Clientes", r"\bclientes?\b|clicont|clicontato|clientesobs|funcli|gruclientes|estcli"),
        ("Movimentos/OS/ROL", r"\bmovcab\b|movroupa|movcontrole|movlocrol|cadlocrol|controleeti|indenrol|movproc|movproces|movtri|orccab|orc"),
        ("Financeiro", r"duplicat|caixa|bolet|credito|clcredito|titulos|titgru|movinicaixa|feccaixa|conta|receb|pag"),
        ("Produtos/Servicos", r"\bprodut\b|cadpro|servic|servi[cç]o|tiptec|tecido|marca|cores?|defeito|tabpre|preco"),
        ("Estoque", r"prodest|movest|estoque|tabprodest|invent|saldo|almox"),
        ("Notas/Fiscal", r"notas?$|notafis|notafispag|notasat|nfe|sat|cupom|fiscal"),
        ("Ocorrencias/Auditoria", r"ocor|anot|log|hist|estat|cancel|can$"),
        ("Usuarios/Permissoes", r"usuarios|senhas|nivel|gruusua|usuasis|usuafil|usuagru"),
        ("Configuracao/Admin", r"ini|param|config|filial|empresa|sistema|novoreg|cadmaq"),
    ]
    for name, pat in rules:
        if re.search(pat, s, re.I):
            return name
    return "Nao classificado"


def classify_field(col):
    c = col.lower()
    rules = [
        ("identificador", r"^(cod|id|num|seq|chave|rol)|cod|codigo|c[oó]digo|numero|n[uú]mero|seq|chave"),
        ("codigo externo", r"extern|integra|exporta|importa|original|pedido|transacao|barras|codbar"),
        ("nome/descricao", r"nome|nom|descr|des|titulo|razao|contato|cargo|setor"),
        ("documento", r"cpf|cnpj|cgc|rg|insest|insmun|documento|doc|ie|im"),
        ("telefone", r"fone|tel|cel|whats|ramal"),
        ("endereco", r"end|cep|bairro|bai|cidade|cid|uf|estado|compl|numero"),
        ("data", r"data|dat|dt|venc|ven|emi|emiss|cad|alt|lan|pag|can|fecha|abert|ent"),
        ("status", r"status|situ|sit|posicao|cancel|ativo|bloq|entreg|pago|fech|baix|liber|aprov|baixa"),
        ("valor financeiro", r"valor|val|vlr|preco|pre[cç]o|total|tot|subtotal|desc|acresc|juros|multa|custo|credito|debito|saldo|pago|pagto|iss|base"),
        ("quantidade", r"qtd|qde|qtde|quant|peso|volume|pecas|pe[cç]as"),
        ("produto", r"codpro|produto|proest|marca|modelo|unid|fornec|codest"),
        ("cliente", r"codcli|cliente|nomcli|grucli"),
        ("usuario", r"usuario|usu|operador|atendente|tecnico|respons|vendedor|codven"),
        ("pagamento", r"pag|fpg|cpg|parc|dup|boleto|vencto|venc"),
        ("observacao", r"obs|observ|histor|hist|motivo|coment|memo"),
        ("log", r"log|hor|hora|ocor|anot|aud"),
        ("sincronizacao", r"sync|sinc|exporta|importa|integra|seqexporta"),
        ("fiscal", r"nfe|nf|nota|serie|cfop|cst|icms|ipi|iss|sat|cupom|chavenfe"),
    ]
    for name, pat in rules:
        if re.search(pat, c, re.I):
            return name
    return "desconhecido"


def visual_label(col):
    c = col.lower()
    labels = [
        (r"codcli", "Codigo do cliente"),
        (r"nomcli", "Nome do cliente"),
        (r"cgc|cpf|cnpj", "Documento"),
        (r"end", "Endereco"),
        (r"cep", "CEP"),
        (r"tel|fone", "Telefone"),
        (r"cel", "Celular"),
        (r"email", "E-mail"),
        (r"rol", "ROL"),
        (r"numos", "Numero OS"),
        (r"codpro", "Produto"),
        (r"datven", "Vencimento"),
        (r"datpag", "Pagamento"),
        (r"valtot|total", "Total"),
        (r"valdup", "Valor duplicata"),
        (r"qde|qtd", "Quantidade"),
        (r"cancel", "Cancelado"),
        (r"codusuario", "Usuario"),
    ]
    for pat, label in labels:
        if re.search(pat, c, re.I):
            return label
    return col


def sidecars(relative_path):
    db_file = DATA_ROOT / relative_path
    stem = db_file.with_suffix("")
    parent = db_file.parent
    if not parent.exists():
        return []
    base = db_file.stem
    out = []
    for p in parent.glob(base + ".*"):
        if p.suffix.lower() != ".db":
            out.append(p.suffix.upper())
    return sorted(set(out))


tables = read_csv(LEGACY_DB / "dicionario-paradox-tabelas.csv")
cols = read_csv(LEGACY_DB / "dicionario-paradox-colunas.csv")
ui_db = read_csv(MODS / "mapa-telas-banco.csv")
ui_screens = read_csv(UI / "mapa-telas-funcional-consolidado.csv")
ui_menus = read_csv(UI / "mapa-menus-submenus-acoes-consolidado.csv")
ui_perms = read_csv(UI / "mapa-permissoes-ui-consolidado.csv")
reports = read_csv(MODS / "matriz-relatorios.csv")

table_to_cols = defaultdict(list)
for c in cols:
    table_to_cols[(c["TableName"], c["RelativePath"])].append(c)

ui_table_hits = Counter()
for r in ui_db:
    target = clean(r.get("TabelaOuSQL", ""))
    if target:
        ui_table_hits[target.lower()] += 1

menus_by_domain = defaultdict(list)
for r in ui_menus:
    menus_by_domain[r.get("ModuloDominio", "")].append(r)

field_rows = []
for c in cols:
    entity = classify_entity(c["TableName"], c["RelativePath"])
    fclass = classify_field(c["ColumnName"])
    table_key = c["TableName"].lower()
    ui_status = "confirmado por UI" if ui_table_hits[table_key] else "nao confirmado"
    index_files = sidecars(c["RelativePath"])
    pk_hint = "possivel" if c["Ordinal"] == "0" or classify_field(c["ColumnName"]) == "identificador" else "nao confirmado"
    fk_hint = "possivel" if fclass in {"cliente", "produto", "usuario", "pagamento", "fiscal"} or match(c["ColumnName"], r"cod|num|rol") else "nao confirmado"
    evidence = ["confirmado por schema: coluna existente no dicionario Paradox"]
    if index_files:
        evidence.append("confirmado por schema: arquivos de indice " + ",".join(index_files[:6]))
    if ui_status == "confirmado por UI":
        evidence.append("confirmado por UI: tabela aparece no mapa UI-banco")
    field_rows.append(
        {
            "Tabela": c["TableName"],
            "Caminho": c["RelativePath"],
            "Campo": c["ColumnName"],
            "Tipo": c.get("DataType", ""),
            "Tamanho": c.get("ColumnSize", ""),
            "Precisao": c.get("DecimalDigits", ""),
            "Obrigatoriedade": "nao confirmado" if c.get("Nullable", "") == "True" else "confirmado por schema",
            "IndiceParadoxDetectado": ";".join(index_files),
            "PossivelChavePrimaria": pk_hint,
            "PossivelChaveEstrangeira": fk_hint,
            "NomeVisualCorrespondente": visual_label(c["ColumnName"]),
            "ModuloRelacionado": entity,
            "TelaRelacionada": "ver mapa UI-banco" if ui_status == "confirmado por UI" else "nao confirmado",
            "EntidadeDominio": entity,
            "ClassificacaoCampo": fclass,
            "Evidencia": " | ".join(evidence),
            "NivelConfianca": "alta" if ui_status == "confirmado por UI" or index_files else "media",
            "Status": "confirmado por schema" + (" + UI" if ui_status == "confirmado por UI" else ""),
        }
    )

write_csv(
    DB / "dicionario-paradox-campo-a-campo.csv",
    field_rows,
    [
        "Tabela",
        "Caminho",
        "Campo",
        "Tipo",
        "Tamanho",
        "Precisao",
        "Obrigatoriedade",
        "IndiceParadoxDetectado",
        "PossivelChavePrimaria",
        "PossivelChaveEstrangeira",
        "NomeVisualCorrespondente",
        "ModuloRelacionado",
        "TelaRelacionada",
        "EntidadeDominio",
        "ClassificacaoCampo",
        "Evidencia",
        "NivelConfianca",
        "Status",
    ],
)

priority_entities = {
    "Clientes": ["Clientes", "CliContato", "ClientesObs", "FunCli", "FunCliRou", "GruClientes"],
    "MovCab / OS / ROL": ["MovCab", "MovLocRol", "CadLocRol", "ControleEti", "IndenRol", "MovControle"],
    "Duplicat / Financeiro": ["Duplicat", "Boletos", "DupBoleto", "CliCredito", "FecCaixa", "MovIniCaixa", "Titulos", "TitGru"],
    "Produt / Produtos": ["Produt", "CadPro"],
    "ProdEst / Estoque Produto": ["ProdEst", "TabProdEst", "ProdEstKit", "ProdEstPac"],
    "MovEst / Movimento Estoque": ["MovEst", "MovEstCan", "MovEstEnc", "MovEstLan"],
    "Notas": ["Notas", "NotasEsc", "NotaSat"],
    "NotaFisPag": ["NotaFisPag"],
    "SAT / Ocorrencias": ["MovSatCli", "MovSatFor", "MovSatInt", "MovSatCliOcor", "MovSatForOcor", "MovSatIntOcor", "NotaSat", "NotaSatCanc"],
    "Usuarios/Permissoes": ["Usuarios", "Senhas", "Nivel", "GruUsuarios"],
}


def matching_tables(names):
    out = []
    for t in tables:
        if any(t["TableName"].lower() == n.lower() or t["TableName"].lower().startswith(n.lower()) for n in names):
            out.append(t)
    return out


def fields_for_table_names(names):
    return [r for r in field_rows if any(r["Tabela"].lower() == n.lower() or r["Tabela"].lower().startswith(n.lower()) for n in names)]


entity_sections = []
entity_csv_rows = []
for entity, names in priority_entities.items():
    mt = matching_tables(names)
    fs = fields_for_table_names(names)
    class_counter = Counter(f["ClassificacaoCampo"] for f in fs)
    status_fields = [f"{f['Tabela']}.{f['Campo']}" for f in fs if f["ClassificacaoCampo"] == "status"][:30]
    value_fields = [f"{f['Tabela']}.{f['Campo']}" for f in fs if f["ClassificacaoCampo"] == "valor financeiro"][:30]
    date_fields = [f"{f['Tabela']}.{f['Campo']}" for f in fs if f["ClassificacaoCampo"] == "data"][:30]
    rel_fields = [f"{f['Tabela']}.{f['Campo']}" for f in fs if f["ClassificacaoCampo"] in {"cliente", "produto", "usuario", "pagamento", "fiscal", "identificador"}][:40]
    related_screens = [r.get("NomeVisualOuTexto", "") for r in ui_screens if classify_entity(r.get("NomeVisualOuTexto", "")) == classify_entity(names[0])][:15]
    related_menus = [r.get("MenuOuAcao", "") for r in ui_menus if classify_entity(r.get("MenuOuAcao", "")) == classify_entity(names[0])][:15]
    related_reports = [r.get("RelatorioOuImpressao", "") for r in reports if classify_entity(r.get("RelatorioOuImpressao", "")) == classify_entity(names[0])][:15]
    evidence = "confirmado por schema: tabelas/campos existentes"
    if related_screens or related_menus:
        evidence += " | confirmado por UI: menus/telas correlatos"
    confidence = "alta" if mt and (related_screens or related_menus) else "media"
    entity_csv_rows.append(
        {
            "Entidade": entity,
            "TabelasCandidatas": "; ".join(sorted({t["TableName"] for t in mt})),
            "CamposPrincipais": "; ".join(f"{f['Tabela']}.{f['Campo']}" for f in fs if f["ClassificacaoCampo"] == "identificador")[:1000],
            "CamposFinanceiros": "; ".join(value_fields),
            "CamposStatus": "; ".join(status_fields),
            "CamposData": "; ".join(date_fields),
            "CamposRelacionamento": "; ".join(rel_fields),
            "TelasRelacionadas": "; ".join(related_screens),
            "MenusRelacionados": "; ".join(related_menus),
            "RelatoriosRelacionados": "; ".join(related_reports),
            "RegrasNegocio": "a validar dinamicamente por fluxo operacional",
            "Evidencias": evidence,
            "NivelConfianca": confidence,
            "PendenciasValidacao": "amostra de registros; captura ProcMon; captura UI tela-a-tela; diff antes/depois",
        }
    )
    entity_sections.append(
        f"""## {entity}

Entidade: {entity}

Tabelas candidatas: {', '.join(sorted({t['TableName'] for t in mt})) or 'nao confirmado'}

Campos principais: {', '.join(f'{f['Tabela']}.{f['Campo']}' for f in fs if f['ClassificacaoCampo'] == 'identificador')[:1200] or 'nao confirmado'}

Campos financeiros: {', '.join(value_fields) or 'nao identificado'}

Campos de status: {', '.join(status_fields) or 'nao identificado'}

Campos de data: {', '.join(date_fields) or 'nao identificado'}

Campos de relacionamento: {', '.join(rel_fields) or 'nao identificado'}

Telas relacionadas: {', '.join(related_screens) or 'hipotese por nome/string; captura pendente'}

Menus relacionados: {', '.join(related_menus) or 'hipotese por nome/string; captura pendente'}

Relatorios relacionados: {', '.join(related_reports) or 'hipotese por nome/string; captura pendente'}

Regras de negocio: a validar dinamicamente por fluxo operacional.

Evidencias: {evidence}

Nivel de confianca: {confidence}

Pendencias de validacao: amostra de registros; captura ProcMon; captura UI tela-a-tela; diff antes/depois.
"""
    )

write_csv(
    DB / "classificacao-entidades-negocio.csv",
    entity_csv_rows,
    [
        "Entidade",
        "TabelasCandidatas",
        "CamposPrincipais",
        "CamposFinanceiros",
        "CamposStatus",
        "CamposData",
        "CamposRelacionamento",
        "TelasRelacionadas",
        "MenusRelacionados",
        "RelatoriosRelacionados",
        "RegrasNegocio",
        "Evidencias",
        "NivelConfianca",
        "PendenciasValidacao",
    ],
)

relations = [
    ("Clientes", "MovCab", "CodCli", "CodCli", "1:N", "campo CodCli existe em Clientes e MovCab; telas/menus indicam fluxo ROL/cliente", "alta", "confirmado por schema + UI"),
    ("Clientes", "Duplicat", "CodCli", "CodCli", "1:N", "campo CodCli existe em Clientes e Duplicat; Duplicat contem vencimento/baixa/valor", "alta", "confirmado por schema"),
    ("Clientes", "Notas", "CodCli", "CodCli", "1:N", "campo CodCli existe em Clientes e Notas", "alta", "confirmado por schema"),
    ("MovCab", "Notas", "NumNot", "NumNot", "N:1 ou 1:1", "campo NumNot existe em MovCab e Notas; papel exato pendente", "media", "hipotese por nome/string"),
    ("MovCab", "CliCredito", "ROL", "Rol", "1:N", "campo Rol em CliCredito e ROL em MovCab", "media-alta", "confirmado por schema"),
    ("ProdEst", "MovEst", "CodProEst", "CodProEst", "1:N", "campo CodProEst existe em ProdEst e MovEst", "alta", "confirmado por schema"),
    ("Produt", "MovCab/Itens", "CodPro", "CodPro", "1:N", "CodPro aparece em tabelas de controle/itens, mas item exato do ROL precisa validar", "media", "hipotese por nome/string"),
    ("Usuarios", "MovCab", "CodUsuario", "CodUsuario", "1:N", "CodUsuario aparece em Usuarios e MovCab", "media-alta", "confirmado por schema"),
    ("Usuarios", "MovEst", "CodUsuario", "CodUsuario", "1:N", "CodUsuario aparece em Usuarios e MovEst", "media-alta", "confirmado por schema"),
    ("Duplicat", "Boletos/DupBoleto", "NumDup/NumFat", "NumDup/NumFat", "1:N", "campos NumFat/NumDup aparecem em duplicata e boleto", "media", "confirmado por schema"),
    ("Notas", "NotaFisPag", "NumNot/NumNotFis", "NumNotFis", "1:N", "campos de nota aparecem em Notas e NotaFisPag, exato join pendente", "media", "hipotese por nome/string"),
    ("SAT", "NotaSatCanc", "NumNotSat", "NumNotSat", "1:0..1", "campo NumNotSat em NotaSat e NotaSatCanc", "alta", "confirmado por schema"),
]

rel_rows = []
for origin, dest, ofield, dfield, typ, evidence, conf, status in relations:
    rel_rows.append(
        {
            "Origem": origin,
            "Destino": dest,
            "CampoOrigem": ofield,
            "CampoDestino": dfield,
            "TipoRelacionamento": typ,
            "Evidencia": evidence,
            "NivelConfianca": conf,
            "Status": status,
            "ComoValidarDinamicamente": "abrir tela relacionada no MOD; rodar ProcMon; alterar dado de teste; comparar diff Paradox antes/depois",
        }
    )

write_csv(
    DB / "matriz-relacionamentos-com-evidencia.csv",
    rel_rows,
    ["Origem", "Destino", "CampoOrigem", "CampoDestino", "TipoRelacionamento", "Evidencia", "NivelConfianca", "Status", "ComoValidarDinamicamente"],
)

ui_bank_rows = []
for r in ui_db:
    table = clean(r.get("TabelaOuSQL", ""))
    ui_bank_rows.append(
        {
            "Tela": r.get("Form", "") or "nao identificado estaticamente",
            "Modulo": r.get("Executavel", ""),
            "EntidadePrincipal": classify_entity(table),
            "EntidadesSecundarias": "a validar",
            "TabelasLidas": table if r.get("OperacaoInferida") == "consulta" else "",
            "TabelasAlteradas": table if r.get("OperacaoInferida") != "consulta" else "nao confirmado",
            "Botoes": "nao confirmado estaticamente",
            "Acoes": r.get("OperacaoInferida", ""),
            "Permissoes": "ver mapa-permissoes-ui-consolidado",
            "Relatorios": "ver matriz-relatorios",
            "FluxoAnterior": "nao confirmado",
            "FluxoPosterior": "nao confirmado",
            "Evidencia": "confirmado por UI-banco estatico" if table else "hipotese por string SQL",
            "Pendencias": "captura dinamica tela/ProcMon para confirmar leitura/escrita",
        }
    )

write_csv(
    MODS / "matriz-ui-banco.csv",
    ui_bank_rows,
    ["Tela", "Modulo", "EntidadePrincipal", "EntidadesSecundarias", "TabelasLidas", "TabelasAlteradas", "Botoes", "Acoes", "Permissoes", "Relatorios", "FluxoAnterior", "FluxoPosterior", "Evidencia", "Pendencias"],
)

action_rows = []
for r in ui_menus:
    text = clean(r.get("MenuOuAcao", ""))
    if not text:
        continue
    action = "consulta/abre tela"
    if match(text, r"altera|editar|modifica"):
        action = "alteracao"
    elif match(text, r"inclui|novo|entrada|cadastro"):
        action = "inclusao"
    elif match(text, r"exclui|cancel|apaga"):
        action = "cancelamento/exclusao"
    elif match(text, r"pag|baixa|fecha|encerra"):
        action = "financeiro/fechamento"
    elif match(text, r"relat|impress|emit"):
        action = "relatorio/impressao"
    ent = classify_entity(text)
    action_rows.append(
        {
            "TelaOuMenu": text,
            "Modulo": r.get("Executavel", ""),
            "Acao": action,
            "EntidadeInferida": ent,
            "TabelaConsulta": "a validar",
            "TabelaInsere": "a validar" if action == "inclusao" else "",
            "TabelaAltera": "a validar" if action in {"alteracao", "financeiro/fechamento", "cancelamento/exclusao"} else "",
            "TabelaExclui": "a validar; preferir cancelamento logico",
            "CampoMuda": "a validar",
            "LogGerado": "a validar",
            "RegraNegocio": "a validar por fluxo dinamico",
            "PermissaoExigida": r.get("OperacaoPermissao", "") or "a validar",
            "Evidencia": r.get("PermissaoOrigem", "") or "strings de UI",
            "Status": "confirmado por UI" if r.get("PermissaoOrigem") else "hipotese por nome/string",
        }
    )

write_csv(
    MODS / "matriz-tela-acao-tabela.csv",
    action_rows,
    [
        "TelaOuMenu",
        "Modulo",
        "Acao",
        "EntidadeInferida",
        "TabelaConsulta",
        "TabelaInsere",
        "TabelaAltera",
        "TabelaExclui",
        "CampoMuda",
        "LogGerado",
        "RegraNegocio",
        "PermissaoExigida",
        "Evidencia",
        "Status",
    ],
)

status_rows = [r for r in field_rows if r["ClassificacaoCampo"] == "status"]
value_rows = [r for r in field_rows if r["ClassificacaoCampo"] == "valor financeiro"]
date_rows = [r for r in field_rows if r["ClassificacaoCampo"] == "data"]

write_csv(MODS / "mapa-status-operacionais.csv", status_rows, list(field_rows[0].keys()))
write_csv(MODS / "mapa-valores-calculos.csv", value_rows, list(field_rows[0].keys()))
write_csv(MODS / "mapa-datas-operacionais.csv", date_rows, list(field_rows[0].keys()))

md_header = """# Dicionario Paradox Campo a Campo

Data: 2026-05-23

Este documento resume o CSV completo `dicionario-paradox-campo-a-campo.csv`.

Regra de evidência:

- Confirmado por schema: tabela/campo existe no dicionario Paradox ou possui indice lateral detectado.
- Confirmado por UI: tabela/campo aparece em mapa UI-banco ou em texto de tela/menu.
- Confirmado por runtime: somente quando houver ProcMon/log/diff. Nesta rodada, nenhum relacionamento de dados foi promovido a runtime.
- Hipotese por nome/string: nome sugere papel, mas precisa validacao.
- Nao confirmado: sem evidencia suficiente.

"""
summary = Counter(r["EntidadeDominio"] for r in field_rows)
md = md_header + "\n## Resumo por entidade\n\n" + "\n".join(f"- {k}: {v} campos" for k, v in summary.most_common()) + "\n\n## Entidades prioritarias\n\n" + "\n".join(entity_sections)
write_md(DB / "dicionario-paradox-campo-a-campo.md", md)

write_md(
    DB / "classificacao-entidades-negocio.md",
    "# Classificacao de Entidades de Negocio\n\nData: 2026-05-23\n\n" + "\n".join(entity_sections),
)

rel_md = "# Matriz de Relacionamentos com Evidencia\n\nData: 2026-05-23\n\n"
for r in rel_rows:
    rel_md += f"""## {r['Origem']} -> {r['Destino']}

Origem: {r['Origem']}

Destino: {r['Destino']}

Campos: {r['CampoOrigem']} -> {r['CampoDestino']}

Tipo: {r['TipoRelacionamento']}

Evidencia: {r['Evidencia']}

Confianca: {r['NivelConfianca']}

Status: {r['Status']}

Como validar dinamicamente: {r['ComoValidarDinamicamente']}

"""
write_md(DB / "matriz-relacionamentos-com-evidencia.md", rel_md)

write_md(
    MODS / "mapa-fluxo-cliente-movimento-financeiro.md",
    """# Mapa de Fluxo Cliente -> Movimento/ROL -> Financeiro

Data: 2026-05-23

## Fluxo confirmado/hipotetico por evidência

1. Cliente nasce em `Clientes` (`CodCli`, `NomCli`, documento, endereco, contato).
2. ROL/movimento nasce em `MovCab`, com `ROL`, `CodCli`, datas, status/posicao, totais e usuario.
3. Pagamento/credito/nota se conectam por `CodCli`, `ROL`, `NumNot`, `NumFat` conforme tabela.
4. `Duplicat` representa duplicatas/recebiveis por cliente, vencimento, baixa e valor pago.
5. `Notas` e `NotaFisPag` representam fiscal/pagamento de nota.

## Status de evidência

- `Clientes.CodCli -> MovCab.CodCli`: confirmado por schema + UI.
- `Clientes.CodCli -> Duplicat.CodCli`: confirmado por schema.
- `MovCab.NumNot -> Notas.NumNot`: hipótese por nome/string, validar dinamicamente.
- `MovCab.ROL -> CliCredito.Rol`: confirmado por schema.

## Validação dinâmica

Criar cliente teste no MOD, capturar diff em `Clientes`; criar ROL teste, capturar diff em `MovCab`; registrar pagamento teste, capturar diff em `Duplicat`, `CliCredito`, `Notas` e `NotaFisPag`. Executar somente com snapshot e rollback.
""",
)

write_md(
    MODS / "mapa-fluxo-produto-estoque-financeiro.md",
    """# Mapa de Fluxo Produto -> Estoque -> Financeiro

Data: 2026-05-23

## Fluxo confirmado/hipotetico por evidência

1. Produto/servico operacional aparece em `Produt`.
2. Produto de estoque aparece em `ProdEst`.
3. Movimento de estoque aparece em `MovEst`, com `CodProEst`, `Qde`, `TipoES`, `ValUnit`, `ValTot`, `CodUsuario`.
4. Cancelamento/encerramento aparece em `MovEstCan` e `MovEstEnc`.
5. Relação estoque -> financeiro ainda não está confirmada diretamente; deve ser validada por fluxo de venda/OS/baixa.

## Status de evidência

- `ProdEst.CodProEst -> MovEst.CodProEst`: confirmado por schema.
- `Produt.CodPro -> itens do ROL`: hipótese por nome/string, validar por tela e diff.
- `MovEst.ValTot/ValUnit -> financeiro`: hipótese por nome/string, não confirmado.

## Validação dinâmica

Criar/alterar produto teste no MOD, capturar diff em `Produt`/`ProdEst`; lançar entrada/baixa controlada, capturar diff em `MovEst`; simular venda/ROL com produto, verificar se há baixa automatica e impacto financeiro.
""",
)

model_domain = "# Modelo de Dominio NextGen\n\nData: 2026-05-23\n\n"
model_domain += "O modelo deve preservar evidência e rastreabilidade. Cada entidade moderna precisa guardar `legacy_table`, `legacy_key`, `evidence_status` e `migration_batch_id`.\n\n"
for e in entity_csv_rows:
    model_domain += f"## {e['Entidade']}\n\nTabelas legadas: {e['TabelasCandidatas']}\n\nCampos relacionamento: {e['CamposRelacionamento']}\n\nStatus inicial: {e['Evidencias']}\n\n"
write_md(NEXT / "modelo-dominio-nextgen.md", model_domain)

write_md(
    NEXT / "modelo-banco-nextgen.md",
    """# Modelo Banco NextGen

Data: 2026-05-23

## Tabelas núcleo

- `customers`
- `customer_contacts`
- `service_orders`
- `service_order_items`
- `service_order_status_history`
- `products`
- `stock_items`
- `stock_movements`
- `receivables`
- `payments`
- `invoices`
- `fiscal_events`
- `users`
- `roles`
- `permissions`
- `audit_events`

## Campos de rastreabilidade obrigatorios

- `legacy_table`
- `legacy_path`
- `legacy_key`
- `legacy_hash`
- `evidence_status`
- `migration_batch_id`

## Regra

Nenhum relacionamento importado deve virar FK obrigatoria sem estar classificado ao menos como confirmado por schema. Relacionamentos por hipótese devem entrar em tabela de staging para validação.
""",
)

write_md(
    NEXT / "modelo-ux-nextgen.md",
    """# Modelo UX NextGen

Data: 2026-05-23

## Telas por entidade

- Clientes: lista, cadastro, contatos, historico de ROL, historico financeiro.
- ROL/OS: entrada, detalhe, itens, status/localizacao, entrega, pagamento, cancelamento.
- Produtos: cadastro, tabela de preco, uso em ROL.
- Estoque: produtos de estoque, entrada, baixa, ajuste, historico.
- Financeiro: duplicatas, pagamentos, caixa, creditos, notas.
- Fiscal/SAT/NFE: emissao, cancelamento, retorno, logs.
- Permissoes: usuarios, perfis, permissoes por tela/botao/relatorio.

## Regra de UX

Toda tela deve exibir seu estado de evidencia durante a fase de homologacao: dados confirmados, dados migrados, campos pendentes de validação e origem legada.
""",
)

print("Campo-a-campo:", len(field_rows))
print("Entidades prioritarias:", len(entity_csv_rows))
print("Relacionamentos:", len(rel_rows))
print("UI-banco:", len(ui_bank_rows))
print("Acoes:", len(action_rows))
print("Status fields:", len(status_rows))
print("Value fields:", len(value_rows))
