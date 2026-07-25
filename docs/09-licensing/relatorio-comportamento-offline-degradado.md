# Relatorio de Comportamento Offline e Degradado

Data: 2026-05-23

## Status

Este relatorio combina evidencias confirmadas e comportamento esperado. O isolamento total de rede do MOD ainda nao foi aplicado porque a regra de firewall exige sessao administrativa. Portanto, os itens abaixo marcados como "esperado" precisam de validacao dinamica.

## Evidencias confirmadas

- `LavFacilLan.exe` abriu conexao TCP para `191.6.218.152:80`.
- `Estoque.exe` abriu conexao TCP para `191.6.218.152:80`.
- `Gerenciador.exe` contem endpoints HTTP de autenticacao, registro, atualizacao, download, dispositivos e nuvem.
- `EquEstruAtu.exe`, `Estoque.exe`, `LavFacilLan.exe` e `NFE.exe` importam `wininet.dll`.
- `Gerenciador.exe` e .NET 32-bit e contem referencias HTTP no proprio binario.
- `LiveUpdate.exe` foi substituido apenas no MOD por stub seguro e politica local de update bloqueado.

## Sem internet

Comportamento esperado:

- operacao local basica tende a funcionar enquanto o modulo depender apenas de BDE/Paradox local;
- funcoes de sincronizacao, nuvem, atualizacao, registro de estacao e autenticacao remota devem falhar ou entrar em timeout;
- modulos com comunicacao real observada (`LavFacilLan.exe`, `Estoque.exe`) podem apresentar lentidao ou timeout;
- `Gerenciador.exe` pode falhar em rotinas administrativas que chamem LavSoft remoto.

Validacao pendente:

- aplicar `apply-mod-network-isolation.ps1` em sessao administrativa;
- executar `LavFacilLan`, `Estoque`, `Financeiro`, `Gerenciador`, `LavSoft`, `Senhas`;
- registrar mensagens, logs, travamentos, timeouts e fallback.

## Timeout ou endpoint indisponivel

Comportamento esperado:

- chamadas HTTP sem timeout curto podem congelar tela ou atrasar inicializacao;
- endpoints HTTP podem retornar erro silencioso;
- atualizacao/sincronizacao pode repetir tentativas;
- funcoes administrativas remotas podem ficar indisponiveis.

Evidencia indireta:

- endpoint `191.6.218.152:80` responde em porta 80;
- requisicao HTTP manual retornou `403 Forbidden`;
- varios endpoints estaticos usam `http://www.lavsoft.com.br`.

## Falha de autenticacao remota

Comportamento esperado:

- login operacional local pode continuar se usar `Usuarios`/`Senhas`/`Nivel`;
- `Gerenciador.exe` pode bloquear rotinas que dependam de `AutenticaGerenciador` ou `TestaAutentica`;
- registro de estacao/dispositivo pode falhar sem impedir todas as telas, mas isso ainda nao esta confirmado.

## Dispositivo alterado

Comportamento esperado:

- mudanca de MAC/nome/loja pode afetar `Registrar.xml`;
- `CampoCC`/`CodLojaOriginal` pode deixar de bater com parametros remotos;
- se houver validacao remota ativa, o modulo administrativo pode exigir novo registro.

Evidencia:

- `Registrar.xml` contem `MacAddress` e `CodLojaOriginal`;
- existe endpoint `RegistraEstacao`;
- existe endpoint `ListarDispositivosPorFilial`.

## Sessao expirada

Status:

- nao foi identificado token moderno ou cache de sessao com expiracao.

Hipotese:

- sessao operacional e local/in-memory apos login;
- expiracao pode ser inexistente ou implementada por tela/modulo;
- autenticacao remota do `Gerenciador.exe` pode ter estado proprio.

## Perda de comunicacao durante operacao

Comportamento esperado:

- modulos com sync podem acumular pendencias ou falhar silenciosamente;
- operacao BDE local deve continuar se nao depender da chamada remota naquele fluxo;
- relatorios e fiscal podem depender de componentes locais e nao da nuvem, exceto integracoes especificas.

## Mensagens esperadas/candidatas

Strings encontradas:

- operacao bloqueada;
- operacao desbloqueada;
- bloqueio de sistema;
- bloqueio de filial;
- vencimento;
- desbloqueio pela internet;
- licenca;
- `NovoReg`.

## Matriz de validacao pendente

| Modulo | Teste offline | Evidencia a capturar | Prioridade |
|---|---|---|---|
| Gerenciador.exe | autenticar, registrar estacao, verificar atualizacao | payload HTTP, timeout, mensagem | critica |
| LavSoft.exe | abrir, logar, acessar menu principal | leitura `NovoReg`, `EquNet.ini`, permissoes | critica |
| Senhas.exe | alterar permissoes/bloqueios | escrita em `Usuarios`, `Senhas`, `Nivel` | critica |
| Financeiro.exe | abrir rotinas bloqueaveis | `ArquivoLicenca`, vencimento, bloqueios | alta |
| LavFacilLan.exe | abrir e aguardar comunicacao | conexao `191.6.218.152:80`, timeout | alta |
| Estoque.exe | abrir e aguardar comunicacao | conexao `191.6.218.152:80`, timeout | alta |
| SAT/NFE | abrir telas fiscais | DLLs fiscais, licencas de terceiros | alta |

## Conclusao

O EquipeExe parece ter boa parte da operacao local, mas com dependencias remotas importantes para administracao, sincronizacao, registro de estacao e atualizacao. O comportamento offline completo ainda precisa de teste isolado no MOD com firewall ativo e captura de evidencias.
