# Roadmap Ultra Detalhado - 150 Fases

Data: 2026-05-23

Escopo: recuperacao, engenharia reversa controlada, visibilidade total e planejamento da nova geracao do EquipeExe.

Regra absoluta: o original `D:\AtelieProd\Equipexe` permanece intocado; execucoes, alteracoes e experimentos devem ocorrer em `D:\AtelieProd\MOD`.

## FASE 001 - Inventario raiz completo

### Objetivo

Criar mapa estrutural completo do sistema legado e MOD.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\01-inventario` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

PowerShell, Python, Get-ChildItem, Get-FileHash, CSV.

### Analises

Analisar area `inventario`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `01-inventario`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

media

### Documentacao

`D:\AtelieProd\MOD\docs\01-inventario` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Inventario raiz completo`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 002 - Hash e integridade

### Objetivo

Criar baseline completo de integridade por SHA256.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\01-inventario` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

PowerShell Get-FileHash, Python hashlib, comparacao CSV.

### Analises

Analisar area `integridade`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `01-inventario`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\01-inventario` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Hash e integridade`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 003 - Snapshot e rollback

### Objetivo

Definir rollback operacional seguro para analises e mudancas MOD.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\19-snapshots` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

ZIP, snapshots, checksums, scripts de rollback.

### Analises

Analisar area `rollback`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `19-snapshots`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\19-snapshots` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Snapshot e rollback`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 004 - Inventario de executaveis

### Objetivo

Mapear EXEs, versoes, imports, runtimes e comportamento previsto.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\01-inventario` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

analyze-pe-imports.ps1, strings, PE headers, ILSpy quando aplicavel.

### Analises

Analisar area `binarios`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `01-inventario`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\01-inventario` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Inventario de executaveis`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 005 - Inventario de DLLs

### Objetivo

Mapear DLLs criticas, orfas, antigas, de terceiros e runtime.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\06-dependencias` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

imports PE, modulos runtime, Process Explorer/ProcMon futuro.

### Analises

Analisar area `dependencias`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `06-dependencias`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\06-dependencias` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Inventario de DLLs`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 006 - Identificacao de frameworks

### Objetivo

Identificar stack tecnologica, runtimes e bibliotecas.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\06-dependencias` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

strings, imports, metadados .NET, Ghidra/ILSpy futuro.

### Analises

Analisar area `arquitetura`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `06-dependencias`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

media

### Documentacao

`D:\AtelieProd\MOD\docs\06-dependencias` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Identificacao de frameworks`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 007 - Engenharia reversa estatica

### Objetivo

Entender arquitetura sem executar o sistema.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\02-runtime` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

strings, imports, Ghidra/ILSpy/dnSpy quando autorizado.

### Analises

Analisar area `reversing`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `02-runtime`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\02-runtime` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Engenharia reversa estatica`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 008 - Engenharia reversa dinamica

### Objetivo

Descobrir comportamento real em runtime MOD.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\02-runtime` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

EquipeExe.Mod.Observability, ProcMon futuro, ETW futuro.

### Analises

Analisar area `runtime`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `02-runtime`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\02-runtime` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Engenharia reversa dinamica`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 009 - Mapa de inicializacao

### Objetivo

Descobrir ordem real de boot e inicializacao.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\02-runtime` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

ProcMon, ETW, API Monitor, logs de modulos runtime.

### Analises

Analisar area `startup`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `02-runtime`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\02-runtime` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Mapa de inicializacao`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 010 - Baseline de memoria

### Objetivo

Criar baseline operacional de RAM, CPU, handles e threads.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\03-memoria` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

EquipeExe.Mod.Observability, Process Explorer, PerfView futuro.

### Analises

Analisar area `memoria`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `03-memoria`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

media

### Documentacao

`D:\AtelieProd\MOD\docs\03-memoria` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Baseline de memoria`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 011 - Baseline de performance

### Objetivo

Medir startup time, CPU, I/O e responsividade.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\04-performance` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

PerfView futuro, ETW, amostragem CPU/RAM/I/O.

### Analises

Analisar area `performance`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `04-performance`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

media

### Documentacao

`D:\AtelieProd\MOD\docs\04-performance` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Baseline de performance`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 012 - Mapa de processos filhos

### Objetivo

Identificar processos filhos e subprocessos auxiliares.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\02-runtime` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

EquipeExe.Mod.Observability, ProcMon futuro, ETW futuro.

### Analises

Analisar area `runtime`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `02-runtime`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

media

### Documentacao

`D:\AtelieProd\MOD\docs\02-runtime` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Mapa de processos filhos`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 013 - Mapa de handles e GDI

### Objetivo

Mapear handles, GDI e recursos Windows.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\03-memoria` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

EquipeExe.Mod.Observability, Process Explorer, PerfView futuro.

### Analises

Analisar area `memoria`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `03-memoria`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

media

### Documentacao

`D:\AtelieProd\MOD\docs\03-memoria` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Mapa de handles e GDI`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 014 - Mapa de I/O e arquivos temporarios

### Objetivo

Mapear arquivos temporarios, cache, locks e diretorios de trabalho.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\02-runtime` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

EquipeExe.Mod.Observability, ProcMon futuro, ETW futuro.

### Analises

Analisar area `runtime`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `02-runtime`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\02-runtime` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Mapa de I/O e arquivos temporarios`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 015 - Mapa de registry

### Objetivo

Identificar chaves de registro acessadas pelo legado.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\02-runtime` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

EquipeExe.Mod.Observability, ProcMon futuro, ETW futuro.

### Analises

Analisar area `runtime`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `02-runtime`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

media

### Documentacao

`D:\AtelieProd\MOD\docs\02-runtime` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Mapa de registry`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 016 - Mapa de configuracoes

### Objetivo

Classificar INI, XML, JSON, CFG e parametros operacionais.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\01-inventario` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

INIs/XML/JSON, registry, ProcMon.

### Analises

Analisar area `config`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `01-inventario`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

media

### Documentacao

`D:\AtelieProd\MOD\docs\01-inventario` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Mapa de configuracoes`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 017 - Mapa de logs legados

### Objetivo

Localizar, classificar e interpretar logs existentes.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\07-observabilidade` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

PowerShell, Python, C#, ferramentas CLI e ferramentas externas controladas quando necessario.

### Analises

Analisar area `logs`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `07-observabilidade`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

media

### Documentacao

`D:\AtelieProd\MOD\docs\07-observabilidade` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Mapa de logs legados`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 018 - Mapa de banco Paradox/BDE

### Objetivo

Mapear tabelas, campos, indices, locks e dependencias BDE.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\10-database` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

Paradox ODBC readonly, dicionario, indices, integridade.

### Analises

Analisar area `database`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `10-database`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

critica

### Documentacao

`D:\AtelieProd\MOD\docs\10-database` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Mapa de banco Paradox/BDE`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 019 - Dicionario de dados refinado

### Objetivo

Refinar dicionario de tabelas, dominios e campos criticos.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\10-database` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

Paradox ODBC readonly, dicionario, indices, integridade.

### Analises

Analisar area `database`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `10-database`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\10-database` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Dicionario de dados refinado`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 020 - Relacionamentos e entidades

### Objetivo

Inferir relacionamentos e entidades de negocio.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\10-database` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

Paradox ODBC readonly, dicionario, indices, integridade.

### Analises

Analisar area `database`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `10-database`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\10-database` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Relacionamentos e entidades`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 021 - Integridade e corrupcao de dados

### Objetivo

Validar duplicidades, indices, inconsistencias e corrupcao.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\10-database` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

Paradox ODBC readonly, dicionario, indices, integridade.

### Analises

Analisar area `database`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `10-database`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

critica

### Documentacao

`D:\AtelieProd\MOD\docs\10-database` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Integridade e corrupcao de dados`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 022 - Mapa de permissoes legado

### Objetivo

Mapear usuarios, grupos, niveis e permissoes por acao.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\08-auth` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

ODBC Paradox readonly, scripts auth, logs, captura dinamica.

### Analises

Analisar area `auth`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `08-auth`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\08-auth` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Mapa de permissoes legado`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 023 - Fluxo de login legado

### Objetivo

Descobrir fluxo real de autenticacao e fallback.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\08-auth` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

ODBC Paradox readonly, scripts auth, logs, captura dinamica.

### Analises

Analisar area `auth`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `08-auth`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\08-auth` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Fluxo de login legado`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 024 - Persistencia de sessao

### Objetivo

Mapear cache, tokens, arquivos temporarios e expiração.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\08-auth` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

ODBC Paradox readonly, scripts auth, logs, captura dinamica.

### Analises

Analisar area `auth`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `08-auth`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\08-auth` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Persistencia de sessao`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 025 - Codificacao de senha

### Objetivo

Confirmar codificacao legada e plano de migracao segura.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\08-auth` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

ODBC Paradox readonly, scripts auth, logs, captura dinamica.

### Analises

Analisar area `auth`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `08-auth`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\08-auth` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Codificacao de senha`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 026 - Licenciamento legado

### Objetivo

Investigar ativacao, serial, validacao e dependencias remotas.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\09-licensing` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

strings, ProcMon, rede, registry, analise de arquivos.

### Analises

Analisar area `licensing`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `09-licensing`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

critica

### Documentacao

`D:\AtelieProd\MOD\docs\09-licensing` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Licenciamento legado`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 027 - Hardware binding legado

### Objetivo

Identificar uso de MAC, disco, CPU, placa e TPM.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\09-licensing` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

strings, ProcMon, rede, registry, analise de arquivos.

### Analises

Analisar area `licensing`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `09-licensing`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\09-licensing` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Hardware binding legado`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 028 - Mapa de dispositivos

### Objetivo

Planejar inventario e gerenciamento de dispositivos autorizados.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\09-licensing` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

strings, ProcMon, rede, registry, analise de arquivos.

### Analises

Analisar area `licensing`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `09-licensing`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

media

### Documentacao

`D:\AtelieProd\MOD\docs\09-licensing` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Mapa de dispositivos`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 029 - Mapa de comunicacao externa

### Objetivo

Consolidar IPs, hosts, portas, protocolos e endpoints.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\05-comunicacoes` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

Get-NetTCPConnection, netsh trace, Wireshark/Fiddler futuro.

### Analises

Analisar area `comunicacao`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `05-comunicacoes`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

critica

### Documentacao

`D:\AtelieProd\MOD\docs\05-comunicacoes` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Mapa de comunicacao externa`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 030 - Captura de trafego HTTP

### Objetivo

Capturar payload, headers, retries e frequencia.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\05-comunicacoes` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

Get-NetTCPConnection, netsh trace, Wireshark/Fiddler futuro.

### Analises

Analisar area `comunicacao`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `05-comunicacoes`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

critica

### Documentacao

`D:\AtelieProd\MOD\docs\05-comunicacoes` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Captura de trafego HTTP`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 031 - Comportamento offline

### Objetivo

Executar sistema isolado e medir falhas, timeouts e fallback.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\05-comunicacoes` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

firewall MOD, isolamento de rede, observabilidade.

### Analises

Analisar area `offline`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `05-comunicacoes`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\05-comunicacoes` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Comportamento offline`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 032 - Comportamento online

### Objetivo

Executar sistema com rede e comparar com baseline offline.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\05-comunicacoes` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

observabilidade, captura de rede, comparacao baseline.

### Analises

Analisar area `online`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `05-comunicacoes`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\05-comunicacoes` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Comportamento online`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 033 - Mapa de sincronizacao

### Objetivo

Descobrir filas, tabelas, endpoints e mecanismos de sync.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\05-comunicacoes` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

logs, banco, filas, rede, outbox/inbound planejados.

### Analises

Analisar area `sync`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `05-comunicacoes`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

critica

### Documentacao

`D:\AtelieProd\MOD\docs\05-comunicacoes` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Mapa de sincronizacao`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 034 - Mapa de atualizacao automatica

### Objetivo

Mapear update legado e bloquear no MOD com rollback.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\05-comunicacoes` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

LiveUpdate stub, firewall MOD, integridade, logs.

### Analises

Analisar area `update`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `05-comunicacoes`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

critica

### Documentacao

`D:\AtelieProd\MOD\docs\05-comunicacoes` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Mapa de atualizacao automatica`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 035 - Classificacao de endpoint externo

### Objetivo

Classificar 191.6.218.152:80 e demais endpoints.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\05-comunicacoes` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

Get-NetTCPConnection, netsh trace, Wireshark/Fiddler futuro.

### Analises

Analisar area `comunicacao`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `05-comunicacoes`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

critica

### Documentacao

`D:\AtelieProd\MOD\docs\05-comunicacoes` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Classificacao de endpoint externo`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 036 - Tracing WinINet/Winsock

### Objetivo

Identificar chamadas, DLL, thread e modulo responsavel por rede.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\05-comunicacoes` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

Get-NetTCPConnection, netsh trace, Wireshark/Fiddler futuro.

### Analises

Analisar area `comunicacao`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `05-comunicacoes`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\05-comunicacoes` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Tracing WinINet/Winsock`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 037 - ProcMon de inicializacao

### Objetivo

Capturar ordem real de arquivos, registry e DLL loading.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\02-runtime` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

ProcMon, ETW, API Monitor, logs de modulos runtime.

### Analises

Analisar area `startup`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `02-runtime`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\02-runtime` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `ProcMon de inicializacao`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 038 - ETW de runtime

### Objetivo

Capturar eventos de processo, rede, disco e CPU.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\07-observabilidade` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

logs estruturados, tracing, metrics, dumps.

### Analises

Analisar area `observabilidade`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `07-observabilidade`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

media

### Documentacao

`D:\AtelieProd\MOD\docs\07-observabilidade` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `ETW de runtime`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 039 - API Monitor de rede

### Objetivo

Monitorar APIs de rede, timers e callbacks.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\05-comunicacoes` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

Get-NetTCPConnection, netsh trace, Wireshark/Fiddler futuro.

### Analises

Analisar area `comunicacao`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `05-comunicacoes`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\05-comunicacoes` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `API Monitor de rede`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 040 - Mapa de modulos principais

### Objetivo

Classificar LavSoft, LavFacilLan, Gerenciador, Financeiro, Estoque, NFE, SAT.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\11-modulos` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

observability, mapa funcional, menus, imports.

### Analises

Analisar area `modulos`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `11-modulos`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\11-modulos` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Mapa de modulos principais`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 041 - Core engine operacional

### Objetivo

Identificar modulo central real do sistema.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\11-modulos` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

observability, mapa funcional, menus, imports.

### Analises

Analisar area `modulos`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `11-modulos`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

critica

### Documentacao

`D:\AtelieProd\MOD\docs\11-modulos` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Core engine operacional`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 042 - Mapa LavFacilLan

### Objetivo

Mapear responsabilidades, dependencias, telas e rede do LavFacilLan.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\11-modulos` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

observability, mapa funcional, menus, imports.

### Analises

Analisar area `modulos`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `11-modulos`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\11-modulos` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Mapa LavFacilLan`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 043 - Mapa LavSoft

### Objetivo

Mapear responsabilidades, dependencias, impressao e fluxo operacional do LavSoft.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\11-modulos` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

observability, mapa funcional, menus, imports.

### Analises

Analisar area `modulos`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `11-modulos`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\11-modulos` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Mapa LavSoft`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 044 - Mapa Gerenciador

### Objetivo

Analisar Gerenciador como modulo administrativo e possivel .NET.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\11-modulos` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

observability, mapa funcional, menus, imports.

### Analises

Analisar area `modulos`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `11-modulos`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\11-modulos` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Mapa Gerenciador`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 045 - Mapa Financeiro

### Objetivo

Mapear fluxos financeiros, relatorios, banco e permissoes.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\11-modulos` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

observability, mapa funcional, menus, imports.

### Analises

Analisar area `modulos`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `11-modulos`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\11-modulos` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Mapa Financeiro`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 046 - Mapa Estoque

### Objetivo

Mapear estoque, comunicacao externa, banco e relatorios.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\11-modulos` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

observability, mapa funcional, menus, imports.

### Analises

Analisar area `modulos`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `11-modulos`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\11-modulos` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Mapa Estoque`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 047 - Mapa NFE

### Objetivo

Mapear NFE em ambiente fiscal controlado.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\11-modulos` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

ambiente fiscal controlado, logs fiscais, ProcMon, drivers.

### Analises

Analisar area `fiscal`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `11-modulos`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

critica

### Documentacao

`D:\AtelieProd\MOD\docs\11-modulos` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Mapa NFE`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 048 - Mapa SAT

### Objetivo

Mapear SAT e dependencias de hardware fiscal.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\11-modulos` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

ambiente fiscal controlado, logs fiscais, ProcMon, drivers.

### Analises

Analisar area `fiscal`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `11-modulos`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

critica

### Documentacao

`D:\AtelieProd\MOD\docs\11-modulos` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Mapa SAT`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 049 - Mapa fiscal consolidado

### Objetivo

Consolidar NFE, SAT, Bematech, Daruma e impressao fiscal.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\11-modulos` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

ambiente fiscal controlado, logs fiscais, ProcMon, drivers.

### Analises

Analisar area `fiscal`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `11-modulos`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

critica

### Documentacao

`D:\AtelieProd\MOD\docs\11-modulos` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Mapa fiscal consolidado`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 050 - Mapa de relatorios

### Objetivo

Catalogar relatorios, parametros, fontes e saidas.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\11-modulos` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

strings, menus, banco, captura visual.

### Analises

Analisar area `relatorios`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `11-modulos`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\11-modulos` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Mapa de relatorios`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 051 - Mapa de menus e telas

### Objetivo

Validar menus, submenus, telas e posicao visual.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\11-modulos` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

captura visual, mapa de menus, screenshots, OCR/manual.

### Analises

Analisar area `frontend`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `11-modulos`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\11-modulos` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Mapa de menus e telas`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 052 - Captura visual de telas

### Objetivo

Capturar screenshots e controles por tela no MOD.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\11-modulos` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

captura visual, mapa de menus, screenshots, OCR/manual.

### Analises

Analisar area `frontend`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `11-modulos`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

media

### Documentacao

`D:\AtelieProd\MOD\docs\11-modulos` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Captura visual de telas`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 053 - Mapa de botoes e acoes

### Objetivo

Associar botoes, atalhos e eventos a permissoes.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\11-modulos` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

captura visual, mapa de menus, screenshots, OCR/manual.

### Analises

Analisar area `frontend`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `11-modulos`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\11-modulos` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Mapa de botoes e acoes`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 054 - Mapa de regras de negocio

### Objetivo

Extrair regras de processos, validacoes e excecoes.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\11-modulos` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

fluxo operacional, banco, telas, logs, entrevistas operacionais.

### Analises

Analisar area `negocio`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `11-modulos`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

critica

### Documentacao

`D:\AtelieProd\MOD\docs\11-modulos` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Mapa de regras de negocio`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 055 - Fluxo Entrada de ROL

### Objetivo

Mapear fluxo completo de entrada de ROL.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\11-modulos` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

fluxo operacional, banco, telas, logs, entrevistas operacionais.

### Analises

Analisar area `negocio`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `11-modulos`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

critica

### Documentacao

`D:\AtelieProd\MOD\docs\11-modulos` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Fluxo Entrada de ROL`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 056 - Fluxo Entrega

### Objetivo

Mapear entrega, baixa e validacoes.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\11-modulos` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

fluxo operacional, banco, telas, logs, entrevistas operacionais.

### Analises

Analisar area `negocio`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `11-modulos`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\11-modulos` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Fluxo Entrega`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 057 - Fluxo Pagamento

### Objetivo

Mapear pagamento, recibos, caixa e auditoria.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\11-modulos` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

fluxo operacional, banco, telas, logs, entrevistas operacionais.

### Analises

Analisar area `negocio`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `11-modulos`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

critica

### Documentacao

`D:\AtelieProd\MOD\docs\11-modulos` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Fluxo Pagamento`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 058 - Fluxo Cancelamento

### Objetivo

Mapear cancelamento, rollback e impactos financeiros.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\11-modulos` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

fluxo operacional, banco, telas, logs, entrevistas operacionais.

### Analises

Analisar area `negocio`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `11-modulos`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

critica

### Documentacao

`D:\AtelieProd\MOD\docs\11-modulos` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Fluxo Cancelamento`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 059 - Fluxo Reemissao

### Objetivo

Mapear reemissao e trilhas de auditoria.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\11-modulos` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

fluxo operacional, banco, telas, logs, entrevistas operacionais.

### Analises

Analisar area `negocio`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `11-modulos`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

media

### Documentacao

`D:\AtelieProd\MOD\docs\11-modulos` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Fluxo Reemissao`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 060 - Fluxo Caixa

### Objetivo

Mapear abertura, fechamento, sangria e relatorios.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\11-modulos` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

banco, telas, relatorios, auditoria.

### Analises

Analisar area `financeiro`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `11-modulos`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

critica

### Documentacao

`D:\AtelieProd\MOD\docs\11-modulos` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Fluxo Caixa`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 061 - Fluxo Faturamento

### Objetivo

Mapear faturamento, cobranca e notas.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\11-modulos` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

banco, telas, relatorios, auditoria.

### Analises

Analisar area `financeiro`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `11-modulos`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\11-modulos` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Fluxo Faturamento`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 062 - Fluxo Cliente

### Objetivo

Mapear cadastro, historico, creditos e bloqueios.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\11-modulos` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

banco, telas, permissao, validacoes.

### Analises

Analisar area `cadastro`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `11-modulos`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\11-modulos` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Fluxo Cliente`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 063 - Fluxo Usuario

### Objetivo

Mapear criacao, alteracao, bloqueio e perfis.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\08-auth` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

ODBC Paradox readonly, scripts auth, logs, captura dinamica.

### Analises

Analisar area `auth`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `08-auth`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\08-auth` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Fluxo Usuario`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 064 - Fluxo Parametros

### Objetivo

Mapear parametros globais e impacto operacional.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\11-modulos` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

INIs/XML/JSON, registry, ProcMon.

### Analises

Analisar area `config`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `11-modulos`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\11-modulos` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Fluxo Parametros`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 065 - Analise de SQL e queries

### Objetivo

Extrair SQL, filtros, selects e relacoes.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\10-database` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

Paradox ODBC readonly, dicionario, indices, integridade.

### Analises

Analisar area `database`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `10-database`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\10-database` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Analise de SQL e queries`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 066 - Analise de indices

### Objetivo

Mapear indices Paradox e impacto de performance.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\10-database` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

Paradox ODBC readonly, dicionario, indices, integridade.

### Analises

Analisar area `database`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `10-database`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\10-database` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Analise de indices`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 067 - Analise de locks BDE

### Objetivo

Mapear PDOXUSRS, LCK, NET DIR e travas.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\10-database` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

Paradox ODBC readonly, dicionario, indices, integridade.

### Analises

Analisar area `database`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `10-database`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

critica

### Documentacao

`D:\AtelieProd\MOD\docs\10-database` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Analise de locks BDE`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 068 - Plano de migracao de banco

### Objetivo

Planejar migracao Paradox para SQLite/API moderna.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\16-migracao` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

scripts ETL, checksums, testes comparativos, rollback.

### Analises

Analisar area `migracao`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `16-migracao`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

critica

### Documentacao

`D:\AtelieProd\MOD\docs\16-migracao` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Plano de migracao de banco`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 069 - Modelo SQLite local

### Objetivo

Projetar schema local offline-first.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\15-nextgen` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

.NET 8, SQLite, API local, arquitetura modular.

### Analises

Analisar area `nextgen`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `15-nextgen`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\15-nextgen` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Modelo SQLite local`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 070 - Outbox local

### Objetivo

Projetar fila local de sincronizacao.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\15-nextgen` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

logs, banco, filas, rede, outbox/inbound planejados.

### Analises

Analisar area `sync`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `15-nextgen`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\15-nextgen` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Outbox local`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 071 - Inbound sync

### Objetivo

Projetar recebimento incremental cloud/local.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\15-nextgen` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

logs, banco, filas, rede, outbox/inbound planejados.

### Analises

Analisar area `sync`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `15-nextgen`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\15-nextgen` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Inbound sync`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 072 - Resolucao de conflitos

### Objetivo

Projetar conflito por entidade e fluxo financeiro/fiscal.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\15-nextgen` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

logs, banco, filas, rede, outbox/inbound planejados.

### Analises

Analisar area `sync`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `15-nextgen`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

critica

### Documentacao

`D:\AtelieProd\MOD\docs\15-nextgen` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Resolucao de conflitos`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 073 - Modelo multi-tenant

### Objetivo

Definir tenant_id, company_id e branch_id.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\15-nextgen` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

Supabase futuro, API, sync, dashboard.

### Analises

Analisar area `cloud`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `15-nextgen`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\15-nextgen` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Modelo multi-tenant`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 074 - Supabase auth

### Objetivo

Planejar autenticacao futura com Supabase.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\14-supabase` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

RLS, Edge Functions, Auth, Postgres, Storage se necessario.

### Analises

Analisar area `supabase`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `14-supabase`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\14-supabase` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Supabase auth`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 075 - Supabase RLS

### Objetivo

Projetar Row Level Security por tenant/filial.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\14-supabase` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

RLS, Edge Functions, Auth, Postgres, Storage se necessario.

### Analises

Analisar area `supabase`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `14-supabase`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

critica

### Documentacao

`D:\AtelieProd\MOD\docs\14-supabase` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Supabase RLS`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 076 - Supabase Edge Functions

### Objetivo

Planejar funcoes para licenca, device e sync.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\14-supabase` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

RLS, Edge Functions, Auth, Postgres, Storage se necessario.

### Analises

Analisar area `supabase`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `14-supabase`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\14-supabase` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Supabase Edge Functions`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 077 - Supabase auditoria

### Objetivo

Projetar auditoria cloud e logs administrativos.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\14-supabase` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

RLS, Edge Functions, Auth, Postgres, Storage se necessario.

### Analises

Analisar area `supabase`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `14-supabase`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\14-supabase` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Supabase auditoria`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 078 - Feature flags

### Objetivo

Projetar flags por tenant, filial, usuario e modulo.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\13-cloud` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

Supabase futuro, API, sync, dashboard.

### Analises

Analisar area `cloud`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `13-cloud`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

media

### Documentacao

`D:\AtelieProd\MOD\docs\13-cloud` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Feature flags`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 079 - Device management

### Objetivo

Projetar cadastro, ativacao, revogacao e auditoria de dispositivos.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\13-cloud` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

Supabase futuro, API, sync, dashboard.

### Analises

Analisar area `cloud`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `13-cloud`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\13-cloud` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Device management`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 080 - Licenciamento moderno

### Objetivo

Projetar licenca local/cloud com janela offline.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\09-licensing` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

strings, ProcMon, rede, registry, analise de arquivos.

### Analises

Analisar area `licensing`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `09-licensing`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\09-licensing` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Licenciamento moderno`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 081 - Device binding moderno

### Objetivo

Projetar fingerprint tolerante a troca parcial de hardware.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\09-licensing` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

strings, ProcMon, rede, registry, analise de arquivos.

### Analises

Analisar area `licensing`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `09-licensing`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\09-licensing` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Device binding moderno`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 082 - Autenticacao local moderna

### Objetivo

Projetar login local, hash forte, sessoes e cache.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\08-auth` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

ODBC Paradox readonly, scripts auth, logs, captura dinamica.

### Analises

Analisar area `auth`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `08-auth`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\08-auth` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Autenticacao local moderna`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 083 - Permissoes granulares

### Objetivo

Projetar permissoes por modulo, tela, botao, acao e API.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\08-auth` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

ODBC Paradox readonly, scripts auth, logs, captura dinamica.

### Analises

Analisar area `auth`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `08-auth`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\08-auth` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Permissoes granulares`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 084 - Auditoria administrativa

### Objetivo

Registrar acoes administrativas e decisoes de permissao.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\17-seguranca` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

hash moderno, auditoria, ACLs, secrets, assinatura.

### Analises

Analisar area `seguranca`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `17-seguranca`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\17-seguranca` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Auditoria administrativa`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 085 - Criptografia e segredos

### Objetivo

Planejar armazenamento seguro de segredos e tokens.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\17-seguranca` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

hash moderno, auditoria, ACLs, secrets, assinatura.

### Analises

Analisar area `seguranca`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `17-seguranca`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

critica

### Documentacao

`D:\AtelieProd\MOD\docs\17-seguranca` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Criptografia e segredos`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 086 - Politica de senhas

### Objetivo

Definir politica moderna e migracao segura.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\17-seguranca` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

hash moderno, auditoria, ACLs, secrets, assinatura.

### Analises

Analisar area `seguranca`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `17-seguranca`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

media

### Documentacao

`D:\AtelieProd\MOD\docs\17-seguranca` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Politica de senhas`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 087 - Hardening local

### Objetivo

Definir protecoes locais, ACLs, logs e integridade.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\17-seguranca` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

hash moderno, auditoria, ACLs, secrets, assinatura.

### Analises

Analisar area `seguranca`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `17-seguranca`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\17-seguranca` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Hardening local`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 088 - Observabilidade estruturada

### Objetivo

Projetar logs estruturados por modulo.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\07-observabilidade` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

logs estruturados, tracing, metrics, dumps.

### Analises

Analisar area `observabilidade`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `07-observabilidade`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\07-observabilidade` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Observabilidade estruturada`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 089 - Tracing distribuido

### Objetivo

Projetar correlation_id entre UI, API, sync e cloud.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\07-observabilidade` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

logs estruturados, tracing, metrics, dumps.

### Analises

Analisar area `observabilidade`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `07-observabilidade`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

media

### Documentacao

`D:\AtelieProd\MOD\docs\07-observabilidade` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Tracing distribuido`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 090 - Metricas locais

### Objetivo

Definir metricas de memoria, CPU, sync e performance.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\07-observabilidade` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

logs estruturados, tracing, metrics, dumps.

### Analises

Analisar area `observabilidade`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `07-observabilidade`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

media

### Documentacao

`D:\AtelieProd\MOD\docs\07-observabilidade` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Metricas locais`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 091 - Crash dumps

### Objetivo

Definir politica de dumps leves e completos.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\07-observabilidade` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

logs estruturados, tracing, metrics, dumps.

### Analises

Analisar area `observabilidade`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `07-observabilidade`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

media

### Documentacao

`D:\AtelieProd\MOD\docs\07-observabilidade` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Crash dumps`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 092 - Health checks

### Objetivo

Projetar checks locais e cloud.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\07-observabilidade` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

logs estruturados, tracing, metrics, dumps.

### Analises

Analisar area `observabilidade`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `07-observabilidade`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

media

### Documentacao

`D:\AtelieProd\MOD\docs\07-observabilidade` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Health checks`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 093 - Dashboard administrativo

### Objetivo

Planejar painel de dispositivos, sync, falhas e versoes.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\13-cloud` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

Supabase futuro, API, sync, dashboard.

### Analises

Analisar area `cloud`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `13-cloud`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

media

### Documentacao

`D:\AtelieProd\MOD\docs\13-cloud` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Dashboard administrativo`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 094 - Updater modular

### Objetivo

Projetar atualizador separado, assinado e reversivel.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\15-nextgen` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

LiveUpdate stub, firewall MOD, integridade, logs.

### Analises

Analisar area `update`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `15-nextgen`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\15-nextgen` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Updater modular`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 095 - Rollback inteligente

### Objetivo

Projetar rollback de modulo, dados e configuracao.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\15-nextgen` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

ZIP, snapshots, checksums, scripts de rollback.

### Analises

Analisar area `rollback`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `15-nextgen`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

critica

### Documentacao

`D:\AtelieProd\MOD\docs\15-nextgen` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Rollback inteligente`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 096 - Delta updates

### Objetivo

Planejar atualizacoes incrementais e canais.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\15-nextgen` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

LiveUpdate stub, firewall MOD, integridade, logs.

### Analises

Analisar area `update`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `15-nextgen`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

media

### Documentacao

`D:\AtelieProd\MOD\docs\15-nextgen` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Delta updates`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 097 - Assinatura digital

### Objetivo

Planejar assinatura de pacotes e verificacao de integridade.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\17-seguranca` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

hash moderno, auditoria, ACLs, secrets, assinatura.

### Analises

Analisar area `seguranca`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `17-seguranca`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\17-seguranca` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Assinatura digital`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 098 - API local

### Objetivo

Projetar API local para desacoplar UI e negocio.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\12-apis` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

OpenAPI futuro, API local .NET, contratos.

### Analises

Analisar area `apis`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `12-apis`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\12-apis` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `API local`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 099 - API auth

### Objetivo

Definir endpoints de login, sessoes e usuario.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\12-apis` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

OpenAPI futuro, API local .NET, contratos.

### Analises

Analisar area `apis`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `12-apis`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\12-apis` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `API auth`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 100 - API permissoes

### Objetivo

Definir endpoints de perfis e decisoes de acesso.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\12-apis` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

OpenAPI futuro, API local .NET, contratos.

### Analises

Analisar area `apis`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `12-apis`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

media

### Documentacao

`D:\AtelieProd\MOD\docs\12-apis` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `API permissoes`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 101 - API financeiro

### Objetivo

Definir contratos financeiros e auditoria.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\12-apis` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

OpenAPI futuro, API local .NET, contratos.

### Analises

Analisar area `apis`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `12-apis`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\12-apis` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `API financeiro`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 102 - API operacional

### Objetivo

Definir contratos de ROL, entrega e pagamento.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\12-apis` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

OpenAPI futuro, API local .NET, contratos.

### Analises

Analisar area `apis`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `12-apis`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\12-apis` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `API operacional`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 103 - API fiscal

### Objetivo

Definir camada fiscal isolada e testavel.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\12-apis` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

OpenAPI futuro, API local .NET, contratos.

### Analises

Analisar area `apis`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `12-apis`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

critica

### Documentacao

`D:\AtelieProd\MOD\docs\12-apis` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `API fiscal`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 104 - API relatorios

### Objetivo

Definir catalogo e execucao de relatorios.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\12-apis` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

OpenAPI futuro, API local .NET, contratos.

### Analises

Analisar area `apis`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `12-apis`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

media

### Documentacao

`D:\AtelieProd\MOD\docs\12-apis` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `API relatorios`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 105 - Frontend moderno

### Objetivo

Planejar UI moderna preservando experiencia operacional.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\15-nextgen` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

captura visual, mapa de menus, screenshots, OCR/manual.

### Analises

Analisar area `frontend`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `15-nextgen`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

media

### Documentacao

`D:\AtelieProd\MOD\docs\15-nextgen` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Frontend moderno`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 106 - Design de telas operacionais

### Objetivo

Projetar telas densas, rapidas e orientadas a teclado.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\15-nextgen` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

captura visual, mapa de menus, screenshots, OCR/manual.

### Analises

Analisar area `frontend`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `15-nextgen`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

media

### Documentacao

`D:\AtelieProd\MOD\docs\15-nextgen` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Design de telas operacionais`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 107 - Compatibilidade com maquinas antigas

### Objetivo

Definir limites de RAM, CPU e inicializacao.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\04-performance` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

PerfView futuro, ETW, amostragem CPU/RAM/I/O.

### Analises

Analisar area `performance`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `04-performance`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\04-performance` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Compatibilidade com maquinas antigas`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 108 - Lazy loading

### Objetivo

Planejar carregamento sob demanda por modulo.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\04-performance` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

PerfView futuro, ETW, amostragem CPU/RAM/I/O.

### Analises

Analisar area `performance`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `04-performance`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

media

### Documentacao

`D:\AtelieProd\MOD\docs\04-performance` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Lazy loading`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 109 - Cache inteligente

### Objetivo

Definir cache local e invalidacao.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\04-performance` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

PerfView futuro, ETW, amostragem CPU/RAM/I/O.

### Analises

Analisar area `performance`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `04-performance`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

media

### Documentacao

`D:\AtelieProd\MOD\docs\04-performance` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Cache inteligente`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 110 - Relatorios performaticos

### Objetivo

Otimizar relatorios por paginacao e indices.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\04-performance` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

PerfView futuro, ETW, amostragem CPU/RAM/I/O.

### Analises

Analisar area `performance`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `04-performance`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

media

### Documentacao

`D:\AtelieProd\MOD\docs\04-performance` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Relatorios performaticos`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 111 - Testes de regressao

### Objetivo

Criar suite de equivalencia legado vs novo.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\16-migracao` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

testes automatizados, comparacao legado/novo.

### Analises

Analisar area `testes`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `16-migracao`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\16-migracao` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Testes de regressao`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 112 - Ambiente de homologacao

### Objetivo

Consolidar ambiente MOD com dados e rollback.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\16-migracao` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

scripts ETL, checksums, testes comparativos, rollback.

### Analises

Analisar area `migracao`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `16-migracao`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\16-migracao` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Ambiente de homologacao`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 113 - Plano piloto

### Objetivo

Definir piloto por modulo/filial.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\16-migracao` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

scripts ETL, checksums, testes comparativos, rollback.

### Analises

Analisar area `migracao`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `16-migracao`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\16-migracao` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Plano piloto`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 114 - Migração de usuarios

### Objetivo

Migrar usuarios, perfis e permissoes.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\16-migracao` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

scripts ETL, checksums, testes comparativos, rollback.

### Analises

Analisar area `migracao`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `16-migracao`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\16-migracao` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Migração de usuarios`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 115 - Migração de cadastros

### Objetivo

Migrar clientes, produtos, parametros e tabelas auxiliares.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\16-migracao` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

scripts ETL, checksums, testes comparativos, rollback.

### Analises

Analisar area `migracao`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `16-migracao`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\16-migracao` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Migração de cadastros`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 116 - Migração financeira

### Objetivo

Migrar contas, caixa e historico com auditoria.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\16-migracao` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

scripts ETL, checksums, testes comparativos, rollback.

### Analises

Analisar area `migracao`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `16-migracao`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

critica

### Documentacao

`D:\AtelieProd\MOD\docs\16-migracao` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Migração financeira`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 117 - Migração operacional

### Objetivo

Migrar ROL, entregas e historico operacional.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\16-migracao` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

scripts ETL, checksums, testes comparativos, rollback.

### Analises

Analisar area `migracao`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `16-migracao`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

critica

### Documentacao

`D:\AtelieProd\MOD\docs\16-migracao` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Migração operacional`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 118 - Migração fiscal

### Objetivo

Migrar documentos fiscais com compliance.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\16-migracao` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

scripts ETL, checksums, testes comparativos, rollback.

### Analises

Analisar area `migracao`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `16-migracao`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

critica

### Documentacao

`D:\AtelieProd\MOD\docs\16-migracao` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Migração fiscal`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 119 - Plano de coexistencia

### Objetivo

Planejar legado e novo operando lado a lado.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\16-migracao` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

scripts ETL, checksums, testes comparativos, rollback.

### Analises

Analisar area `migracao`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `16-migracao`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

critica

### Documentacao

`D:\AtelieProd\MOD\docs\16-migracao` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Plano de coexistencia`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 120 - Plano de corte gradual

### Objetivo

Definir criterios de ativacao por modulo.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\16-migracao` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

scripts ETL, checksums, testes comparativos, rollback.

### Analises

Analisar area `migracao`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `16-migracao`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\16-migracao` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Plano de corte gradual`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 121 - Plano de disaster recovery

### Objetivo

Definir backup, restore, RPO e RTO.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\18-risk` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

matriz de risco, criticidade, mitigacao.

### Analises

Analisar area `risco`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `18-risk`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

critica

### Documentacao

`D:\AtelieProd\MOD\docs\18-risk` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Plano de disaster recovery`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 122 - Plano de backup inteligente

### Objetivo

Projetar backups locais/cloud e verificacao.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\18-risk` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

matriz de risco, criticidade, mitigacao.

### Analises

Analisar area `risco`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `18-risk`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\18-risk` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Plano de backup inteligente`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 123 - Analise de riscos

### Objetivo

Classificar riscos tecnicos, operacionais e fiscais.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\18-risk` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

matriz de risco, criticidade, mitigacao.

### Analises

Analisar area `risco`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `18-risk`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\18-risk` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Analise de riscos`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 124 - Matriz de criticidade

### Objetivo

Criar matriz por modulo/dependencia/fluxo.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\18-risk` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

matriz de risco, criticidade, mitigacao.

### Analises

Analisar area `risco`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `18-risk`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\18-risk` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Matriz de criticidade`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 125 - Matriz de substituicao

### Objetivo

Classificar componentes substituiveis, criticos e obsoletos.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\18-risk` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

matriz de risco, criticidade, mitigacao.

### Analises

Analisar area `risco`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `18-risk`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\18-risk` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Matriz de substituicao`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 126 - Plano de ferramentas

### Objetivo

Documentar ferramentas instaladas, motivo, risco e rollback.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\00-controle` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

log-de-alteracoes, Projeto_Novo_Atelie_2026, CSV de rastreio.

### Analises

Analisar area `controle`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `00-controle`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

media

### Documentacao

`D:\AtelieProd\MOD\docs\00-controle` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Plano de ferramentas`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 127 - Governanca de documentacao

### Objetivo

Padronizar evidencias, logs e relatorios.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\00-controle` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

log-de-alteracoes, Projeto_Novo_Atelie_2026, CSV de rastreio.

### Analises

Analisar area `controle`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `00-controle`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

media

### Documentacao

`D:\AtelieProd\MOD\docs\00-controle` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Governanca de documentacao`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 128 - Rastreabilidade de achados

### Objetivo

Ligar fase, evidencia, arquivo, risco e decisao.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\00-controle` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

log-de-alteracoes, Projeto_Novo_Atelie_2026, CSV de rastreio.

### Analises

Analisar area `controle`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `00-controle`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

media

### Documentacao

`D:\AtelieProd\MOD\docs\00-controle` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Rastreabilidade de achados`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 129 - Relatorio executivo

### Objetivo

Consolidar status para decisao administrativa.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\10-relatorio-final` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

Markdown, CSV, evidencias, anexos.

### Analises

Analisar area `relatorio`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `10-relatorio-final`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

media

### Documentacao

`D:\AtelieProd\MOD\docs\10-relatorio-final` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Relatorio executivo`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 130 - Relatorio tecnico profundo

### Objetivo

Consolidar arquitetura, dependencias e fluxo real.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\10-relatorio-final` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

Markdown, CSV, evidencias, anexos.

### Analises

Analisar area `relatorio`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `10-relatorio-final`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\10-relatorio-final` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Relatorio tecnico profundo`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 131 - Manual tecnico

### Objetivo

Criar manual de operacao, suporte e diagnostico.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\09-manuais` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

documentacao operacional e administrativa.

### Analises

Analisar area `manual`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `09-manuais`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

media

### Documentacao

`D:\AtelieProd\MOD\docs\09-manuais` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Manual tecnico`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 132 - Manual administrativo

### Objetivo

Criar manual de usuarios, permissoes e auditoria.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\09-manuais` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

documentacao operacional e administrativa.

### Analises

Analisar area `manual`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `09-manuais`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

media

### Documentacao

`D:\AtelieProd\MOD\docs\09-manuais` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Manual administrativo`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 133 - Manual de rollback

### Objetivo

Documentar rollback por modulo, banco e update.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\09-manuais` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

documentacao operacional e administrativa.

### Analises

Analisar area `manual`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `09-manuais`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\09-manuais` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Manual de rollback`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 134 - Treinamento operacional

### Objetivo

Planejar treinamento de usuarios e administradores.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\09-manuais` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

checklists, monitoramento, suporte assistido.

### Analises

Analisar area `operacao`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `09-manuais`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

media

### Documentacao

`D:\AtelieProd\MOD\docs\09-manuais` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Treinamento operacional`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 135 - Validacao de equivalencia

### Objetivo

Comparar resultados legado e novo por fluxo.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\16-migracao` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

scripts ETL, checksums, testes comparativos, rollback.

### Analises

Analisar area `migracao`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `16-migracao`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

critica

### Documentacao

`D:\AtelieProd\MOD\docs\16-migracao` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Validacao de equivalencia`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 136 - Aceite por modulo

### Objetivo

Definir criterios e checklist de aceite.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\16-migracao` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

scripts ETL, checksums, testes comparativos, rollback.

### Analises

Analisar area `migracao`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `16-migracao`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\16-migracao` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Aceite por modulo`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 137 - Desativacao de dependencias antigas

### Objetivo

Planejar retirada gradual de componentes legados.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\16-migracao` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

scripts ETL, checksums, testes comparativos, rollback.

### Analises

Analisar area `migracao`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `16-migracao`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\16-migracao` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Desativacao de dependencias antigas`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 138 - Ativacao cloud controlada

### Objetivo

Ativar cloud por tenant com fallback local.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\13-cloud` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

Supabase futuro, API, sync, dashboard.

### Analises

Analisar area `cloud`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `13-cloud`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\13-cloud` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Ativacao cloud controlada`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 139 - Operacao assistida

### Objetivo

Monitorar fase inicial de uso real.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\07-operacao` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

checklists, monitoramento, suporte assistido.

### Analises

Analisar area `operacao`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `07-operacao`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\07-operacao` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Operacao assistida`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 140 - Otimização pos-migracao

### Objetivo

Ajustar performance, memoria e relatórios.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\04-performance` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

PerfView futuro, ETW, amostragem CPU/RAM/I/O.

### Analises

Analisar area `performance`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `04-performance`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

media

### Documentacao

`D:\AtelieProd\MOD\docs\04-performance` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Otimização pos-migracao`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 141 - Auditoria final

### Objetivo

Validar seguranca, logs, permissoes e integridade.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\17-seguranca` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

hash moderno, auditoria, ACLs, secrets, assinatura.

### Analises

Analisar area `seguranca`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `17-seguranca`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\17-seguranca` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Auditoria final`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 142 - Entrega da nova geracao

### Objetivo

Consolidar plataforma moderna pronta para evolucao continua.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\15-nextgen` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

.NET 8, SQLite, API local, arquitetura modular.

### Analises

Analisar area `nextgen`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `15-nextgen`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

critica

### Documentacao

`D:\AtelieProd\MOD\docs\15-nextgen` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Entrega da nova geracao`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 143 - Hypercare pos-entrega

### Objetivo

Acompanhar operacao real com suporte intensivo e metricas.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\07-operacao` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

checklists, monitoramento, suporte assistido.

### Analises

Analisar area `operacao`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `07-operacao`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\07-operacao` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Hypercare pos-entrega`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 144 - Monitoramento de regressao

### Objetivo

Detectar regressao de memoria, CPU, rede e UX entre versoes.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\04-performance` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

PerfView futuro, ETW, amostragem CPU/RAM/I/O.

### Analises

Analisar area `performance`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `04-performance`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\04-performance` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Monitoramento de regressao`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 145 - Governanca de releases

### Objetivo

Definir processo de release, aprovacao, changelog e rollback.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\00-controle` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

log-de-alteracoes, Projeto_Novo_Atelie_2026, CSV de rastreio.

### Analises

Analisar area `controle`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `00-controle`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\00-controle` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Governanca de releases`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 146 - Gestao de configuracao

### Objetivo

Controlar parametros, ambientes, segredos e variaveis por tenant.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\00-controle` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

log-de-alteracoes, Projeto_Novo_Atelie_2026, CSV de rastreio.

### Analises

Analisar area `controle`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `00-controle`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\00-controle` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Gestao de configuracao`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 147 - Revisao periodica de seguranca

### Objetivo

Executar revisoes recorrentes de permissoes, segredos e auditoria.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\17-seguranca` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

hash moderno, auditoria, ACLs, secrets, assinatura.

### Analises

Analisar area `seguranca`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `17-seguranca`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\17-seguranca` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Revisao periodica de seguranca`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 148 - Revisao periodica de dados

### Objetivo

Auditar integridade, crescimento, indices e retencao de dados.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\10-database` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

Paradox ODBC readonly, dicionario, indices, integridade.

### Analises

Analisar area `database`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `10-database`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

alta

### Documentacao

`D:\AtelieProd\MOD\docs\10-database` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Revisao periodica de dados`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 149 - Roadmap de evolucao continua

### Objetivo

Planejar ciclos futuros, prioridades e depreciacao de componentes.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\15-nextgen` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

.NET 8, SQLite, API local, arquitetura modular.

### Analises

Analisar area `nextgen`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `15-nextgen`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

media

### Documentacao

`D:\AtelieProd\MOD\docs\15-nextgen` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Roadmap de evolucao continua`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.

## FASE 150 - Encerramento da recuperacao legada

### Objetivo

Formalizar conclusao da recuperacao e estado final do legado.

### Escopo

Documentar e executar esta fase dentro de `D:\AtelieProd\MOD\docs\10-relatorio-final` usando evidencias controladas.

### Tarefas

- coletar evidencias;
- classificar achados;
- cruzar com mapas existentes;
- atualizar log tecnico;
- atualizar arquivo mestre do projeto.

### Ferramentas

Markdown, CSV, evidencias, anexos.

### Analises

Analisar area `relatorio`, dependencias, impacto operacional, riscos e relacao com modulos principais.

### Validacoes

- validar que o original nao foi alterado;
- validar arquivos gerados;
- validar consistencia com evidencias anteriores;
- registrar limitacoes.

### Evidencias

CSV, Markdown, logs, snapshots ou capturas associados a `10-relatorio-final`.

### Logs

`D:\AtelieProd\MOD\docs\00-controle\log-de-alteracoes.md` e logs tecnicos especificos da fase.

### Rollback

Remover ou ignorar apenas artefatos gerados no MOD; nunca reverter ou tocar no original.

### Criticidade

critica

### Documentacao

`D:\AtelieProd\MOD\docs\10-relatorio-final` e referencia em `Projeto_Novo_Atelie_2026.md` quando houver achado relevante.

### Entregaveis

Relatorio da fase, evidencias, classificacao de risco e proximas acoes para `Encerramento da recuperacao legada`.

### Impacto operacional

Somente MOD/homologacao; original deve permanecer intacto; qualquer execucao deve ter rollback documentado.

### Impacto em memoria

Medir ou estimar impacto; preservar baixo consumo; registrar picos quando houver runtime.

### Impacto em CPU

Medir CPU quando aplicavel; evitar ferramentas intrusivas fora de janela controlada.

### Impacto em rede

Documentar conexoes; bloquear ou isolar somente MOD quando necessario e autorizado.

### Impacto em autenticacao

Nao alterar credenciais legadas; usar copia readonly e modulo MOD proprio.

### Impacto em sincronizacao

Nao acionar sincronizacao de producao; validar comportamento em MOD e registrar endpoints.

### Impacto em licenciamento

Nao burlar licenca; mapear comportamento e projetar substituto autorizado para nova versao.
