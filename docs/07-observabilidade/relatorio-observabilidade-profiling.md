# Relatorio de Observabilidade e Profiling

Data: 2026-05-23

Escopo: coletas dinamicas e snapshots gravados em `D:\AtelieProd\MOD\logs\observability`.

## Execucoes monitoradas

### Estoque.exe

- PID inicial: `4168`
- Duracao: `12.2s`
- Saiu sozinho: `False`
- Codigo de saida: `None`
- Amostras: `11`
- Pico Working Set: `37.75 MB`
- Pico Private Memory: `10.02 MB`
- Pico Threads: `12`
- Pico Handles: `486`
- Observacoes de processos filhos: `0`
- Observacoes de rede: `9`
- Modulos/DLLs observados: `87`

### Financeiro.exe

- PID inicial: `29204`
- Duracao: `12.03s`
- Saiu sozinho: `False`
- Codigo de saida: `None`
- Amostras: `11`
- Pico Working Set: `21.06 MB`
- Pico Private Memory: `4.77 MB`
- Pico Threads: `6`
- Pico Handles: `245`
- Observacoes de processos filhos: `0`
- Observacoes de rede: `0`
- Modulos/DLLs observados: `46`

### Gerenciador.exe

- PID inicial: `5876`
- Duracao: `12.02s`
- Saiu sozinho: `False`
- Codigo de saida: `None`
- Amostras: `11`
- Pico Working Set: `20.41 MB`
- Pico Private Memory: `22.95 MB`
- Pico Threads: `6`
- Pico Handles: `232`
- Observacoes de processos filhos: `0`
- Observacoes de rede: `0`
- Modulos/DLLs observados: `40`

### LavFacilLan.exe

- PID inicial: `21084`
- Duracao: `12.15s`
- Saiu sozinho: `False`
- Codigo de saida: `None`
- Amostras: `11`
- Pico Working Set: `51.16 MB`
- Pico Private Memory: `16.14 MB`
- Pico Threads: `10`
- Pico Handles: `475`
- Observacoes de processos filhos: `0`
- Observacoes de rede: `9`
- Modulos/DLLs observados: `88`

### LavSoft.exe

- PID inicial: `23008`
- Duracao: `1.35s`
- Saiu sozinho: `True`
- Codigo de saida: `-1073741515`
- Amostras: `1`
- Pico Working Set: `5.06 MB`
- Pico Private Memory: `1.48 MB`
- Pico Threads: `2`
- Pico Handles: `90`
- Observacoes de processos filhos: `0`
- Observacoes de rede: `0`
- Modulos/DLLs observados: `0`

### LavSoft.exe

- PID inicial: `17360`
- Duracao: `20.99s`
- Saiu sozinho: `False`
- Codigo de saida: `None`
- Amostras: `19`
- Pico Working Set: `19.44 MB`
- Pico Private Memory: `6.73 MB`
- Pico Threads: `6`
- Pico Handles: `257`
- Observacoes de processos filhos: `18`
- Observacoes de rede: `0`
- Modulos/DLLs observados: `53`

## Snapshots de ambiente

- Snapshots JSON: `1`
- Snapshots de processos: `1`
- Snapshots de rede: `1`

## Limitacoes

- A coleta dinamica deve ser feita somente no runtime MOD.
- A abertura de telas exige acompanhamento visual para associar amostras a menus e acoes.
- Dumps de memoria, ProcMon, Wireshark, Fiddler e debuggers devem ser tratados como etapas controladas separadas, com evidencia e rollback documentados.