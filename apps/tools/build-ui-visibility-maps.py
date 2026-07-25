import csv
import os
import re
from collections import Counter, defaultdict
from datetime import datetime
from pathlib import Path


MOD = Path(r"D:\AtelieProd\MOD")
LEGACY = Path(r"D:\AtelieProd\Equipexe")
ARCH = MOD / "docs" / "02-arquitetura-legada"
AUTH = MOD / "docs" / "04-autenticacao-permissoes"
OUT = MOD / "docs" / "11-modulos" / "ui-reverse-engineering"
NEXT = MOD / "docs" / "15-nextgen"

OUT.mkdir(parents=True, exist_ok=True)
NEXT.mkdir(parents=True, exist_ok=True)


def read_csv(path):
    if not path.exists() or path.stat().st_size == 0:
        return []
    with path.open("r", encoding="utf-8-sig", newline="") as f:
        return list(csv.DictReader(f))


def write_csv(path, rows, fields):
    with path.open("w", encoding="utf-8-sig", newline="") as f:
        writer = csv.DictWriter(f, fieldnames=fields)
        writer.writeheader()
        writer.writerows(rows)


def clean_text(value):
    value = (value or "").replace("\x00", " ").strip()
    value = re.sub(r"\s+", " ", value)
    return value[:240]


def classify_domain(text):
    t = text.lower()
    checks = [
        ("auth/permissao", ["senha", "usuario", "usuário", "permiss", "nivel", "grupo", "bloq", "desbloq"]),
        ("financeiro/caixa", ["caixa", "pagamento", "conta", "duplic", "credito", "cobran", "fatur", "recibo", "desconto"]),
        ("estoque/produto", ["estoque", "produto", "entrada", "baixa", "saldo", "invent", "almox"]),
        ("lavanderia/rol", ["rol", "peça", "peca", "lavagem", "entrega", "passadoria", "terceir", "localizacao"]),
        ("fiscal", ["nfe", "nf-e", "nota fiscal", "sat", "cf ", "cupom", "impressora fiscal"]),
        ("relatorio/impressao", ["relat", "impress", "etiqueta", "grafico", "gráfico"]),
        ("cadastro", ["cadastro", "cliente", "fornecedor", "marca", "cor", "servico", "serviço", "tabela"]),
        ("sync/update/api", ["sync", "sinc", "nuvem", "download", "upload", "atualiza", "http", "webservice"]),
    ]
    for name, terms in checks:
        if any(term in t for term in terms):
            return name
    return "operacional/outro"


def infer_action(text):
    t = text.lower()
    if any(x in t for x in ["inclui", "incluir", "novo", "entrada", "cadastrar", "cadastro"]):
        return "criar/cadastrar"
    if any(x in t for x in ["altera", "alterar", "editar", "modifica"]):
        return "editar/alterar"
    if any(x in t for x in ["exclui", "excluir", "apaga", "cancel"]):
        return "excluir/cancelar"
    if any(x in t for x in ["consulta", "procura", "pesquisa", "visualiza"]):
        return "consultar"
    if any(x in t for x in ["relat", "impress", "emitir", "etiqueta"]):
        return "relatorio/imprimir"
    if any(x in t for x in ["pagamento", "baixa", "fechamento", "encerramento"]):
        return "processar/fechar"
    if any(x in t for x in ["envia", "recebe", "download", "upload", "atualiza"]):
        return "sincronizar/atualizar"
    return "abrir/executar"


functional = read_csv(ARCH / "mapa-funcional-telas" / "mapa-funcional-executaveis.csv")
correlations = read_csv(ARCH / "mapa-funcional-telas" / "correlacao-menu-permissao-executavel.csv")
menu_perms = read_csv(ARCH / "menus-permissoes-niveldb.csv")
menu_strings = read_csv(ARCH / "menus-strings-executaveis.csv")
controls = read_csv(ARCH / "layouts-telas" / "controles-extraidos.csv")
db_links = read_csv(ARCH / "layouts-telas" / "vinculos-banco-telas.csv")
auth_perms = read_csv(AUTH / "mapa-permissoes-legado-nivel.csv")

screen_rows = []
seen_screen = set()
for row in functional:
    exe = row.get("Executavel", "")
    text = clean_text(row.get("Texto", ""))
    if not exe or len(text) < 3:
        continue
    domain = classify_domain(text)
    action = infer_action(text)
    key = (exe, domain, action, text.lower())
    if key in seen_screen:
        continue
    seen_screen.add(key)
    confidence = "media"
    if row.get("Categoria") in {"relatorio/impressao", "cadastro", "financeiro/caixa", "fiscal"}:
        confidence = "media-alta"
    screen_rows.append(
        {
            "Executavel": exe,
            "ModuloDominio": domain,
            "TipoAcao": action,
            "NomeVisualOuTexto": text,
            "CategoriaOrigem": row.get("Categoria", ""),
            "StatusLayout": row.get("StatusLayout", ""),
            "Fonte": "strings funcionais do executavel",
            "Confianca": confidence,
            "ValidacaoPendente": "captura visual no runtime MOD e cruzamento ProcMon",
        }
    )

write_csv(
    OUT / "mapa-telas-funcional-consolidado.csv",
    screen_rows,
    ["Executavel", "ModuloDominio", "TipoAcao", "NomeVisualOuTexto", "CategoriaOrigem", "StatusLayout", "Fonte", "Confianca", "ValidacaoPendente"],
)

menu_rows = []
seen_menu = set()
for row in correlations:
    text = clean_text(row.get("TextoEncontrado", ""))
    op = clean_text(row.get("OperacaoPermissao", ""))
    exe = row.get("Executavel", "")
    if not text and not op:
        continue
    display = text or op
    key = (exe, op.lower(), display.lower())
    if key in seen_menu:
        continue
    seen_menu.add(key)
    menu_rows.append(
        {
            "Sistema": row.get("Sistema", ""),
            "Executavel": exe,
            "MenuOuAcao": display,
            "OperacaoPermissao": op,
            "ModuloDominio": classify_domain(display + " " + op),
            "TipoAcao": infer_action(display + " " + op),
            "PermissaoOrigem": "Nivel.DB/correlacao",
            "Confianca": row.get("Confianca", "media"),
            "Observacao": row.get("Observacao", "correlacao por texto"),
        }
    )

for row in menu_strings:
    text = clean_text(row.get("Texto", ""))
    exe = row.get("Executavel", "")
    if not text:
        continue
    key = (exe, "", text.lower())
    if key in seen_menu:
        continue
    seen_menu.add(key)
    menu_rows.append(
        {
            "Sistema": "",
            "Executavel": exe,
            "MenuOuAcao": text,
            "OperacaoPermissao": "",
            "ModuloDominio": classify_domain(text),
            "TipoAcao": infer_action(text),
            "PermissaoOrigem": "strings executaveis",
            "Confianca": "baixa-media",
            "Observacao": "menu/caption extraido por string; exige validacao visual",
        }
    )

write_csv(
    OUT / "mapa-menus-submenus-acoes-consolidado.csv",
    menu_rows,
    ["Sistema", "Executavel", "MenuOuAcao", "OperacaoPermissao", "ModuloDominio", "TipoAcao", "PermissaoOrigem", "Confianca", "Observacao"],
)

perm_summary = defaultdict(lambda: {"usuarios": set(), "filiais": set(), "ops": set()})
for row in auth_perms:
    op = clean_text(row.get("Op", ""))
    if not op:
        continue
    item = perm_summary[op]
    if row.get("CodUsuario"):
        item["usuarios"].add(row["CodUsuario"])
    if row.get("CodFil"):
        item["filiais"].add(row["CodFil"])
    if row.get("CodSistema"):
        item["ops"].add(row["CodSistema"])

perm_rows = []
for op, data in sorted(perm_summary.items()):
    perm_rows.append(
        {
            "OperacaoPermissao": op,
            "Sistemas": "; ".join(sorted(data["ops"])),
            "UsuariosDistintos": len(data["usuarios"]),
            "FiliaisDistintas": len(data["filiais"]),
            "EscopoProvavel": classify_domain(op),
            "Observacao": "derivado de Nivel/permisoes legadas; niveis I/A/E/T precisam validacao semantica",
        }
    )

write_csv(
    OUT / "mapa-permissoes-ui-consolidado.csv",
    perm_rows,
    ["OperacaoPermissao", "Sistemas", "UsuariosDistintos", "FiliaisDistintas", "EscopoProvavel", "Observacao"],
)

control_rows = []
for row in controls:
    raw = clean_text(row.get("Raw", ""))
    tipo = row.get("Tipo", "")
    name = row.get("Nome", "")
    if not tipo and not raw:
        continue
    control_rows.append(
        {
            "Executavel": row.get("Executavel", ""),
            "Form": row.get("Form", ""),
            "TipoComponente": tipo,
            "Nome": name,
            "Caption": clean_text(row.get("Caption", "")),
            "DataSource": row.get("DataSource", ""),
            "DataSet": row.get("DataSet", ""),
            "FieldName": row.get("FieldName", ""),
            "TableName": row.get("TableName", ""),
            "OnClick": row.get("OnClick", ""),
            "OnKeyPress": row.get("OnKeyPress", ""),
            "PapelUXProvavel": classify_domain(raw + " " + name + " " + tipo),
            "Observacao": "extracao estatica TPF0/strings; posicao pode estar incompleta",
        }
    )

write_csv(
    OUT / "mapa-componentes-ui.csv",
    control_rows,
    ["Executavel", "Form", "TipoComponente", "Nome", "Caption", "DataSource", "DataSet", "FieldName", "TableName", "OnClick", "OnKeyPress", "PapelUXProvavel", "Observacao"],
)

layout_dir = ARCH / "layouts-telas"
layout_rows = []
for file in sorted(layout_dir.glob("*.tpf0.*.txt")):
    m = re.match(r"(.+)\.tpf0\.(\d+)\.txt$", file.name)
    if not m:
        continue
    text = file.read_text(encoding="utf-8", errors="ignore")
    types = Counter(re.findall(r"\bT[A-Za-z0-9_]+", text))
    layout_rows.append(
        {
            "Executavel": m.group(1),
            "ArquivoLayout": str(file),
            "Indice": m.group(2),
            "TamanhoBytes": file.stat().st_size,
            "ComponentesTPrefix": sum(types.values()),
            "TiposMaisFrequentes": "; ".join(f"{k}:{v}" for k, v in types.most_common(8)),
            "PossuiQuery": "sim" if "TQuery" in text or "SELECT " in text.upper() else "nao",
            "PossuiGrid": "sim" if "Grid" in text or "DBGrid" in text else "nao",
            "PossuiMenu": "sim" if "TMenuItem" in text or "MainMenu" in text else "nao",
            "PossuiRelatorio": "sim" if re.search(r"relat|quickrep|report", text, re.I) else "nao",
        }
    )

write_csv(
    OUT / "mapa-layouts-delphi-tpf0.csv",
    layout_rows,
    ["Executavel", "ArquivoLayout", "Indice", "TamanhoBytes", "ComponentesTPrefix", "TiposMaisFrequentes", "PossuiQuery", "PossuiGrid", "PossuiMenu", "PossuiRelatorio"],
)

db_rows = []
for row in db_links:
    text = clean_text(row.get("Texto", ""))
    table = clean_text(row.get("TableName", ""))
    if not table and not re.search(r"\b(select|from|join|insert|update|delete)\b", text, re.I):
        continue
    db_rows.append(
        {
            "Executavel": row.get("Executavel", ""),
            "Form": row.get("Form", ""),
            "TableName": table,
            "DatabaseName": row.get("DatabaseName", ""),
            "FieldName": row.get("FieldName", ""),
            "DataSource": row.get("DataSource", ""),
            "DataSet": row.get("DataSet", ""),
            "TrechoSQLouTexto": text,
            "OperacaoInferida": "consulta" if re.search(r"\b(select|from|join)\b", text, re.I) else "escrita/controle" if re.search(r"\b(insert|update|delete)\b", text, re.I) else "vinculo-dataset",
            "Confianca": "media" if table else "baixa-media",
        }
    )

write_csv(
    OUT / "mapa-ui-banco-interligacoes.csv",
    db_rows,
    ["Executavel", "Form", "TableName", "DatabaseName", "FieldName", "DataSource", "DataSet", "TrechoSQLouTexto", "OperacaoInferida", "Confianca"],
)

asset_rows = []
visual_ext = {".bmp", ".jpg", ".jpeg", ".png", ".ico", ".gif", ".cur", ".ani", ".res", ".wmf", ".emf"}
for root, _, files in os.walk(LEGACY):
    for name in files:
        p = Path(root) / name
        if p.suffix.lower() not in visual_ext:
            continue
        rel = str(p.relative_to(LEGACY))
        area = "logos/figuras" if "figuras" in rel.lower() else "icones/recursos" if p.suffix.lower() in {".ico", ".cur", ".ani"} else "recurso-visual"
        asset_rows.append(
            {
                "RelativePath": rel,
                "Extensao": p.suffix.lower(),
                "TamanhoBytes": p.stat().st_size,
                "UltimaAlteracao": datetime.fromtimestamp(p.stat().st_mtime).isoformat(timespec="seconds"),
                "AreaProvavel": area,
                "PossibilidadeSubstituicao": "alta, desde que caminho/configuracao seja preservado ou migrado",
            }
        )

write_csv(
    OUT / "mapa-assets-visuais.csv",
    asset_rows,
    ["RelativePath", "Extensao", "TamanhoBytes", "UltimaAlteracao", "AreaProvavel", "PossibilidadeSubstituicao"],
)

summary_rows = []
for exe in sorted({r["Executavel"] for r in screen_rows if r["Executavel"]}):
    screens = [r for r in screen_rows if r["Executavel"] == exe]
    menus = [r for r in menu_rows if r["Executavel"] == exe]
    layouts = [r for r in layout_rows if r["Executavel"] == exe]
    dbs = [r for r in db_rows if r["Executavel"] == exe]
    summary_rows.append(
        {
            "Executavel": exe,
            "TextosTelaAcao": len(screens),
            "MenusAcoes": len(menus),
            "LayoutsTPF0": len(layouts),
            "VinculosBancoSQL": len(dbs),
            "DominiosPrincipais": "; ".join(x for x, _ in Counter(r["ModuloDominio"] for r in screens).most_common(5)),
            "AcoesPrincipais": "; ".join(x for x, _ in Counter(r["TipoAcao"] for r in screens).most_common(5)),
            "ProximaValidacao": "captura dinamica tela-a-tela no runtime MOD",
        }
    )

write_csv(
    OUT / "resumo-ui-por-modulo.csv",
    summary_rows,
    ["Executavel", "TextosTelaAcao", "MenusAcoes", "LayoutsTPF0", "VinculosBancoSQL", "DominiosPrincipais", "AcoesPrincipais", "ProximaValidacao"],
)

print("Mapas UI gerados em", OUT)
print("Telas/acoes:", len(screen_rows))
print("Menus/acoes:", len(menu_rows))
print("Layouts TPF0:", len(layout_rows))
print("Vinculos banco:", len(db_rows))
print("Assets visuais:", len(asset_rows))
