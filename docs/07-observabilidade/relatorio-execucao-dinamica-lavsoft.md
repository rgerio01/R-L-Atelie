# Relatorio de Execucao Dinamica - LavSoft MOD

Data: 2026-05-23

## Escopo

Execucao controlada do `LavSoft.exe` no runtime MOD:

`D:\AtelieProd\MOD\apps\legacy-runtime\Equipexe\Exe\LavSoft.exe`

O sistema original em `D:\AtelieProd\Equipexe` nao foi executado nem alterado.

## Preparacao realizada

Foi identificado que a primeira tentativa de execucao do `LavSoft.exe` no MOD encerrava com codigo `-1073741515`, indicando falha de carregamento de dependencia/DLL.

Acao tomada:

- criado script `D:\AtelieProd\MOD\apps\tools\ensure-mod-runtime-dependencies.ps1`;
- copiadas 50 dependencias `.dll`/`.ocx` do diretorio `Exe` original para o runtime MOD;
- gerado inventario `D:\AtelieProd\MOD\docs\01-inventario\dependencias-copiadas-runtime-mod.csv`;
- o original permaneceu somente como origem de leitura/copia.

## Execucao monitorada

Comando:

```powershell
D:\AtelieProd\MOD\apps\tools\run-observability-monitor.ps1 -Target LavSoft -Seconds 20 -IntervalMs 1000 -Close
```

Resumo:

- PID inicial: `17360`
- Duracao: `20,99s`
- Amostras: `19`
- Pico Working Set: aproximadamente `19,44 MB`
- Pico memoria privada: aproximadamente `6,73 MB`
- Pico threads: `6`
- Pico handles: `257`
- Processos filhos observados: `splwow64.exe`
- Conexoes de rede observadas: `0`
- Modulos/DLLs observados: `53`

Arquivos:

- `D:\AtelieProd\MOD\logs\observability\LavSoft-summary-20260523-123943.json`
- `D:\AtelieProd\MOD\logs\observability\LavSoft-samples-20260523-123943.csv`
- `D:\AtelieProd\MOD\logs\observability\LavSoft-children-20260523-123943.csv`
- `D:\AtelieProd\MOD\logs\observability\LavSoft-network-20260523-123943.csv`
- `D:\AtelieProd\MOD\logs\observability\LavSoft-modules-20260523-123943.csv`

## Achados

1. O runtime MOD precisava das DLLs auxiliares do diretorio `Exe` para iniciar o `LavSoft`.
2. O `LavSoft` iniciou no MOD depois da copia controlada das dependencias.
3. O processo acionou `splwow64.exe`, indicando uso do subsistema de impressao Windows para compatibilidade 32-bit/64-bit.
4. Nao houve conexao TCP observada nos 20 segundos de amostragem.
5. O consumo inicial observado e baixo, coerente com uma aplicacao desktop Delphi/BDE leve.

## Riscos e limitacoes

- A execucao curta nao valida todos os menus, relatorios ou fluxos.
- A ausencia de rede nessa janela nao prova ausencia de rede em outras telas.
- Algumas DLLs carregadas podem depender de drivers fiscais/hardware reais.
- O processo de impressao `splwow64.exe` pode permanecer temporariamente apos fechamento do aplicativo.

## Proximas acoes

1. Repetir monitoramento com login e abertura de telas especificas.
2. Monitorar `LavFacilLan`, `Gerenciador`, `Financeiro`, `Estoque`, `NFE` e `SAT`.
3. Cruzar cada execucao com o mapa de menus/permissoes.
4. Capturar evidencias de tela para posicao de campos, botoes e relatorios.
5. Executar ProcMon/Process Explorer em etapa propria, se necessario.
