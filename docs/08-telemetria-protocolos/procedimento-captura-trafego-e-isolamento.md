# Procedimento Controlado - Captura de Trafego e Isolamento MOD

Data: 2026-05-23

## Objetivo

Capturar trafego real, payloads, endpoints, comportamento offline e impacto operacional dos modulos do runtime MOD.

## Estado atual

Ferramentas confirmadas no ambiente:

- `netsh.exe`
- `netstat.exe`
- `Get-NetTCPConnection`

Ferramentas nao encontradas no PATH nesta verificacao:

- Wireshark/tshark
- Fiddler
- ProcMon
- Process Explorer
- TCPView
- ILSpy/dnSpy/dotPeek
- Ghidra
- x64dbg
- WinDbg

## Isolamento de rede

Script preparado:

`D:\AtelieProd\MOD\apps\tools\apply-mod-network-isolation.ps1`

Rollback:

`D:\AtelieProd\MOD\apps\tools\rollback-mod-network-isolation.ps1`

Status:

- aplicacao falhou por `Acesso negado`;
- exige PowerShell elevado como administrador;
- nenhuma regra foi confirmada ativa.

Validacao apos aplicar:

```powershell
Get-NetFirewallRule -DisplayName 'EquipeExe MOD Block Outbound - *' |
  Select-Object DisplayName,Enabled,Action,Direction
```

Confirmar que o escopo aponta apenas para:

`D:\AtelieProd\MOD\apps\legacy-runtime\Equipexe\Exe`

## Captura com netsh trace

Observacao:

- normalmente exige elevacao administrativa.
- deve ser usada apenas no MOD.

Inicio:

```powershell
netsh trace start capture=yes report=yes persistent=no tracefile=D:\AtelieProd\MOD\logs\observability\phase08-network.etl
```

Execucao:

1. Abrir `LavFacilLan` no runtime MOD.
2. Aguardar 30 segundos.
3. Fechar.
4. Abrir `Estoque` no runtime MOD.
5. Aguardar 30 segundos.
6. Fechar.

Parada:

```powershell
netsh trace stop
```

Arquivos esperados:

- `.etl`
- `.cab`/relatorio se gerado pelo Windows

## Captura com Wireshark/tshark

Se instalado futuramente:

Filtro de captura:

```text
host 191.6.218.152 and tcp port 80
```

Filtro de exibicao:

```text
ip.addr == 191.6.218.152 && tcp.port == 80
```

Registrar:

- DNS;
- SYN/SYN-ACK;
- HTTP request;
- headers;
- payload;
- resposta;
- retries;
- tempo ate timeout;
- user-agent;
- host header;
- caminho HTTP.

## Captura com Fiddler

Usar somente se o modulo respeitar proxy do Windows/WinINet.

Hipotese:

- `LavFacilLan` e `Estoque` carregam `wininet.dll`, entao podem respeitar proxy configurado.

Riscos:

- modificar proxy global pode afetar outros apps;
- deve ser feito em janela curta;
- precisa rollback de proxy.

## Captura com ProcMon

Objetivo:

- ordem real de leitura de INI/XML/DB;
- Load Image de DLLs;
- acesso a registry;
- arquivos temporarios;
- locks BDE;
- logs criados;
- tentativas de rede indiretas.

Filtros:

- `Process Name is LavFacilLan.exe`
- `Process Name is Estoque.exe`
- `Path contains D:\AtelieProd\MOD`
- `Operation is Load Image`
- `Operation is RegQueryValue`
- `Operation is CreateFile`
- `Operation is TCP Connect`

## Evidencia minima por execucao

Cada execucao deve registrar:

- data/hora;
- modulo;
- comando;
- usuario Windows;
- se firewall MOD estava ativo;
- conexoes;
- payload se capturado;
- erros visiveis;
- travamento/lentidao;
- arquivos novos em `logs`;
- comportamento offline;
- rollback aplicado.

## Resultado esperado

Ao final, atualizar:

- `mapa-real-comunicacao.csv`;
- `relatorio-fase08-telemetria-protocolos-dependencias.md`;
- `achado-conexao-http-mod.md`;
- `Projeto_Novo_Atelie_2026.md`;
- `log-de-alteracoes.md`.
