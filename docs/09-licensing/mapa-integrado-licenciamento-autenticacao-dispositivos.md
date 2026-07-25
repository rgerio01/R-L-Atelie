# Mapa Integrado - Licenciamento, Autenticacao e Dispositivos

Data: 2026-05-23

## Escopo

Este documento consolida a investigacao controlada sobre licenciamento, autenticacao, sessoes, device binding, endpoints, dependencias e comportamento operacional do EquipeExe.

Regras preservadas:

- original intacto em `D:\AtelieProd\Equipexe`;
- analise baseada em copia readonly e runtime MOD;
- nenhuma tentativa de quebrar protecao;
- foco em documentacao, continuidade, substituicao gradual e nova arquitetura propria.

## Mapas gerados

- `docs\09-licensing\mapa-controle-auth-licensing-componentes.csv`
- `docs\09-licensing\mapa-device-binding.csv`
- `docs\09-licensing\mapa-endpoints-apis-classificado.csv`
- `docs\08-auth\mapa-sessoes-autenticacao.csv`
- `docs\08-auth\sinais-sessao-autenticacao.csv`
- `docs\09-licensing\sinais-device-binding.csv`
- `docs\09-licensing\dependencias-auth-licensing-imports.csv`
- `docs\18-risk\riscos-auth-licensing-operacional.csv`

## Componentes principais

### Gerenciador.exe

Papel provavel: broker remoto/admin forte.

Evidencias:

- executavel .NET 32-bit (`mscoree.dll`);
- endpoints HTTP de autenticacao, registro, atualizacao, download, dispositivos e nuvem;
- sinais `AutenticaGerenciador`, `TestaAutentica`, `RegistraEstacao`, `VerificaAtualizacoes`, `DownloadDados`, `ListarDispositivosPorFilial`.

Criticidade: critica.

Pendencia:

- analise IL com ILSpy/dnSpy;
- captura de payload HTTP;
- ProcMon para arquivos lidos/escritos durante autenticacao e registro.

### Senhas.exe

Papel provavel: controle local de usuarios, permissoes e bloqueios.

Evidencias:

- 146 sinais de autenticacao/permissao;
- 34 sinais de licenciamento/bloqueio;
- funcoes de bloquear/desbloquear sistemas e filiais;
- relacao direta com tabelas `Usuarios`, `Senhas`, `Nivel`, `GruUsuarios`.

Criticidade: critica.

Pendencia:

- mapear telas administrativas;
- mapear efeitos de cada bloqueio no banco;
- cruzar `OpcMenu`, `Sistema`, `Nivel` e grupos.

### LavSoft.exe

Papel provavel: core operacional com aplicacao de bloqueios/licenca.

Evidencias:

- `NovoReg`, `NovoReg.Db`, `dbo.NovoReg`, `Tb_NovoReg`;
- `Desbloqueio pela internet`;
- `ControlaBloq.exe`;
- mensagens de operacao bloqueada/desbloqueada;
- 128 sinais de autenticacao/permissao.

Criticidade: critica.

Pendencia:

- ProcMon filtrando `NovoReg`, `EquNet.ini`, `Registrar.xml`;
- teste offline controlado no MOD;
- captura de mensagens de tela.

### Financeiro.exe

Papel provavel: modulo operacional com regras de licenca/bloqueio/vencimento.

Evidencias:

- `ArquivoLicenca`;
- `FE_Licenca`;
- `Licenca`;
- `Vencimento`;
- `NovoReg.Db`;
- `BloqSec1` ate `BloqSec9`;
- 133 sinais de autenticacao/permissao.

Criticidade: critica.

Pendencia:

- identificar se `ArquivoLicenca` e arquivo real, recurso interno ou classe;
- mapear quais funcoes financeiras sao bloqueadas.

### SAT.exe e NFE.exe

Papel provavel: modulos fiscais com regras de permissao, bloqueio e componentes externos.

Evidencias:

- `SAT.exe` possui `NovoReg.Db`, `NovoRegBD.db`, `Tb_NovoRegX`, `IncNovoRegCli`;
- `NFE.exe` possui sinais como `LicenseKey`, `SerialNum`, `FSerialNum`, possivelmente ligados a biblioteca/SDK fiscal de terceiro;
- dependencia de componentes fiscais e comunicacao.

Criticidade: critica para continuidade fiscal.

Pendencia:

- separar licenciamento do EquipeExe de licencas de componentes fiscais externos;
- mapear certificados, DLLs fiscais e validade operacional.

### LavFacilLan.exe e Estoque.exe

Papel provavel: modulos operacionais com comunicacao real observada.

Evidencias:

- conexao runtime para `191.6.218.152:80`;
- dependencia `wininet.dll`;
- sinais SOAP/HTTP.

Criticidade: alta.

Pendencia:

- capturar payload e determinar se a comunicacao e sync, telemetria, update, auth ou consulta.

### EquEstruAtu.exe e EquConfig.exe

Papel provavel: infraestrutura, configuracao e atualizacao.

Evidencias:

- `EquEstruAtu.exe` importa `wininet.dll`;
- sinais SOAP/HTTP/update;
- possivel manutencao de estruturas.

Criticidade: alta/media.

Pendencia:

- mapear quando sao executados;
- verificar escrita em estrutura de banco e arquivos de configuracao.

## Persistencia local

### Banco Paradox/BDE

Persistencias confirmadas/candidatas:

- `Usuarios`: usuarios e senha legada;
- `Senhas`: permissoes por menu/sistema;
- `Nivel`: niveis/perfis;
- `GruUsuarios`: grupos;
- `NovoReg*`: registro/licenciamento historico;
- `CadCart.Licenca`: campo candidato a licenca;
- `Estruturas\Inis.ATIVACAO`: campo candidato a ativacao.

Observacao:

As tabelas `NovoReg*` acessiveis estavam sem registros na amostra atual. Isso nao elimina seu papel, pois os executaveis possuem referencias fortes a elas.

### Arquivos locais

- `Registrar.xml`: identidade da estacao.
- `EquNet.ini`: parametros codificados, caminho de dados, loja/equipe/maquina.
- `EquSenha.ini`: estado de janela do modulo de senhas.

## Endpoints e APIs

Total classificado: 71 endpoints.

Categorias:

- autenticacao remota;
- device management;
- atualizacao/download;
- sincronizacao/nuvem;
- integracoes operacionais.

Endpoints de maior interesse:

- `http://www.lavsoft.com.br/AutenticaGerenciador`
- `http://www.lavsoft.com.br/TestaAutentica`
- `http://www.lavsoft.com.br/RegistraEstacao`
- `http://www.lavsoft.com.br/ListarDispositivosPorFilial`
- `http://www.lavsoft.com.br/VerificaAtualizacoes`
- `http://www.lavsoft.com.br/DownloadDados`
- `http://lavsoft.com.br/ws/Nuvem/Enviar`
- `http://lavsoft.com.br/ws/nuvem/v1/UploadArquivo.asmx`

Risco:

Todos os endpoints extraidos nesta fase aparecem como HTTP, sem TLS aparente. Isso aumenta risco de interceptacao, indisponibilidade e dependencia externa.

## Device binding

Sinais confirmados:

- `Registrar.xml` com `MacAddress`;
- `CodLojaOriginal`;
- nome do computador;
- usuario Windows;
- `EquNet.ini` com `CampoCC`, `EquipeZ`, `TesteW1` e `COMPUTADOR.NOME`;
- endpoint `RegistraEstacao`;
- endpoint `ListarDispositivosPorFilial`.

Hipotese:

O legado associa estacao + loja/filial + parametros codificados + possivel validacao remota. A tolerancia a troca de hardware ainda e desconhecida.

## Sessao e autenticacao

Evidencia atual:

- sessao moderna/token nao foi identificada;
- ha persistencia local de usuarios/permissoes em Paradox;
- ha sinais de autenticacao remota no `Gerenciador.exe`;
- ha aplicacao de permissao e bloqueio distribuida por modulo.

Hipotese:

O login operacional e local, baseado em `Usuarios`/`Senhas`/`Nivel`, enquanto o `Gerenciador.exe` pode executar autenticacao administrativa/remota e sincronizacao.

## Fluxo provavel

1. Aplicacao inicia e carrega BDE/Paradox, INIs e DLLs.
2. Le parametros de caminho, loja/equipe e computador.
3. Valida usuario/permissao local em tabelas Paradox.
4. Modulos aplicam bloqueios por permissao, filial, sistema, vencimento ou parametro.
5. Quando ha internet, modulos com WinINet/SOAP chamam endpoints LavSoft.
6. `Gerenciador.exe` concentra chamadas remotas administrativas: autenticar, registrar estacao, atualizar, baixar/sincronizar dados.
7. Em falha de comunicacao, comportamento ainda precisa ser confirmado por isolamento controlado.

## Proximas investigacoes tecnicas

1. `Gerenciador.exe` com ILSpy/dnSpy.
2. ProcMon filtrando:
   - `Registrar.xml`;
   - `EquNet.ini`;
   - `NovoReg`;
   - `Usuarios.DB`;
   - `Senhas.DB`;
   - `Nivel.DB`.
3. Captura HTTP no MOD:
   - Fiddler/Wireshark/API Monitor;
   - endpoints de autenticacao, registro e atualizacao.
4. API Monitor para WinINet:
   - `InternetOpen`;
   - `InternetConnect`;
   - `HttpOpenRequest`;
   - `HttpSendRequest`;
   - `InternetReadFile`.
5. Teste offline/degradado apos aplicar isolamento MOD com sessao administrativa.

## Conclusao

A arquitetura de controle do EquipeExe e distribuida. Nao ha, ate aqui, um unico arquivo ou modulo que sozinho explique todo o licenciamento. O mapa mais forte aponta para:

- `Gerenciador.exe` como broker remoto;
- `Senhas.exe` como controle local de permissoes/bloqueios;
- `LavSoft.exe` como core operacional;
- `Financeiro.exe`, `SAT.exe` e `NFE.exe` como modulos criticos com regras proprias;
- `Registrar.xml`, `EquNet.ini` e `NovoReg*` como persistencias/candidatos locais;
- endpoints HTTP LavSoft como dependencia remota critica.
