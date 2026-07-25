# Auditoria Nada Fica Para Tras - EquipeExe -> Atelie Linux

Data: 2026-05-26

## Resultado executivo

A ISO Linux atual ja cobre o nucleo operacional reconstruido: login, clientes, servicos/precos, ROL, orcamentos, agenda, caixa, financeiro basico, relatorios basicos, usuarios, configuracoes e importacao inicial de clientes/servicos reais.

Porem, a regra **NADA FICA PARA TRAS** ainda nao pode ser marcada como concluida absoluta. A auditoria encontrou evidencias objetivas de funcionalidades, regras, telas, permissoes, impressao e modulos legados que ainda nao estao completamente refletidos no aplicativo Linux.

Status honesto:

- **Operacao basica PDV/ROL no Linux:** GO parcial.
- **Migracao de clientes e tabela de precos:** GO inicial.
- **Paridade total de UI:** NO-GO.
- **Paridade total de impressao:** NO-GO.
- **Paridade total de fiscal/SAT/NFE:** NO-GO.
- **Paridade total de estoque/producao/terceirizacao/pacotes/pontos:** NO-GO.
- **Paridade total de permissoes por botao/menu:** NO-GO.

## Fontes verificadas

### Original EquipeExe

Diretorio: `D:\AtelieProd\Equipexe`

Inventario por extensao observado:

- `.DB`: 962 tabelas Paradox.
- `.PX`: 944 indices primarios.
- `.XG*` / `.YG*`: milhares de indices secundarios.
- `.exe`: 47 executaveis.
- `.Ini`: 30 configuracoes.
- `.BMP`: 1.491 assets.
- `.jpg`: 102 assets.
- `.gif`: 5 assets.
- `.txt`: 46.752 arquivos.

### Artefatos MOD de engenharia reversa

Arquivos principais usados:

- `docs/11-modulos/ui-reverse-engineering/relatorio-visibilidade-total-ui.md`
- `docs/11-modulos/ui-reverse-engineering/mapa-menus-submenus-acoes-consolidado.csv`
- `docs/11-modulos/ui-reverse-engineering/mapa-telas-funcional-consolidado.csv`
- `docs/11-modulos/ui-reverse-engineering/mapa-permissoes-ui-consolidado.csv`
- `docs/11-modulos/ui-reverse-engineering/mapa-ui-banco-interligacoes.csv`
- `docs/11-modulos/matriz-relatorios.csv`
- `docs/10-database/dicionario-paradox-campo-a-campo.csv`
- `docs/11-modulos/mapa-clientes-os-produtos.csv`
- `docs/07-observabilidade/relatorio-execucao-dinamica-lavsoft.md`
- `docs/07-observabilidade/relatorio-execucao-dinamica-modulos-principais.md`
- `final-execution-parity/evidence/ui-parity-evidence.json`
- `final-execution-parity/evidence/print-parity-evidence.json`

## Evidencias quantitativas

Do relatorio de visibilidade total:

- Telas/acoes/textos funcionais candidatos: 18.724.
- Menus/submenus/acoes consolidados: 1.936.
- Blocos Delphi TPF0 extraidos: 521.
- Vinculos UI -> banco/SQL: 757.
- Assets visuais inventariados: 1.598.
- Permissoes UI consolidadas: 144.
- Relatorios/impressao candidatos: 1.362.
- Campos Paradox mapeados: 4.996.

Distribuicao de menus/acoes por dominio:

- cadastro: 469.
- operacional/outro: 403.
- fiscal: 264.
- estoque/produto: 201.
- auth/permissao: 190.
- lavanderia/ROL: 183.
- financeiro/caixa: 142.
- relatorio/impressao: 79.
- sync/update/API: 5.

Distribuicao por executavel:

- LavSoft: 6.781 textos/acoes, 601 menus/acoes, 329 layouts TPF0.
- LavFacilLan: 5.576 textos/acoes, 237 menus/acoes, 174 layouts TPF0.
- Estoque: 1.749 textos/acoes, 318 menus/acoes, 18 layouts TPF0.
- Financeiro: 972 textos/acoes, 288 menus/acoes.
- SAT: 2.673 textos/acoes, 342 menus/acoes.
- NFE: 776 textos/acoes, 119 menus/acoes.
- Gerenciador: 197 textos/acoes, 31 menus/acoes.

## Cobertura atual do Linux

Implementado no app Linux atual:

- Login `gabriela / 12345`.
- Usuarios e perfis basicos.
- Clientes com CRUD.
- Importacao inicial de `CLIENTES.csv`.
- Servicos/precos com CRUD.
- Importacao inicial de `Exportar-Servicos.csv`.
- ROL: criar, listar, itens, pronta, entregar, pagar, cancelar, recibo.
- Orcamentos: criar, listar, itens, converter em ROL, cancelar.
- Agenda: criar, listar, alterar, cancelar.
- Caixa: abrir, fechar, suprimento, sangria, historico/movimentos.
- Financeiro basico: listar, resumo, receber.
- Relatorios basicos: movimento dia, movimento periodo, ROLs abertos, entrega, caixa dia, clientes em debito, servicos periodo, frequencia clientes.
- Configuracoes basicas de empresa.
- ISO/kiosk Linux com Chromium e backend local.

## Lacunas objetivas encontradas

### 1. Regras finas de ROL/entrada

Evidencia: `EquLav00001.Ini`.

Parametros encontrados e ainda nao refletidos integralmente:

- `HabF7Pagto=1`: pagamento por F7.
- `F4AlteraPreco=2`: regra para alteracao de preco.
- `ChkBox_F2MenLan=1`: menu de lancamento por F2.
- `ChkBox_F11BloqEstat=0`: comportamento F11/estatistica.
- `QdeViasRol=2`, `QdeViasPag=1`, `QdeViasEnt=1`: vias de impressao.
- `ObrigMotDesc=1`: motivo obrigatorio para desconto.
- `FormaPedirVen=3-Pede Senha`: vendedor exige senha.
- `PedeSenhaCred=0`, `PedeSenhaDesc=0`: regras de senha para credito/desconto.
- `ChkBox_Cor=1`, `ChkBox_Car=1`, `ChkBox_Def=1`, `ChkBox_Marca=1`, `ChkBox_Peso=1`, `ChkBox_Obs=1`: campos operacionais habilitados.
- `ChkBox_Prazo=1`, `ChkBox_PedeAdito=1`, `ChkBox_AvisoPagto=1`: fluxo de prazo/adicional/aviso.
- `HorEnt`, `HorLim`, `HorEnt1..7`: horarios por dia.
- `EntSabado=1`, `EntDomingo=1`: regra de entrega em fim de semana.

Status Linux: parcialmente coberto. O formulario possui cor/marca/defeito/observacao, mas nao reproduz todos os atalhos, bloqueios, senhas, vias, horarios e validacoes condicionais.

### 2. Tabelas de ROL muito mais amplas que a modelagem Linux

Evidencia: `Lav\FILIAL`.

Tabelas fortes no legado:

- `MovCab.DB` com 14.487.552 bytes.
- `MovItem.DB` com 36.407.296 bytes.
- `MovItemSer.DB` com 37.029.888 bytes.
- `Notas.DB` com 13.043.712 bytes.
- `NotComp.DB` com 5.113.856 bytes.
- `CadLocPec.DB` com 9.209.856 bytes.
- `MovPontos.DB` com 264.192 bytes.
- alem de `MovLocRol`, `ControleEti`, `IndenRol`, `MovRetirada`, `MovSaida`, `MovTerc`, `MovPacot`, `ProcesMaqMovCab`, `ProcesMaqMovItem`.

Status Linux: ROL foi reconstruida como cabecalho + itens + historico. Ainda faltam ramificacoes de:

- localizacao por peca;
- controle de etiquetas;
- pontos;
- pacotes;
- retirada;
- saida;
- terceirizacao;
- processamento por maquina;
- indenizacao;
- controle de producao;
- estoque no cliente.

### 3. Estoque/produto

Evidencia:

- 201 menus/acoes em `estoque/produto`.
- 318 menus/acoes do executavel `Estoque`.
- Tabelas fortes: `ProdEst`, `MovEst`, `MovEstCan`, `MovEstEnc`, `MovEstLan`, `TabProdEst`, `ProdEstKit`, `ProdEstPac`.

Status Linux: possui servicos/precos, mas nao possui modulo completo de estoque.

Falta:

- cadastro de produtos de estoque;
- entrada/baixa;
- encerramento de estoque;
- estoque minimo/maximo;
- kits/pacotes;
- movimentacao e cancelamento;
- relatorios de estoque;
- vinculo de produto operacional com produto de estoque.

### 4. Fiscal, SAT, NFE e nota

Evidencia:

- 264 menus/acoes no dominio fiscal.
- SAT: 342 menus/acoes e 122 vinculos banco.
- NFE: 119 menus/acoes.
- INI possui secao `[NFE]` com `CodSerNfe=05627`, `EmitNotaNfe=0` e dezenas de parametros fiscais.
- Modulos fiscais nao foram validados dinamicamente na rodada curta por risco de certificado/webservice/hardware.

Status Linux: sem emissao fiscal real.

Falta:

- SAT;
- NFE/RPS;
- nota fiscal;
- cancelamento fiscal;
- leitura/rotinas fiscais;
- parametros fiscais;
- integracao com certificado/webservices/hardware;
- logs fiscais e contingencia.

### 5. Impressao fiel

Evidencia:

- `print-parity-evidence.json`: status `NO-GO`, `validated=false`.
- `LavSoft` acionou `splwow64.exe` em execucao dinamica, indicando impressao Windows 32/64-bit.
- INI aponta `ImpLancamento=\\DESKTOP-KRCPB44\rolss`, `ImpPagamento=\\DESKTOP-KRCPB44\rolss`.
- `TipoEti=ARGOXRABBIT214`.
- `ModeloRol=24-Mecaf-IM423-TS-003`.
- `ModeloFitas=26-Argox (PPLB) Lay Out 2`.
- `Argox.ini` usa comandos PPLB: `N`, `q300`, `D7`, `S2`, `OC1`.

Status Linux: recibo HTML basico. Nao ha paridade de impressao.

Falta:

- layout exato do ROL;
- layout de pagamento;
- layout de entrega;
- etiqueta/fita Argox PPLB;
- quantidade de vias;
- corte;
- codigos de barra;
- impressao direta em porta/rede;
- validacao em impressora real.

### 6. Relatorios

Evidencia:

- `matriz-relatorios.csv`: 1.362 candidatos de relatorio/impressao.
- Menus legados listam Movimento Analitico/Sintetico, Entrada/Saida, Movimento por Produto, Servicos/Peso, Servico Mensal, Faturamento, Entrega, Conferencia de ROL, Controle de Pontos, Estoque no Cliente, Fatores de Precos, Frequencia de Cliente, Previsao de Entrega, Comissao ROL/Pagamento, RSL etc.

Status Linux: 8 relatorios basicos.

Falta:

- relatorios analiticos/sinteticos completos;
- filtros equivalentes;
- ordenacoes legadas;
- exportacao/impressao;
- agrupamentos;
- relatorios por produto/servico/peso/pontos/comissao/faturamento/NF/estoque no cliente.

### 7. Permissoes por botao/menu

Evidencia:

- `mapa-permissoes-ui-consolidado.csv`: 144 permissoes.
- `menus-permissoes-niveldb.csv` e `Nivel.DB` indicam controle por operacao, sistema, usuario e filial.
- Exemplos: `AlteraCliente1`, `AnaMovDia1`, `Anotacoes1`, `CFAbertura1`, rotinas de cancelamento e fiscal.

Status Linux: perfis basicos por permissao ampla.

Falta:

- permissao por rotina/menu/botao;
- semantica dos niveis `I/A/E/T`;
- usuario por filial;
- bloqueio dinamico de componentes conforme permissao;
- auditoria por operacao equivalente.

### 8. UI visual, posicoes e fluxo de clique

Evidencia:

- `ui-parity-evidence.json`: status `NO-GO`, `validated=false`.
- 521 blocos Delphi TPF0 extraidos.
- 757 vinculos UI -> banco.
- 1.598 assets visuais.

Status Linux: UI operacional moderna, nao replica fielmente posicoes, grids, botoes e telas Delphi.

Falta:

- captura dinamica tela a tela;
- hierarquia real de menus;
- posicoes dos campos;
- botoes e atalhos;
- mensagens de erro/confirmacao;
- ordem de foco;
- duplo clique/grids;
- comparacao visual.

### 9. Comunicacao externa, sync e update

Evidencia:

- Execucao dinamica curta detectou conexoes HTTP para `191.6.218.152:80` em `LavFacilLan` e `Estoque`.
- `GeraNuvem=1` em INI.
- Existem diretorios `Sincroniza`, `GraficoWeb`, `GraficoWebLog`, `Importa`, `Transfere`.

Status Linux: offline-first local, sem equivalencia completa de sincronizacao legada.

Falta:

- classificar finalidade da comunicacao;
- bloquear/substituir endpoints inseguros;
- migrar sync/import/export/grafico web;
- criar painel administrativo equivalente ao `Gerenciador`.

### 10. Modulos ainda nao reconstruidos integralmente

Cobertura insuficiente:

- Estoque completo.
- Financeiro completo com titulos, boletos, duplicatas, contas rapidas, cobranca, faturamento.
- SAT.
- NFE/RPS.
- Gerenciador/sync/update/logs/tarefas.
- Terceirizacao.
- Passadoria.
- Controle de lavagem/producao.
- Retirada/saida por peca.
- Controle de pontos.
- Etiquetas/fitas.
- Controle de localizacao.
- Comissoes.
- Biometria.
- Balanca.
- SMS/e-mail.
- Mala direta.
- Feriados/prazos avancados.
- Multi-filial real.

## Matriz de decisao

| Area | Cobertura Linux atual | Status |
|---|---:|---|
| Login basico | Alta | GO |
| Clientes importados | Alta inicial | GO parcial |
| Tabela de precos importada | Alta inicial | GO parcial |
| ROL basica | Media | GO parcial |
| Orcamento basico | Media | GO parcial |
| Agenda basica | Media | GO parcial |
| Caixa basico | Media | GO parcial |
| Financeiro basico | Baixa-media | NO-GO para paridade |
| Relatorios | Baixa | NO-GO |
| Impressao | Baixa | NO-GO |
| Estoque | Baixa | NO-GO |
| Fiscal/SAT/NFE | Ausente | NO-GO |
| Permissoes por operacao | Baixa | NO-GO |
| UI/menus/submenus fieis | Baixa | NO-GO |
| Regras ocultas INI | Baixa-media | NO-GO |
| Sync/update/gerenciador | Baixa | NO-GO |

## Proxima execucao obrigatoria

Para cumprir literalmente **NADA FICA PARA TRAS**, a sequencia correta e:

1. Criar uma tabela de cobertura no banco novo para rastrear cada item legado.
2. Importar para essa tabela:
   - 1.936 menus/acoes;
   - 18.724 textos/telas/acoes;
   - 144 permissoes;
   - 1.362 relatorios/impressao;
   - 4.996 campos Paradox;
   - parametros `.INI` criticos.
3. Implementar modulo de parametrizacao legacy para carregar regras do `EquLav00001.Ini`.
4. Implementar atalhos F2/F4/F7/F8/F10/F11 e bloqueios correspondentes.
5. Implementar impressao ESC/POS/PPLB/Argox com layouts reais.
6. Implementar estoque e fiscal como modulos proprios.
7. Capturar dinamicamente telas do legado e anexar evidencia aos gates `ui` e `print`.
8. So marcar GO quando cada item tiver: `migrado`, `substituido com justificativa`, ou `descartado por decisao explicita`.

## Conclusao

A auditoria confirma que o aplicativo Linux atual e uma base funcional importante, mas ainda nao e uma substituicao absoluta do EquipeExe.

Pela regra do projeto, nada pode ser tratado como concluido enquanto os gates de UI, impressao, fiscal, permissoes, estoque, relatorios e regras INI estiverem em NO-GO.

