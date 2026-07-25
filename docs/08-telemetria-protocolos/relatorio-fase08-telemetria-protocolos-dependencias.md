# Fase 08 - Telemetria, Protocolos e Dependencias Reais

Data: 2026-05-23

## Escopo

Consolidacao das evidencias dinamicas existentes no runtime MOD.

## Arquivos gerados

- `D:\AtelieProd\MOD\docs\08-telemetria-protocolos\mapa-real-comunicacao.csv`
- `D:\AtelieProd\MOD\docs\06-dependencias\mapa-dependencias-runtime.csv`
- `D:\AtelieProd\MOD\docs\08-telemetria-protocolos\mapa-inicializacao-runtime.csv`
- `D:\AtelieProd\MOD\docs\08-telemetria-protocolos\baseline-memoria-runtime.csv`

## Comunicacao real

### Estoque.exe

- `192.168.0.101:52049 -> 191.6.218.152:80` estado `Established`

### LavFacilLan.exe

- `192.168.0.101:52033 -> 191.6.218.152:80` estado `Established`

## Endpoint externo confirmado

- IP: `191.6.218.152`
- Porta: `80`
- Reverse DNS observado: `web22f62.kinghost.net`
- Teste manual HTTP HEAD: resposta `403 Forbidden`
- Classificacao: dependencia externa real ainda sem finalidade identificada

## Dependencias por criticidade

- alta: 23
- baixa: 11
- critica: 10
- media: 267

## Dependencias por categoria

- banco/BDE: 7
- fiscal/hardware: 3
- impressao: 8
- mod/runtime MOD: 5
- outro: 11
- rede/protocolo: 11
- runtime legado: 4
- windows: 262

## Hipotese de Core Engine

Com base nos baselines atuais:

- `LavFacilLan.exe` aparenta ser um nucleo operacional forte: carrega BDE, fiscal/hardware, WinINet/Winsock e realiza comunicacao externa.
- `LavSoft.exe` e nucleo operacional classico, com impressao e dependencias fiscais, mas nesta janela nao abriu rede.
- `Estoque.exe` possui dependencia real de rede e BDE, devendo ser tratado como modulo com integracao externa.
- `Gerenciador.exe` aparenta componente administrativo/.NET, mas ainda precisa de analise ILSpy/dnSpy.
- `Financeiro.exe` iniciou leve e sem rede observada, mas precisa de fluxo funcional com telas.

## Limitacoes

- O mapa de inicializacao usa enumeracao de modulos ao final da coleta, nao a ordem exata de LoadLibrary.
- Para ordem exata sera necessario ProcMon, ETW ou instrumentacao API Monitor em sessao controlada.
- O firewall de isolamento MOD nao foi aplicado por falta de elevacao administrativa.
- Payload HTTP ainda nao foi capturado.

## Proximas acoes

1. Aplicar firewall MOD com elevacao administrativa e repetir baselines.
2. Capturar payload/protocolo HTTP de `LavFacilLan` e `Estoque` em laboratorio.
3. Analisar `Gerenciador.exe` como .NET com ILSpy/dnSpy.
4. Executar ProcMon/ETW para ordem real de inicializacao.
5. Executar fase fiscal separada para `NFE` e `SAT`.