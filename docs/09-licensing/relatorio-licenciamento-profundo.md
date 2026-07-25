# Relatorio de Licenciamento Profundo - EquipeExe

Data: 2026-05-23

## Escopo

Analise executada em modo controlado, sem alterar o sistema original.

- Original preservado: `D:\AtelieProd\Equipexe`
- Runtime MOD analisado: `D:\AtelieProd\MOD\apps\legacy-runtime\Equipexe`
- Banco analisado a partir de copia somente leitura: `D:\AtelieProd\MOD\data\original-readonly\Equipexe`

Esta etapa nao teve objetivo de burlar, remover ou contornar licenciamento. O objetivo foi identificar onde o legado guarda, calcula, valida ou sincroniza sinais de licenca, autenticacao, bloqueio, registro de estacao e atualizacao.

## Artefatos gerados

- `sinais-licenciamento-executaveis.csv`
- `mapa-tabelas-licenciamento.csv`
- `sinais-licenciamento-configs.csv`
- `endpoints-licenciamento-autenticacao-atualizacao.csv`
- `amostras-mascaradas-tabelas-licenciamento-auth.csv`
- `resumo-licenciamento-profundo.csv`

Scripts criados:

- `D:\AtelieProd\MOD\apps\tools\analyze-licensing-static.py`
- `D:\AtelieProd\MOD\apps\tools\sample-licensing-tables-32bit.ps1`

## Resumo numerico

- Sinais em executaveis: 2.170
- Sinais em schema Paradox: 248
- Sinais em configuracoes relevantes: 17
- URLs/endpoints extraidos e normalizados: 71
- Amostras mascaradas de tabelas auth/licensing: 143

Observacao: a coleta de configuracoes foi filtrada para arquivos relevantes (`Registrar.xml`, `EquNet.ini`, `EquSenha.ini` e politica MOD de update), evitando ruido de arquivos de runtime.

## Achados fortes

### 1. Registro de estacao e hardware binding

Arquivo:

- `D:\AtelieProd\Equipexe\Registrar.xml`

Conteudo relevante:

- `MacAddress`
- `Nome`
- `Usuario`
- `VersaoWindows`
- `CodLojaOriginal`

Interpretacao:

Este arquivo parece ser uma persistencia local de identidade da estacao. Ele provavelmente participa de registro de maquina, associacao com loja/filial ou validacao remota. O campo `CodLojaOriginal` liga a maquina a um identificador operacional.

### 2. Configuracao codificada no EquNet.ini

Arquivo:

- `D:\AtelieProd\Equipexe\EquNet.Ini`

Campos relevantes encontrados:

- `BloqFran=3`
- `CampoCC=EQU0000001215`
- `EquipeZ=46147`
- `TesteW1=46147`
- `TesteB2=33`
- `Teste2T=2`
- `TesteB3=5`
- `Tested2=33g4:j98;@`
- `Tested4=14g4;j98;@`
- `[COMPUTADOR] NOME=DESKTOP-KRCPB44`

Interpretacao:

Ha forte indicio de parametros codificados relacionados a loja/equipe/estacao. `CampoCC=EQU0000001215` conversa com `CodLojaOriginal=0000001215` do `Registrar.xml`. Os campos `Teste*`, `EquipeZ` e `TesteW1` devem ser tratados como candidatos a segredo/configuracao codificada ate validacao dinamica.

### 3. Tabelas Paradox candidatas a licenciamento/registro

Tabelas fortes:

- `Ger\Dados\NovoReg.DB`
- `Ger\Dados\NovoReg.BD.DB`
- `Ger\Dados\NovoReg.db.DB`
- `Ger\Dados\NovoRegBD.DB`
- `Ger\Filial\NovoReg.DB`
- `Lav\FILIAL\NovoRegLavFilial.DB`

Schema:

- As tabelas `NovoReg*` possuem coluna `NovoReg`.
- `CadCart.DB` possui coluna `Licenca`.
- `Estruturas\Inis.DB` possui coluna `ATIVACAO`.

Leitura atual:

- As tabelas `NovoReg*` acessiveis em `Ger\Dados` e `Ger\Filial` estao sem registros na amostra atual.
- `NovoRegLavFilial` retornou erro do driver Paradox `9499`, indicando possivel limitacao/corrupcao/peculiaridade de driver.
- `Usuarios`, `Nivel` e `Senhas` foram lidas com mascaramento, confirmando persistencia de autenticacao/permissao local.

### 4. Executaveis com sinais diretos de licenciamento/bloqueio

Sinais mais relevantes:

- `LavSoft.exe`
  - `NovoReg`
  - `NovoReg.Db`
  - `dbo.NovoReg`
  - `Tb_NovoReg`
  - `IncNovoRegCli`
  - `ExibeMenNovoReg`
  - `Desbloqueio pela internet`
  - `ControlaBloq.exe`
  - mensagens de bloqueio/desbloqueio

- `Financeiro.exe`
  - `ArquivoLicenca`
  - `FE_Licenca`
  - `Licenca`
  - `NovoReg.Db`
  - `Tb_NovoReg`
  - `Vencimento`
  - `CB_Bloqueia`
  - flags `BloqSec1` ate `BloqSec9`

- `SAT.exe`
  - `NovoReg.Db`
  - `NovoRegBD.db`
  - `Tb_NovoRegX`
  - `IncNovoRegCli`
  - flags de bloqueio por acao

- `Senhas.exe`
  - funcoes de bloquear/desbloquear sistemas e filiais
  - mensagens de operacao bloqueada/desbloqueada

Interpretacao:

O licenciamento legado nao parece estar concentrado em um unico arquivo simples. Os sinais apontam para uma combinacao de:

- registro local de estacao;
- parametros codificados em INI;
- tabelas `NovoReg*`;
- regras de bloqueio/desbloqueio por sistema/filial;
- validacao/comunicacao remota em pelo menos um modulo administrativo.

### 5. Gerenciador.exe como eixo remoto

O `Gerenciador.exe` contem endpoints HTTP relacionados a autenticacao, atualizacao, registro de estacao, sincronizacao e nuvem.

Endpoints relevantes:

- `http://www.lavsoft.com.br/AutenticaGerenciador`
- `http://www.lavsoft.com.br/TestaAutentica`
- `http://www.lavsoft.com.br/RegistraEstacao`
- `http://www.lavsoft.com.br/VerificaAtualizacoes`
- `http://www.lavsoft.com.br/TesteVerificaAtualizacoes`
- `http://www.lavsoft.com.br/DownloadDados`
- `http://www.lavsoft.com.br/ListarDispositivosPorFilial`
- `http://lavsoft.com.br/ws/Nuvem/Enviar`
- `http://lavsoft.com.br/ws/nuvem/v1/UploadArquivo.asmx`
- `http://www.lavsoft.com.br/ws/Equipe/v2/Geral.ASMX`
- `http://www.lavsoft.com.br/ws/Equipe/v2/AtuTabelas.asmx`

Interpretacao:

Este e o achado mais importante desta fase. O `Gerenciador.exe` provavelmente participa de autenticacao administrativa, registro de estacao, sincronizacao e atualizacao. Ainda nao ha payload capturado, portanto nao se deve afirmar que cada endpoint e licenciamento. A classificacao correta neste momento e dependencia remota critica com possivel impacto em auth/licensing/update/sync.

## Hipotese operacional do fluxo legado

Fluxo provavel, ainda a validar dinamicamente:

1. O sistema identifica a maquina por `Registrar.xml` e dados do computador.
2. O sistema cruza loja/estacao com parametros do `EquNet.ini`.
3. Modulos operacionais consultam tabelas `NovoReg*`, flags de bloqueio e permissoes.
4. `Senhas.exe` controla bloqueios/desbloqueios de sistemas/filiais/perfis.
5. `Gerenciador.exe` comunica com endpoints LavSoft para autenticar, registrar estacao, verificar atualizacoes e sincronizar dados.
6. `LavSoft.exe`, `Financeiro.exe` e `SAT.exe` aplicam bloqueios funcionais por vencimento, permissao, filial ou parametro.

## Relacao com a senha legada

A hipotese anterior de deslocamento ASCII `+1` continua registrada para senhas legadas simples. Nesta fase, a tabela `Usuarios` foi lida com valores mascarados; a amostra mostra senha com comprimento 5, mas nao confirma isoladamente que todas as senhas usam apenas `+1`.

Conclusao:

- o codec `+1` segue como hipotese forte para alguns casos;
- a senha atual do MOD (`12345`) pertence a nova autenticacao interna e nao depende do mecanismo legado;
- a investigacao de licenciamento nao deve depender de alterar senha legada diretamente.

## Riscos

- Dependencia remota em HTTP sem TLS aparente para endpoints LavSoft.
- Acoplamento entre atualizacao, autenticacao, sincronizacao e registro de estacao.
- Uso de BDE/Paradox 32-bit e driver ODBC 32-bit para leitura confiavel.
- Possivel corrupcao/peculiaridade em `NovoRegLavFilial`.
- Varias strings de bloqueio/desbloqueio espalhadas por modulos, indicando regra distribuida.
- Firewall de isolamento MOD ainda nao aplicado por falta de sessao administrativa elevada.

## Proximas validacoes recomendadas

1. Executar `Gerenciador.exe` no MOD sob captura de rede e ProcMon.
2. Capturar payload HTTP de `AutenticaGerenciador`, `TestaAutentica`, `RegistraEstacao` e `VerificaAtualizacoes`.
3. Confirmar se `Registrar.xml` e enviado nos endpoints remotos.
4. Verificar leitura/escrita de `EquNet.ini` durante inicializacao.
5. Monitorar acesso a `NovoReg*.DB` por processo, usando ProcMon.
6. Repetir leitura de `NovoRegLavFilial` com ferramenta Paradox/BDE alternativa.
7. Mapear no `Senhas.exe` quais telas executam bloquear/desbloquear sistemas e filiais.

## Conclusao

O licenciamento/autenticacao legado tem indicios de ser hibrido: local, por arquivos/tabelas, e remoto, via endpoints LavSoft. O componente remoto mais expressivo e o `Gerenciador.exe`; os componentes operacionais `LavSoft.exe`, `Financeiro.exe`, `SAT.exe` e `Senhas.exe` aplicam regras de bloqueio e permissao.

Para a nova geracao, a substituicao deve ser feita por modulo proprio de autenticacao/licenciamento no MOD, preservando o legado apenas como referencia de regra de negocio e nunca como dependencia administrativa futura.
