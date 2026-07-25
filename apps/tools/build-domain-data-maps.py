import csv
import re
from collections import Counter, defaultdict
from datetime import datetime
from pathlib import Path


MOD = Path(r"D:\AtelieProd\MOD")
DB_DOC = MOD / "docs" / "10-database"
MOD_DOC = MOD / "docs" / "11-modulos"
NEXT_DOC = MOD / "docs" / "15-nextgen"
UI_DOC = MOD / "docs" / "11-modulos" / "ui-reverse-engineering"
LEGACY_DB = MOD / "docs" / "03-banco-de-dados"
AUTH_DOC = MOD / "docs" / "04-autenticacao-permissoes"

for d in (DB_DOC, MOD_DOC, NEXT_DOC):
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


def clean(value, limit=220):
    value = (value or "").replace("\x00", " ").strip()
    value = re.sub(r"\s+", " ", value)
    return value[:limit]


def has(text, pattern):
    return re.search(pattern, text or "", re.I) is not None


def classify_table(table, path=""):
    s = f"{table} {path}".lower()
    rules = [
        ("cliente", r"\bclientes?\b|clicont|clicontato|funcli|estcli|cliente|cadcli"),
        ("ordem_servico_rol", r"\bmovcab\b|movroupa|movcontrole|rol|orc?cab|orccab|orcitem|movproc|movproces|movtri"),
        ("produto_servico", r"\bprodut\b|cadpro|produto|servic|servi[cç]o|tecido|marca|cor|defeito|grupo"),
        ("estoque", r"prodest|movest|estoque|almox|saldo|entrada|baixa|invent"),
        ("financeiro", r"duplicat|caixa|conta|pag|receb|credito|fatur|nota|notas|notafis|boleto|cobr|recibo"),
        ("fiscal", r"nfe|sat|nota|cupom|fiscal|cf"),
        ("permissao_auth", r"usuarios|senhas|nivel|gruusua|usuasis|usuafil|usuagru"),
        ("auditoria_log", r"log|hist|anot|ocor|estat|aud"),
        ("config_admin", r"ini|param|config|filial|empresa|sistema|novoreg|cadmaq"),
    ]
    for name, pattern in rules:
        if re.search(pattern, s, re.I):
            return name
    return "outro"


def classify_column(col):
    c = (col or "").lower()
    rules = [
        ("id_codigo", r"^(cod|id|num|chave)|cod|codigo|c[oó]digo|numero|n[uú]mero|chave"),
        ("cliente_ref", r"codcli|cliente|nomcli|cpf|cnpj|rg|fone|tel|cel|email|cep|end|bairro|cidade|uf"),
        ("produto_ref", r"codpro|produto|despro|codbar|barra|marca|modelo|unid|fornec"),
        ("ordem_rol_ref", r"rol|os|orc|mov|numrol|codrol|peca|pe[cç]a"),
        ("valor_monetario", r"valor|vlr|preco|pre[cç]o|total|subtotal|desc|acresc|juros|multa|custo|saldo|credito|debito|pago|pagto"),
        ("quantidade", r"qtd|quant|qtde|peso|volume"),
        ("data_hora", r"data|dat|hora|dt|venc|emiss|cad|alt|fecha|abert"),
        ("status_situacao", r"status|situ|sit|cancel|ativo|bloq|entreg|pago|fech|baix|liber|aprov"),
        ("usuario_auditoria", r"usuario|usu|operador|atendente|tecnico|respons|vendedor|login"),
        ("texto_observacao", r"obs|observ|descr|desc|histor|hist|motivo|complemento|email|nome|razao"),
        ("fiscal_documento", r"nfe|nf|nota|serie|cfop|cst|icms|ipi|iss|sat|cupom|chave"),
    ]
    for name, pattern in rules:
        if re.search(pattern, c, re.I):
            return name
    return "outro"


def infer_visual_label(col):
    c = col or ""
    labels = [
        (r"codcli|cliente", "Cliente"),
        (r"nomcli|nome", "Nome"),
        (r"cpf", "CPF"),
        (r"cnpj", "CNPJ"),
        (r"fone|telefone|tel", "Telefone"),
        (r"cel", "Celular"),
        (r"email", "E-mail"),
        (r"end", "Endereco"),
        (r"cep", "CEP"),
        (r"bairro", "Bairro"),
        (r"cidade", "Cidade"),
        (r"uf|estado", "UF"),
        (r"rol", "ROL/OS"),
        (r"produto|despro", "Produto"),
        (r"qtd|quant", "Quantidade"),
        (r"valor|vlr|preco|total", "Valor"),
        (r"desc", "Desconto/Descricao"),
        (r"data|dat", "Data"),
        (r"hora", "Hora"),
        (r"status|situ", "Status"),
    ]
    for pat, label in labels:
        if re.search(pat, c, re.I):
            return label
    return c


def required_hint(col, data_type):
    if has(col, r"cod|id|num|chave|nome|valor|data|qtd|total"):
        return "provavel"
    return "desconhecido"


tables = read_csv(LEGACY_DB / "dicionario-paradox-tabelas.csv")
columns = read_csv(LEGACY_DB / "dicionario-paradox-colunas.csv")
ui_db = read_csv(UI_DOC / "mapa-ui-banco-interligacoes.csv")
ui_screens = read_csv(UI_DOC / "mapa-telas-funcional-consolidado.csv")
ui_menus = read_csv(UI_DOC / "mapa-menus-submenus-acoes-consolidado.csv")
perm_ui = read_csv(UI_DOC / "mapa-permissoes-ui-consolidado.csv")

table_meta = {r["RelativePath"]: r for r in tables}
table_domain = {}
for r in tables:
    table_domain[(r["TableName"], r["RelativePath"])] = classify_table(r["TableName"], r["RelativePath"])

full_dict_rows = []
for r in columns:
    domain = classify_table(r["TableName"], r["RelativePath"])
    col_class = classify_column(r["ColumnName"])
    full_dict_rows.append(
        {
            "Dominio": domain,
            "Tabela": r["TableName"],
            "Caminho": r["RelativePath"],
            "CampoBanco": r["ColumnName"],
            "CampoVisualProvavel": infer_visual_label(r["ColumnName"]),
            "ClasseCampo": col_class,
            "TipoDado": r.get("DataType", ""),
            "Tamanho": r.get("ColumnSize", ""),
            "ObrigatoriedadeInferida": required_hint(r["ColumnName"], r.get("DataType", "")),
            "NullableODBC": r.get("Nullable", ""),
            "OrigemInferida": "Paradox/BDE",
            "UsoProvavel": "identificador/vinculo" if col_class.endswith("_ref") or col_class == "id_codigo" else col_class,
            "ValidacaoPendente": "validar em tela, ProcMon e amostra de dados",
        }
    )

write_csv(
    DB_DOC / "dicionario-de-dados-completo.csv",
    full_dict_rows,
    ["Dominio", "Tabela", "Caminho", "CampoBanco", "CampoVisualProvavel", "ClasseCampo", "TipoDado", "Tamanho", "ObrigatoriedadeInferida", "NullableODBC", "OrigemInferida", "UsoProvavel", "ValidacaoPendente"],
)

domain_tables = defaultdict(list)
for r in tables:
    domain_tables[classify_table(r["TableName"], r["RelativePath"])].append(r)

entity_rows = []
for domain, rows in sorted(domain_tables.items()):
    for r in rows:
        cols = [c for c in columns if c["TableName"] == r["TableName"] and c["RelativePath"] == r["RelativePath"]]
        classes = Counter(classify_column(c["ColumnName"]) for c in cols)
        entity_rows.append(
            {
                "Dominio": domain,
                "Tabela": r["TableName"],
                "Caminho": r["RelativePath"],
                "TamanhoArquivo": r.get("Length", ""),
                "QtdColunas": r.get("ColumnCount", ""),
                "ClassesCampo": "; ".join(f"{k}:{v}" for k, v in classes.most_common(8)),
                "CamposPrincipais": "; ".join(c["ColumnName"] for c in cols[:18]),
                "Criticidade": "critica" if domain in {"cliente", "ordem_servico_rol", "produto_servico", "estoque", "financeiro"} else "alta" if domain in {"permissao_auth", "fiscal"} else "media",
            }
        )

write_csv(
    DB_DOC / "mapa-entidades-dominio.csv",
    entity_rows,
    ["Dominio", "Tabela", "Caminho", "TamanhoArquivo", "QtdColunas", "ClassesCampo", "CamposPrincipais", "Criticidade"],
)

relationship_rows = []
key_cols = defaultdict(list)
for r in columns:
    cls = classify_column(r["ColumnName"])
    if cls in {"cliente_ref", "produto_ref", "ordem_rol_ref", "valor_monetario", "status_situacao", "data_hora"}:
        key_cols[cls].append(r)

rel_specs = [
    ("cliente", "ordem_servico_rol", "cliente_ref", "cliente -> OS/ROL", "campos de cliente em tabelas de movimento/ROL"),
    ("cliente", "financeiro", "cliente_ref", "cliente -> financeiro", "campos de cliente em duplicatas/notas/caixa"),
    ("ordem_servico_rol", "produto_servico", "produto_ref", "OS/ROL -> produto/servico", "campos de produto em movimento/itens"),
    ("produto_servico", "estoque", "produto_ref", "produto -> estoque", "campos de produto em estoque/movimentacao"),
    ("ordem_servico_rol", "financeiro", "valor_monetario", "OS/ROL -> financeiro", "valores em movimentos/notas/duplicatas"),
    ("financeiro", "fiscal", "fiscal_documento", "financeiro -> fiscal", "notas, pagamentos, documento fiscal"),
]

for source_domain, target_domain, col_class, name, evidence in rel_specs:
    sources = [r for r in full_dict_rows if r["Dominio"] == source_domain]
    targets = [r for r in full_dict_rows if r["Dominio"] == target_domain and r["ClasseCampo"] == col_class]
    relationship_rows.append(
        {
            "Relacionamento": name,
            "DominioOrigem": source_domain,
            "DominioDestino": target_domain,
            "CampoChaveInferido": col_class,
            "TabelasOrigemCandidatas": "; ".join(sorted({r["Tabela"] for r in sources})[:20]),
            "TabelasDestinoCandidatas": "; ".join(sorted({r["Tabela"] for r in targets})[:20]),
            "Evidencia": evidence,
            "Confianca": "media",
            "ValidacaoPendente": "confirmar por dados reais, indices e navegacao UI",
        }
    )

write_csv(
    DB_DOC / "mapa-relacionamentos.csv",
    relationship_rows,
    ["Relacionamento", "DominioOrigem", "DominioDestino", "CampoChaveInferido", "TabelasOrigemCandidatas", "TabelasDestinoCandidatas", "Evidencia", "Confianca", "ValidacaoPendente"],
)

def table_domain_lookup(table):
    domains = [classify_table(r["TableName"], r["RelativePath"]) for r in tables if r["TableName"].lower() == (table or "").lower()]
    if domains:
        return Counter(domains).most_common(1)[0][0]
    return classify_table(table or "")

screen_db_rows = []
for row in ui_db:
    table = clean(row.get("TableName"))
    sql = clean(row.get("TrechoSQLouTexto"))
    if not table and not sql:
        continue
    screen_db_rows.append(
        {
            "Executavel": row.get("Executavel", ""),
            "Form": row.get("Form", ""),
            "DominioTabela": table_domain_lookup(table or sql),
            "TabelaOuSQL": table or sql,
            "Campo": row.get("FieldName", ""),
            "OperacaoInferida": row.get("OperacaoInferida", ""),
            "Confianca": row.get("Confianca", ""),
            "ValidacaoPendente": "captura dinamica tela + ProcMon/BDE",
        }
    )

write_csv(
    MOD_DOC / "mapa-telas-banco.csv",
    screen_db_rows,
    ["Executavel", "Form", "DominioTabela", "TabelaOuSQL", "Campo", "OperacaoInferida", "Confianca", "ValidacaoPendente"],
)

domain_flow_specs = [
    ("cliente", ["Clientes", "CliContato", "CliCont", "FunCli"], "Clientes cadastrados e referenciados por movimentos, financeiro e relatorios."),
    ("ordem_servico_rol", ["MovCab", "MovRoupaLot", "MovControle", "OrcCab", "MovProcItem"], "OS/ROL nasce em movimentos/cabecalhos e se ramifica para itens, status, producao e entrega."),
    ("produto_servico", ["Produt", "CadPro", "Servicos", "TiposServico"], "Produtos/servicos abastecem ROL, estoque, fiscal e financeiro."),
    ("estoque", ["ProdEst", "MovEst"], "Estoque registra saldo/movimento de produtos e impactos de entrada/baixa."),
    ("financeiro", ["Duplicat", "Caixa", "NotaFisPag", "Notas"], "Financeiro registra pagamentos, parcelas, caixa, notas e duplicatas."),
]

client_os_product_rows = []
for domain, table_names, desc in domain_flow_specs:
    matches = [r for r in entity_rows if r["Dominio"] == domain or r["Tabela"] in table_names]
    client_os_product_rows.append(
        {
            "Dominio": domain,
            "TabelasFortes": "; ".join(sorted({m["Tabela"] for m in matches})[:30]),
            "Descricao": desc,
            "CamposChaveCandidatos": "; ".join(sorted({r["CampoBanco"] for r in full_dict_rows if r["Dominio"] == domain and r["ClasseCampo"] in {"id_codigo", "cliente_ref", "produto_ref", "ordem_rol_ref"}})[:40]),
            "TelasMenusRelacionados": "; ".join(sorted({r.get("NomeVisualOuTexto", "") for r in ui_screens if domain.replace("_", "/") in r.get("ModuloDominio", "") or classify_table(r.get("NomeVisualOuTexto", "")) == domain})[:25]),
            "ValidacaoPendente": "amostra de registros e fluxo dinamico",
        }
    )

write_csv(
    MOD_DOC / "mapa-clientes-os-produtos.csv",
    client_os_product_rows,
    ["Dominio", "TabelasFortes", "Descricao", "CamposChaveCandidatos", "TelasMenusRelacionados", "ValidacaoPendente"],
)

flow_rows = [
    {
        "Fluxo": "Cadastro de cliente",
        "Inicio": "menu/tela de Clientes",
        "Meio": "preenchimento de identificacao, contato, endereco, observacoes",
        "Fim": "cliente disponivel para ROL/OS, financeiro e relatorios",
        "TabelasCandidatas": "Clientes; CliContato; CliCont; FunCli",
        "PermissoesCandidatas": "AlteraCliente1 e operacoes Cliente*",
        "RegrasPendentes": "obrigatoriedade CPF/CNPJ, duplicidade, status ativo/inativo",
    },
    {
        "Fluxo": "Criacao de OS/ROL",
        "Inicio": "Entrada/Lancamento de ROL",
        "Meio": "cliente, pecas/produtos/servicos, valores, prazo, observacoes",
        "Fim": "movimento aberto para producao, entrega, pagamento e relatorios",
        "TabelasCandidatas": "MovCab; MovRoupaLot; MovControle; MovProcItem",
        "PermissoesCandidatas": "operacoes ROL/Entrada/Lancamento/Pagamento/Cancelamento",
        "RegrasPendentes": "numero sequencial, status inicial, calculo de total, impressao",
    },
    {
        "Fluxo": "Pagamento/financeiro",
        "Inicio": "Pagamento de ROL ou caixa",
        "Meio": "forma de pagamento, desconto, credito, duplicata/nota",
        "Fim": "caixa/financeiro atualizado e relatorios disponiveis",
        "TabelasCandidatas": "Duplicat; Caixa; NotaFisPag; Notas; MovPdv",
        "PermissoesCandidatas": "Caixa, Pagamento, Fechamento, Desconto",
        "RegrasPendentes": "parcelamento, estorno, sangria, fechamento, bloqueio",
    },
    {
        "Fluxo": "Movimentacao de estoque",
        "Inicio": "Entrada/Baixa/Ajuste de estoque",
        "Meio": "produto, quantidade, origem, motivo, usuario",
        "Fim": "saldo atualizado e historico de movimento",
        "TabelasCandidatas": "Produt; ProdEst; MovEst",
        "PermissoesCandidatas": "Entrada no Estoque; Baixa Estoque; Atualiza Estoque",
        "RegrasPendentes": "baixa automatica por venda/OS, devolucao, estoque minimo",
    },
    {
        "Fluxo": "Relatorios",
        "Inicio": "menu Relatorios",
        "Meio": "filtros, periodo, agrupamento, ordenacao",
        "Fim": "preview/impressao/exportacao",
        "TabelasCandidatas": "MovCab; Clientes; Produt; Duplicat; Notas; MovEst",
        "PermissoesCandidatas": "operacoes Relatorio*",
        "RegrasPendentes": "totais, filtros obrigatorios, permissao por relatorio",
    },
]
write_csv(
    MOD_DOC / "mapa-fluxos-operacionais.csv",
    flow_rows,
    ["Fluxo", "Inicio", "Meio", "Fim", "TabelasCandidatas", "PermissoesCandidatas", "RegrasPendentes"],
)

rule_rows = []
for flow in flow_rows:
    rule_rows.append(
        {
            "Regra": flow["Fluxo"],
            "Descricao": flow["Meio"],
            "TabelasEnvolvidas": flow["TabelasCandidatas"],
            "Permissoes": flow["PermissoesCandidatas"],
            "Condicao": "a validar por execucao assistida",
            "Resultado": flow["Fim"],
            "ExcecoesPendentes": flow["RegrasPendentes"],
            "Criticidade": "critica",
        }
    )
write_csv(
    MOD_DOC / "matriz-regras-negocio.csv",
    rule_rows,
    ["Regra", "Descricao", "TabelasEnvolvidas", "Permissoes", "Condicao", "Resultado", "ExcecoesPendentes", "Criticidade"],
)

report_rows = []
for row in ui_screens:
    if row.get("ModuloDominio") == "relatorio/impressao" or has(row.get("NomeVisualOuTexto", ""), r"relat|impress|movimento|resumo|comissao|faturamento"):
        report_rows.append(
            {
                "Executavel": row.get("Executavel", ""),
                "RelatorioOuImpressao": clean(row.get("NomeVisualOuTexto")),
                "Dominio": classify_table(row.get("NomeVisualOuTexto", "")) if classify_table(row.get("NomeVisualOuTexto", "")) != "outro" else row.get("ModuloDominio", ""),
                "Fonte": "strings de UI",
                "TabelasCandidatas": "",
                "FiltrosProvaveis": "periodo; cliente; produto; status; filial; usuario",
                "ValidacaoPendente": "abrir relatorio e capturar SQL/tabelas",
            }
        )
write_csv(
    MOD_DOC / "matriz-relatorios.csv",
    report_rows,
    ["Executavel", "RelatorioOuImpressao", "Dominio", "Fonte", "TabelasCandidatas", "FiltrosProvaveis", "ValidacaoPendente"],
)

print("Dicionario campos:", len(full_dict_rows))
print("Entidades:", len(entity_rows))
print("Relacionamentos:", len(relationship_rows))
print("Tela-banco:", len(screen_db_rows))
print("Relatorios:", len(report_rows))
