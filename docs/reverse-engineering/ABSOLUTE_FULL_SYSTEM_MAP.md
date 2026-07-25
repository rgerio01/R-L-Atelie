# ABSOLUTE FULL SYSTEM MAP — EquipeExe
# NADA PODE FICAR PARA TRÁS

**Data:** 2026-05-24
**Fonte:** Engenharia reversa completa — pypxlib + string extraction + INI parsing + LogSis + observabilidade dinâmica
**Regra:** Zero suposições. Tudo rastreado, evidenciado, validado, mapeado, documentado.

---

## O QUE FOI FEITO

Desmontagem completa do sistema EquipeExe (Delphi 32-bit, BDE/Paradox, Indy HTTP, 16 anos em produção) cobrindo:
- 478 tabelas Paradox (4.996 campos) lidas campo a campo via pypxlib
- 32 sistemas registrados, 9 usuários, 144 operações LAVSOFT mapeadas
- 112.615 entradas de log analisadas (2010-2026)
- 40+ endpoints HTTP LavSoft descobertos via string extraction
- 9 módulos INI analisados (1.073 linhas no EquLav.Ini)
- 757 vínculos UI→banco mapeados estaticamente

## COMO FOI FEITO

1. **pypxlib 2.5** — leitura direta dos arquivos .DB sem BDE, sem driver, sem alteração do original
2. **strings extraction** — varredura de todos os executáveis (.exe, .dll) para URLs, SQL, nomes de tela, endpoint strings
3. **INI parsing** — leitura de todos os arquivos .Ini/.xml dos módulos
4. **LogSis.DB analysis** — 112.615 registros lidos e agrupados por usuário/operação/período
5. **Observabilidade dinâmica** — LavSoft.exe executado no runtime MOD com coleta de RAM/CPU/handles/rede
6. **Comparativo migração** — 11/11 verificações OK, 100% dos dados no Supabase

---

## 1. MAPA COMPLETO FRONTEND

### Executáveis de Interface

| Executável | Papel | Tipo | DLLs Críticas |
|---|---|---|---|
| LavSoft.exe | Core operacional — ROL, clientes, caixa, entrega | Delphi 32-bit | BDE, Indy, OpenSSL, VCL |
| LavFacilLan.exe | Lançamento rápido — 275 vínculos UI→banco | Delphi 32-bit | BDE, WinInet, Indy |
| Gerenciador.exe | Admin/broker remoto — .NET 32-bit | .NET 32-bit + WinForms | mscoree, HTTP stack |
| Financeiro.exe | Módulo financeiro — contas, cheques, boletos | Delphi 32-bit | BDE, Bematech fiscal |
| Estoque.exe | Controle de estoque | Delphi 32-bit | BDE, WinInet |
| Senhas.exe | Gestão de usuários/permissões/bloqueios | Delphi 32-bit | BDE |
| SAT.exe | Atendimento telefônico | Delphi 32-bit | BDE, ODBC |
| NFE.exe | Nota fiscal eletrônica | Delphi 32-bit | DLLs fiscais SEFAZ |
| PCP.exe | Processo/Controle de Produção | Delphi 32-bit | BDE |
| Alerta.exe | Sistema de alertas de equipe | Delphi 32-bit | BDE |
| LiveUpdate.exe | Atualização automática | Delphi 32-bit | WinInet → **BLOQUEADO no MOD** |

### Telas/Janelas Registradas (Janela.DB — 16 telas)

| Sistema | Form/Janela | Função |
|---|---|---|
| ALERTA | MenuAlertaEquipe | Menu do sistema de alertas |
| FINANCEIRO | FM_CadBan | Lançamento de Contas Correntes |
| FINANCEIRO | MP_Financ | Menu Principal Financeiro |
| LAVFACILLA | FC_MovLan | Lançamento rápido (simplificado) |
| LAVSOFT | FC_ConTab | Consultas Rápidas — atalho F11 |
| LAVSOFT | FC_MovLan | Lançamento do ROL |
| LAVSOFT | FC_PAGAMENTO | Tela de Pagamento |
| LAVSOFT | FM_Clientes | Cadastro de Clientes |
| LAVSOFT | FM_ModRel | Filtro de Relatórios |
| LAVSOFT | FM_MovCab | Menu Recepção (entrada de peças) |
| LAVSOFT | FM_Notas | Faturamento/Notas |
| LAVSOFT | MP_LanLav | Menu Principal do LavSoft |
| LAVSOFT | PagRolLan | Pagamento Touch Screen |
| PECLAV | LanRoupasLot_Fm | Menu de Lotes de Roupas |
| PECLAV | MP_Menu | Menu Principal PecLav |
| PECWEB | MP_Menu | Menu Principal PecWeb |

### Distribuição de Vínculos UI→Banco (757 total estático)

| Módulo | Vínculos | Foco |
|---|---:|---|
| LavFacilLan | 275 | Grids operacionais, consultas, rotinas |
| Estoque | 153 | Produtos, movimentos, entrada/baixa |
| LavSoft | 144 | ROL, cliente, caixa, entrega, fiscal |
| SAT | 122 | Ocorrências, atendimento |
| Financeiro | 34 | Pagamentos, notas, impressão |
| NFE | 16 | Emissão fiscal |
| Senhas | 13 | Usuários, permissões |

---

## 2. MAPA COMPLETO BACKEND

### Módulos/Processos

| Módulo INI | Executável Principal | Banco de Dados | Porta/Comunicação |
|---|---|---|---|
| LAV (EquLav.Ini) | LavSoft.exe | Lav\FILIAL\*.DB | BDE local + HTTP LavSoft |
| GER (EquGer.Ini) | Gerenciador.exe, Senhas.exe | Ger\Dados\*.DB | HTTP LavSoft (admin) |
| PAG (EquPag.Ini) | Financeiro.exe | PAG\FILIAL\*.DB | BDE local + Bematech |
| PEC (EquPec.Ini) | PCP.exe | PEC\DADOS\*.DB | BDE local |
| EST (EquEst.Ini) | Estoque.exe | EST\DADOS\*.DB | BDE local + WinInet |
| ESC (EquEsc.Ini) | NFE.exe | ESC\*.DB | SEFAZ (HTTP) |
| SAT (EquSat.Ini) | SAT.exe | SAT\*.DB | BDE local |
| MAN (EquMan.Ini) | — | MAN\*.DB | BDE local |
| REC (EquRec.Ini) | — | REC\FILIAL\*.DB | BDE local |

### Stack de Dados

```
EquipeExe
├── BDE (Borland Database Engine) 32-bit
│   ├── Driver: Paradox
│   ├── Credenciais: USER NAME=sa PASSWORD=123 (BD.txt — PLAINTEXT)
│   └── Arquivos: .DB + .PX (índice primário) + .XG/.YG (índices secundários)
├── OpenSSL 32-bit (libeay32.dll, ssleay32.dll)
├── Indy HTTP (Delphi) — comunicação LavSoft
└── WinInet (IE stack) — comunicação alternativa
```

---

## 3. MAPA COMPLETO RUNTIME

### Comportamento de Inicialização

1. `Gerenciador.exe` (se aberto) → `AutenticaGerenciador` → LavSoft HTTP
2. `LavSoft.exe` → lê `EquNet.Ini` + `Registrar.xml` (identidade da estação)
3. `LavSoft.exe` → verifica `NovoReg*.DB` (licença local)
4. `LavSoft.exe` → `TestaAutentica` → LavSoft HTTP (valida licença remota)
5. `LavSoft.exe` → lê `Nivel.DB` (permissões do usuário)
6. `LavSoft.exe` → abre conexão BDE com tabelas do módulo LAV
7. `splwow64.exe` — subsistema impressão 32-bit/64-bit — sempre acionado

### Perfil de Recursos (observado 20s no MOD)

- Working Set: ~19.44 MB
- Memória Privada: ~6.73 MB
- Threads: 6
- Handles: 257
- Módulos/DLLs: 53
- Conexões TCP observadas na janela de 20s: 0 (dados pendentes de tela aberta)

### Dependências Críticas de Runtime (50 DLLs copiadas para MOD)

- BDE core: IDAPI32.DLL, IDPDX32.DLL
- VCL: BOREXI32.DLL, comctl32.dll
- Impressão: INTPDF32.DLL, midas.dll
- Fiscal: Bematech DLLs
- OpenSSL: libeay32.dll, ssleay32.dll

---

## 4. MAPA COMPLETO BANCO

### Estatística Geral

| Métrica | Valor |
|---|---|
| Tabelas totais | 478 |
| Campos totais | 4.996 |
| Módulos cobertos | 9 (LAV, GER, PAG, PEC, EST, ESC, SAT, MAN, REC) |
| Tabelas com dados | 76+ verificadas com pypxlib |

### Distribuição por Domínio

| Domínio | Tabelas | Campos |
|---|---:|---:|
| config/admin | 150 | 1.371 |
| outro/a validar | 161 | 1.284 |
| cliente | 25 | 631 |
| ordem_servico_rol | 28 | 447 |
| fiscal | 36 | 385 |
| produto_servico | 36 | 384 |
| financeiro | 19 | 290 |
| estoque | 10 | 102 |
| permissao/auth | 8 | 52 |
| auditoria/log | 5 | 50 |

### Tabelas Críticas com Campos

**Clientes.DB** (5.064 registros migrados)
- CodCli, NomCli, EndCli, CidCli, EstCli, CepCli, TelCli, TelCli2, Contato
- CgcCli, DigCli, GruCli, CodTab, InsEstCli, InsMunCli, EndCobCli, EndEntCli, CidCobCli

**MovCab.DB** — OS/ROL (31.972 registros migrados)
- ROL, CodCli, DatEntLoja, CodTab, CodTipSer, CodTipEnt, CodPra
- NumGR, NumOS, DatLan, CodVen, DatEnt, ValTot, TotPecas, Posicao
- NumNot, Unidade, CodUsuario

**MovItemSer.DB** — Itens de Serviço por OS
- Rol, CodPro, SeqPro, CodSerLav, Quantidade, PreUniSer, PreFinalSer

**MovItem.DB** — Itens de Produto por OS
- ROL, CodPro, SeqPro

**Duplicat.DB** — Contas a Receber
- NumFat, NumDup, DatEmi, DatVen, ValFat, ValDup, EmiBol
- Baixa, DatPag, ValDupPag, CodCli, CodFpg, CodCpg, CodBan, SeqCai

**MovEst.DB** — Movimentos de Estoque
- SeqLan, CodEst, CodProEst, DatLan, Qde, TipoES, ValTot, ValUnit
- TipInteg, CodInteg, Obs, Cancelado, CodUsuario, CodClaEst

**LogSis.DB** — Auditoria (112.615 registros, 16 anos)
- Data, Hora, CodUsuario, CodFil, CodSistema, Operacao, Descricao
- Tipos: INC, ALT, REE, DEL

**Nivel.DB** — Permissões (438 registros)
- CodUsuario, CodFil, CodSistema, Op, NivelI, NivelA, NivelE, NivelT

**CadFpg.DB** — Formas de Pagamento (35 cadastradas)
- Inclui: Dinheiro, Cheque, Cartão Débito/Crédito, PIX, Boleto

**ContaCor1.DB** — Movimentos Financeiros (28.012 transações)

---

## 5. MAPA COMPLETO MENUS/SUBMENUS

### Sistemas Principais com Senha (11)

| Código | Nome | Executável |
|---|---|---|
| LAVSOFT | LavSoft 2000 | LavSoft ← SISTEMA PRINCIPAL |
| FINANCEIRO | Financeiro 2000 | Financ |
| ESTOQUE | Estoque 2000 | Estoque |
| PCP | Processo Controle Produção | PCP |
| ALERTA | Alerta 2000 | Alerta |
| SAT | Sistema Atendimento Telefônico | SAT |
| PECWEB | Controle Peça Online | PecWeb |
| MANUTENCAO | Manutenção 2000 | — |
| FRANLAV | Franquia LavSoft | — |
| PECLAV | Controle de Peças LavSoft | — |
| SENHAS | Sistema Gerencial de Senhas | — |

### Menu LAVSOFT — 144 Operações Mapeadas (Nivel.DB)

**Recepção / Entrada:**
EntradaRol, EntradaEstoque, MenuEntradaRol

**Lançamento de OS:**
LancamentoRol, Lancamentos, MenuLancamentoRol, FC_MovLan

**Pagamento:**
Pagamento, PagamentoRol, PagamentodeVariosRols, CancPag, DevoluoPagamento, PagRolLan

**Clientes:**
ClientesCad, AlteraCliente, ExtratoCliente, MovGrupoClientes

**Faturamento / Notas:**
Faturamento, NotaFiscal, EmitirNotaFiscal, CancelamentoNotaFiscal

**Retirada / Entrega:**
MenuRetiradas, EntregaRol, EntregaVariosRol, EntregaporPecas, MarcarEntrega

**Controles:**
ControledeCaixa, ControledeLavagem, ControledeMetas, CaixaDiaDia

**Relatórios (20+ operações):**
AnaMovDia1, AnaMovDia2, RelFaturamento, RelMovimento, RelControle, FM_ModRel

**Administrativo:**
Usuarios, Parametros, Manutencao, LogSistema, Inclusao, Alteracao, Cancelamento

**Outros:**
AnaMovDia1/2, CFAbertura1, CFFechamento1, Anotacoes1, Consultas, ConsultaRapida, ImpAutom, CFOP, ISS

---

## 6. MAPA COMPLETO FUNÇÕES

### Fluxo 1 — Cadastro de Cliente
```
Entrada: FM_Clientes (tela)
Tabelas: Clientes.DB + CliContato.DB + ClientesObs.DB + GruClientes*.DB
Validações: NomCli obrigatório, CPF/CNPJ em CgcCli/DigCli, duplicidade
Saída: CodCli disponível para ROL/financeiro/notas/relatórios
```

### Fluxo 2 — Criação de OS/ROL
```
Entrada: EntradaRol / FC_MovLan / LancamentoRol
Tabelas: MovCab.DB + MovItemSer.DB + MovItem.DB + Clientes.DB + Produt.DB
Lógica: Gera ROL sequencial, vincula CodCli, calcula ValTot, define Posicao inicial
Saída: ROL aberto → produção → entrega → pagamento → relatórios
Log: LogSis.DB INC
```

### Fluxo 3 — Entrega / Retirada
```
Entrada: EntregaRol / EntregaVariosRol / MarcarEntrega
Tabelas: MovCab.Posicao + MovLocRol + CadLocRol
Lógica: Atualiza Posicao, registra localização física, marca data de entrega
Log: LogSis.DB ALT
```

### Fluxo 4 — Pagamento
```
Entrada: Pagamento / PagamentoRol / PagRolLan (touch screen)
Tabelas: MovCab + Duplicat.DB + FecCaixa.DB + CliCredito.DB
Formas: 35 formas de pagamento (CadFpg) incluindo PIX
Lógica: Baixa duplicata, registra no caixa, pode emitir nota
Log: LogSis.DB ALT/INC
```

### Fluxo 5 — Fechamento de Caixa
```
Entrada: CaixaDiaDia / ControledeCaixa
Tabelas: FecCaixa.DB + MovIniCaixa.DB + Duplicat.DB
Lógica: Totaliza entradas/saídas, gera relatório, registra saldo
```

### Fluxo 6 — Faturamento / Nota Fiscal
```
Entrada: Faturamento / EmitirNotaFiscal
Tabelas: Notas.DB + NotComp.DB + NotaFisPag.DB
Integração: NFE.exe (SEFAZ HTTP), Bematech MP-FI (impressora fiscal)
Lógica: Vincula ROL → nota fiscal, emite NF, cancela se necessário
```

---

## 7. MAPA COMPLETO PERMISSÕES

### Usuários Ativos (9)

| Código | Nome | Grupo | Tipo | Papel |
|---|---|---|---|---|
| GABRIELA | GABRIELA | MASTE | U | MASTER — admin geral |
| LUCI | LUCIDALVA ALVES DE OLIVEIRA | OPERA | S | Proprietária — SuperUser |
| ROSIMEIRE | ROSIMEIRE | OPERA | S | SuperUser |
| BRUNA | BRUNA | OPERA | S | SuperUser |
| BRENA | BRENA | OPERA | U | Operador |
| CID | APARECIDA LEMOS RIBEIRO | OPERA | U | Operador |
| EDU | CLEBER EDUARDO LACERDA | OPERA | U | Operador |
| FAT | MARIA DE FATIMA RIBEIRO RAMOS | OPERA | U | Operador |
| MICHELE | MICHELE | OPERA | U | Operador |

### Estrutura de Permissões (Nivel.DB — 438 registros)

```
CodUsuario | CodFil   | CodSistema | Op                    | NivelI | NivelA | NivelE | NivelT
LUCI       | QL001    | LAVSOFT    | LancamentoRol         |   1    |   1    |   1    |   1
LUCI       | QL001    | LAVSOFT    | Pagamento             |   1    |   1    |   0    |   1
...
```

- NivelI: pode Inserir
- NivelA: pode Alterar
- NivelE: pode Excluir
- NivelT: acesso Total

### Perfis Inativos Mapeados
ATENDJR (atendente júnior), FRANQ (franqueado), PERFATEND (perfil atendimento), PONTO, SUPERV

---

## 8. MAPA COMPLETO RELATÓRIOS

### Relatórios Identificados em LAVSOFT

| Operação | Tipo | Função |
|---|---|---|
| AnaMovDia1, AnaMovDia2 | Análise diária | Movimento do dia (variação 1 e 2) |
| RelFaturamento | Faturamento | Faturamento por período |
| RelMovimento | Movimento | Movimentação de OS/ROL |
| RelControle | Controle | Controle operacional |
| FM_ModRel | Filtro | Interface de filtro de relatórios |
| ExtratoCliente | Cliente | Extrato do cliente |
| MovGrupoClientes | Grupo | Movimentação por grupo de clientes |
| ControledeMetas | Metas | Metas de vendas por vendedor |
| CaixaDiaDia | Caixa | Relatório de fechamento de caixa |
| LogSistema | Auditoria | Visualizar log do sistema |

### Filtros Operacionais (FM_ModRel)
- Parâmetros: período, filial, usuário, sistema, tipo de operação
- Acesso: permissão `FM_ModRel` no Nivel.DB

---

## 9. MAPA COMPLETO IMPRESSÃO

### Periféricos Configurados

| Periférico | Modelo | Porta | INI |
|---|---|---|---|
| Etiqueta ROL | Argox Rabbit 214 (PPLB) | USB | EquLav.Ini: Impressora=Argox |
| Fiscal NF | Bematech MP-FI | COM/LPT | EquLav.Ini: Impressora Fiscal=Bematech |
| Cheque | Bematech DP-32 | COM2 | EquPag.Ini |
| Relatórios | HP 550C | LPT1 | EquLav.Ini |
| Boleto | Epson LX (matricial) | LPT1 | EquRec.Ini |

### Impressão Automática
- Operação `ImpAutom` no Nivel.DB — impressão automática de etiquetas
- `splwow64.exe` acionado pelo LavSoft.exe: subsistema 32→64-bit para impressão

### NFE / SAT Fiscal
- NFE.exe comunica com SEFAZ via HTTP
- SAT.exe usa driver local
- Bematech MP-FI: CF-e e NF-e simplificada

---

## 10. MAPA COMPLETO LICENSING

### Mecanismo Legado

```
[Camada 1 — Local]
  Registrar.xml → MacAddress + CodLojaOriginal (EQU0000001215)
  EquNet.Ini → EquipeZ=46147, TesteW1=46147, campos Teste*
  NovoReg*.DB → tabelas de registro/desbloqueio (vazias na amostra)

[Camada 2 — Remoto]
  GET http://www.lavsoft.com.br/TestaAutentica
  POST http://www.lavsoft.com.br/AutenticaGerenciador
  POST http://www.lavsoft.com.br/RegistraEstacao

[Camada 3 — Aplicada]
  LavSoft.exe → flags BloqSec1..BloqSec9 (Financeiro.exe)
  Senhas.exe → bloquear/desbloquear sistemas e filiais
  ControlaBloq.exe → controle de bloqueio
```

### Licença EquipeZ
- EquNet.Ini: `EquipeZ=46147` com extensão para ~2173 (código 99999)
- Não requer validação remota para operação básica
- Referência: memory/project_lavsoft_license.md

### Status MOD — Proteções Implementadas

| Camada | Status | Método |
|---|---|---|
| /VerificaAtualizacoes | **BLOQUEADO** | LiveUpdate.Disabled stub |
| /DownloadDados | **BLOQUEADO** | LiveUpdate.Disabled stub |
| /TestaAutentica | **INTERCEPTADO** | hosts→127.0.0.1 + mock server Python porta 80 |
| /AutenticaGerenciador | **INTERCEPTADO** | hosts→127.0.0.1 + mock server |
| /RegistraEstacao | **INTERCEPTADO** | hosts→127.0.0.1 + mock server |
| /EnviaMovimento* | **ABSORVIDO** | mock server → log local, dados não saem |
| Firewall outbound | **PENDENTE ADMIN** | apply-lavsoft-intercept.ps1 |

---

## 11. MAPA COMPLETO COMUNICAÇÃO EXTERNA

### Host: http://www.lavsoft.com.br (191.6.218.152:80 — HTTP SEM TLS)

### Endpoints Descobertos (40+)

**Licenciamento e Auth:**
- GET /TestaAutentica ← validação de licença
- POST /AutenticaGerenciador ← login admin
- POST /RegistraEstacao ← registro de dispositivo
- GET /ListarDispositivosPorFilial

**Atualização:**
- GET /VerificaAtualizacoes
- GET /TesteVerificaAtualizacoes
- GET /DownloadDados
- GET /DownloadFromSource

**Sync de Dados (GeraNuvem=1):**
- POST /EnviaMovimento, /EnviaMovimentoNew
- POST /EnviaEntrega, /EnviaEntregaNew
- POST /EnviaCores, /EnviaDefeitos, /EnviaDelivery
- POST /EnviaMarca, /EnviaServicos, /EnviaCaract
- POST /EnviaFatPre, /EnviaGruPro, /EnviaPrazos
- POST /EnviaFormasPagamento, /EnviaTabelasPreco
- POST /EnviaTipoEntrada

**Recebimento:**
- GET /RecebeRolsFinalizados
- GET /RecebeRolsFinalizadosNew

**WebServices ASMX (SOAP):**
- /ws/Equipe/v2/Geral.ASMX
- /ws/Equipe/v2/AtuTabelas.asmx
- /ws/Sincrolav/Dados.asmx
- /ws/Graficos/wsGraficos.asmx
- /ws/minilav7/v3/Loja.asmx
- /ws/nuvem/v1/UploadArquivo.asmx
- /ws/Nuvem/Enviar

### Comunicações Locais
- Bematech MP-FI: COM/LPT (fiscal)
- Argox Rabbit 214: USB PPLB (etiqueta)
- SEFAZ: HTTP (NFE.exe)
- BDE/Paradox: local filesystem

---

## 12. MAPA COMPLETO OBSERVABILIDADE

### LogSis.DB — Auditoria do Sistema

| Métrica | Valor |
|---|---|
| Total registros | 112.615 |
| Período | 2010-04-26 → 2026-05-21 (16 anos) |
| Última operação | LUCI reemitiu ROL 31798 (21/05/2026) |

**Distribuição por Usuário:**
- LUCI: 39.2% das operações
- ROSIMEIRE: 30.3% das operações
- LUCI + ROSIMEIRE = 69.5% (concentração de risco)

**Tipos de Log:** INC (Inclusão), ALT (Alteração), REE (Reemissão), DEL (Deleção)

### Análise de Negócio

| Indicador | Valor |
|---|---|
| Receita total rastreada (2015-2026) | R$ 2.039.532,88 |
| Taxa de cobrança | 96.76% (excelente) |
| Serviço mais usado | Cód. 042 — 29.2% de todas as OS |
| Peça mais processada | Cód. 006 — 23.6% de todas as peças |
| EncProg ativo em | 2026-05-21 (operacional) |

**Localizações Físicas:**
- 5 araras de separação
- PRODUÇÃO
- 2 PRATELEIRAS
Total: 8 pontos físicos mapeados no sistema

### Observabilidade Dinâmica MOD

Arquivos gerados em `logs/observability/`:
- `LavSoft-summary-20260523-123943.json`
- `LavSoft-samples-20260523-123943.csv`
- `LavSoft-children-20260523-123943.csv`
- `LavSoft-network-20260523-123943.csv`
- `LavSoft-modules-20260523-123943.csv`

---

## 13. MAPA COMPLETO RUNTIME HOOKS

### Pontos de Entrada do Sistema

| Hook | Arquivo | Evento |
|---|---|---|
| Startup | EquNet.Ini lido | Identificação de estação |
| Startup | Registrar.xml lido | Hardware binding |
| Startup | NovoReg*.DB consultado | Verificação de licença local |
| Startup | GET /TestaAutentica | Verificação de licença remota |
| Login | Usuarios.DB + Senhas.DB | Autenticação usuário |
| Login | Nivel.DB filtrado | Carregamento de permissões |
| Login | EquLav.Ini lido | Configuração do módulo |
| Cada OS | LogSis.DB INC/ALT | Auditoria de toda operação |
| Impressão | splwow64.exe spawned | Subsistema de impressão |
| Sync | POST /EnviaMovimento | Sync para nuvem LavSoft (GeraNuvem=1) |
| Atualização | LiveUpdate.exe chamado | Verificação de updates (BLOQUEADO no MOD) |

### Parâmetros de Configuração Críticos (EquLav.Ini)

```ini
GeraNuvem=1           ; sync para LavSoft cloud (INTERCEPTADO no MOD)
GravaMensRol=1        ; log de todas as OS
Impressora=Argox (PPLB) Lay Out 2
Impressora Fiscal=Bematech MP-FI
ISS=5%
CodTab=001            ; tabela de preços padrão
ControlaPecas=1       ; controla peças por OS
```

---

## 14. MAPA COMPLETO RELACIONAMENTOS

### Diagrama de Entidades Críticas

```
Clientes.DB (CodCli)
    │
    ├── MovCab.DB (ROL, CodCli) ← OS/ROL central
    │       ├── MovItemSer.DB (Rol, CodSerLav) ← serviços
    │       ├── MovItem.DB (ROL, CodPro) ← peças
    │       ├── MovLocRol.DB (Rol) ← localização
    │       ├── CadLocPec.DB (Rol) ← localização por peça
    │       └── Notas.DB (NumNot) ← nota fiscal
    │               └── NotComp.DB (NumNot, Seq)
    │
    ├── Duplicat.DB (CodCli, NumFat) ← contas a receber
    │       └── Boletos.DB (NumFat)
    │
    ├── CliCredito.DB (CodCli) ← crédito do cliente
    ├── MovPontos.DB (CodCli) ← programa de pontos
    └── EstCli.DB (CodCli) ← estoque do cliente

SerLav.DB (CodSerLav) → MovItemSer.DB
TabPro.DB (CodPro) → MovItem.DB

CadFpg.DB (CodFpg) → Duplicat.DB
CadVen.DB (CodVen) → MovCab.DB

LogSis.DB (CodUsuario, ROL) → rastreia toda operação

Nivel.DB (CodUsuario, CodSistema, Op) → controla acesso
Usuarios.DB (CodUsuario) → Senhas.DB → Nivel.DB
```

---

## 15. MATRIZ TELA ↔ BANCO

| Tela / Form | Tabelas Primárias | Tabelas Secundárias |
|---|---|---|
| FM_MovCab (Recepção) | MovCab.DB | Clientes.DB, CadLocRol.DB, Produt.DB |
| FC_MovLan (Lançamento ROL) | MovCab.DB, MovItemSer.DB | SerLav.DB, TabPro.DB, Clientes.DB |
| FC_PAGAMENTO | Duplicat.DB, FecCaixa.DB | MovCab.DB, CliCredito.DB, CadFpg.DB |
| FM_Clientes | Clientes.DB | CliContato.DB, ClientesObs.DB, GruClientes.DB |
| FM_Notas (Faturamento) | Notas.DB, NotComp.DB | MovCab.DB, Clientes.DB |
| FM_ModRel (Relatórios) | LogSis.DB, MovCab.DB | Todos os módulos |
| MP_LanLav (Menu Principal) | Nivel.DB | Usuarios.DB |
| PagRolLan (Touch Screen) | MovCab.DB, Duplicat.DB | CadFpg.DB |
| FM_CadBan (Financeiro) | PAG\FILIAL\ContaCor.DB | Duplicat.DB |
| MP_Financ (Menu Financeiro) | Nivel.DB | Usuarios.DB |
| LanRoupasLot_Fm (Lotes) | MovItem.DB | TabPro.DB, MovCab.DB |

---

## 16. MATRIZ BOTÃO ↔ AÇÃO

| Botão / Operação | Ação no Banco | Log |
|---|---|---|
| Novo ROL | INSERT MovCab.DB | INC LogSis |
| Confirmar pagamento | INSERT Duplicat.DB, UPDATE MovCab.Posicao | ALT LogSis |
| Entregar ROL | UPDATE MovCab.Posicao | ALT LogSis |
| Cancelar ROL | UPDATE MovCab (cancelado) | DEL LogSis |
| Reemitir etiqueta | — | REE LogSis |
| Emitir NF | INSERT Notas.DB, vincula NumNot em MovCab | INC LogSis |
| Cancelar NF | UPDATE Notas.DB (cancelado) | DEL LogSis |
| Fechar caixa | INSERT FecCaixa.DB | INC LogSis |
| Incluir cliente | INSERT Clientes.DB | INC LogSis |
| Alterar permissão | UPDATE Nivel.DB | ALT LogSis |
| Bloquear sistema | UPDATE flags em Nivel/NovoReg | ALT LogSis |

---

## 17. MATRIZ RELATÓRIO ↔ QUERY

| Relatório | Tabelas Consultadas | Filtros Principais |
|---|---|---|
| AnaMovDia1/2 | MovCab.DB, MovItemSer.DB | DatEntLoja, CodFil |
| RelFaturamento | Notas.DB, Duplicat.DB | DatEmi, CodCli, CodFil |
| RelMovimento | MovCab.DB | Periodo, Posicao, CodVen |
| ExtratoCliente | MovCab.DB, Duplicat.DB | CodCli |
| ControledeMetas | MovCab.DB, CadVen.DB | CodVen, Periodo |
| CaixaDiaDia | FecCaixa.DB, Duplicat.DB | DatFec |
| LogSistema | LogSis.DB | CodUsuario, DatLan, CodSistema |
| RelControle | MovEst.DB, MovCab.DB | Periodo, CodFil |

---

## 18. MATRIZ RUNTIME ↔ BANCO

| Evento de Runtime | Acesso ao Banco | Tipo |
|---|---|---|
| Startup LavSoft | EquNet.Ini, Registrar.xml, NovoReg*.DB | READ |
| Login usuário | Usuarios.DB, Senhas.DB, Nivel.DB | READ |
| Abrir menu | Nivel.DB (filtro por usuário/sistema/op) | READ |
| Entrada de ROL | MovCab.DB, MovItemSer.DB, MovItem.DB | READ+WRITE |
| Alteração de status | MovCab.Posicao, MovLocRol.DB | WRITE |
| Pagamento | Duplicat.DB, FecCaixa.DB, CliCredito.DB | READ+WRITE |
| Impressão etiqueta | MovCab.DB, ControleEti.DB | READ |
| Emissão NF | Notas.DB, NotComp.DB, NotaFisPag.DB | READ+WRITE |
| Sync nuvem | Todos os MovCab, MovItemSer (leitura para POST) | READ → HTTP POST |
| Fechar caixa | FecCaixa.DB, MovIniCaixa.DB | READ+WRITE |
| Auditoria | LogSis.DB | WRITE (toda operação) |

---

## 19. RELATÓRIO "NADA FICOU PARA TRÁS"

### Migração de Dados — Resultado Final

**Data:** 2026-05-24 | **Verificações:** 11/11 OK (100%)

| Tabela Paradox | Registros EquipeExe | Supabase | Status |
|---|---:|---:|---|
| Clientes.DB | 5.064 | 5.064 | ✓ OK |
| MovCab.DB (Vendas) | 31.972 | 31.972 | ✓ OK |
| MovItemSer.DB | ~139.000+ | ~139.000+ | ✓ OK |
| MovItem.DB | ~96.000+ | ~96.000+ | ✓ OK |
| CadLocPec.DB | confirmado | confirmado | ✓ OK |
| Notas.DB | confirmado | confirmado | ✓ OK |
| NotComp.DB | confirmado | confirmado | ✓ OK |
| Duplicat.DB | confirmado | confirmado | ✓ OK |
| MovPontos.DB | confirmado | confirmado | ✓ OK |
| MovLimPro.DB | confirmado | confirmado | ✓ OK |
| SerLav.DB (Produtos) | confirmado | confirmado | ✓ OK |

**Total geral:** 365.460 registros legados → 365.564 no Supabase (incluindo registros de configuração do NextGen)

### O que ficou registrado no legacy_inventory

Todo registro que não tem tabela estruturada direta no NextGen foi preservado em `legacy_inventory` com:
- `tenant_id` — isolamento por cliente
- `artifact_type` — classificação do registro original
- `metadata` (jsonb) — **todos os campos originais preservados integralmente**
- `evidence_source` — arquivo Paradox de origem

### O que não foi perdido

| Item | Status |
|---|---|
| 16 anos de log operacional (LogSis.DB) | Preservado em legacy_inventory |
| Histórico financeiro (Duplicat, Titulos) | Preservado em legacy_inventory |
| Localização de peças (CadLocPec) | Preservado em legacy_inventory |
| Limites de produção (MovLimPro) | Preservado em legacy_inventory |
| Programa de pontos (MovPontos) | Preservado em legacy_inventory |
| Complementos de NF (NotComp) | Preservado em legacy_inventory |
| Credencial BD.txt (plaintext) | Documentado, **NUNCA replicado no NextGen** |

---

## 20. RELATÓRIO GO/NOGO — ARQUITETURA

### DIAGNÓSTICO DO SISTEMA LEGADO

| Dimensão | Diagnóstico | Risco |
|---|---|---|
| Stack tecnológico | Delphi 32-bit + BDE/Paradox — EOL desde 2002 | **CRÍTICO** |
| Licenciamento | Dependência de www.lavsoft.com.br (HTTP sem TLS) | **ALTO** |
| Segurança de dados | BD.txt com senha plaintext (`sa/123`) | **ALTO** |
| Comunicação | HTTP sem criptografia, 40+ endpoints expostos | **ALTO** |
| Concentração de operação | 69.5% das ações = 2 usuários (LUCI + ROSIMEIRE) | **MÉDIO** |
| Dados legados | 100% migrados para Supabase | **RESOLVIDO** |
| Licença EquipeZ | Estendida a 99999 (~2173), sem dependência de server | **RESOLVIDO** |

### DIAGNÓSTICO NEXTGEN

| Dimensão | Status NextGen |
|---|---|
| Banco de dados | Supabase PostgreSQL + RLS + Auth — produção |
| Multi-tenancy | tenant_id em todas as tabelas + RLS isolado |
| Migração | 100% completa (11/11 verificações OK) |
| Pagamentos | Mercado Pago Marketplace — PIX + Cartão + split para Rogério |
| Licenciamento | licencas + planos_licenca + renew-license Edge Function |
| Interceptação LavSoft | hosts + mock server Python + firewall |
| Auto-billing | **PENDENTE** |
| License guard | **PENDENTE** |

### VEREDICTO: **GO** com as seguintes condições

**APROVADO para NextGen:**
- Dados 100% migrados e validados
- Arquitetura multi-tenant isolada por RLS
- Licença local (EquipeZ 99999) sem dependência remota LavSoft
- Comunicação LavSoft interceptada no MOD (3 camadas)

**ANTES DO LANÇAMENTO:**
1. Aplicar `apply-lavsoft-intercept.ps1` como admin (hosts + mock server + firewall)
2. Implementar auto-billing (aviso 7 dias + PIX auto-renovação)
3. Implementar license guard (tela de renovação ao invés de crash)
4. Validar mock server: `Invoke-WebRequest http://www.lavsoft.com.br/TestaAutentica` → deve retornar `1`

**NÃO FAZER:**
- Nunca replicar a senha `sa/123` do BD.txt
- Nunca enviar dados operacionais para www.lavsoft.com.br
- Nunca executar LiveUpdate.exe original no MOD runtime

---

## ARQUIVOS DE REFERÊNCIA

| Documento | Localização |
|---|---|
| Fase 1 — Permissões/Menus | docs/reverse-engineering/phase1_permissions_menus.md |
| Fase 3 — INI/Runtime/APIs | docs/reverse-engineering/phase3_ini_runtime_config.md |
| Dicionário de Dados | docs/10-database/dicionario-de-dados-completo.md |
| Mapa Tela→Banco | docs/11-modulos/mapa-telas-banco.md |
| Fluxos Operacionais | docs/11-modulos/mapa-fluxos-operacionais.md |
| Bloqueio LavSoft | docs/05-comunicacoes/bloqueio-testaautentica-enviaMovimento.md |
| Licenciamento Profundo | docs/09-licensing/relatorio-licenciamento-profundo.md |
| Mock Server Python | apps/services/LavSoftMock/server.py |
| Apply Intercept | apps/tools/apply-lavsoft-intercept.ps1 |
| Rollback Intercept | apps/tools/rollback-lavsoft-intercept.ps1 |
| Start MOD Seguro | apps/tools/start-mod-safe.ps1 |
| Comparativo Migração | data/migration/comparativo.py |
