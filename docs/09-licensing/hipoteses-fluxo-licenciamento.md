# Hipoteses do Fluxo de Licenciamento Legado

Data: 2026-05-23

## Status

Documento de hipoteses tecnicas. Nada aqui deve ser tratado como regra confirmada ate validacao dinamica com ProcMon, captura HTTP e tracing dos modulos.

## Hipotese A - Registro local da estacao

Evidencias:

- `Registrar.xml` possui `MacAddress`, `Nome`, `Usuario`, `VersaoWindows` e `CodLojaOriginal`.
- `EquNet.ini` possui `[COMPUTADOR] NOME=...` e `CampoCC=EQU0000001215`.
- `Gerenciador.exe` contem endpoint `RegistraEstacao`.

Leitura:

O sistema provavelmente cria ou le uma identidade local de maquina e associa esta identidade a uma loja/filial. Essa identidade pode ser enviada ao servidor LavSoft quando ha internet.

## Hipotese B - Parametros codificados em INI

Evidencias:

- `EquNet.ini` contem campos `TesteB2`, `Teste2T`, `TesteB3`, `Tested2`, `Tested4`, `EquipeZ`, `TesteW1`.
- `CampoCC=EQU0000001215` possui relacao aparente com `CodLojaOriginal=0000001215`.

Leitura:

Os campos `Teste*` podem ser parametros internos codificados. A forma exata ainda nao foi confirmada. Nao ha evidencia suficiente para afirmar que eles sao senhas, licencas ou checksums; eles devem permanecer classificados como segredo/configuracao codificada ate teste dinamico.

## Hipotese C - NovoReg como armazenamento de registro/licenca

Evidencias:

- Executaveis referenciam `NovoReg`, `NovoReg.Db`, `dbo.NovoReg`, `Tb_NovoReg`, `Tb_IncNovoReg`.
- Existem tabelas Paradox `NovoReg*` com coluna `NovoReg`.
- `LavSoft.exe`, `Financeiro.exe` e `SAT.exe` referenciam essas estruturas.

Leitura:

`NovoReg` parece ser uma estrutura historica de registro/desbloqueio/licenca. Na amostra atual, as tabelas acessiveis estavam sem registros, entao a regra pode estar inativa, migrada para outro local, preenchida dinamicamente, dependente de filial, ou controlada remotamente.

## Hipotese D - Gerenciador como broker remoto

Evidencias:

- `Gerenciador.exe` contem endpoints de autenticacao, registro, atualizacao, download e nuvem.
- Endpoints principais:
  - `AutenticaGerenciador`
  - `TestaAutentica`
  - `RegistraEstacao`
  - `VerificaAtualizacoes`
  - `DownloadDados`
  - `ListarDispositivosPorFilial`
  - `ws/Nuvem/Enviar`

Leitura:

O `Gerenciador.exe` e forte candidato a broker administrativo/remoto. Ele pode controlar atualizacao, autenticacao remota, sincronizacao e registro de dispositivo. A funcao exata de licenciamento depende de captura de payload.

## Hipotese E - Bloqueio distribuido por modulo

Evidencias:

- `Senhas.exe` contem funcoes de bloquear/desbloquear sistemas e filiais.
- `Financeiro.exe` contem `ArquivoLicenca`, `FE_Licenca`, `BloqSec*`, `CB_Bloqueia` e `Vencimento`.
- `LavSoft.exe` contem `ControlaBloq.exe`, `Desbloqueio pela internet`, `NovoReg` e mensagens de bloqueio.
- `SAT.exe` contem flags de bloqueio operacional.

Leitura:

A regra de bloqueio nao parece centralizada em uma unica DLL. Ela parece distribuida entre permissao, filial, modulo, vencimento e comunicacao remota.

## Validacoes pendentes

- Captura de rede com payload HTTP.
- ProcMon filtrando `NovoReg`, `Registrar.xml`, `EquNet.ini` e endpoints.
- API Monitor/ETW para chamadas WinINet/WinHTTP/Winsock.
- Analise IL do `Gerenciador.exe`.
- Validacao de `NovoRegLavFilial` com ferramenta Paradox alternativa.
- Teste offline controlado apos isolamento MOD em firewall administrativo.
