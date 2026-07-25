# Matriz de Entregaveis da Investigacao Total

Data: 2026-05-23

## Entregaveis obrigatorios

| # | Entregavel | Artefato principal | Status |
|---:|---|---|---|
| 1 | inventario completo | `docs\01-inventario\inventario-arquivos.csv` | confirmado por filesystem |
| 2 | engenharia reversa completa | `docs\07-observabilidade`, `docs\09-licensing`, `docs\11-modulos` | parcial, runtime profundo pendente |
| 3 | modelo de dominio completo | `docs\10-database\dicionario-paradox-campo-a-campo.md`, `D:\AtelieProd\docs\dominio\modelo-dominio-consolidado.md` | schema/UI forte, runtime pendente |
| 4 | mapa UI -> banco | `docs\11-modulos\matriz-ui-banco.csv` | confirmado por cruzamento UI/schema |
| 5 | mapa permissoes | `docs\08-auth\analise-usuarios-permissoes.md` | schema/UI, semantica fina pendente |
| 6 | mapa impressao | `D:\AtelieProd\docs\printer\Blueprint-impressao-termica-fiel.md` | blueprint, captura fisica pendente |
| 7 | mapa Bluetooth | `D:\AtelieProd\docs\bluetooth\blueprint-bluetooth-linux.md` | blueprint, hardware pendente |
| 8 | mapa financeiro | `docs\11-modulos\mapa-valores-calculos.md`, `docs\10-database\classificacao-entidades-negocio.md` | schema/amostra, regras pendentes |
| 9 | mapa estoque | `docs\11-modulos\mapa-fluxo-produto-estoque-financeiro.md` | schema, runtime pendente |
| 10 | mapa clientes | `docs\11-modulos\mapa-clientes-os-produtos.md` | schema/UI |
| 11 | mapa ordens | `docs\11-modulos\mapa-fluxo-cliente-movimento-financeiro.md` | schema/UI, ciclo real pendente |
| 12 | mapa runtime | `docs\07-observabilidade\relatorio-execucao-dinamica-modulos-principais.md` | runtime parcial |
| 13 | mapa memoria | `docs\07-observabilidade\relatorio-observabilidade-profiling.md` | baseline parcial |
| 14 | mapa threads | `docs\07-observabilidade\plano-observabilidade-profiling.md` | planejado, ETW pendente |
| 15 | mapa DLLs | `docs\01-inventario\inventario-executaveis-dlls.csv` | filesystem/imports |
| 16 | mapa APIs | `docs\09-licensing\mapa-endpoints-apis-classificado.csv`, `docs\12-apis` | strings/configs, payload pendente |
| 17 | mapa endpoints | `docs\05-comunicacoes`, `docs\09-licensing\endpoints-licenciamento-autenticacao-atualizacao.csv` | parcial |
| 18 | blueprint Windows | `D:\AtelieProd\docs\windows\blueprint-windows-nextgen.md` | definido |
| 19 | blueprint Linux | `D:\AtelieProd\docs\linux\blueprint-linux-nextgen.md` | definido |
| 20 | blueprint appliance | `D:\AtelieProd\docs\pdv\blueprint-pdv-auto-inicializavel.md` | definido |
| 21 | blueprint Supabase | `D:\AtelieProd\docs\nextgen\blueprint-supabase-nextgen.md` | definido |
| 22 | blueprint PDV | `D:\AtelieProd\docs\pdv\blueprint-pdv-auto-inicializavel.md` | definido |
| 23 | blueprint impressao | `D:\AtelieProd\docs\printer\Blueprint-impressao-termica-fiel.md` | definido |
| 24 | blueprint licensing | `docs\15-nextgen\plano-substituicao-auth-licensing-offline-first.md` | definido |
| 25 | roadmap migracao | `D:\AtelieProd\docs\migracao\roadmap-migracao-reconstrucao.md` | definido |
| 26 | roadmap NextGen | `docs\15-nextgen\roadmap-desenvolvimento-novo-sistema.md` | definido |
| 27 | arquitetura final recomendada | `D:\AtelieProd\docs\arquitetura\recomendacao-final-arquitetura.md` | definido |

## Lacunas criticas

- captura de payload HTTP;
- ETW/ProcMon/API Monitor para ordem real de inicializacao;
- mapeamento de threads/timers;
- impressao fisica e comparativo de comprovantes;
- Bluetooth real em Dell;
- teste Openbox kiosk em VM e Dell;
- semantica final dos status Paradox;
- validacao insert/update/delete por tela;
- desempenho real no hardware alvo.

## Proxima coleta recomendada

1. VM Debian minimal para validar scripts Linux.
2. Dell real para inventario de drivers.
3. ProcMon/ETW no MOD para inicializacao e impressao.
4. Captura de comprovante legado.
5. Teste controlado de criar cliente/ROL/produto/pagamento em copia MOD.
