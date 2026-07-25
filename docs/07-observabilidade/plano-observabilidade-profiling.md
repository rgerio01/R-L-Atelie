# Plano de Observabilidade, Tracing e Profiling

Data: 2026-05-23

## Objetivo

Instrumentar o EquipeExe em ambiente MOD para entender:

- inicializacao;
- processos filhos;
- DLLs carregadas;
- consumo de RAM;
- uso de CPU;
- handles;
- threads;
- conexoes de rede;
- comportamento de update/sincronizacao;
- comportamento por tela/menu/relatorio.

## Regra de seguranca

- Executar somente em `D:\AtelieProd\MOD\apps\legacy-runtime\Equipexe`.
- Nao executar testes destrutivos no original.
- Nao liberar atualizacao automatica.
- Registrar logs em `D:\AtelieProd\MOD\logs\observability`.
- Associar cada execucao a um objetivo claro.

## Ferramenta propria criada

Projeto C#:

`D:\AtelieProd\MOD\apps\tools\EquipeExe.Mod.Observability`

Funcoes:

- snapshot de processos;
- snapshot de rede com PID;
- monitoramento de executavel MOD;
- amostragem de memoria, threads, handles e CPU;
- deteccao de processos filhos via WMI;
- inventario de DLLs/modulos carregados;
- exportacao CSV e JSON.

## Scripts

- `D:\AtelieProd\MOD\apps\tools\run-observability-monitor.ps1`
- `D:\AtelieProd\MOD\apps\tools\summarize-observability.py`

## Comandos

Snapshot:

```powershell
dotnet D:\AtelieProd\MOD\apps\tools\EquipeExe.Mod.Observability\bin\Release\net8.0\EquipeExe.Mod.Observability.dll snapshot --out D:\AtelieProd\MOD\logs\observability
```

Monitorar executavel MOD:

```powershell
D:\AtelieProd\MOD\apps\tools\run-observability-monitor.ps1 -Target LavSoft -Seconds 60 -IntervalMs 1000 -Close
```

Gerar resumo:

```powershell
python D:\AtelieProd\MOD\apps\tools\summarize-observability.py
```

## Ferramentas externas permitidas futuramente

- ProcMon para leitura/escrita/registro/processos.
- Process Explorer para DLLs, handles e arvore de processos.
- Wireshark/Fiddler para rede, se houver trafego autorizado em laboratorio.
- ILSpy/dnSpy para `Gerenciador.exe` e DLLs .NET.
- WinDbg/x64dbg/API Monitor para chamadas criticas, quando necessario.
- PerfView/Visual Studio Diagnostics para profiling .NET/CPU quando aplicavel.

## Politica de instalacao controlada

Antes de instalar ferramenta externa:

1. Registrar motivo.
2. Registrar origem.
3. Registrar impacto.
4. Registrar risco.
5. Registrar rollback.
6. Instalar fora do original.

## Proximas execucoes recomendadas

1. Snapshot baseline sem executavel legado aberto.
2. Monitorar `LavSoft` por 60 segundos.
3. Monitorar `LavFacilLan` por 60 segundos.
4. Monitorar `Gerenciador` por 60 segundos.
5. Monitorar `Financeiro`, `Estoque`, `NFE` e `SAT` individualmente.
6. Repetir abrindo menus especificos e anotando manualmente qual tela estava ativa.
