# Procedimento de Captura Dinamica de UI

Data: 2026-05-23

## Objetivo

Transformar os mapas estaticos em evidencia visual real, tela por tela, no runtime MOD.

## Escopo

Modulos prioritarios:

- `LavSoft.exe`
- `LavFacilLan.exe`
- `Estoque.exe`
- `Financeiro.exe`
- `Senhas.exe`
- `Gerenciador.exe`
- `NFE.exe`
- `SAT.exe`

## Pre-condicoes

- Usar somente `D:\AtelieProd\MOD\apps\legacy-runtime\Equipexe`.
- Manter atualizacao automatica bloqueada no MOD.
- Nao executar no original.
- Registrar horario, usuario, modulo e acao.
- Se for aplicar isolamento de rede, usar apenas script MOD e sessao administrativa.

## Coleta por tela

Para cada tela aberta:

1. Capturar screenshot.
2. Registrar titulo da janela.
3. Registrar caminho de menu usado.
4. Registrar botoes visiveis.
5. Registrar campos obrigatorios.
6. Registrar grids/tabelas visiveis.
7. Registrar atalhos de teclado exibidos.
8. Registrar mensagens/popups.
9. Registrar tempo de abertura.
10. Registrar memoria, threads e handles.
11. Registrar arquivos `.DB/.PX/.INI/.XML` acessados.
12. Registrar conexoes TCP.

## Ferramentas recomendadas

- Observability tool MOD ja criada.
- ProcMon com filtros por processo e caminho MOD.
- Process Explorer para memoria e DLLs.
- PowerShell para screenshots se necessario.
- API Monitor para WinINet/WinHTTP/BDE.
- Fiddler/Wireshark para rede.

## Nome dos artefatos

Padrao sugerido:

- `docs\11-modulos\ui-reverse-engineering\screenshots\<modulo>\<ordem>-<menu>-<tela>.png`
- `docs\11-modulos\ui-reverse-engineering\capturas\<modulo>-captura-ui.csv`
- `logs\observability\ui\<modulo>-<timestamp>.json`

## CSV de captura dinamica

Campos:

- `Timestamp`
- `Modulo`
- `ProcessId`
- `MenuPath`
- `WindowTitle`
- `ScreenName`
- `Action`
- `VisibleButtons`
- `VisibleFields`
- `VisibleGrids`
- `ShortcutKeys`
- `OpenedFiles`
- `TcpConnections`
- `WorkingSetMB`
- `PrivateMemoryMB`
- `Threads`
- `Handles`
- `ScreenshotPath`
- `OperatorNotes`

## Validacao cruzada

Cada captura deve ser cruzada com:

- `mapa-telas-funcional-consolidado.csv`
- `mapa-menus-submenus-acoes-consolidado.csv`
- `mapa-permissoes-ui-consolidado.csv`
- `mapa-ui-banco-interligacoes.csv`
- `mapa-layouts-delphi-tpf0.csv`

## Resultado esperado

Ao final, cada tela tera:

- evidencia visual;
- caminho de navegacao;
- acao operacional;
- permissao provavel;
- tabelas acessadas;
- riscos;
- prioridade de reconstrucao.
