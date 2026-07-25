# Mapa de Dependencias Criticas

Data: 2026-05-23

## Origem

Este mapa foi gerado a partir de:

- imports PE;
- modulos carregados em runtime MOD;
- baselines dinamicos;
- observacao de comunicacao externa.

CSV principal:

`D:\AtelieProd\MOD\docs\06-dependencias\mapa-dependencias-runtime.csv`

## Criticidade consolidada

- Critica: BDE/Paradox e fiscal/hardware.
- Alta: rede/protocolo, impressao e runtime legado.
- Media: Windows e modulos do runtime MOD.
- Baixa: bibliotecas auxiliares sem risco operacional imediato identificado.

## Dependencias criticas confirmadas

### Banco/BDE

Componentes:

- `IDAPI32.DLL`
- `Tutil32.dll`
- familia BDE/Paradox

Impacto:

- acesso ao banco legado;
- locks;
- tabelas Paradox;
- risco de corrupcao/indice;
- dependencia 32-bit.

Rollback:

- manter original intocado;
- operar apenas em copia/MOD;
- preservar backups.

### Fiscal/Hardware

Componentes:

- `BEMAFI32.DLL`
- `Mp20fi32.dll`
- `general32.dll`
- Daruma/SAT/NFE em fases separadas.

Impacto:

- impressao fiscal;
- cupom;
- SAT/NFE;
- dispositivos fisicos;
- risco de falha operacional se removido sem substituto.

### Rede/Protocolo

Componentes:

- `wininet.dll`
- `WSOCK32.DLL`
- `urlmon.dll`
- `mswsock.dll`

Endpoint confirmado:

- `191.6.218.152:80`

Impacto:

- dependencia externa real;
- finalidade ainda desconhecida;
- risco de update, validacao, sync ou telemetria.

### Impressao

Componentes:

- `winspool.drv`
- `splwow64.exe`
- `SPOOLSS.DLL`

Impacto:

- relatorios;
- cupom;
- impressoras fiscais e comuns;
- dependencia do subsistema Windows.

## Componentes obsoletos ou sensiveis

- BDE/Paradox;
- OpenSSL legado (`LIBEAY32.dll`, `ssleay32.dll`) identificado em analise estatica;
- componentes 32-bit;
- OCX/DLLs antigas;
- integracoes fiscais legadas.

## Proximas acoes

1. Completar dependencias de `NFE` e `SAT` em fase fiscal.
2. Capturar payload HTTP.
3. Confirmar se `191.6.218.152` e update, licenca, sync ou telemetria.
4. Mapear ordem exata de inicializacao com ProcMon/ETW.
5. Criar plano de substituicao por modulo.
