# Matriz de Visibilidade Total do EquipeExe

Data: 2026-05-23

## Objetivo

Criar visibilidade total e progressiva do EquipeExe antes de qualquer reconstrução massiva.

Esta matriz controla o que ja foi observado, o que esta parcialmente conhecido e o que ainda exige captura dinamica, ferramenta externa ou validacao operacional.

## Principios

- O original `D:\AtelieProd\Equipexe` permanece intocado.
- Coletas dinamicas devem usar o runtime MOD.
- Evidencias brutas grandes devem ser resumidas antes de uso executivo.
- Cada achado deve ter origem, impacto, risco e proxima acao.
- A nova arquitetura deve preservar a leveza operacional do legado.

## Matriz

| Area | Status | Evidencia atual | Risco | Proxima acao |
|---|---|---|---|---|
| Inventario de arquivos | Parcial alto | CSVs e resumo em `docs\01-inventario` | Baixo | Classificar criticidade por tipo |
| Executaveis principais | Parcial alto | `LavSoft`, `LavFacilLan`, `Gerenciador`, `Financeiro`, `Estoque`, `NFE`, `SAT` mapeados | Medio | Monitorar todos no MOD |
| DLLs/imports PE | Parcial alto | `imports-executaveis-dlls.csv`, `imports-resumo.csv` | Medio | Gerar mapa de DLL por modulo |
| Banco Paradox/BDE | Parcial alto | 478 tabelas e 4.996 colunas extraidas | Alto | Validar relacionamentos e tabelas criticas |
| Menus/permissoes | Parcial alto | `Nivel.DB`, mapas de menus e correlacoes | Medio | Validar por captura visual |
| Layout de telas | Parcial baixo | Extracao TPF0 ruidosa | Medio | Captura dinamica por tela |
| Relatorios | Parcial medio | Strings e permissoes | Medio | Identificar fontes de dados e parametros |
| Autenticacao legada | Parcial medio | Usuarios, niveis, hipotese senha +1 | Alto | Confirmar fluxo de login no MOD |
| Licenciamento | Parcial baixo | Sinais estaticos ruidosos | Alto | Isolar arquivos legiveis e fluxos online |
| Hardware binding | Parcial baixo | Sinais de MAC/maquina/serial em varredura | Alto | Separar evidencias por arquivo e validacao dinamica |
| Atualizacao automatica | Parcial alto | `LiveUpdate` bloqueado no MOD | Alto | Monitorar outros pontos de update/sync |
| Comunicacoes externas | Parcial medio | WinINet/Winsock/urlmon/OpenSSL legado | Alto | Captura dinamica com PID e endpoints |
| Memoria/performance | Inicial | `LavSoft` monitorado por 20s | Medio | Amostrar por tela/modulo |
| Processos filhos | Inicial | `splwow64.exe` observado no `LavSoft` | Medio | Mapear por modulo |
| Impressao/fiscal | Parcial medio | Bematech, Daruma, SAT, NFE/NFSe | Alto | Inventariar drivers e fluxos fiscais |
| Sincronizacao/nuvem | Parcial baixo | Sinais `Sincroniza\Nuvem` e update | Alto | Mapear filas, endpoints e tabelas |
| Logs legados | Parcial baixo | Inventario inicial | Medio | Classificar por modulo e erro |
| Configuracoes | Parcial medio | INIs/XMLs/JSONs inventariados | Medio | Extrair chaves operacionais |
| Observabilidade MOD | Inicial alto | Coletor C# criado e validado | Baixo | Repetir para todos os modulos |
| Arquitetura futura | Em elaboracao | Documentos `11-arquitetura-futura` | Medio | Refinar com achados dinamicos |

## Fases de visibilidade

### Fase A - Estatica

- Inventario completo.
- Imports/exports PE.
- Strings relevantes.
- Banco e permissao.
- Configuracoes.
- Relatorios e menus.

### Fase B - Dinamica leve

- Abrir cada executavel no MOD.
- Medir RAM, CPU, handles, threads.
- Capturar processos filhos.
- Capturar conexoes TCP.
- Identificar DLLs carregadas.

### Fase C - Fluxo operacional

- Login.
- Entrada de ROL.
- Entrega.
- Pagamento.
- Caixa.
- Financeiro.
- Estoque.
- NFE/SAT.
- Relatorios principais.

### Fase D - Instrumentacao avancada

- ProcMon para arquivos/registro/processos.
- Process Explorer para DLLs e handles.
- Fiddler/Wireshark para trafego autorizado.
- ILSpy/dnSpy para componentes .NET.
- WinDbg/x64dbg/API Monitor somente quando necessario.

## Criterio de visibilidade total

Uma area so deve ser considerada totalmente visivel quando possuir:

- evidencia estatica;
- evidencia dinamica;
- impacto operacional;
- risco;
- dependencia;
- rollback;
- plano de modernizacao;
- dono/modulo futuro sugerido.
