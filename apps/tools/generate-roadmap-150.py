from pathlib import Path
import csv

ROOT = Path(r"D:\AtelieProd\MOD")
OUT = ROOT / "docs" / "00-controle"
OUT.mkdir(parents=True, exist_ok=True)

phases = [
("Inventario raiz completo", "01-inventario", "Criar mapa estrutural completo do sistema legado e MOD.", "inventario", "media"),
("Hash e integridade", "01-inventario", "Criar baseline completo de integridade por SHA256.", "integridade", "alta"),
("Snapshot e rollback", "19-snapshots", "Definir rollback operacional seguro para analises e mudancas MOD.", "rollback", "alta"),
("Inventario de executaveis", "01-inventario", "Mapear EXEs, versoes, imports, runtimes e comportamento previsto.", "binarios", "alta"),
("Inventario de DLLs", "06-dependencias", "Mapear DLLs criticas, orfas, antigas, de terceiros e runtime.", "dependencias", "alta"),
("Identificacao de frameworks", "06-dependencias", "Identificar stack tecnologica, runtimes e bibliotecas.", "arquitetura", "media"),
("Engenharia reversa estatica", "02-runtime", "Entender arquitetura sem executar o sistema.", "reversing", "alta"),
("Engenharia reversa dinamica", "02-runtime", "Descobrir comportamento real em runtime MOD.", "runtime", "alta"),
("Mapa de inicializacao", "02-runtime", "Descobrir ordem real de boot e inicializacao.", "startup", "alta"),
("Baseline de memoria", "03-memoria", "Criar baseline operacional de RAM, CPU, handles e threads.", "memoria", "media"),
("Baseline de performance", "04-performance", "Medir startup time, CPU, I/O e responsividade.", "performance", "media"),
("Mapa de processos filhos", "02-runtime", "Identificar processos filhos e subprocessos auxiliares.", "runtime", "media"),
("Mapa de handles e GDI", "03-memoria", "Mapear handles, GDI e recursos Windows.", "memoria", "media"),
("Mapa de I/O e arquivos temporarios", "02-runtime", "Mapear arquivos temporarios, cache, locks e diretorios de trabalho.", "runtime", "alta"),
("Mapa de registry", "02-runtime", "Identificar chaves de registro acessadas pelo legado.", "runtime", "media"),
("Mapa de configuracoes", "01-inventario", "Classificar INI, XML, JSON, CFG e parametros operacionais.", "config", "media"),
("Mapa de logs legados", "07-observabilidade", "Localizar, classificar e interpretar logs existentes.", "logs", "media"),
("Mapa de banco Paradox/BDE", "10-database", "Mapear tabelas, campos, indices, locks e dependencias BDE.", "database", "critica"),
("Dicionario de dados refinado", "10-database", "Refinar dicionario de tabelas, dominios e campos criticos.", "database", "alta"),
("Relacionamentos e entidades", "10-database", "Inferir relacionamentos e entidades de negocio.", "database", "alta"),
("Integridade e corrupcao de dados", "10-database", "Validar duplicidades, indices, inconsistencias e corrupcao.", "database", "critica"),
("Mapa de permissoes legado", "08-auth", "Mapear usuarios, grupos, niveis e permissoes por acao.", "auth", "alta"),
("Fluxo de login legado", "08-auth", "Descobrir fluxo real de autenticacao e fallback.", "auth", "alta"),
("Persistencia de sessao", "08-auth", "Mapear cache, tokens, arquivos temporarios e expiração.", "auth", "alta"),
("Codificacao de senha", "08-auth", "Confirmar codificacao legada e plano de migracao segura.", "auth", "alta"),
("Licenciamento legado", "09-licensing", "Investigar ativacao, serial, validacao e dependencias remotas.", "licensing", "critica"),
("Hardware binding legado", "09-licensing", "Identificar uso de MAC, disco, CPU, placa e TPM.", "licensing", "alta"),
("Mapa de dispositivos", "09-licensing", "Planejar inventario e gerenciamento de dispositivos autorizados.", "licensing", "media"),
("Mapa de comunicacao externa", "05-comunicacoes", "Consolidar IPs, hosts, portas, protocolos e endpoints.", "comunicacao", "critica"),
("Captura de trafego HTTP", "05-comunicacoes", "Capturar payload, headers, retries e frequencia.", "comunicacao", "critica"),
("Comportamento offline", "05-comunicacoes", "Executar sistema isolado e medir falhas, timeouts e fallback.", "offline", "alta"),
("Comportamento online", "05-comunicacoes", "Executar sistema com rede e comparar com baseline offline.", "online", "alta"),
("Mapa de sincronizacao", "05-comunicacoes", "Descobrir filas, tabelas, endpoints e mecanismos de sync.", "sync", "critica"),
("Mapa de atualizacao automatica", "05-comunicacoes", "Mapear update legado e bloquear no MOD com rollback.", "update", "critica"),
("Classificacao de endpoint externo", "05-comunicacoes", "Classificar 191.6.218.152:80 e demais endpoints.", "comunicacao", "critica"),
("Tracing WinINet/Winsock", "05-comunicacoes", "Identificar chamadas, DLL, thread e modulo responsavel por rede.", "comunicacao", "alta"),
("ProcMon de inicializacao", "02-runtime", "Capturar ordem real de arquivos, registry e DLL loading.", "startup", "alta"),
("ETW de runtime", "07-observabilidade", "Capturar eventos de processo, rede, disco e CPU.", "observabilidade", "media"),
("API Monitor de rede", "05-comunicacoes", "Monitorar APIs de rede, timers e callbacks.", "comunicacao", "alta"),
("Mapa de modulos principais", "11-modulos", "Classificar LavSoft, LavFacilLan, Gerenciador, Financeiro, Estoque, NFE, SAT.", "modulos", "alta"),
("Core engine operacional", "11-modulos", "Identificar modulo central real do sistema.", "modulos", "critica"),
("Mapa LavFacilLan", "11-modulos", "Mapear responsabilidades, dependencias, telas e rede do LavFacilLan.", "modulos", "alta"),
("Mapa LavSoft", "11-modulos", "Mapear responsabilidades, dependencias, impressao e fluxo operacional do LavSoft.", "modulos", "alta"),
("Mapa Gerenciador", "11-modulos", "Analisar Gerenciador como modulo administrativo e possivel .NET.", "modulos", "alta"),
("Mapa Financeiro", "11-modulos", "Mapear fluxos financeiros, relatorios, banco e permissoes.", "modulos", "alta"),
("Mapa Estoque", "11-modulos", "Mapear estoque, comunicacao externa, banco e relatorios.", "modulos", "alta"),
("Mapa NFE", "11-modulos", "Mapear NFE em ambiente fiscal controlado.", "fiscal", "critica"),
("Mapa SAT", "11-modulos", "Mapear SAT e dependencias de hardware fiscal.", "fiscal", "critica"),
("Mapa fiscal consolidado", "11-modulos", "Consolidar NFE, SAT, Bematech, Daruma e impressao fiscal.", "fiscal", "critica"),
("Mapa de relatorios", "11-modulos", "Catalogar relatorios, parametros, fontes e saidas.", "relatorios", "alta"),
("Mapa de menus e telas", "11-modulos", "Validar menus, submenus, telas e posicao visual.", "frontend", "alta"),
("Captura visual de telas", "11-modulos", "Capturar screenshots e controles por tela no MOD.", "frontend", "media"),
("Mapa de botoes e acoes", "11-modulos", "Associar botoes, atalhos e eventos a permissoes.", "frontend", "alta"),
("Mapa de regras de negocio", "11-modulos", "Extrair regras de processos, validacoes e excecoes.", "negocio", "critica"),
("Fluxo Entrada de ROL", "11-modulos", "Mapear fluxo completo de entrada de ROL.", "negocio", "critica"),
("Fluxo Entrega", "11-modulos", "Mapear entrega, baixa e validacoes.", "negocio", "alta"),
("Fluxo Pagamento", "11-modulos", "Mapear pagamento, recibos, caixa e auditoria.", "negocio", "critica"),
("Fluxo Cancelamento", "11-modulos", "Mapear cancelamento, rollback e impactos financeiros.", "negocio", "critica"),
("Fluxo Reemissao", "11-modulos", "Mapear reemissao e trilhas de auditoria.", "negocio", "media"),
("Fluxo Caixa", "11-modulos", "Mapear abertura, fechamento, sangria e relatorios.", "financeiro", "critica"),
("Fluxo Faturamento", "11-modulos", "Mapear faturamento, cobranca e notas.", "financeiro", "alta"),
("Fluxo Cliente", "11-modulos", "Mapear cadastro, historico, creditos e bloqueios.", "cadastro", "alta"),
("Fluxo Usuario", "08-auth", "Mapear criacao, alteracao, bloqueio e perfis.", "auth", "alta"),
("Fluxo Parametros", "11-modulos", "Mapear parametros globais e impacto operacional.", "config", "alta"),
("Analise de SQL e queries", "10-database", "Extrair SQL, filtros, selects e relacoes.", "database", "alta"),
("Analise de indices", "10-database", "Mapear indices Paradox e impacto de performance.", "database", "alta"),
("Analise de locks BDE", "10-database", "Mapear PDOXUSRS, LCK, NET DIR e travas.", "database", "critica"),
("Plano de migracao de banco", "16-migracao", "Planejar migracao Paradox para SQLite/API moderna.", "migracao", "critica"),
("Modelo SQLite local", "15-nextgen", "Projetar schema local offline-first.", "nextgen", "alta"),
("Outbox local", "15-nextgen", "Projetar fila local de sincronizacao.", "sync", "alta"),
("Inbound sync", "15-nextgen", "Projetar recebimento incremental cloud/local.", "sync", "alta"),
("Resolucao de conflitos", "15-nextgen", "Projetar conflito por entidade e fluxo financeiro/fiscal.", "sync", "critica"),
("Modelo multi-tenant", "15-nextgen", "Definir tenant_id, company_id e branch_id.", "cloud", "alta"),
("Supabase auth", "14-supabase", "Planejar autenticacao futura com Supabase.", "supabase", "alta"),
("Supabase RLS", "14-supabase", "Projetar Row Level Security por tenant/filial.", "supabase", "critica"),
("Supabase Edge Functions", "14-supabase", "Planejar funcoes para licenca, device e sync.", "supabase", "alta"),
("Supabase auditoria", "14-supabase", "Projetar auditoria cloud e logs administrativos.", "supabase", "alta"),
("Feature flags", "13-cloud", "Projetar flags por tenant, filial, usuario e modulo.", "cloud", "media"),
("Device management", "13-cloud", "Projetar cadastro, ativacao, revogacao e auditoria de dispositivos.", "cloud", "alta"),
("Licenciamento moderno", "09-licensing", "Projetar licenca local/cloud com janela offline.", "licensing", "alta"),
("Device binding moderno", "09-licensing", "Projetar fingerprint tolerante a troca parcial de hardware.", "licensing", "alta"),
("Autenticacao local moderna", "08-auth", "Projetar login local, hash forte, sessoes e cache.", "auth", "alta"),
("Permissoes granulares", "08-auth", "Projetar permissoes por modulo, tela, botao, acao e API.", "auth", "alta"),
("Auditoria administrativa", "17-seguranca", "Registrar acoes administrativas e decisoes de permissao.", "seguranca", "alta"),
("Criptografia e segredos", "17-seguranca", "Planejar armazenamento seguro de segredos e tokens.", "seguranca", "critica"),
("Politica de senhas", "17-seguranca", "Definir politica moderna e migracao segura.", "seguranca", "media"),
("Hardening local", "17-seguranca", "Definir protecoes locais, ACLs, logs e integridade.", "seguranca", "alta"),
("Observabilidade estruturada", "07-observabilidade", "Projetar logs estruturados por modulo.", "observabilidade", "alta"),
("Tracing distribuido", "07-observabilidade", "Projetar correlation_id entre UI, API, sync e cloud.", "observabilidade", "media"),
("Metricas locais", "07-observabilidade", "Definir metricas de memoria, CPU, sync e performance.", "observabilidade", "media"),
("Crash dumps", "07-observabilidade", "Definir politica de dumps leves e completos.", "observabilidade", "media"),
("Health checks", "07-observabilidade", "Projetar checks locais e cloud.", "observabilidade", "media"),
("Dashboard administrativo", "13-cloud", "Planejar painel de dispositivos, sync, falhas e versoes.", "cloud", "media"),
("Updater modular", "15-nextgen", "Projetar atualizador separado, assinado e reversivel.", "update", "alta"),
("Rollback inteligente", "15-nextgen", "Projetar rollback de modulo, dados e configuracao.", "rollback", "critica"),
("Delta updates", "15-nextgen", "Planejar atualizacoes incrementais e canais.", "update", "media"),
("Assinatura digital", "17-seguranca", "Planejar assinatura de pacotes e verificacao de integridade.", "seguranca", "alta"),
("API local", "12-apis", "Projetar API local para desacoplar UI e negocio.", "apis", "alta"),
("API auth", "12-apis", "Definir endpoints de login, sessoes e usuario.", "apis", "alta"),
("API permissoes", "12-apis", "Definir endpoints de perfis e decisoes de acesso.", "apis", "media"),
("API financeiro", "12-apis", "Definir contratos financeiros e auditoria.", "apis", "alta"),
("API operacional", "12-apis", "Definir contratos de ROL, entrega e pagamento.", "apis", "alta"),
("API fiscal", "12-apis", "Definir camada fiscal isolada e testavel.", "apis", "critica"),
("API relatorios", "12-apis", "Definir catalogo e execucao de relatorios.", "apis", "media"),
("Frontend moderno", "15-nextgen", "Planejar UI moderna preservando experiencia operacional.", "frontend", "media"),
("Design de telas operacionais", "15-nextgen", "Projetar telas densas, rapidas e orientadas a teclado.", "frontend", "media"),
("Compatibilidade com maquinas antigas", "04-performance", "Definir limites de RAM, CPU e inicializacao.", "performance", "alta"),
("Lazy loading", "04-performance", "Planejar carregamento sob demanda por modulo.", "performance", "media"),
("Cache inteligente", "04-performance", "Definir cache local e invalidacao.", "performance", "media"),
("Relatorios performaticos", "04-performance", "Otimizar relatorios por paginacao e indices.", "performance", "media"),
("Testes de regressao", "16-migracao", "Criar suite de equivalencia legado vs novo.", "testes", "alta"),
("Ambiente de homologacao", "16-migracao", "Consolidar ambiente MOD com dados e rollback.", "migracao", "alta"),
("Plano piloto", "16-migracao", "Definir piloto por modulo/filial.", "migracao", "alta"),
("Migração de usuarios", "16-migracao", "Migrar usuarios, perfis e permissoes.", "migracao", "alta"),
("Migração de cadastros", "16-migracao", "Migrar clientes, produtos, parametros e tabelas auxiliares.", "migracao", "alta"),
("Migração financeira", "16-migracao", "Migrar contas, caixa e historico com auditoria.", "migracao", "critica"),
("Migração operacional", "16-migracao", "Migrar ROL, entregas e historico operacional.", "migracao", "critica"),
("Migração fiscal", "16-migracao", "Migrar documentos fiscais com compliance.", "migracao", "critica"),
("Plano de coexistencia", "16-migracao", "Planejar legado e novo operando lado a lado.", "migracao", "critica"),
("Plano de corte gradual", "16-migracao", "Definir criterios de ativacao por modulo.", "migracao", "alta"),
("Plano de disaster recovery", "18-risk", "Definir backup, restore, RPO e RTO.", "risco", "critica"),
("Plano de backup inteligente", "18-risk", "Projetar backups locais/cloud e verificacao.", "risco", "alta"),
("Analise de riscos", "18-risk", "Classificar riscos tecnicos, operacionais e fiscais.", "risco", "alta"),
("Matriz de criticidade", "18-risk", "Criar matriz por modulo/dependencia/fluxo.", "risco", "alta"),
("Matriz de substituicao", "18-risk", "Classificar componentes substituiveis, criticos e obsoletos.", "risco", "alta"),
("Plano de ferramentas", "00-controle", "Documentar ferramentas instaladas, motivo, risco e rollback.", "controle", "media"),
("Governanca de documentacao", "00-controle", "Padronizar evidencias, logs e relatorios.", "controle", "media"),
("Rastreabilidade de achados", "00-controle", "Ligar fase, evidencia, arquivo, risco e decisao.", "controle", "media"),
("Relatorio executivo", "10-relatorio-final", "Consolidar status para decisao administrativa.", "relatorio", "media"),
("Relatorio tecnico profundo", "10-relatorio-final", "Consolidar arquitetura, dependencias e fluxo real.", "relatorio", "alta"),
("Manual tecnico", "09-manuais", "Criar manual de operacao, suporte e diagnostico.", "manual", "media"),
("Manual administrativo", "09-manuais", "Criar manual de usuarios, permissoes e auditoria.", "manual", "media"),
("Manual de rollback", "09-manuais", "Documentar rollback por modulo, banco e update.", "manual", "alta"),
("Treinamento operacional", "09-manuais", "Planejar treinamento de usuarios e administradores.", "operacao", "media"),
("Validacao de equivalencia", "16-migracao", "Comparar resultados legado e novo por fluxo.", "migracao", "critica"),
("Aceite por modulo", "16-migracao", "Definir criterios e checklist de aceite.", "migracao", "alta"),
("Desativacao de dependencias antigas", "16-migracao", "Planejar retirada gradual de componentes legados.", "migracao", "alta"),
("Ativacao cloud controlada", "13-cloud", "Ativar cloud por tenant com fallback local.", "cloud", "alta"),
("Operacao assistida", "07-operacao", "Monitorar fase inicial de uso real.", "operacao", "alta"),
("Otimização pos-migracao", "04-performance", "Ajustar performance, memoria e relatórios.", "performance", "media"),
("Auditoria final", "17-seguranca", "Validar seguranca, logs, permissoes e integridade.", "seguranca", "alta"),
("Entrega da nova geracao", "15-nextgen", "Consolidar plataforma moderna pronta para evolucao continua.", "nextgen", "critica"),
("Hypercare pos-entrega", "07-operacao", "Acompanhar operacao real com suporte intensivo e metricas.", "operacao", "alta"),
("Monitoramento de regressao", "04-performance", "Detectar regressao de memoria, CPU, rede e UX entre versoes.", "performance", "alta"),
("Governanca de releases", "00-controle", "Definir processo de release, aprovacao, changelog e rollback.", "controle", "alta"),
("Gestao de configuracao", "00-controle", "Controlar parametros, ambientes, segredos e variaveis por tenant.", "controle", "alta"),
("Revisao periodica de seguranca", "17-seguranca", "Executar revisoes recorrentes de permissoes, segredos e auditoria.", "seguranca", "alta"),
("Revisao periodica de dados", "10-database", "Auditar integridade, crescimento, indices e retencao de dados.", "database", "alta"),
("Roadmap de evolucao continua", "15-nextgen", "Planejar ciclos futuros, prioridades e depreciacao de componentes.", "nextgen", "media"),
("Encerramento da recuperacao legada", "10-relatorio-final", "Formalizar conclusao da recuperacao e estado final do legado.", "relatorio", "critica"),
]

required_sections = [
    "Objetivo",
    "Escopo",
    "Tarefas",
    "Ferramentas",
    "Analises",
    "Validacoes",
    "Evidencias",
    "Logs",
    "Rollback",
    "Criticidade",
    "Documentacao",
    "Entregaveis",
    "Impacto operacional",
    "Impacto em memoria",
    "Impacto em CPU",
    "Impacto em rede",
    "Impacto em autenticacao",
    "Impacto em sincronizacao",
    "Impacto em licenciamento",
]

tools_by_area = {
    "inventario": "PowerShell, Python, Get-ChildItem, Get-FileHash, CSV.",
    "integridade": "PowerShell Get-FileHash, Python hashlib, comparacao CSV.",
    "rollback": "ZIP, snapshots, checksums, scripts de rollback.",
    "binarios": "analyze-pe-imports.ps1, strings, PE headers, ILSpy quando aplicavel.",
    "dependencias": "imports PE, modulos runtime, Process Explorer/ProcMon futuro.",
    "arquitetura": "strings, imports, metadados .NET, Ghidra/ILSpy futuro.",
    "reversing": "strings, imports, Ghidra/ILSpy/dnSpy quando autorizado.",
    "runtime": "EquipeExe.Mod.Observability, ProcMon futuro, ETW futuro.",
    "startup": "ProcMon, ETW, API Monitor, logs de modulos runtime.",
    "memoria": "EquipeExe.Mod.Observability, Process Explorer, PerfView futuro.",
    "performance": "PerfView futuro, ETW, amostragem CPU/RAM/I/O.",
    "auth": "ODBC Paradox readonly, scripts auth, logs, captura dinamica.",
    "licensing": "strings, ProcMon, rede, registry, analise de arquivos.",
    "comunicacao": "Get-NetTCPConnection, netsh trace, Wireshark/Fiddler futuro.",
    "offline": "firewall MOD, isolamento de rede, observabilidade.",
    "online": "observabilidade, captura de rede, comparacao baseline.",
    "sync": "logs, banco, filas, rede, outbox/inbound planejados.",
    "update": "LiveUpdate stub, firewall MOD, integridade, logs.",
    "modulos": "observability, mapa funcional, menus, imports.",
    "fiscal": "ambiente fiscal controlado, logs fiscais, ProcMon, drivers.",
    "relatorios": "strings, menus, banco, captura visual.",
    "frontend": "captura visual, mapa de menus, screenshots, OCR/manual.",
    "negocio": "fluxo operacional, banco, telas, logs, entrevistas operacionais.",
    "financeiro": "banco, telas, relatorios, auditoria.",
    "cadastro": "banco, telas, permissao, validacoes.",
    "config": "INIs/XML/JSON, registry, ProcMon.",
    "database": "Paradox ODBC readonly, dicionario, indices, integridade.",
    "migracao": "scripts ETL, checksums, testes comparativos, rollback.",
    "nextgen": ".NET 8, SQLite, API local, arquitetura modular.",
    "cloud": "Supabase futuro, API, sync, dashboard.",
    "supabase": "RLS, Edge Functions, Auth, Postgres, Storage se necessario.",
    "seguranca": "hash moderno, auditoria, ACLs, secrets, assinatura.",
    "observabilidade": "logs estruturados, tracing, metrics, dumps.",
    "apis": "OpenAPI futuro, API local .NET, contratos.",
    "testes": "testes automatizados, comparacao legado/novo.",
    "risco": "matriz de risco, criticidade, mitigacao.",
    "controle": "log-de-alteracoes, Projeto_Novo_Atelie_2026, CSV de rastreio.",
    "relatorio": "Markdown, CSV, evidencias, anexos.",
    "manual": "documentacao operacional e administrativa.",
    "operacao": "checklists, monitoramento, suporte assistido.",
}

def impact(area, kind):
    if kind == "operacional":
        return "Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado."
    if kind == "memoria":
        return "Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime."
    if kind == "cpu":
        return "Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada."
    if kind == "rede":
        return "Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado."
    if kind == "auth":
        return "Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio."
    if kind == "sync":
        return "Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints."
    if kind == "lic":
        return "Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao."
    return ""

md = []
md.append("# Roadmap Ultra Detalhado - 150 Fases")
md.append("")
md.append("Data: 2026-05-23")
md.append("")
md.append("Escopo: recuperacao, engenharia reversa controlada, visibilidade total e planejamento da nova geracao do EquipeExe.")
md.append("")
md.append("Regra absoluta: o original `D:\\AtelieProd\\Equipexe` permanece intocado; execucoes, alteracoes e experimentos devem ocorrer em `D:\\AtelieProd\\MOD`.")
md.append("")

index_rows = []
for i, (title, folder, objective, area, criticality) in enumerate(phases, start=1):
    phase_id = f"FASE {i:03d}"
    md.append(f"## {phase_id} - {title}")
    md.append("")
    values = {
        "Objetivo": objective,
        "Escopo": f"Documentar e executar esta fase dentro de `D:\\AtelieProd\\MOD\\docs\\{folder}` usando evidencias controladas.",
        "Tarefas": "- coletar evidencias;\n- classificar achados;\n- cruzar com mapas existentes;\n- atualizar log tecnico;\n- atualizar arquivo mestre do projeto.",
        "Ferramentas": tools_by_area.get(area, "PowerShell, Python, C#, ferramentas CLI e ferramentas externas controladas quando necessario."),
        "Analises": f"Analisar area `{area}`, dependencias, impacto operacional, riscos e relacao com modulos principais.",
        "Validacoes": "- validar que o original nao foi alterado;\n- validar arquivos gerados;\n- validar consistencia com evidencias anteriores;\n- registrar limitacoes.",
        "Evidencias": f"CSV, Markdown, logs, snapshots ou capturas associados a `{folder}`.",
        "Logs": "`D:\\AtelieProd\\MOD\\docs\\00-controle\\log-de-alteracoes.md` e logs tecnicos especificos da fase.",
        "Rollback": "Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.",
        "Criticidade": criticality,
        "Documentacao": f"`D:\\AtelieProd\\MOD\\docs\\{folder}` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.",
        "Entregaveis": f"Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `{title}`.",
        "Impacto operacional": impact(area, "operacional"),
        "Impacto em memoria": impact(area, "memoria"),
        "Impacto em CPU": impact(area, "cpu"),
        "Impacto em rede": impact(area, "rede"),
        "Impacto em autenticacao": impact(area, "auth"),
        "Impacto em sincronizacao": impact(area, "sync"),
        "Impacto em licenciamento": impact(area, "lic"),
    }
    for section in required_sections:
        md.append(f"### {section}")
        md.append("")
        md.append(values[section])
        md.append("")
    index_rows.append({
        "Fase": phase_id,
        "Titulo": title,
        "Diretorio": f"docs\\{folder}",
        "Area": area,
        "Criticidade": criticality,
        "Status": "planejada" if i > 8 else "parcial/em andamento",
    })

(OUT / "roadmap-150-fases.md").write_text("\n".join(md), encoding="utf-8")
with (OUT / "roadmap-150-fases.csv").open("w", encoding="utf-8-sig", newline="") as f:
    writer = csv.DictWriter(f, fieldnames=["Fase", "Titulo", "Diretorio", "Area", "Criticidade", "Status"])
    writer.writeheader()
    writer.writerows(index_rows)

print(OUT / "roadmap-150-fases.md")
print(OUT / "roadmap-150-fases.csv")
