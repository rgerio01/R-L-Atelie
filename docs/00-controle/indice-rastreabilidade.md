# Indice de Rastreabilidade

Data: 2026-05-23

## Objetivo

Relacionar fases, documentos, evidencias e status de descoberta.

## Arquivos principais

- Roadmap: `D:\AtelieProd\MOD\docs\00-controle\roadmap-150-fases.md`
- Indice CSV: `D:\AtelieProd\MOD\docs\00-controle\roadmap-150-fases.csv`
- Projeto mestre: `D:\AtelieProd\MOD\Projeto_Novo_Atelie_2026.md`
- Log tecnico: `D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md`

## Evidencias ja existentes

| Area | Documento/evidencia | Status |
|---|---|---|
| Inventario | `docs\01-inventario` | parcial alto |
| Banco | `docs\03-banco-de-dados` | parcial alto |
| Auth/permissoes | `docs\04-autenticacao-permissoes` | parcial medio |
| Comunicacao | `docs\05-comunicacoes` | parcial alto |
| Dependencias | `docs\06-dependencias` | parcial medio |
| Observabilidade | `docs\07-observabilidade` | parcial medio |
| Telemetria/protocolos | `docs\08-telemetria-protocolos` | inicial alto |
| Arquitetura futura | `docs\11-arquitetura-futura` | planejado alto |
| Visibilidade total | `docs\12-visibilidade-total` | matriz criada |

## Achados criticos atuais

- Original permanece intocado.
- MOD possui runtime separado.
- Update automatico via `LiveUpdate.exe` foi neutralizado no MOD.
- `LavFacilLan.exe` e `Estoque.exe` abriram conexao HTTP para `191.6.218.152:80`.
- `191.6.218.152` resolve para `web22f62.kinghost.net`.
- Firewall de isolamento MOD esta preparado, mas exige elevacao administrativa.
- `LavFacilLan.exe` e candidato forte a core engine operacional.
- BDE/Paradox e dependencia critica.
- Fiscal/hardware e dependencia critica.

## Status do roadmap

O arquivo `roadmap-150-fases.csv` deve ser atualizado conforme cada fase mudar de:

- `planejada`
- `parcial/em andamento`
- `em execucao`
- `bloqueada`
- `concluida`
- `validada`

## Proxima revisao

Ao executar nova fase:

1. registrar evidencias;
2. atualizar roadmap CSV;
3. atualizar log;
4. atualizar projeto mestre;
5. registrar rollback;
6. registrar criticidade.
