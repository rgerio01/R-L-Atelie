# Phase 1 — Permissions, Menus & System Structure
# Extracted: 2026-05-24 via pypxlib (direct read, zero assumptions)

## SISTEMAS CADASTRADOS (32 total)

### Sistemas Principais com Controle de Senha (11)
| Código | Nome | Executável |
|--------|------|-----------|
| ALERTA | Alerta 2000 | Alerta |
| ESTOQUE | Estoque 2000 | Estoque |
| FINANCEIRO | Financeiro 2000 | Financ |
| FRANLAV | Franquia LavSoft | - |
| LAVSOFT | LavSoft 2000 | LavSoft ← SISTEMA PRINCIPAL |
| MANUTENCAO | Manutenção 2000 | - |
| PCP | Processo Controle Produção | PCP |
| PECLAV | Controle de Peças LavSoft | - |
| PECWEB | Controle Peça Online | PecWeb |
| SAT | Sistema Atendimento Telefônico | SAT |
| SENHAS | Sistema Gerencial de Senhas | - |

### Sistemas de Suporte (21)
BACKUP, CAMERA, CEP, CONEXAO, ENVEMAILEQ, EQUCONFIG, EQUKILL, EQUMAIL,
EXPEDICAO, EXPWASHTEC, IDCT, IMPAUTOMLA, LAVFACILLA, LAVMACHINE, LSLAVSOFT,
LSPT, MANPONTOS, MOBILAV, TRAVARELOG, TRIAGEM, UNITRONICS

## USUÁRIOS CADASTRADOS (9 ativos)

| Código | Nome | Grupo | Tipo | Obs |
|--------|------|-------|------|-----|
| BRENA | BRENA | OPERA | U | Operador |
| BRUNA | BRUNA | OPERA | S | SuperUser |
| CID | APARECIDA LEMOS RIBEIRO | OPERA | U | Operador |
| EDU | CLEBER EDUARDO LACERDA | OPERA | U | Operador |
| FAT | MARIA DE FATIMA RIBEIRO RAMOS | OPERA | U | Operador |
| GABRIELA | GABRIELA | MASTE | U | MASTER — admin geral |
| LUCI | LUCIDALVA ALVES DE OLIVEIRA | OPERA | S | Proprietária — SuperUser |
| MICHELE | MICHELE | OPERA | U | Operador |
| ROSIMEIRE | ROSIMEIRE | OPERA | S | SuperUser |

Inativos no Nivel.DB: ANA, PONTO, ATENDJR, FRANQ, PERFATEND, SUPERV

## VENDEDORES (CadVen.DB — 13 total)
- 00001 ANA — CANCELADO
- 00002 LOPES — CANCELADO
- 00003 REGINA — CANCELADO
- 00004 EDUARDO — CANCELADO
- 00005 LUCI — ATIVO (vendedora principal)
- 00006 a 00013 — status variado

## TELAS/JANELAS (Janela.DB — 16 telas)

| Sistema | Janela/Form | Descrição |
|---------|------------|-----------|
| ALERTA | MenuAlertaEquipe | Menu do sistema de alertas |
| FINANCEIRO | FM_CadBan | Lançamento de Contas Correntes |
| FINANCEIRO | MP_Financ | Menu Principal Financeiro |
| LAVFACILLA | FC_MovLan | Lançamento rápido (modo simplificado) |
| LAVSOFT | FC_ConTab | Consultas Rápidas — atalho F11 |
| LAVSOFT | FC_MovLan | Lançamento do ROL (ordem de serviço) |
| LAVSOFT | FC_PAGAMENTO | Tela de Pagamento |
| LAVSOFT | FM_Clientes | Cadastro de Clientes |
| LAVSOFT | FM_ModRel | Filtro de Relatórios |
| LAVSOFT | FM_MovCab | Menu Recepção (entrada de peças) |
| LAVSOFT | FM_Notas | Faturamento / Notas |
| LAVSOFT | MP_LanLav | Menu Principal do LavSoft |
| LAVSOFT | PagRolLan | Pagamento Touch Screen |
| PECLAV | LanRoupasLot_Fm | Menu de Lotes de Roupas |
| PECLAV | MP_Menu | Menu Principal PecLav |
| PECWEB | MP_Menu | Menu Principal PecWeb |

## MAPA DE MENUS — LAVSOFT (144 operações/telas)

### Recepção / Entrada
- EntradaRol — entrada de nova OS
- EntradaEstoque — entrada no estoque
- MenuEntradaRol — menu de entrada

### Lançamento de OS
- LancamentoRol — lançamento principal
- Lancamentos — lançamentos gerais
- MenuLancamentoRol — menu de lançamento
- FC_MovLan — tela de lançamento rápido

### Pagamento
- Pagamento — pagamento de OS
- PagamentoRol — pagamento por ROL
- PagamentodeVariosRols — pagamento múltiplo
- CancPag — cancelamento de pagamento
- DevoluoPagamento — devolução de pagamento
- PagRolLan — pagamento touch screen

### Clientes
- ClientesCad — cadastro de clientes
- AlteraCliente — alteração de cliente
- ExtratoCliente — extrato do cliente
- MovGrupoClientes — movimentação por grupo

### Faturamento / Notas
- Faturamento — faturamento geral
- NotaFiscal — emissão de NF
- EmitirNotaFiscal — emissão direta
- CancelamentoNotaFiscal — cancelamento de NF

### Retirada / Entrega
- MenuRetiradas — menu de retiradas
- EntregaRol — entrega de OS
- EntregaVariosRol — entrega múltipla
- EntregaporPecas — entrega por peças
- MarcarEntrega — marcação de entrega

### Controles
- ControledeCaixa — controle do caixa
- ControledeLavagem — controle de lavagem
- ControledeMetas — metas de vendas
- CaixaDiaDia — fechamento do caixa

### Relatórios (20+ operações)
- AnaMovDia1, AnaMovDia2... — análise diária
- RelFaturamento — relatório de faturamento
- RelMovimento — relatório de movimento
- RelControle — relatório de controle
- FM_ModRel — filtro de relatórios

### Administrativo
- Usuarios — gestão de usuários
- Parametros — parâmetros do sistema
- Manutencao — manutenção
- LogSistema — visualizar log
- Inclusao / Alteracao / Cancelamento — permissões CRUD

### Outros (mapeados no Nivel.DB)
- AnaMovDia1, AnaMovDia2, CFAbertura1, CFFechamento1
- Anotacoes1, Consultas, ConsultaRapida
- ImpAutom — impressão automática
- CFOP, ISS — configurações fiscais

## PERMISSÕES (Nivel.DB — 438 registros)

### Estrutura dos Campos
- CodUsuario: usuário
- CodFil: filial (QL001 = filial principal)
- CodSistema: sistema (LAVSOFT)
- Op: operação/tela (AlteraCliente1, Pagamento...)
- NivelI: pode Inserir (0/1)
- NivelA: pode Alterar (0/1)
- NivelE: pode Excluir (0/1)
- NivelT: acesso Total (0/1)

### Usuários com permissões mapeadas
- ATENDJR — atendente júnior (restrito)
- FRANQ — franqueado (restrito)
- PERFATEND — perfil atendimento
- PONTO — acesso ao ponto eletrônico
- SUPERV — supervisor

## LOG DO SISTEMA (LogSis.DB)

- **Total registros:** 112.615
- **Período:** 2010-04-26 → 2026-05-21 (16 anos)
- **Última operação:** LUCI reemitiu ROL 31798 (21/05/2026)
- **Tipos de log:** INC (Inclusão), ALT (Alteração), REE (Reemissão), DEL (Deleção)

## PARÂMETROS (Parametros.DB — 610 parâmetros)

- Tipos: ST (String), IN (Integer), CB (Checkbox/boolean)
- Sistemas cobertos: ALERTA, FINANCEIRO, LAVSOFT + outros
- Exemplo FINANCEIRO: até 9 bancos configuráveis (Agência, Banco, CC)
