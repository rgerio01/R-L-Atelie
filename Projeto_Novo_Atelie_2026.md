# Projeto Novo Atelie 2026

## Objetivo

Reconstruir o sistema EquipeExe em uma nova estrutura moderna, controlada e administravel internamente, mantendo o sistema original intacto.

## Regra operacional

- Original preservado: `D:\AtelieProd\Equipexe`
- Nova versao e homologacao: `D:\AtelieProd\MOD`
- Toda alteracao deve ocorrer apenas em `MOD`.
- O original nao deve ser sobrescrito, apagado ou alterado.

## Estado atual do projeto

### Concluido

- Estrutura `MOD` criada.
- Backup completo do original criado.
- Copia readonly dos bancos Paradox/BDE criada.
- Dicionario inicial de banco extraido.
- API MOD criada em `.NET 8`.
- Autenticacao propria criada.
- Usuario `gabriela` criado como administradora principal.
- Senha da Gabriela no MOD definida como `12345`.
- Troca obrigatoria de senha desativada no MOD.
- Investigacao da senha legada realizada.
- Bloqueio de atualizacao automatica implementado no MOD.
- Mapa inicial de menus/submenus gerado.
- Inicio de extracao de layouts/telas dos executaveis.
- Analise profunda Fase 3 executada: imports PE, sinais de rede, atualizacao, autenticacao/licenca, hardware binding, memoria/processos e locks.
- Relatorio consolidado da Fase 3 criado.
- Mapa funcional de telas, menus e relatorios gerado contra executaveis principais.
- Correlacao inicial entre `Nivel.DB`, menus/permissoes e strings dos executaveis gerada.
- Camada propria de observabilidade/profiling criada em C#.
- Snapshot baseline de processos/rede criado.
- Primeira execucao dinamica controlada do `LavSoft` no runtime MOD realizada.
- Dependencias `.dll`/`.ocx` necessarias ao runtime MOD copiadas de forma controlada, sem alterar o original.
- Criada matriz de visibilidade total do EquipeExe.
- Criada estrategia de arquitetura futura consolidada.
- Criado plano offline-first, sync e Supabase.
- Criado plano de modularizacao.
- Criado plano de observabilidade futura.
- Criada estrategia final de modernizacao incremental.
- Executados baselines dinamicos curtos de `LavFacilLan`, `Gerenciador`, `Financeiro` e `Estoque` no runtime MOD.
- Identificada comunicacao HTTP externa de `LavFacilLan` e `Estoque` para `191.6.218.152:80`.
- Criados scripts de isolamento de rede MOD por firewall, com rollback.
- Tentativa de aplicar regras de firewall falhou por `Acesso negado`, exigindo execucao elevada.

## Backup

Backup completo:

`D:\AtelieProd\MOD\backups\original\Equipexe-original-20260523-111141.zip`

SHA-256:

`26EFD94C7C18A12E0977C46B339006AA03ABE778EFE72AA93A7360991B89CC43`

## Arquitetura encontrada no legado

O sistema atual aparenta ser uma aplicacao desktop Windows legada baseada em Borland/Delphi, BDE e Paradox.

Evidencias:

- Arquivos `.DB`, `.PX`, `.XG*`, `.YG*`, `.MB`.
- DLLs Borland/BDE.
- Executaveis separados por modulo.
- Uso de `Senhas.exe`.
- Uso de `LiveUpdate.exe`.
- Uso de `Sincroniza\Nuvem`.
- Integrações fiscais e hardware: Bematech, Daruma, SAT, NFE, impressoras e possivel biometria.

## Nova arquitetura proposta

### Backend

Local:

`D:\AtelieProd\MOD\apps\backend\EquipeExe.Mod.Api`

Tecnologia:

`.NET 8`

Funcoes iniciais:

- login;
- token local;
- usuarios;
- perfis;
- permissoes;
- auditoria;
- inventario legado.

### Frontend

Local proposto:

`D:\AtelieProd\MOD\apps\frontend`

Tecnologia sugerida:

React + Vite consumindo a API MOD.

Primeiras telas:

- Login
- Dashboard
- Usuarios
- Permissoes
- Inventario legado
- Banco de dados
- Atualizacoes bloqueadas
- Logs/Auditoria
- Modulos operacionais

### Runtime legado MOD

Local:

`D:\AtelieProd\MOD\apps\legacy-runtime\Equipexe`

Objetivo:

Executar e analisar o comportamento do legado em ambiente isolado, com bloqueios e controles proprios da MOD.

## Bloqueio de atualizacao automatica

O original possui:

`D:\AtelieProd\Equipexe\Exe\LiveUpdate.exe`

Na MOD, foi criado substituto seguro:

`D:\AtelieProd\MOD\apps\legacy-runtime\Equipexe\Exe\LiveUpdate.exe`

Comportamento:

- nao busca atualizacoes;
- nao baixa arquivos;
- nao instala nada;
- registra log local;
- retorna sucesso para evitar erro na abertura.

Politica:

`D:\AtelieProd\MOD\config\env\update-policy.json`

Validacao:

`D:\AtelieProd\MOD\apps\tools\verify-update-block.ps1`

## Banco de dados

Copia readonly:

`D:\AtelieProd\MOD\data\original-readonly\Equipexe`

Resultado inicial:

- 4.562 arquivos copiados readonly.
- 478 tabelas extraidas.
- 4.996 colunas extraidas.
- 485 tabelas com falha no driver ODBC antigo.

Arquivos:

- `D:\AtelieProd\MOD\docs\03-banco-de-dados\dicionario-paradox-tabelas.csv`
- `D:\AtelieProd\MOD\docs\03-banco-de-dados\dicionario-paradox-colunas.csv`
- `D:\AtelieProd\MOD\docs\03-banco-de-dados\dicionario-paradox-falhas.csv`

Tabelas importantes:

- `Ger\Dados\Usuarios.DB`
- `Ger\Dados\Nivel.DB`
- `Ger\Dados\UsuaSis.DB`
- `Ger\Dados\Clientes.DB`
- `Lav\FILIAL\MovCab.DB`
- `Lav\FILIAL\Notas.DB`
- `REC\FILIAL\Duplicat.DB`

## Autenticacao e senha

### MOD

Usuario principal:

- usuario: `gabriela`
- perfil: `administrador`
- senha: `12345`
- troca obrigatoria: desativada

### Legado

Tabelas analisadas:

- `Usuarios.DB`
- `UsuaSis.DB`
- `Nivel.DB`
- `Senhas.DB`

Hipotese forte:

O campo `Usuarios.Senha` usa deslocamento ASCII `+1`.

Exemplo:

- senha digitada: `12345`
- valor armazenado esperado: `23456`

Utilitario:

`D:\AtelieProd\MOD\apps\tools\legacy-password-codec.ps1`

## Menus e submenus identificados

Arquivo detalhado:

`D:\AtelieProd\MOD\docs\02-arquitetura-legada\menus-submenus-identificados.md`

### LavSoft

Operacional:

- Entrada de ROL
- Lancamento de ROL
- Entrega de ROL
- Entrega por Pecas
- Pagamento de ROL
- Cancelamento
- Reemissao
- ROLs em Aberto
- ROLs Pagos
- ROLs Cancelados
- Situacao dos ROLs
- Controle de Lavagem
- Passadoria
- Terceirizacao
- Localizacao de pecas

Caixa/financeiro:

- Caixa Dia a Dia
- Controle de Caixa
- Fechamento de Caixa
- Recibo de Caixa
- Creditos
- Cobranca
- Carta de Cobranca
- Faturamento
- Nota Fiscal
- Descontos
- Devolucoes

Fiscal:

- CF Abertura
- CF Fechamento
- CF Fechamento Parcial
- CF Fundo de Caixa
- CF Sangria
- CF Leitura X
- CF Leitura Memoria Fiscal
- CF Horario de Verao
- CF Destrava Impressora Fiscal

Cadastros:

- Clientes
- Usuarios
- Filiais/Matriz
- Parametros
- Tabela de Preco
- Forma de Pagamento
- Condicao de Pagamento
- Tipos de Entrada
- Tipos de Servico
- Tipos de Tecido
- Cores
- Marcas
- Defeitos
- Custos
- Feriados

Relatorios:

- Movimento Analitico
- Movimento Sintetico
- Movimento por Produto
- Movimento por Servico
- Movimento por Dia da Semana
- Relatorio para Entrega
- Conferencia de ROL
- Estoque no Cliente
- Previsao de Entrega
- Comissoes
- Frequencia de Cliente

### Estoque

- Cadastros
- Produtos
- Alterar Produto
- Procurar Produtos
- Movimentos de Estoque
- Entrada de Estoque
- Baixa de Estoque
- Atualiza Estoque
- Encerramento
- Cancelamento
- Nota Fiscal
- Relatorios

### Gerenciador

- Download
- Upload
- Tarefas
- Logs
- Opcoes de conexao
- Idioma
- Usuario
- Sincronizacao
- Verificacao de atualizacoes

### SAT

- Solicitacoes
- Ocorrencias
- Anotacoes
- Alteracoes/projetos
- Usuarios/permissoes
- Relatorios

## Telas e layouts

Inicio de extracao:

`D:\AtelieProd\MOD\docs\02-arquitetura-legada\layouts-telas`

Primeiros resultados:

- `LavSoft`: 329 blocos de tela/recurso encontrados.
- `LavFacilLan`: 174 blocos encontrados.
- `Estoque`: 18 blocos encontrados antes da interrupcao por tempo.

Objetivo da proxima fase:

Mapear cada submenu para:

- tela aberta;
- titulo da tela;
- campos;
- botoes;
- posicao visual;
- tabela acessada;
- campo do banco;
- permissao exigida;
- relatorio ou processo chamado;
- dependencia externa.

Modelo de mapeamento:

```text
Modulo | Menu | Submenu | Tela | Campo visual | Tabela | Campo DB | Botao | Evento | Permissao | Observacao
```

## Prioridade de implementacao do novo frontend

### Fase 1 - Administrativa

- Login
- Dashboard
- Usuarios
- Perfis
- Permissoes
- Logs
- Atualizacoes bloqueadas
- Inventario tecnico

### Fase 2 - Operacional LavSoft

- Clientes
- Entrada de ROL
- Pagamento de ROL
- Entrega de ROL
- Controle de Caixa
- Situacao dos ROLs

### Fase 3 - Financeiro/Fiscal

- Faturamento
- Nota Fiscal
- Recibos
- Relatorios fiscais
- Integrações SAT/NFE

### Fase 4 - Estoque

- Produtos
- Movimentos
- Entrada/Baixa
- Encerramento
- Relatorios

## Entregaveis vivos

Este arquivo deve ser atualizado continuamente conforme novas coletas forem feitas.

## Diretriz permanente de registro

A partir desta etapa, toda analise, decisao, implementacao, teste, bloqueio, script, documento, descoberta tecnica e hipotese relevante deve ser registrada tambem neste arquivo mestre `Projeto_Novo_Atelie_2026.md`.

Os documentos especificos continuam existindo, mas este arquivo passa a ser o indice executivo e tecnico consolidado do projeto.

## Diretriz de arquitetura profunda

O trabalho deve ser conduzido como engenharia reversa controlada e recuperacao de sistema critico, com foco em:

- descobrir o maximo possivel da arquitetura real;
- identificar comportamentos ocultos;
- entender logica operacional;
- mapear dependencias e riscos;
- preservar continuidade operacional;
- reconstruir gradualmente uma nova versao administravel;
- preservar leveza, velocidade e baixo consumo de recursos;
- preparar arquitetura local/nuvem hibrida;
- preparar futura integracao com Supabase;
- criar nova autenticacao, licenciamento e controle de dispositivos.

## Analise profunda obrigatoria

Devem ser analisados, inventariados e documentados:

- executaveis;
- DLLs;
- servicos;
- modulos internos;
- APIs;
- arquivos de configuracao;
- INIs;
- XMLs;
- JSONs;
- bancos de dados;
- logs;
- cache;
- servicos do Windows;
- drivers;
- tarefas agendadas;
- dependencias externas;
- autenticacao;
- licenciamento;
- identificacao de hardware;
- comunicacao remota;
- telemetria;
- trackers;
- chamadas HTTP/HTTPS;
- sockets;
- atualizacao automatica;
- threads;
- uso de memoria;
- gerenciamento de RAM;
- arquivos temporarios;
- carregamento de modulos;
- fluxo de inicializacao;
- gargalos;
- possiveis vazamentos;
- processos filhos;
- comportamento offline;
- comportamento online;
- comportamento degradado;
- mecanismos administrativos ocultos;
- paineis internos;
- permissoes internas;
- regras de negocio ocultas;
- dependencias nao documentadas.

## Mapeamento de capacidades

O projeto deve descobrir e classificar:

- tudo o que o sistema consegue fazer;
- funcionalidades ocultas;
- funcoes nao utilizadas;
- modulos abandonados;
- recursos parcialmente implementados;
- integracoes desativadas;
- funcionalidades administrativas;
- permissoes avancadas;
- rotinas automaticas;
- possiveis recursos futuros;
- partes criticas;
- partes substituiveis;
- partes modernizaveis;
- partes que devem permanecer iguais;
- partes dependentes da estrutura antiga.

## Estrategia de modernizacao

A modernizacao deve considerar:

- reconstrucao gradual;
- migracao parcial;
- migracao total;
- coexistencia entre legado e novo;
- modularizacao;
- separacao frontend/backend;
- APIs internas;
- servicos independentes;
- escalabilidade futura;
- atualizacao tecnologica;
- arquitetura hibrida;
- cloud hibrida;
- rollback seguro;
- manutencao futura.

## Autenticacao e licenciamento futuros

O legado deve ser analisado para entender:

- autenticacao atual;
- licenciamento atual;
- validacao por hardware;
- identificacao de maquina;
- dependencias remotas;
- fluxo de ativacao;
- gerenciamento de usuarios;
- permissoes administrativas;
- comunicacao de autenticacao.

A nova estrutura deve planejar:

- autenticacao propria;
- autenticacao local;
- autenticacao hibrida;
- autenticacao via nuvem;
- gerenciamento centralizado;
- gerenciamento de dispositivos;
- gerenciamento de sessoes;
- gerenciamento de permissoes;
- gerenciamento de clientes;
- auditoria administrativa.

## Planejamento Supabase e cloud hibrida

O Supabase deve ser considerado como camada futura para:

- autenticacao;
- usuarios;
- permissoes;
- licenciamento;
- sessoes;
- dispositivos autorizados;
- bloqueio por hardware;
- logs;
- auditoria;
- sincronizacao;
- atualizacoes;
- telemetria administrativa;
- gerenciamento centralizado.

Arquitetura futura desejada:

```text
Aplicacao Desktop/Web MOD
        |
        |-- Banco local/cache operacional
        |-- API local MOD
        |-- Servicos locais
        |
        |-- Sincronizador controlado
                |
                |-- Supabase Auth
                |-- Supabase Postgres
                |-- Supabase Storage
                |-- Edge Functions
                |-- Logs/Auditoria centralizados
```

Requisitos da arquitetura hibrida:

- funcionamento local mesmo sem internet;
- fila de sincronizacao;
- cache local;
- controle de conflito;
- revogacao remota de dispositivos;
- bloqueio administrativo;
- auditoria local e remota;
- sincronizacao configuravel;
- atualizacao controlada e reversivel;
- dashboard administrativo futuro.

## Controle por hardware futuro

O sistema legado deve ser analisado para descobrir como identifica a maquina e se usa:

- nome do computador;
- usuario Windows;
- MAC address;
- serial de disco;
- registro do Windows;
- arquivo de ativacao;
- validacao remota;
- codigo de loja;
- identificadores em `Registrar.xml`, INIs ou tabelas.

A nova solucao deve usar device binding moderno com:

- multiplos identificadores;
- tolerancia a troca parcial de hardware;
- politica de ativacao;
- politica de troca de equipamento;
- politica de bloqueio;
- politica de recuperacao;
- politica de auditoria;
- aprovacao administrativa de novos dispositivos;
- revogacao remota quando houver cloud disponivel.

## Memoria e performance

O novo sistema deve preservar a leveza operacional do EquipeExe.

Metas:

- baixo consumo de RAM;
- inicializacao rapida;
- baixo uso de CPU;
- funcionamento fluido em maquinas antigas;
- carregamento sob demanda;
- telas modulares;
- cache controlado;
- minimo uso de processos residentes;
- logs eficientes;
- consultas paginadas;
- indices adequados no banco moderno;
- evitar frameworks pesados sem necessidade operacional.

Analises futuras:

- consumo por executavel legado;
- processos filhos;
- memoria inicial;
- memoria por tela aberta;
- tempo de abertura;
- leitura/escrita em disco;
- arquivos temporarios;
- locks Paradox;
- gargalos de rede;
- gargalos de relatorios.

## Analise profunda - Fase 3

Relatorio gerado:

`D:\AtelieProd\MOD\docs\02-arquitetura-legada\relatorio-analise-profunda-fase-3.md`

Artefatos principais:

- `D:\AtelieProd\MOD\docs\02-arquitetura-legada\pe-imports\imports-executaveis-dlls.csv`
- `D:\AtelieProd\MOD\docs\02-arquitetura-legada\pe-imports\imports-resumo.csv`
- `D:\AtelieProd\MOD\docs\02-arquitetura-legada\sinais-profundos`

Resumo dos achados:

- Predominancia de binarios Windows 32-bit (`0x014C`).
- Presenca pontual de binario 64-bit.
- Dependencia forte de BDE/Paradox.
- Superficie de comunicacao externa em `wininet.dll`, `wsock32.dll` e `urlmon.dll`.
- Dependencias fiscais/hardware em Bematech, Daruma, SAT, NFE/NFSe e impressao Windows.
- Dependencia de OpenSSL legado por `LIBEAY32.dll` e `ssleay32.dll`.
- `LavSoft.exe` e `LavFacilLan.exe` possuem ligacao fiscal/impressao e acesso a recursos de rede.
- `LavFacilLan.exe`, `Estoque.exe` e `NFE.exe` possuem capacidade direta de comunicacao via `wininet.dll`.
- `Gerenciador.exe` importa `mscoree.dll`, indicando componente .NET ou bootstrapper .NET.
- Existem sinais de atualizacao/sincronizacao em `LiveUpdate`, `EquEstruAtu` e diretorios/termos relacionados a `Sincroniza\Nuvem`.
- O bloqueio de atualizacao automatica esta implementado somente no runtime MOD.
- Os sinais de hardware binding/licenciamento ainda exigem refinamento por arquivo legivel e captura dinamica.
- Os arquivos de sinais profundos sao evidencia bruta grande e ruidosa; nao devem ser publicados integralmente.

Proximas acoes tecnicas:

- Refinar extracao de layouts Delphi por executavel principal.
- Criar matriz tela/menu/relatorio com status de identificacao por tela.
- Executar captura dinamica no runtime MOD.
- Medir processos, memoria, handles, threads e rede por tela.
- Extrair endpoints/hosts com redacao de dados sensiveis.
- Analisar `Gerenciador.exe` com ferramentas especificas para .NET.

## Mapa funcional de telas, menus e relatorios

Relatorio gerado:

`D:\AtelieProd\MOD\docs\02-arquitetura-legada\mapa-funcional-telas\relatorio-mapa-funcional-telas.md`

Arquivos principais:

- `D:\AtelieProd\MOD\docs\02-arquitetura-legada\mapa-funcional-telas\mapa-funcional-executaveis.csv`
- `D:\AtelieProd\MOD\docs\02-arquitetura-legada\mapa-funcional-telas\correlacao-menu-permissao-executavel.csv`

Resultado:

- 18.724 textos funcionais candidatos.
- 1.216 correlacoes entre operacoes/permissoes e textos dos executaveis.
- `LavSoft`: 6.781 textos candidatos.
- `LavFacilLan`: 5.576 textos candidatos.
- `SAT`: 2.673 textos candidatos.
- `Estoque`: 1.749 textos candidatos.
- `Financeiro`: 972 textos candidatos.
- `NFE`: 776 textos candidatos.
- `Gerenciador`: 197 textos candidatos.

Categorias identificadas:

- cadastro: 6.077
- operacional: 5.501
- fiscal: 1.590
- relatorio/impressao: 1.360
- financeiro/caixa: 1.327
- comunicacao/update: 1.036
- autenticacao/permissao: 287

Limitacao:

- Esta coleta mostra o que cada executavel aparenta acessar/exibir.
- A posicao exata de campos, botoes, grades e fluxo apos clique ainda exige captura dinamica no runtime MOD.
- A extracao estatica de layout Delphi por `TPF0` permanece parcial e ruidosa.

## Relatorios obrigatorios do projeto

Devem ser produzidos e atualizados:

- relatorio tecnico profundo;
- mapa de arquitetura;
- mapa de modulos;
- mapa de permissoes;
- mapa de conexoes;
- mapa de autenticacao;
- mapa de licenciamento;
- mapa de hardware binding;
- mapa de memoria;
- mapa de processos;
- analise de gargalos;
- analise de riscos;
- analise de dependencias;
- analise de estabilidade;
- analise de modernizacao;
- plano de evolucao tecnologica;
- plano de migracao;
- plano de arquitetura cloud;
- plano Supabase;
- plano de continuidade operacional.

## Observabilidade, tracing e profiling

Ferramenta propria criada:

`D:\AtelieProd\MOD\apps\tools\EquipeExe.Mod.Observability`

Tecnologia:

`.NET 8 / C#`

Objetivo:

- snapshot de processos;
- snapshot de rede com PID;
- monitoramento de executaveis do MOD;
- consumo de RAM;
- memoria privada;
- CPU acumulada;
- threads;
- handles;
- processos filhos;
- DLLs/modulos carregados;
- conexoes TCP.

Scripts:

- `D:\AtelieProd\MOD\apps\tools\run-observability-monitor.ps1`
- `D:\AtelieProd\MOD\apps\tools\summarize-observability.py`
- `D:\AtelieProd\MOD\apps\tools\ensure-mod-runtime-dependencies.ps1`

Documentos:

- `D:\AtelieProd\MOD\docs\07-observabilidade\plano-observabilidade-profiling.md`
- `D:\AtelieProd\MOD\docs\07-observabilidade\relatorio-observabilidade-profiling.md`
- `D:\AtelieProd\MOD\docs\07-observabilidade\relatorio-execucao-dinamica-lavsoft.md`

Execucao dinamica inicial:

- alvo: `LavSoft.exe` no runtime MOD;
- duracao: 20 segundos;
- pico Working Set: aproximadamente 19,44 MB;
- pico memoria privada: aproximadamente 6,73 MB;
- pico threads: 6;
- pico handles: 257;
- processo filho observado: `splwow64.exe`;
- conexoes TCP observadas: 0.

Baselines adicionais:

- `LavFacilLan`: 51,16 MB de Working Set pico, 16,14 MB de memoria privada, 10 threads, 475 handles, 88 modulos/DLLs, conexao HTTP observada.
- `Gerenciador`: 20,41 MB de Working Set pico, 22,95 MB de memoria privada, 6 threads, 232 handles, 40 modulos/DLLs, sem conexao observada.
- `Financeiro`: 21,06 MB de Working Set pico, 4,77 MB de memoria privada, 6 threads, 245 handles, 46 modulos/DLLs, sem conexao observada.
- `Estoque`: 37,75 MB de Working Set pico, 10,02 MB de memoria privada, 12 threads, 486 handles, 87 modulos/DLLs, conexao HTTP observada.

Achado operacional:

- A primeira execucao do `LavSoft` no MOD falhou com codigo `-1073741515`, indicando dependencia ausente.
- Apos copiar 50 DLLs/OCXs auxiliares para o runtime MOD, o `LavSoft` iniciou em ambiente de homologacao.
- O original permaneceu intocado.

Achado de comunicacao externa:

- `LavFacilLan` abriu conexao para `191.6.218.152:80`.
- `Estoque` abriu conexao para `191.6.218.152:80`.
- A comunicacao usa porta 80 e deve ser classificada antes de qualquer permissao definitiva.
- O bloqueio de `LiveUpdate` nao cobre esses modulos.
- Scripts de firewall foram criados para bloquear apenas os executaveis MOD, mas a aplicacao das regras exige privilegio administrativo.
- Busca estatica nao encontrou o IP como texto direto nos arquivos pesquisados; origem pode ser DNS, banco, DLL compactada/ofuscada ou resposta remota.

## Fase 08 - Telemetria, protocolos e dependencias reais

Estruturas criadas:

- `D:\AtelieProd\MOD\docs\06-dependencias`
- `D:\AtelieProd\MOD\docs\08-telemetria-protocolos`

Script criado:

`D:\AtelieProd\MOD\apps\tools\build-phase08-dynamic-maps.py`

Arquivos gerados:

- `D:\AtelieProd\MOD\docs\08-telemetria-protocolos\mapa-real-comunicacao.csv`
- `D:\AtelieProd\MOD\docs\06-dependencias\mapa-dependencias-runtime.csv`
- `D:\AtelieProd\MOD\docs\08-telemetria-protocolos\mapa-inicializacao-runtime.csv`
- `D:\AtelieProd\MOD\docs\08-telemetria-protocolos\baseline-memoria-runtime.csv`
- `D:\AtelieProd\MOD\docs\08-telemetria-protocolos\relatorio-fase08-telemetria-protocolos-dependencias.md`
- `D:\AtelieProd\MOD\docs\08-telemetria-protocolos\procedimento-captura-trafego-e-isolamento.md`
- `D:\AtelieProd\MOD\docs\06-dependencias\mapa-dependencias-criticas.md`

Endpoint externo confirmado:

- IP: `191.6.218.152`
- porta: `80`
- reverse DNS: `web22f62.kinghost.net`
- teste TCP: porta aberta
- teste HTTP manual: `403 Forbidden`
- modulos observados: `LavFacilLan.exe` e `Estoque.exe`
- classificacao: dependencia externa real ainda sem finalidade identificada

Dependencias reais classificadas:

- banco/BDE: critica;
- fiscal/hardware: critica;
- rede/protocolo: alta;
- impressao: alta;
- runtime legado: alta;
- Windows: media;
- mod/runtime MOD: media.

Hipotese de core engine:

- `LavFacilLan.exe` aparenta ser nucleo operacional forte por carregar BDE, fiscal/hardware, WinINet/Winsock e realizar comunicacao externa.
- `LavSoft.exe` segue como nucleo operacional classico, com impressao e dependencias fiscais.
- `Estoque.exe` possui dependencia real de rede e BDE.
- `Gerenciador.exe` exige analise .NET com ILSpy/dnSpy.
- `Financeiro.exe` iniciou leve e sem rede observada nesta janela.

Limitacoes da Fase 08:

- payload HTTP ainda nao foi capturado;
- firewall MOD ainda nao foi aplicado por falta de elevacao administrativa;
- ordem exata de inicializacao requer ProcMon, ETW ou API Monitor;
- NFE/SAT devem ser analisados em fase fiscal separada.

## Roadmap ultra detalhado - 150 fases

Foi criada a estrutura documental obrigatoria:

- `docs\00-controle`
- `docs\01-inventario`
- `docs\02-runtime`
- `docs\03-memoria`
- `docs\04-performance`
- `docs\05-comunicacoes`
- `docs\06-dependencias`
- `docs\07-observabilidade`
- `docs\08-auth`
- `docs\09-licensing`
- `docs\10-database`
- `docs\11-modulos`
- `docs\12-apis`
- `docs\13-cloud`
- `docs\14-supabase`
- `docs\15-nextgen`
- `docs\16-migracao`
- `docs\17-seguranca`
- `docs\18-risk`
- `docs\19-snapshots`

Pastas historicas foram preservadas para nao quebrar referencias ja existentes.

Arquivos criados:

- `D:\AtelieProd\MOD\apps\tools\generate-roadmap-150.py`
- `D:\AtelieProd\MOD\docs\00-controle\roadmap-150-fases.md`
- `D:\AtelieProd\MOD\docs\00-controle\roadmap-150-fases.csv`
- `D:\AtelieProd\MOD\docs\00-controle\estrutura-documentacao-obrigatoria.md`
- `D:\AtelieProd\MOD\docs\00-controle\indice-rastreabilidade.md`

O roadmap contem 150 fases validadas, cada uma com:

- objetivo;
- escopo;
- tarefas;
- ferramentas;
- analises;
- validacoes;
- evidencias;
- logs;
- rollback;
- criticidade;
- documentacao;
- entregaveis;
- impacto operacional;
- impacto em memoria;
- impacto em CPU;
- impacto em rede;
- impacto em autenticacao;
- impacto em sincronizacao;
- impacto em licenciamento.

## Visibilidade total

Documento criado:

`D:\AtelieProd\MOD\docs\12-visibilidade-total\matriz-visibilidade-total.md`

Objetivo:

- controlar o grau de conhecimento por area;
- separar evidencia estatica de evidencia dinamica;
- indicar riscos e proximas acoes;
- evitar reconstrucao baseada em suposicao;
- orientar captura de telas, memoria, rede, banco, autenticacao e licenciamento.

Areas controladas:

- inventario;
- executaveis;
- DLLs/imports;
- banco Paradox/BDE;
- menus/permissoes;
- layout de telas;
- relatorios;
- autenticacao;
- licenciamento;
- hardware binding;
- atualizacao automatica;
- comunicacoes externas;
- memoria/performance;
- processos filhos;
- impressao/fiscal;
- sincronizacao/nuvem;
- logs;
- configuracoes;
- observabilidade MOD;
- arquitetura futura.

## Arquitetura futura offline-first/cloud hibrida

Documentos criados:

- `D:\AtelieProd\MOD\docs\11-arquitetura-futura\estrategia-arquitetura-futura-consolidada.md`
- `D:\AtelieProd\MOD\docs\11-arquitetura-futura\plano-offline-first-sync-supabase.md`
- `D:\AtelieProd\MOD\docs\11-arquitetura-futura\plano-modularizacao.md`
- `D:\AtelieProd\MOD\docs\11-arquitetura-futura\plano-observabilidade-futura.md`
- `D:\AtelieProd\MOD\docs\11-arquitetura-futura\estrategia-final-modernizacao.md`

Direcao arquitetural:

- offline-first;
- banco local moderno;
- cache local;
- fila de sincronizacao;
- logs locais;
- autenticacao cacheada;
- cloud hibrida;
- Supabase futuro para plano administrativo;
- multi-tenant com `tenant_id`, `company_id`, `branch_id`;
- modulos independentes;
- rollback modular;
- observabilidade completa;
- baixo consumo de memoria.

Modulos futuros:

- auth;
- licensing;
- sync;
- core;
- financeiro;
- pdv/operacional;
- fiscal;
- estoque;
- usuarios;
- relatorios;
- admin;
- telemetria.

Supabase futuro:

- autenticacao;
- usuarios;
- permissoes;
- licenciamento;
- dispositivos;
- sessoes;
- auditoria;
- telemetria administrativa;
- feature flags;
- sync;
- dashboard administrativo.

Estrategia:

- manter o legado como referencia controlada;
- capturar evidencias;
- criar nucleos modernos equivalentes;
- migrar modulo por modulo;
- preservar continuidade operacional;
- introduzir cloud somente quando o local estiver confiavel;
- manter rollback documentado.

## Estrategia final desejada

O entregavel final deve permitir:

- continuidade operacional;
- independencia administrativa;
- nova autenticacao;
- novo licenciamento;
- controle de dispositivos;
- operacao local controlada;
- cloud hibrida opcional;
- integracao futura com Supabase;
- migracao gradual e segura;
- rollback documentado;
- modernizacao de interface;
- preservacao das regras de negocio;
- baixa dependencia do fornecedor original;
- observabilidade e auditoria;
- escalabilidade futura.

Documentos complementares:

- `README.md`
- `docs\00-controle\log-de-alteracoes.md`
- `docs\01-inventario\resumo-inventario.md`
- `docs\02-arquitetura-legada\arquitetura-encontrada.md`
- `docs\02-arquitetura-legada\menus-submenus-identificados.md`
- `docs\02-arquitetura-legada\relatorio-analise-profunda-fase-3.md`
- `docs\02-arquitetura-legada\mapa-funcional-telas\relatorio-mapa-funcional-telas.md`
- `docs\03-banco-de-dados\dicionario-dados-fase-2.md`
- `docs\04-autenticacao-permissoes\relatorio-autenticacao.md`
- `docs\05-comunicacoes\bloqueio-atualizacao-automatica.md`
- `docs\06-migracao\plano-migracao-rollback.md`
- `docs\07-observabilidade\plano-observabilidade-profiling.md`
- `docs\07-observabilidade\relatorio-observabilidade-profiling.md`
- `docs\07-observabilidade\relatorio-execucao-dinamica-lavsoft.md`
- `docs\07-observabilidade\relatorio-execucao-dinamica-modulos-principais.md`
- `docs\05-comunicacoes\achado-conexao-http-mod.md`
- `docs\11-arquitetura-futura\estrategia-arquitetura-futura-consolidada.md`
- `docs\11-arquitetura-futura\plano-offline-first-sync-supabase.md`
- `docs\11-arquitetura-futura\plano-modularizacao.md`
- `docs\11-arquitetura-futura\plano-observabilidade-futura.md`
- `docs\11-arquitetura-futura\estrategia-final-modernizacao.md`
- `docs\12-visibilidade-total\matriz-visibilidade-total.md`
- `docs\06-dependencias\mapa-dependencias-runtime.csv`
- `docs\06-dependencias\mapa-dependencias-criticas.md`
- `docs\08-telemetria-protocolos\relatorio-fase08-telemetria-protocolos-dependencias.md`
- `docs\08-telemetria-protocolos\procedimento-captura-trafego-e-isolamento.md`
- `docs\00-controle\roadmap-150-fases.md`
- `docs\00-controle\roadmap-150-fases.csv`
- `docs\00-controle\estrutura-documentacao-obrigatoria.md`
- `docs\00-controle\indice-rastreabilidade.md`
- `docs\09-licensing\relatorio-licenciamento-profundo.md`
- `docs\09-licensing\hipoteses-fluxo-licenciamento.md`
- `docs\09-licensing\sinais-licenciamento-executaveis.csv`
- `docs\09-licensing\mapa-tabelas-licenciamento.csv`
- `docs\09-licensing\endpoints-licenciamento-autenticacao-atualizacao.csv`
- `docs\09-licensing\amostras-mascaradas-tabelas-licenciamento-auth.csv`
- `docs\09-licensing\mapa-integrado-licenciamento-autenticacao-dispositivos.md`
- `docs\09-licensing\relatorio-comportamento-offline-degradado.md`
- `docs\09-licensing\mapa-controle-auth-licensing-componentes.csv`
- `docs\09-licensing\mapa-device-binding.csv`
- `docs\09-licensing\mapa-endpoints-apis-classificado.csv`
- `docs\08-auth\mapa-sessoes-autenticacao.csv`
- `docs\18-risk\riscos-auth-licensing-operacional.csv`
- `docs\15-nextgen\plano-substituicao-auth-licensing-offline-first.md`
- `docs\14-supabase\plano-supabase-auth-licensing-devices.md`
- `docs\11-modulos\ui-reverse-engineering\relatorio-visibilidade-total-ui.md`
- `docs\11-modulos\ui-reverse-engineering\blueprint-navegacao-operacional.md`
- `docs\11-modulos\ui-reverse-engineering\procedimento-captura-dinamica-ui.md`
- `docs\11-modulos\ui-reverse-engineering\mapa-telas-funcional-consolidado.csv`
- `docs\11-modulos\ui-reverse-engineering\mapa-menus-submenus-acoes-consolidado.csv`
- `docs\11-modulos\ui-reverse-engineering\mapa-permissoes-ui-consolidado.csv`
- `docs\11-modulos\ui-reverse-engineering\mapa-componentes-ui.csv`
- `docs\11-modulos\ui-reverse-engineering\mapa-layouts-delphi-tpf0.csv`
- `docs\11-modulos\ui-reverse-engineering\mapa-ui-banco-interligacoes.csv`
- `docs\11-modulos\ui-reverse-engineering\mapa-assets-visuais.csv`
- `docs\11-modulos\ui-reverse-engineering\resumo-ui-por-modulo.csv`
- `docs\15-nextgen\blueprint-ux-nextgen-equipeexe.md`
- `docs\10-database\dicionario-de-dados-completo.md`
- `docs\10-database\dicionario-de-dados-completo.csv`
- `docs\10-database\mapa-entidades-dominio.csv`
- `docs\10-database\mapa-relacionamentos.md`
- `docs\10-database\mapa-relacionamentos.csv`
- `docs\11-modulos\mapa-telas-banco.md`
- `docs\11-modulos\mapa-telas-banco.csv`
- `docs\11-modulos\mapa-clientes-os-produtos.md`
- `docs\11-modulos\mapa-clientes-os-produtos.csv`
- `docs\11-modulos\mapa-fluxos-operacionais.md`
- `docs\11-modulos\mapa-fluxos-operacionais.csv`
- `docs\11-modulos\matriz-regras-negocio.csv`
- `docs\11-modulos\matriz-relatorios.csv`
- `docs\15-nextgen\blueprint-dominio.md`
- `docs\15-nextgen\blueprint-banco-nextgen.md`
- `docs\15-nextgen\blueprint-telas-nextgen.md`
- `docs\10-database\dicionario-paradox-campo-a-campo.md`
- `docs\10-database\dicionario-paradox-campo-a-campo.csv`
- `docs\10-database\classificacao-entidades-negocio.md`
- `docs\10-database\classificacao-entidades-negocio.csv`
- `docs\10-database\matriz-relacionamentos-com-evidencia.md`
- `docs\10-database\matriz-relacionamentos-com-evidencia.csv`
- `docs\11-modulos\matriz-ui-banco.md`
- `docs\11-modulos\matriz-ui-banco.csv`
- `docs\11-modulos\matriz-tela-acao-tabela.md`
- `docs\11-modulos\matriz-tela-acao-tabela.csv`
- `docs\11-modulos\mapa-fluxo-cliente-movimento-financeiro.md`
- `docs\11-modulos\mapa-fluxo-produto-estoque-financeiro.md`
- `docs\11-modulos\mapa-status-operacionais.md`
- `docs\11-modulos\mapa-status-operacionais.csv`
- `docs\11-modulos\mapa-valores-calculos.md`
- `docs\11-modulos\mapa-valores-calculos.csv`
- `docs\11-modulos\procedimento-validacao-dinamica-dominio.md`
- `docs\15-nextgen\modelo-dominio-nextgen.md`
- `docs\15-nextgen\modelo-banco-nextgen.md`
- `docs\15-nextgen\modelo-ux-nextgen.md`
- `docs\10-database\perfil-dados-prioritarios-readonly.md`
- `docs\10-database\perfil-tabelas-prioritarias-linhas.csv`
- `docs\10-database\perfil-status-valores-distintos.csv`
- `docs\10-database\perfil-valores-monetarios-estatisticas.csv`
- `docs\10-database\perfil-datas-faixas.csv`
- `docs\10-database\perfil-campos-presenca.csv`

## Analise profunda de licenciamento - 2026-05-23

Foi executada verificacao adicional sobre licenciamento, autenticacao remota, registro de estacao e bloqueios funcionais, sempre em modo controlado e sem modificar o sistema original.

Achados consolidados:

- `Registrar.xml` contem identidade local de estacao: `MacAddress`, `Nome`, `Usuario`, `VersaoWindows` e `CodLojaOriginal`.
- `EquNet.ini` contem sinais codificados e parametros de controle: `BloqFran`, `CampoCC`, `EquipeZ`, `TesteW1`, `TesteB*`, `Tested*` e nome do computador.
- `CampoCC=EQU0000001215` se relaciona com `CodLojaOriginal=0000001215`, sugerindo vinculo entre loja/estacao/configuracao.
- Tabelas `NovoReg*` possuem coluna `NovoReg` e sao fortemente candidatas a registro/licenciamento legado.
- `CadCart.DB` possui coluna `Licenca`.
- `Estruturas\Inis.DB` possui coluna `ATIVACAO`.
- `LavSoft.exe`, `Financeiro.exe`, `SAT.exe` e `Senhas.exe` contem sinais de bloqueio/desbloqueio, `NovoReg`, licenca, vencimento e permissoes.
- `Gerenciador.exe` contem endpoints HTTP relevantes: `AutenticaGerenciador`, `TestaAutentica`, `RegistraEstacao`, `VerificaAtualizacoes`, `DownloadDados`, `ListarDispositivosPorFilial` e endpoints `ws/Nuvem`.
- As tabelas `NovoReg*` acessiveis na amostra atual estavam sem registros; `NovoRegLavFilial` retornou erro do driver Paradox `9499`.
- A leitura de amostras sensiveis foi mascarada com comprimento/hash parcial, sem expor senhas ou possiveis chaves.

Hipotese atual:

O licenciamento/autenticacao legado parece hibrido. Ha persistencia local de estacao, parametros codificados em INI, estruturas `NovoReg*`, bloqueios por modulo/filial/permissao e dependencia remota LavSoft concentrada especialmente no `Gerenciador.exe`.

Pendencias:

- Capturar payload HTTP dos endpoints de autenticacao/registro/atualizacao.
- Validar acesso real a `NovoReg*`, `Registrar.xml` e `EquNet.ini` com ProcMon.
- Analisar IL do `Gerenciador.exe`.
- Validar `NovoRegLavFilial` com ferramenta Paradox/BDE alternativa.
- Aplicar isolamento de rede MOD somente em sessao administrativa e com rollback ja documentado.

## Mapa integrado de controle - 2026-05-23

Foi criada uma segunda camada de documentacao para consolidar licenciamento, autenticacao, sessoes, device binding, endpoints, riscos e plano de substituicao gradual.

Classificacao atual dos componentes:

- `Gerenciador.exe`: broker remoto/admin forte; contem endpoints de autenticacao, registro, atualizacao, download, dispositivos e nuvem.
- `Senhas.exe`: controle local de usuarios, permissoes, bloqueios por sistema e filial.
- `LavSoft.exe`: core operacional com sinais de `NovoReg`, bloqueio e desbloqueio pela internet.
- `Financeiro.exe`: modulo critico com `ArquivoLicenca`, `FE_Licenca`, `Licenca`, `Vencimento` e flags `BloqSec*`.
- `SAT.exe` e `NFE.exe`: modulos fiscais criticos com sinais de permissao, bloqueio e possiveis licencas de componentes fiscais.
- `LavFacilLan.exe` e `Estoque.exe`: modulos com comunicacao real observada para `191.6.218.152:80`.
- `EquEstruAtu.exe` e `EquConfig.exe`: infraestrutura/configuracao/atualizacao.

Conclusao tecnica:

O controle operacional do legado e distribuido. O licenciamento/autenticacao nao esta em um unico ponto. A substituicao futura deve criar uma camada propria no MOD com:

- autenticacao local;
- permissoes granulares;
- licenca local assinada;
- device binding tolerante;
- fila offline-first;
- auditoria;
- sync cloud opcional;
- Supabase como administracao central futura, sem bloquear operacao local.

## Visibilidade total de interface - 2026-05-23

Foi criada uma fase especifica de engenharia reversa de UI/UX, consolidando telas, menus, submenus, acoes, permissoes, componentes, layouts Delphi TPF0, assets visuais e vinculos UI-banco.

Resultados quantitativos:

- 18.724 textos/telas/acoes funcionais candidatos.
- 1.936 menus/submenus/acoes consolidados.
- 521 blocos Delphi TPF0 extraidos.
- 757 vinculos UI -> banco/SQL.
- 1.598 assets visuais inventariados.

Distribuicao principal:

- `LavSoft`: 6.781 textos/telas/acoes, 601 menus/acoes, 329 layouts TPF0.
- `LavFacilLan`: 5.576 textos/telas/acoes, 237 menus/acoes, 174 layouts TPF0.
- `SAT`: 2.673 textos/telas/acoes, 342 menus/acoes.
- `Estoque`: 1.749 textos/telas/acoes, 318 menus/acoes, 153 vinculos banco.
- `Financeiro`: 972 textos/telas/acoes, 288 menus/acoes.
- `NFE`: 776 textos/telas/acoes.
- `Gerenciador`: 197 textos/telas/acoes, com perfil administrativo/remoto.

Leitura arquitetural:

- A experiencia principal esta centrada em `LavSoft` e `LavFacilLan`.
- A UI e majoritariamente Delphi/Borland/VCL, com `Gerenciador.exe` em Windows Forms/.NET.
- A nova UX deve preservar navegacao operacional rapida, nomes familiares e densidade de informacao.
- Captura dinamica tela-a-tela ainda e necessaria para confirmar posicoes, botoes, grids, atalhos e mensagens reais.

## Dominio, dados e fluxos criticos - 2026-05-23

Foi criada uma fase de modelagem de dominio e banco focada em clientes, OS/ROL, produtos, valores, financeiro, estoque, telas, permissoes, relatorios e migracao.

Resultados:

- 478 tabelas classificadas por dominio.
- 4.996 colunas classificadas campo a campo.
- 25 tabelas relacionadas a cliente.
- 28 tabelas relacionadas a OS/ROL.
- 36 tabelas relacionadas a produto/servico.
- 10 tabelas relacionadas a estoque.
- 19 tabelas relacionadas a financeiro.
- 36 tabelas fiscais.
- 757 vinculos tela -> banco/SQL consolidados.
- 1.362 itens candidatos a relatorio/impressao.

Eixo central identificado:

`Clientes.CodCli -> MovCab.ROL/CodCli -> itens/produtos/servicos -> financeiro/notas/duplicatas -> estoque/relatorios/auditoria`.

Tabelas criticas:

- Clientes: `Clientes`, `CliContato`, `ClientesObs`, `FunCli`, `FunCliRou`, `GruClientes`.
- OS/ROL: `MovCab`, `MovLocRol`, `CadLocRol`, `ControleEti`, `IndenRol`, `MovControle`.
- Produtos/servicos: `Produt`, `ProdEst`, `TabProdEst`, `ProdEstKit`, `ProdEstPac`.
- Estoque: `MovEst`, `MovEstCan`, `MovEstEnc`, `ProdEst`.
- Financeiro: `Duplicat`, `Boletos`, `DupBoleto`, `CliCredito`, `FecCaixa`, `MovIniCaixa`, `Titulos`, `TitGru`.
- Fiscal: `Notas`, `NotaFisPag`, `NotaSat`, `NotaSatCanc`.
- Permissoes: `Usuarios`, `Senhas`, `Nivel`, `GruUsuarios`.

Pendencias:

- validar indices/chaves Paradox;
- coletar amostras estatisticas por tabela critica;
- capturar fluxo dinamico tela -> arquivo DB via ProcMon;
- confirmar regras de calculo de valores, descontos, estoque e fechamento.

## Modelagem profunda com matriz de evidencia - 2026-05-23

Foi adicionada uma camada de classificacao explicita de evidencia para dominio, campos, relacionamentos, UI-banco, tela-acao-tabela, status e valores.

Regra aplicada:

- confirmado por schema: tabela/campo existe no dicionario Paradox ou ha indice lateral `.PX/.XG*/.YG*`.
- confirmado por UI: aparece em tela/menu/string/mapa UI-banco.
- confirmado por runtime: somente quando houver ProcMon/log/diff. Nesta etapa, nao houve promocao de relacao de dados para runtime porque nao foram feitas gravacoes teste.
- hipotese por nome/string: nome sugere papel, mas precisa validacao.
- nao confirmado: sem evidencia suficiente.

Resultados:

- 4.996 campos classificados campo a campo.
- 10 entidades prioritarias descritas no formato obrigatorio.
- 12 relacionamentos com evidencia e plano de validacao dinamica.
- 1.936 acoes/telas/menus cruzadas para matriz tela -> acao -> tabela.
- 757 entradas na matriz UI -> banco.
- 97 campos de status operacionais.
- 147 campos de valores/calculos.

Relacionamentos fortes:

- `Clientes.CodCli -> MovCab.CodCli`: confirmado por schema + UI.
- `Clientes.CodCli -> Duplicat.CodCli`: confirmado por schema.
- `Clientes.CodCli -> Notas.CodCli`: confirmado por schema.
- `ProdEst.CodProEst -> MovEst.CodProEst`: confirmado por schema.
- `SAT.NotaSat.NumNotSat -> NotaSatCanc.NumNotSat`: confirmado por schema.

Hipoteses importantes a validar:

- `MovCab.NumNot -> Notas.NumNot`.
- `Notas/NotaFisPag` por `NumNot`/`NumNotFis`.
- `Produt.CodPro -> itens reais do ROL`.
- impacto financeiro de `MovEst.ValTot/ValUnit`.

## Perfil readonly dos dados prioritarios - 2026-05-23

Foi executada leitura somente leitura das tabelas prioritarias via ODBC 32-bit sobre a copia em `MOD\data\original-readonly`.

Resultados:

- 44 tabelas prioritarias lidas com sucesso.
- 893 campos perfilados.
- 20 valores distintos de status observados em dados reais copiados.
- 33 estatisticas de valor/quantidade geradas.
- 21 faixas de datas geradas.
- 0 falhas de leitura na amostra prioritaria.

Maiores tabelas:

- `MovCab`: 31.972 registros.
- `Notas`: 21.679 registros.
- `Duplicat`: 19.717 registros.
- `Clientes`: 5.064 registros.
- `MovSatCliOcor`: 4.303 registros.

Achados reforcados:

- `MovCab` e a maior tabela operacional prioritaria, reforcando papel de cabecalho de ROL/movimento.
- `Duplicat` confirma forte papel financeiro/recebiveis por volume e campos de baixa/pagamento.
- `Notas` confirma papel fiscal/financeiro por volume.
- `MovSatCliOcor` mostra historico/ocorrencias SAT relevante.
- `MovEst` tem apenas 1 registro na base atual, indicando baixo uso de estoque nesta copia ou uso em outro conjunto de tabelas.

Status observados sem significado ainda confirmado:

- `MovCab.Posicao`: `S` e `E`.
- `MovCab.SitRol`: `P`.
- `Duplicat.Baixa`: `S` e `N`.
- `CliCredito.Sit`: `B` e `C`.
- `MovSatCliOcor.SitOcor`: `0`.

Regra: esses codigos foram observados nos dados, mas o significado operacional so deve ser confirmado por UI/runtime.

## Usuarios, permissoes, pagamentos e escopo NextGen - 2026-05-23

Foi adicionada a fase de desenho do novo sistema com foco em usuarios, permissoes, pagamentos Mercado Pago/Mercado Livre, licenciamento proprio, taxa de servico e arquitetura segura.

Achados do legado usados como base:

- 9 usuarios identificados em `Usuarios.DB`.
- Grupos observados: `MASTE` e `OPERA`.
- `GABRIELA` classificada como Administrador Geral por grupo `MASTE` e regra do projeto.
- 438 permissoes legadas extraidas de `Nivel.DB`.
- 1.936 acoes/telas cruzadas na matriz tela -> acao -> tabela.
- Semantica de `TipUsuario` e `NivelI/A/E/T` ainda pendente de validacao runtime/UI.

Novos artefatos:

- `docs\08-auth\analise-usuarios-permissoes.md`
- `docs\08-auth\matriz-usuarios-legado.csv`
- `docs\08-auth\matriz-permissoes-legado-classificada.csv`
- `docs\08-auth\matriz-usuario-perfil-tela-botao-acao-tabela.csv`
- `docs\08-auth\matriz-perfis-novo-sistema.csv`
- `docs\15-nextgen\escopo-funcional-novo-sistema.md`
- `docs\15-nextgen\escopo-tecnico-novo-sistema.md`
- `docs\15-nextgen\blueprint-novo-programa.md`
- `docs\15-nextgen\roadmap-desenvolvimento-novo-sistema.md`
- `docs\16-migracao\plano-migracao-equipeexe-para-nextgen.md`
- `docs\13-cloud\pagamentos\arquitetura-pagamentos-mercado-pago-mercado-livre.md`
- `docs\13-cloud\pagamentos\arquitetura-licenciamento-pagamentos.md`
- `docs\13-cloud\pagamentos\arquitetura-repasse-taxa-servico.md`
- `docs\13-cloud\pagamentos\fluxos-pagamento-pix-cartao-dinheiro.md`
- `docs\13-cloud\pagamentos\modelo-banco-pagamentos-licencas-taxas.md`
- `docs\17-seguranca\arquitetura-segura-credenciais-mercado-pago.md`

Decisoes arquiteturais:

- credenciais Mercado Pago/Mercado Livre ficam somente no backend/API, nunca no executavel desktop.
- licencas sao recebidas por Rogerio.
- vendas de produtos/servicos sao recebidas por Luci.
- taxa de servico inicial de R$ 0,05 por venda de Luci para Rogerio deve ser transparente, auditavel e conciliavel.
- cartao presencial deve seguir integracao por terminal/ordem/status/webhook; Bluetooth local nao deve substituir autorizacao oficial do provedor.
- PIX e cartao exigem confirmacao online; dinheiro pode operar offline com auditoria e sincronizacao posterior.

## Separacao obrigatoria por plataforma - 2026-05-23

Foi criada a estrutura fisica separada por plataforma em `D:\AtelieProd`:

- `ORIGINAL`: estrutura-alvo/documental para preservacao do legado; o legado real ainda permanece em `Equipexe` para nao quebrar caminhos existentes.
- `MOD`: engenharia reversa, homologacao e documentacao tecnica.
- `Atelie_Windows`: estrutura exclusiva da versao Windows.
- `Atelie_Linux`: estrutura exclusiva da versao Linux appliance/kiosk.
- `docs`: documentacao consolidada por plataforma e dominio.

Regras definidas:

- nao misturar binarios, runtimes, configs, drivers, logs, bancos, cache, scripts ou builds entre Windows e Linux.
- Windows deve priorizar compatibilidade, perifericos Windows, migracao gradual e baixo consumo.
- Linux deve priorizar appliance PDV, kiosk, boot rapido, minimo consumo de RAM/CPU, CUPS/ESC-POS, BlueZ, watchdog, recovery, snapshots e ISO propria.

Documentos criados:

- `D:\AtelieProd\docs\arquitetura\separacao-plataformas.md`
- `D:\AtelieProd\docs\windows\arquitetura-windows.md`
- `D:\AtelieProd\docs\linux\arquitetura-linux-appliance.md`
- `D:\AtelieProd\docs\linux\atualizacao-recovery-linux.md`
- `D:\AtelieProd\docs\printer\impressora-termica-linux.md`
- `D:\AtelieProd\docs\performance\metas-performance-linux-windows.md`
- `D:\AtelieProd\Atelie_Windows\README.md`
- `D:\AtelieProd\Atelie_Linux\README.md`
- `D:\AtelieProd\ORIGINAL\README.md`

## Arquitetura multiplataforma PDV Windows/Linux - 2026-05-23

Foi criada uma camada consolidada de decisao arquitetural para a nova geracao do EquipeExe/Atelie, considerando Windows otimizado e Linux appliance/kiosk para hardware Intel 2a geracao, 4 GB RAM e SSD.

Decisao recomendada:

- construir do zero com core offline-first, SQLite local e API local;
- entregar Windows NextGen primeiro para compatibilidade e migracao gradual;
- validar Linux kiosk em Debian minimal como PDV dedicado;
- evoluir para appliance Linux com snapshots/rollback depois de homologar impressora, Bluetooth, pagamentos e watchdog;
- usar .NET/Avalonia como stack inicial de UI multiplataforma, evitando Electron;
- reservar Rust/Go/C++ para componentes criticos de baixo nivel quando medições justificarem.

Documentos criados:

- `D:\AtelieProd\docs\arquitetura\avaliacao-windows-linux-nextgen.md`
- `D:\AtelieProd\docs\arquitetura\recomendacao-final-arquitetura.md`
- `D:\AtelieProd\docs\linux\comparativo-bases-linux.md`
- `D:\AtelieProd\docs\windows\blueprint-windows-nextgen.md`
- `D:\AtelieProd\docs\linux\blueprint-linux-nextgen.md`
- `D:\AtelieProd\docs\pdv\blueprint-pdv-auto-inicializavel.md`
- `D:\AtelieProd\docs\printer\Blueprint-impressao-termica-fiel.md`
- `D:\AtelieProd\docs\bluetooth\blueprint-bluetooth-maquininha.md`
- `D:\AtelieProd\docs\nextgen\blueprint-supabase-nextgen.md`
- `D:\AtelieProd\docs\pdv\blueprint-mercado-pago-mercado-livre.md`
- `D:\AtelieProd\docs\dominio\modelo-dominio-consolidado.md`
- `D:\AtelieProd\docs\auth\matriz-permissoes-consolidada.md`
- `D:\AtelieProd\docs\database\modelo-banco-novo-consolidado.md`
- `D:\AtelieProd\docs\migracao\roadmap-migracao-reconstrucao.md`
- `D:\AtelieProd\docs\performance\plano-performance-hardware-fraco.md`

Notas:

- Kurumin deve ser tratado apenas como estudo/sandbox se uma imagem for fornecida; nao e recomendado como base de producao moderna.
- Wine pode servir para estudo, mas nao como produto final por risco em impressao, Bluetooth, BDE/Paradox e manutencao.
- A fidelidade da impressao termica deve ser homologada com evidencias comparando comprovante legado e NextGen.

## Migracao funcional real para Linux - 2026-05-23

Foi adicionada a fase explicita de migracao funcional para Linux. O objetivo registrado nao e apenas criar uma imagem Linux vazia, mas tornar o Atelie Linux capaz de operar PDV, impressao, clientes, OS/ROL, produtos, estoque, financeiro, relatorios, Bluetooth, sincronizacao, offline, update, recovery e rollback.

Cenarios avaliados:

- legado via Wine: permitido para laboratorio/observacao, nao recomendado como produto final.
- bridges Linux: recomendadas para impressao, importacao, sync, pagamentos e watchdog durante migracao.
- modulos nativos graduais: caminho recomendado.
- reescrita de modulos criticos: recomendada para PDV, pagamentos, licenciamento, impressao e auditoria.
- runtime hibrido: aceitavel apenas em homologacao.

Scripts criados em `Atelie_Linux`:

- `drivers\detect-dell-hardware.sh`
- `drivers\install-linux-drivers.sh`
- `drivers\autoconfigure-linux-hardware.sh`
- `kiosk\install-openbox-kiosk.sh`
- `kiosk\start-atelie-pdv.sh`
- `services\atelie-pdv.service`
- `services\atelie-watchdog.service`
- `watchdog\atelie-watchdog.sh`
- `recovery\recovery-menu.sh`
- `recovery\rollback-last-version.sh`
- `updater\atelie-updater.sh`
- `backup\backup-local-db.sh`

Documentos criados:

- `docs\linux\migracao-funcional-windows-linux.md`
- `docs\linux\analise-wine-proton-mono.md`
- `docs\linux\analise-dotnet-avalonia-linux.md`
- `docs\linux\openbox-kiosk.md`
- `docs\linux\drivers-dell-linux.md`
- `docs\linux\arquitetura-final-atelie-linux.md`
- `docs\printer\blueprint-impressao-linux-migracao.md`
- `docs\bluetooth\blueprint-bluetooth-linux.md`
- `docs\migracao\roadmap-migracao-linux-funcional.md`

Observacao: os scripts foram preparados em ambiente Windows e ainda precisam ser executados/testados em VM Linux e Dell real.

## Dossie de investigacao total - 2026-05-23

Foi criado um dossie consolidado da investigacao total do EquipeExe, conectando inventario, engenharia reversa, dominio, UI, permissoes, runtime, memoria, comunicacoes, impressao, Bluetooth, licenciamento, Windows NextGen, Linux NextGen, appliance, Supabase e migracao.

Artefatos principais:

- `D:\AtelieProd\docs\arquitetura\dossie-investigacao-total-equipeexe.md`
- `D:\AtelieProd\MOD\docs\00-controle\matriz-entregaveis-investigacao-total.md`
- `D:\AtelieProd\docs\observabilidade\plano-execucao-profundidade-runtime.md`

Status consolidado:

- 707 artefatos existentes em `MOD\docs`.
- 30 documentos consolidados existentes em `D:\AtelieProd\docs` antes desta consolidacao.
- Entregaveis obrigatorios 1 a 27 mapeados para arquivos existentes ou pendencias runtime.

Lacunas criticas registradas:

- payload HTTP;
- ETW/ProcMon/API Monitor para ordem real de inicializacao;
- threads/timers;
- impressao fisica comparativa;
- Bluetooth real em Dell;
- Openbox kiosk em VM e Dell;
- semantica final de status Paradox;
- insert/update/delete por tela;
- desempenho real no hardware alvo.

## Objetivo estrategico final - novo software do zero - 2026-05-23

Foi registrada a diretriz maxima do projeto: criar um novo software completamente do zero, inspirado no EquipeExe, usando o legado como fonte de conhecimento operacional e nao como dependencia arquitetural permanente.

Principios:

- preservar dominio, regras, fluxos, produtividade, permissoes, relatorios e impressao equivalente;
- modernizar arquitetura, banco, autenticacao, licenciamento, sync, observabilidade, UI/UX, runtime, deploy, update, recovery e performance;
- preparar Windows moderno e Linux appliance/kiosk;
- usar strangler pattern para substituir o legado gradualmente;
- manter compatibilidade operacional durante a transicao.

Artefatos criados:

- `D:\AtelieProd\docs\nextgen\diretriz-estrategica-novo-software-do-zero.md`
- `D:\AtelieProd\docs\nextgen\blueprint-mestre-plataforma-nextgen.md`
- `D:\AtelieProd\docs\nextgen\matriz-blueprints-reconstrucao-total.md`
- `D:\AtelieProd\docs\migracao\estrategia-strangler-substituicao-progressiva.md`

Decisao:

- o codigo legado nao deve ser carregado como limitacao permanente;
- as regras recuperadas do legado devem ser reimplementadas em arquitetura nova, modular, offline-first, observavel e multiplataforma.

## Matriz dos 100 topicos e roadmap completo NextGen - 2026-05-23

Foi criada a matriz de rastreabilidade dos 100 topicos obrigatorios de investigacao e desenvolvimento, conectando cada item a evidencias existentes, lacunas runtime/hardware e proximas acoes.

Artefatos criados:

- `D:\AtelieProd\docs\nextgen\matriz-100-topicos-nextgen.md`
- `D:\AtelieProd\docs\nextgen\roadmap-completo-nextgen.md`
- `D:\AtelieProd\docs\nextgen\arquitetura-final-recomendada-nextgen.md`
- `D:\AtelieProd\docs\nextgen\blueprint-observabilidade-telemetria.md`
- `D:\AtelieProd\docs\nextgen\blueprint-sync-offline-first.md`

Decisao registrada:

- Debian minimal + Openbox kiosk para o primeiro Linux appliance/kiosk;
- .NET/Avalonia + SQLite para o eixo inicial multiplataforma;
- Supabase como plano cloud futuro;
- Mercado Pago/Mercado Livre via backend proprio;
- implementacao nova e modular, preservando comportamento operacional e descartando fragilidades tecnicas do legado.

## Consolidacao total e plano de execucao da migracao - 2026-05-23

Foi criada a camada estrategica de execucao da migracao, saindo da investigacao pura para a decisao de como iniciar a reconstrucao e a substituicao gradual.

Artefatos criados:

- `D:\AtelieProd\docs\migracao\consolidacao-total-engenharia-reversa.md`
- `D:\AtelieProd\docs\migracao\matriz-estrategica-migracao.md`
- `D:\AtelieProd\docs\migracao\matriz-riscos-execucao.md`
- `D:\AtelieProd\docs\migracao\matriz-dependencias-criticas.md`
- `D:\AtelieProd\docs\migracao\estrategia-definitiva-execucao.md`
- `D:\AtelieProd\docs\migracao\roadmap-tecnico-operacional-execucao.md`
- `D:\AtelieProd\docs\nextgen\blueprint-tenant-manager.md`
- `D:\AtelieProd\docs\nextgen\blueprint-gerenciamento-remoto.md`
- `D:\AtelieProd\docs\windows\estrategia-windows-execucao.md`
- `D:\AtelieProd\docs\linux\estrategia-linux-execucao.md`

Decisao de inicio:

- comecar por core de dominio, SQLite local, auth/permissoes, clientes/produtos e OS/ROL basico;
- em paralelo, validar impressao, payload HTTP, semantica de status, VM Linux e Dell real;
- manter Paradox e EquipeExe como fonte readonly/fallback temporario;
- reescrever do zero auth, licensing, sync, PDV, pagamentos, auditoria, observabilidade, updater, recovery, UI, impressao, Bluetooth/perifericos, tenant manager e banco local.

Classificacao:

- local/offline: SQLite, PDV, clientes, produtos, OS/ROL, caixa dinheiro, auditoria local, fila sync, impressao.
- cloud: tenant manager, licencas, dispositivos, feature flags, logs, telemetria, webhooks e dashboard.
- servicos locais: API local, sync worker, printer service, peripheral service, watchdog, updater, backup e telemetry collector.

## Eliminacao gradual da dependencia do EquipeExe original - 2026-05-23

Foi criada a estrategia especifica para remover toda dependencia operacional, cognitiva e tecnica do EquipeExe original.

Artefatos criados:

- `D:\AtelieProd\docs\migracao\matriz-completa-dependencias-legado.md`
- `D:\AtelieProd\docs\migracao\matriz-criticidade-independencia-legado.md`
- `D:\AtelieProd\docs\migracao\plano-eliminacao-dependencias-legado.md`
- `D:\AtelieProd\docs\dominio\plano-extracao-definitiva-dominio.md`
- `D:\AtelieProd\docs\nextgen\blueprint-runtime-proprio.md`
- `D:\AtelieProd\docs\printer\blueprint-engine-impressao-propria.md`
- `D:\AtelieProd\docs\licensing\blueprint-licensing-proprio.md`
- `D:\AtelieProd\docs\nextgen\blueprint-ux-propria.md`
- `D:\AtelieProd\docs\migracao\roadmap-independencia-total-legado.md`
- `D:\AtelieProd\docs\migracao\roadmap-desligamento-futuro-equipeexe.md`
- `D:\AtelieProd\docs\migracao\plano-coexistencia-legado-novo.md`
- `D:\AtelieProd\docs\migracao\estrategia-final-substituicao-total.md`

Objetivo registrado:

- EquipeExe deixa gradualmente de ser fonte operacional e validador manual;
- Paradox vira fonte readonly/staging ate migracao completa;
- runtime, impressao, licensing, sync, observabilidade, UI e banco passam a ter implementacao propria no NextGen;
- legado termina como historico, auditoria e fallback temporario encerravel.

## Execucao da independencia total sem perder informacoes - 2026-05-23

Foi criada a estrategia executavel para substituir o EquipeExe sem perder dados, historico, consistencia financeira, estoque, relatorios, operacao ou produtividade.

Artefatos criados:

- `D:\AtelieProd\docs\migracao\matriz-critica-preservacao.md`
- `D:\AtelieProd\docs\migracao\estrategia-real-coexistencia.md`
- `D:\AtelieProd\docs\migracao\estrategia-dual-write.md`
- `D:\AtelieProd\docs\migracao\estrategia-shadow-database.md`
- `D:\AtelieProd\docs\migracao\estrategia-shadow-runtime.md`
- `D:\AtelieProd\docs\migracao\estrategia-validacao-automatica.md`
- `D:\AtelieProd\docs\migracao\estrategia-migracao-incremental.md`
- `D:\AtelieProd\docs\migracao\estrategia-desacoplamento-executavel.md`
- `D:\AtelieProd\docs\migracao\estrategia-substituicao-gradual-executavel.md`
- `D:\AtelieProd\docs\migracao\estrategia-desligamento-legado.md`
- `D:\AtelieProd\docs\migracao\pipeline-etl-definitivo.md`
- `D:\AtelieProd\docs\nextgen\blueprint-reconciliador-automatico.md`
- `D:\AtelieProd\docs\nextgen\blueprint-comparador-automatico.md`
- `D:\AtelieProd\docs\nextgen\blueprint-rollback-executavel.md`
- `D:\AtelieProd\docs\nextgen\blueprint-recovery-executavel.md`
- `D:\AtelieProd\docs\migracao\roadmap-executavel-migracao.md`
- `D:\AtelieProd\docs\nextgen\roadmap-executavel-nextgen.md`
- `D:\AtelieProd\docs\migracao\criterios-definitivos-independencia.md`
- `D:\AtelieProd\docs\nextgen\arquitetura-final-executavel.md`
- `D:\AtelieProd\docs\migracao\estrategia-definitiva-substituir-sem-perder-informacoes.md`

Decisao importante:

- dual write direto no Paradox original deve ser evitado;
- a estrategia preferida e shadow database, importacao incremental, comparadores, reconciliador e corte por modulo com fonte oficial unica;
- financeiro, fiscal e estoque exigem reconciliacao e janela controlada antes de qualquer corte.

## Transformacao do NextGen em fonte oficial - 2026-05-23

Foi definida a estrategia do primeiro corte oficial do NextGen.

Decisao:

- primeiro modulo oficial recomendado: consultas/relatorios readonly + clientes;
- Windows deve ser a primeira plataforma oficial durante a migracao;
- Linux appliance vem depois que core, PDV e impressao estiverem validados;
- fonte oficial passa a ser definida por modulo via `module_ownership`;
- nunca deve haver duas fontes oficiais simultaneas para a mesma operacao critica.

Artefatos criados:

- `D:\AtelieProd\docs\migracao\estrategia-primeiro-corte-real.md`
- `D:\AtelieProd\docs\migracao\estrategia-primeiro-modulo-oficial-nextgen.md`
- `D:\AtelieProd\docs\migracao\matriz-ownership-modulos.md`
- `D:\AtelieProd\docs\migracao\matriz-ownership-entidades.md`
- `D:\AtelieProd\docs\nextgen\blueprint-runtime-operacional.md`
- `D:\AtelieProd\docs\nextgen\blueprint-sqlite-operacional.md`
- `D:\AtelieProd\docs\nextgen\blueprint-comparadores-reais.md`
- `D:\AtelieProd\docs\nextgen\blueprint-reconciliador-operacional.md`
- `D:\AtelieProd\docs\nextgen\blueprint-observabilidade-total-operacional.md`
- `D:\AtelieProd\docs\printer\blueprint-impressao-operacional.md`
- `D:\AtelieProd\docs\nextgen\blueprint-rollback-operacional.md`
- `D:\AtelieProd\docs\nextgen\blueprint-recovery-operacional.md`
- `D:\AtelieProd\docs\migracao\roadmap-primeiro-corte.md`
- `D:\AtelieProd\docs\migracao\roadmap-fonte-oficial-nextgen.md`
- `D:\AtelieProd\docs\migracao\roadmap-desligamento-gradual-legado.md`
- `D:\AtelieProd\docs\nextgen\arquitetura-executavel-final-fonte-oficial.md`
- `D:\AtelieProd\docs\linux\estrategia-appliance-definitiva.md`
- `D:\AtelieProd\docs\linux\estrategia-linux-definitiva.md`
- `D:\AtelieProd\docs\windows\estrategia-windows-definitiva.md`
- `D:\AtelieProd\docs\migracao\estrategia-definitiva-nextgen-independente-equipeexe.md`

## Regra absoluta de preservacao total - 2026-05-23

Foi registrada a regra critica e nao negociavel: nenhuma informacao pode ficar para tras.

Isso inclui dados, historicos, registros, relacionamentos, regras, comportamentos, vinculos, auditoria, logs, relatorios, calculos, validacoes, permissoes, configuracoes, parametros, templates, impressoes, atalhos, produtividade operacional, experiencia operacional e comportamento esperado pelos usuarios.

Artefatos criados:

- `D:\AtelieProd\docs\migracao\politica-zero-information-loss.md`
- `D:\AtelieProd\docs\migracao\matriz-preservacao-total-informacoes.md`
- `D:\AtelieProd\docs\migracao\gates-obrigatorios-migracao.md`
- `D:\AtelieProd\docs\migracao\classificacao-dados-descartaveis-arquivaveis.md`
- `D:\AtelieProd\docs\migracao\plano-preservacao-relacionamentos-implicitos.md`
- `D:\AtelieProd\docs\migracao\plano-preservacao-regras-negocio.md`
- `D:\AtelieProd\docs\migracao\plano-preservacao-operacional-ux.md`
- `D:\AtelieProd\docs\printer\plano-preservacao-impressao-total.md`
- `D:\AtelieProd\docs\migracao\plano-preservacao-auditoria-rastreabilidade.md`
- `D:\AtelieProd\docs\migracao\plano-preservacao-financeira-estoque-fiscal.md`

Regra de ouro:

- antes de qualquer corte, migracao, substituicao, desligamento, reescrita, sincronizacao ou mudanca de ownership devem existir validacao automatica, reconciliador, comparador, backup, snapshot, rollback, recovery, auditoria, logs e rastreabilidade.

## Total operational validation e doublecheck absoluto - 2026-05-23

Foi criada a camada de validacao operacional total para provar consistencia antes do primeiro ownership real.

Artefatos criados:

- `D:\AtelieProd\docs\observabilidade\relatorio-doublecheck-total.md`
- `D:\AtelieProd\docs\observabilidade\relatorio-runtime-validation.md`
- `D:\AtelieProd\docs\printer\relatorio-impressao-validation.md`
- `D:\AtelieProd\docs\migracao\relatorio-sat-fiscal-validation.md`
- `D:\AtelieProd\docs\auth\relatorio-permissoes-validation.md`
- `D:\AtelieProd\docs\migracao\relatorio-relatorios-validation.md`
- `D:\AtelieProd\docs\nextgen\blueprint-operation-replay-engine.md`
- `D:\AtelieProd\docs\nextgen\blueprint-runtime-recorder.md`
- `D:\AtelieProd\docs\printer\blueprint-print-replay-engine.md`
- `D:\AtelieProd\docs\nextgen\blueprint-ownership-simulator.md`
- `D:\AtelieProd\docs\nextgen\blueprint-cutover-simulator.md`
- `D:\AtelieProd\docs\nextgen\blueprint-chaos-testing-engine.md`
- `D:\AtelieProd\docs\nextgen\blueprint-readiness-scoring-engine.md`
- `D:\AtelieProd\docs\observabilidade\blueprint-observabilidade-profunda.md`
- `D:\AtelieProd\docs\observabilidade\blueprint-runtime-tracing.md`
- `D:\AtelieProd\docs\migracao\estrategia-replay-operacional.md`
- `D:\AtelieProd\docs\migracao\estrategia-execucao-hibrida-controlada.md`
- `D:\AtelieProd\docs\migracao\estrategia-primeiro-ownership-real.md`
- `D:\AtelieProd\docs\migracao\estrategia-primeiro-corte-controlado.md`
- `D:\AtelieProd\docs\migracao\estrategia-validacao-total-nextgen.md`
- `D:\AtelieProd\MOD\apps\tools\nextgen_validation_engines.py`

Decisao:

- nenhum corte ocorre sem readiness score `GO`;
- divergencia critica, rollback nao testado, auditoria inativa, backup ausente ou recovery nao testado bloqueiam ownership.

Execucao inicial do scaffold:

- `D:\AtelieProd\MOD\apps\tools\nextgen_validation_engines.py init`
- `D:\AtelieProd\MOD\apps\tools\nextgen_validation_engines.py score`

Resultado inicial:

- status: `NO-GO`.
- motivo: scaffold inicial ainda contem bloqueios obrigatorios (`critical_divergence`, `rollback_not_tested`, `audit_disabled`, `backup_missing`, `recovery_not_tested`).
- interpretacao: nenhum ownership/corte real esta aprovado ainda; o resultado serve como baseline de seguranca.

## Operational hardening e autorizacao ISO Linux - 2026-05-23

Foi registrada a autorizacao para customizacao completa da ISO Linux e criada a camada inicial de hardening operacional do NextGen.

Artefatos criados:

- `D:\AtelieProd\docs\hardening\backup-engine-operacional.md`
- `D:\AtelieProd\docs\hardening\restore-validator-operacional.md`
- `D:\AtelieProd\docs\hardening\recovery-engine-operacional.md`
- `D:\AtelieProd\docs\hardening\recovery-simulator.md`
- `D:\AtelieProd\docs\hardening\rollback-engine-operacional.md`
- `D:\AtelieProd\docs\hardening\rollback-simulator.md`
- `D:\AtelieProd\docs\hardening\audit-engine-operacional.md`
- `D:\AtelieProd\docs\hardening\event-journal.md`
- `D:\AtelieProd\docs\hardening\runtime-journal.md`
- `D:\AtelieProd\docs\hardening\divergence-classifier.md`
- `D:\AtelieProd\docs\hardening\critical-divergence-engine.md`
- `D:\AtelieProd\docs\hardening\shadow-execution-real.md`
- `D:\AtelieProd\docs\hardening\chaos-engine-operacional.md`
- `D:\AtelieProd\docs\hardening\readiness-engine-operacional.md`
- `D:\AtelieProd\docs\appliance\appliance-hardening.md`
- `D:\AtelieProd\docs\appliance\customizacao-iso-linux.md`
- `D:\AtelieProd\Atelie_Linux\image_build\profiles\debian-minimal-openbox-kiosk.json`
- `D:\AtelieProd\Atelie_Linux\image_build\README.md`
- `D:\AtelieProd\MOD\apps\tools\nextgen_hardening_engine.py`

Gates:

- nenhuma ISO foi remasterizada ainda porque a ISO base ainda nao foi fornecida/testada;
- appliance permanece `NO-GO` ate validacao em VM, Dell real, impressora real, Bluetooth real, rollback e recovery.

Execucao inicial do hardening:

- `D:\AtelieProd\MOD\apps\tools\nextgen_hardening_engine.py init`
- backup controlado de `D:\AtelieProd\MOD\validation`
- readiness operacional inicial

Resultado:

- backup criado: `D:\AtelieProd\MOD\hardening\backups\20260523-232816-e84f36c4`
- readiness: `NO-GO`
- scores: backup `10`, audit `10`, restore `0`, rollback `0`, recovery `0`, appliance `0`
- bloqueios: `restore_not_validated`, `rollback_not_tested`, `recovery_not_tested`, `critical_divergence_gate_active`, `hardware_gates_pending`

Interpretacao:

- backup e auditoria inicial existem;
- ainda nao ha permissao operacional para ownership/cutover;
- gates reais permanecem pendentes ate validacao controlada.

## Real execution validation fisica - 2026-05-23

Foi criada a camada de validacao operacional fisica real. Todos os itens que dependem de Dell real, impressora real, Bluetooth real, appliance real, ISO real, restore real, rollback real e recovery real permanecem `NO-GO` ate execucao fisica.

Artefatos criados:

- `D:\AtelieProd\docs\physical-validation\hardware-validation-engine.md`
- `D:\AtelieProd\docs\physical-validation\printer-validation-engine.md`
- `D:\AtelieProd\docs\physical-validation\bluetooth-validation-engine.md`
- `D:\AtelieProd\docs\physical-validation\appliance-boot-validator.md`
- `D:\AtelieProd\docs\physical-validation\restore-execution-validator.md`
- `D:\AtelieProd\docs\physical-validation\recovery-execution-validator.md`
- `D:\AtelieProd\docs\physical-validation\rollback-execution-validator.md`
- `D:\AtelieProd\docs\physical-validation\sqlite-corruption-simulator.md`
- `D:\AtelieProd\docs\physical-validation\runtime-failure-simulator.md`
- `D:\AtelieProd\docs\physical-validation\physical-device-replay.md`
- `D:\AtelieProd\docs\physical-validation\shadow-execution-real.md`
- `D:\AtelieProd\docs\physical-validation\chaos-execution-real.md`
- `D:\AtelieProd\docs\physical-validation\readiness-hardening.md`
- `D:\AtelieProd\docs\physical-validation\appliance-iso-customization.md`
- `D:\AtelieProd\docs\physical-validation\runtime-replay-fisico.md`
- `D:\AtelieProd\docs\physical-validation\printer-replay-fisico.md`
- `D:\AtelieProd\docs\physical-validation\appliance-replay-fisico.md`
- `D:\AtelieProd\docs\physical-validation\hardware-readiness-report.md`
- `D:\AtelieProd\docs\physical-validation\appliance-readiness-report.md`
- `D:\AtelieProd\docs\physical-validation\estrategia-definitiva-real-execution-validation.md`
- `D:\AtelieProd\Atelie_Linux\diagnostics\physical-validation.sh`
- `D:\AtelieProd\MOD\apps\tools\physical_validation_engine.py`

Regra:

- nenhuma equivalencia fisica deve ser presumida; hardware, impressora, Bluetooth, appliance, restore, rollback e recovery exigem evidencia real.

Execucao inicial:

- `D:\AtelieProd\MOD\apps\tools\physical_validation_engine.py`

Resultado:

- overall: `NO-GO`.
- gates `NO-GO`: hardware, printer, bluetooth, appliance, restore, recovery, rollback, runtime_replay, shadow_execution e chaos.
- motivo: evidencia fisica obrigatoria ainda ausente.

Evidencias obrigatorias pendentes:

- inventario Dell;
- teste fisico de impressora;
- teste fisico Bluetooth;
- boot appliance em VM;
- boot appliance em Dell;
- restore execution;
- rollback execution;
- recovery execution.

## Absolute legacy parity - 2026-05-24

Foi criada a fase de certeza matematica de equivalencia total entre EquipeExe e NextGen. A regra central desta fase e que o NextGen nao pode assumir ownership oficial enquanto paridade de dominio, runtime, impressao, UI, relatorios, permissoes, replay operacional, shadow execution, chaos, appliance, forense digital e readiness fisico nao estiverem aprovados por evidencia.

Artefatos criados:

- `D:\AtelieProd\docs\absolute-parity\domain-parity-report.md`
- `D:\AtelieProd\docs\absolute-parity\runtime-parity-report.md`
- `D:\AtelieProd\docs\absolute-parity\print-parity-report.md`
- `D:\AtelieProd\docs\absolute-parity\ui-parity-report.md`
- `D:\AtelieProd\docs\absolute-parity\report-parity-report.md`
- `D:\AtelieProd\docs\absolute-parity\permission-parity-report.md`
- `D:\AtelieProd\docs\absolute-parity\operation-replay-absoluto.md`
- `D:\AtelieProd\docs\absolute-parity\shadow-execution-absoluto.md`
- `D:\AtelieProd\docs\absolute-parity\chaos-validation-absoluta.md`
- `D:\AtelieProd\docs\absolute-parity\appliance-parity-report.md`
- `D:\AtelieProd\docs\absolute-parity\digital-forensics-report.md`
- `D:\AtelieProd\docs\absolute-parity\divergence-detector-absoluto.md`
- `D:\AtelieProd\docs\absolute-parity\hidden-behavior-detector.md`
- `D:\AtelieProd\docs\absolute-parity\implicit-rule-detector.md`
- `D:\AtelieProd\docs\absolute-parity\operational-parity-score.md`
- `D:\AtelieProd\docs\absolute-parity\runtime-parity-score.md`
- `D:\AtelieProd\docs\absolute-parity\print-parity-score.md`
- `D:\AtelieProd\docs\absolute-parity\ownership-readiness-score.md`
- `D:\AtelieProd\docs\absolute-parity\legacy-knowledge-extraction-report.md`
- `D:\AtelieProd\docs\absolute-parity\estrategia-definitiva-absolute-legacy-parity.md`
- `D:\AtelieProd\MOD\apps\tools\absolute_legacy_parity_engine.py`

Execucao inicial:

- `python D:\AtelieProd\MOD\apps\tools\absolute_legacy_parity_engine.py`

Resultado:

- readiness absoluto: `NO-GO`;
- gates `NO-GO`: domain_parity, runtime_parity, print_parity, ui_parity, report_parity, permission_parity, operation_replay, shadow_execution, chaos_validation, appliance_parity, digital_forensics, divergence_detector, hidden_behavior_detector, implicit_rule_detector, legacy_knowledge_extraction e physical_readiness;
- bloqueios principais: absolute_replay_not_executed, runtime_parity_not_traced, print_parity_not_physically_validated, ui_parity_not_replayed, report_parity_not_diffed, permission_parity_not_replayed, critical_divergence_gate_active, hardware_gates_pending e physical_readiness_no_go.

Regra:

- nenhum ownership, cutover, runtime oficial, SQLite oficial ou impressao oficial pode ocorrer enquanto existir gate fisico pendente ou paridade critica incompleta.
- Sistema legado original nao foi alterado.

## Final execution parity - 2026-05-24

Foi criada a camada de execucao final de paridade, voltada a evidencias reais. Esta fase nao promove nenhum gate por blueprint ou simulacao; cada gate exige arquivo de evidencia preenchido, validado, com `critical_divergences` igual a zero.

Artefatos criados:

- `D:\AtelieProd\docs\final-execution-parity\replay-evidence-real.md`
- `D:\AtelieProd\docs\final-execution-parity\runtime-parity-evidence.md`
- `D:\AtelieProd\docs\final-execution-parity\print-parity-evidence.md`
- `D:\AtelieProd\docs\final-execution-parity\ui-parity-evidence.md`
- `D:\AtelieProd\docs\final-execution-parity\report-diff-evidence.md`
- `D:\AtelieProd\docs\final-execution-parity\permission-replay-evidence.md`
- `D:\AtelieProd\docs\final-execution-parity\dell-hardware-validation.md`
- `D:\AtelieProd\docs\final-execution-parity\bluetooth-validation-real.md`
- `D:\AtelieProd\docs\final-execution-parity\appliance-validation-real.md`
- `D:\AtelieProd\docs\final-execution-parity\restore-validation-real.md`
- `D:\AtelieProd\docs\final-execution-parity\recovery-validation-real.md`
- `D:\AtelieProd\docs\final-execution-parity\rollback-validation-real.md`
- `D:\AtelieProd\docs\final-execution-parity\shadow-execution-definitivo.md`
- `D:\AtelieProd\docs\final-execution-parity\divergence-elimination-report.md`
- `D:\AtelieProd\docs\final-execution-parity\runtime-tracing-evidence.md`
- `D:\AtelieProd\docs\final-execution-parity\appliance-replay-evidence.md`
- `D:\AtelieProd\docs\final-execution-parity\print-replay-evidence.md`
- `D:\AtelieProd\docs\final-execution-parity\operational-parity-evidence.md`
- `D:\AtelieProd\docs\final-execution-parity\conditional-go-report.md`
- `D:\AtelieProd\docs\final-execution-parity\estrategia-definitiva-final-execution-parity.md`
- `D:\AtelieProd\MOD\apps\tools\final_execution_parity_engine.py`

Evidencias JSON inicializadas em:

- `D:\AtelieProd\MOD\final-execution-parity\evidence`

Relatorio gerado:

- `D:\AtelieProd\MOD\final-execution-parity\reports\final-execution-readiness.json`

Resultado:

- level: `NO-GO`;
- ownership_allowed: `false`;
- shadow_go_allowed: `false`;
- upstream `NO-GO`: absolute_legacy_parity, physical_readiness e hardening;
- gates locais `NO-GO`: replay, runtime, print, UI, reports, permissions, dell_hardware, bluetooth, appliance, restore, recovery, rollback, shadow_execution e divergence_elimination.

Regra:

- `CONDITIONAL-GO` so podera ser emitido quando todos os gates reais estiverem validados por evidencia e divergencia critica zero.
- Sistema legado original nao foi alterado.

## Validacao total + Supabase + GitHub + Mercado Pago + Licensing - 2026-05-24

Foi criada a fase de integracao segura com Supabase, GitHub, Mercado Pago, licenciamento Rogerio, vendas Luci e taxa de servico Rogerio, mantendo a regra de que nenhum ownership/cutover/shadow-go pode ser liberado com credencial insegura, RLS ausente, webhook nao validado, pagamento nao testado ou informacao pendente do EquipeExe.

Artefatos principais:

- `D:\AtelieProd\MOD\supabase\migrations\20260524_0001_nextgen_core.sql`
- `D:\AtelieProd\docs\supabase\sql-completo-supabase.md`
- `D:\AtelieProd\docs\supabase\rls-policies.md`
- `D:\AtelieProd\docs\supabase\configuracao-supabase.md`
- `D:\AtelieProd\docs\github\configuracao-github-segura.md`
- `D:\AtelieProd\MOD\github-actions-nextgen-ci.yml`
- `D:\AtelieProd\MOD\.env.example`
- `D:\AtelieProd\docs\mercado-pago\configuracao-mercado-pago-rogerio.md`
- `D:\AtelieProd\docs\mercado-pago\configuracao-mercado-pago-luci.md`
- `D:\AtelieProd\docs\mercado-pago\webhook-mercado-pago.md`
- `D:\AtelieProd\docs\mercado-pago\vendas-luci.md`
- `D:\AtelieProd\docs\mercado-pago\taxa-servico-rogerio.md`
- `D:\AtelieProd\docs\licensing\licensing-rogerio.md`
- `D:\AtelieProd\docs\seguranca\checklist-seguranca-supabase-github-mercadopago.md`
- `D:\AtelieProd\docs\seguranca\checklist-go-nogo.md`
- `D:\AtelieProd\docs\validacao-total\relatorio-nada-ficou-para-tras.md`
- `D:\AtelieProd\docs\validacao-total\matriz-completa-equipeexe.md`
- `D:\AtelieProd\docs\validacao-total\matriz-tabela-campo-entidade.md`
- `D:\AtelieProd\docs\validacao-total\matriz-tela-banco.md`
- `D:\AtelieProd\docs\validacao-total\matriz-historico-vinculo.md`
- `D:\AtelieProd\docs\validacao-total\matriz-permissoes.md`
- `D:\AtelieProd\docs\validacao-total\plano-final-execucao.md`
- `D:\AtelieProd\docs\windows\comunicacao-backend-nextgen.md`
- `D:\AtelieProd\docs\linux\comunicacao-backend-nextgen.md`

Scripts criados:

- `D:\AtelieProd\MOD\apps\tools\payments\mercado_pago_validate_accounts.py`
- `D:\AtelieProd\MOD\apps\tools\payments\mercado_pago_webhook_validator.py`
- `D:\AtelieProd\MOD\apps\tools\payments\mercado_pago_create_pix_payload.py`
- `D:\AtelieProd\MOD\apps\tools\build_total_legacy_validation.py`

Execucoes:

- inventario read-only com hash completo em `D:\AtelieProd\Equipexe`;
- total de arquivos inventariados: 53.179;
- candidatos a tabela Paradox: 965;
- indices/metadados Paradox: 2.413;
- binarios/runtime: 110;
- assets visuais: 1.593;
- matriz gerada: `D:\AtelieProd\MOD\total-validation\matriz-completa-equipeexe.csv`;
- resumo gerado: `D:\AtelieProd\MOD\total-validation\resumo-validacao-total.json`;
- validacao Mercado Pago executada sem secrets no ambiente, resultando `NO-GO` por `missing_env`, sem logar tokens.

Seguranca:

- tokens Mercado Pago exibidos em imagem/chat devem ser rotacionados antes de producao;
- tokens nao foram gravados em codigo, log, JSON versionado ou scripts;
- GitHub token deve ficar apenas em GitHub Secrets;
- Mercado Pago tokens devem ficar apenas em secrets/backend;
- Supabase service role key nao deve ir para cliente Windows/Linux.

Status:

- `NO-GO`.
- Sistema legado original nao foi alterado.

## Uso seguro e validacao total das integracoes - 2026-05-24

Continuidade da fase de integracoes com reforco de seguranca, device-aware sync, contratos de API e validadores locais.

Novos artefatos:

- `D:\AtelieProd\MOD\.gitignore`
- `D:\AtelieProd\MOD\supabase\migrations\20260524_0002_security_rpc_device_sync.sql`
- `D:\AtelieProd\MOD\apps\tools\security_secret_scan.py`
- `D:\AtelieProd\MOD\apps\tools\validate_integration_config.py`
- `D:\AtelieProd\MOD\apps\tools\payments\mercado_pago_server_side.py`
- `D:\AtelieProd\Atelie_Windows\database\local_schema.sql`
- `D:\AtelieProd\Atelie_Linux\database\local_schema.sql`
- `D:\AtelieProd\docs\github\secrets-e-pipeline.md`
- `D:\AtelieProd\docs\arquitetura\contratos-api-nextgen.md`
- `D:\AtelieProd\docs\arquitetura\offline-first-sync-reconciliacao.md`
- `D:\AtelieProd\docs\mercado-pago\integracao-server-side.md`
- `D:\AtelieProd\docs\seguranca\validacao-config-integracoes.md`

Melhorias:

- `.env.example` mantido sem secrets reais;
- pipeline GitHub atualizado para executar scanner de secrets;
- segunda migration adiciona `device_access_grants`, `sync_conflicts`, `payment_audit_events`, RPCs de log/sync/readiness/appliance e validacao de dispositivo autorizado;
- schemas SQLite locais criados para Windows e Linux;
- scanner de secrets executado com resultado `PASS`;
- validador de configuracao executado com resultado `NO-GO` por variaveis de ambiente ausentes, sem imprimir valores.

Status:

- `NO-GO`, como esperado ate secrets seguros, Mercado Pago, Supabase, webhook, RLS, replay e divergencias serem validados.
- Sistema legado original nao foi alterado.

## Execucao total das validacoes pendentes - tentativa segura - 2026-05-24

Foi criado e executado o orquestrador seguro das validacoes pendentes:

- `D:\AtelieProd\MOD\apps\tools\run_pending_validations.py`

Relatorios/evidencias gerados:

- `D:\AtelieProd\MOD\final-execution-parity\reports\pending-validations-execution.json`
- `D:\AtelieProd\MOD\final-execution-parity\evidence\supabase-validation.json`
- `D:\AtelieProd\MOD\final-execution-parity\evidence\github-validation.json`
- `D:\AtelieProd\MOD\final-execution-parity\evidence\windows-linux-sync-validation.json`
- `D:\AtelieProd\docs\validacao-total\execucao-validacoes-pendentes.md`
- `D:\AtelieProd\docs\validacao-total\roadmap-final-ownership-controlado.md`

Resultado:

- overall: `NO-GO`;
- ownership_allowed: `false`;
- shadow_go_allowed: `false`;
- tokens_logged: `false`;
- Supabase endpoint alcancavel, com resposta `401` sem apikey, indicando autenticacao obrigatoria;
- migrations locais presentes;
- schemas SQLite Windows/Linux presentes;
- Git e Python disponiveis;
- Supabase CLI ausente;
- `psql` ausente;
- variaveis de ambiente sensiveis ausentes nesta sessao;
- GitHub foi testado sem token, mas validacao completa ainda exige secret novo;
- impressao fisica, Bluetooth real, Dell real, appliance real, pagamento/webhook e UI replay fisico permanecem nao executaveis nesta maquina sem ambiente externo preparado.

Sistema legado original nao foi alterado.

## Supabase absolute data validation - 2026-05-24

Foi criada a fase de validacao absoluta do Supabase com foco na regra `NADA PODE FICAR PARA TRAS`.

Implementado:

- `D:\AtelieProd\MOD\apps\tools\supabase_absolute_data_validation.py`;
- `D:\AtelieProd\MOD\supabase\migrations\202605240005_absolute_data_validation_catalog.sql`;
- relatorios em `D:\AtelieProd\docs\supabase\absolute-data-validation`;
- espelho dos relatorios em `D:\AtelieProd\MOD\docs\supabase\absolute-data-validation`;
- readiness em `D:\AtelieProd\MOD\final-execution-parity\reports\cloud-absolute-readiness.json`.

A migration `202605240005_absolute_data_validation_catalog.sql` adiciona estrutura cloud para:

- `legacy_evidence_sources`;
- `legacy_evidence_items`;
- `cloud_validation_runs`;
- `cloud_validation_findings`;
- `cloud_diff_matrix`;
- view `v_cloud_absolute_validation_summary`.

Fontes locais analisadas:

- dicionario Paradox campo a campo: 4.996 linhas;
- mapa entidades dominio: 478 linhas;
- matriz relacionamentos: 12 linhas;
- matriz UI banco: 757 linhas;
- matriz tela acao tabela: 1.936 linhas;
- matriz relatorios: 1.362 linhas;
- campos de status: 97 linhas;
- valores financeiros: 147 linhas;
- datas operacionais: 856 linhas.

Resultado da execucao:

- status: `NO-GO`;
- migration local preparada: sim;
- aplicacao live da migration/importacao cloud: bloqueada nesta execucao porque a conexao PostgreSQL live nao estava autenticada no runtime atual;
- schema por migrations locais possui 70 tabelas candidatas;
- schemas SQLite locais comparados: Windows 6 tabelas, Linux 7 tabelas;
- catalogo legado cloud ainda nao importado nesta execucao;
- findings: 25 criticos e 56 altos;
- ownership_allowed=false;
- shadow_go_allowed=false;
- cloud_official_allowed=false.

Conclusao:

- a estrutura de validacao absoluta foi criada;
- os artefatos locais foram medidos, hasheados e rastreados;
- o Supabase nao pode ser declarado fonte cloud oficial ainda;
- e necessario provisionar `SUPABASE_DB_URL`/senha no runtime seguro ou pooler autenticado e reexecutar a validacao para aplicar a migration 005 e importar o catalogo legado.

Sistema legado original nao foi alterado.

## GitHub como central oficial de releases e atualizacoes - 2026-05-24

Foi configurada a arquitetura do GitHub como central oficial de versionamento, releases, distribuicao de builds, manifestos de atualizacao, checksums, assinatura futura e rollback.

Implementado no repositório versionável `D:\AtelieProd\MOD`:

- `.github/workflows/nextgen-ci.yml`;
- `.github/workflows/nextgen-release.yml`;
- `apps/updater/release_updater.py`;
- `apps/tools/release_gate.py`;
- `apps/tools/generate_release_manifest.py`;
- `release/update-manifest.json`;
- `release/latest.json`;
- `release/checksums.txt`;
- `release/changelog.md`;
- `release/release-notes.md`;
- `docs/releases/arquitetura-github-releases.md`;
- `docs/releases/estrategia-branches.md`;
- `docs/releases/readiness-release-gate.md`;
- `docs/releases/updater-nextgen.md`.

Implementado também nos diretórios operacionais separados por plataforma:

- `D:\AtelieProd\Atelie_Windows\updater\Invoke-AtelieUpdate.ps1`;
- `D:\AtelieProd\Atelie_Windows\updater\Rollback-AtelieUpdate.ps1`;
- `D:\AtelieProd\Atelie_Windows\installer\README.md`;
- `D:\AtelieProd\Atelie_Linux\updater\atelie-updater.sh`;
- `D:\AtelieProd\Atelie_Linux\updater\atelie-update-check.sh`;
- `D:\AtelieProd\Atelie_Linux\image_build\RELEASE-ASSETS.md`.

Politica definida:

- branches: `main`, `develop`, `homolog`, `release/*`, `hotfix/*`;
- tags semanticas: `v1.0.0`, `v1.0.1`, `v1.1.0`;
- canais: `stable`, `beta`, `homolog`, `appliance`;
- releases com manifest, SHA256, assinatura, changelog e release notes;
- updater bloqueia atualizacao durante venda, pagamento, impressao, sync critico ou migracao;
- rollback automatico/manual preparado.

Validacoes executadas:

- scripts novos compilados com `py_compile`;
- updater validou leitura do manifest e detectou atualizacao disponivel;
- scanner de secrets `PASS`;
- release gate bloqueou corretamente publicacao operacional por readiness `NO-GO`;
- auto-management permanece `NO-GO`;
- ownership_allowed=false;
- shadow_go_allowed=false.

Status:

- GitHub Releases estruturado como canal oficial;
- pipeline de release preparado, mas publicacao operacional permanece bloqueada;
- assinatura real de assets ainda precisa provisionamento de chave;
- readiness fisico/replay/appliance/webhook/pagamentos continua pendente;
- sistema legado original nao foi alterado.

## Correcao do ambiente local Supabase/PostgreSQL/Git - 2026-05-24

Foi executada a correcao local para permitir futura aplicacao real das migrations Supabase.

Instalado/configurado:

- Supabase CLI `2.101.0` em `D:\AtelieProd\MOD\tools\supabase`;
- PostgreSQL/psql `17.10` via winget;
- PATH do usuario atualizado com Supabase CLI e PostgreSQL bin;
- `supabase init` executado em `D:\AtelieProd\MOD`;
- Git inicializado em `D:\AtelieProd\MOD`;
- remote `origin` configurado para `https://github.com/rgerio01/Luci_atelie.git`;
- `.env` local criado e protegido por `.gitignore`;
- `.env.example` atualizado com placeholders;
- `.gitignore` atualizado com `.crt`;
- arquivo `D:\AtelieProd\MOD\supabase\validate_rls_and_tables.sql` criado;
- validador `D:\AtelieProd\MOD\apps\tools\validate_supabase_environment.py` criado.

Relatorios:

- `D:\AtelieProd\MOD\docs\supabase\relatorio-validacao-ambiente-supabase.md`
- `D:\AtelieProd\MOD\final-execution-parity\reports\supabase-environment-validation.json`
- `D:\AtelieProd\MOD\final-execution-parity\reports\supabase-environment-readiness.json`

Status:

- `supabase_cli_installed=true`;
- `psql_installed=true`;
- `git_installed=true`;
- `supabase_initialized=true`;
- `supabase_linked=false`;
- `db_connection_validated=false`;
- `migrations_applied=false`;
- `rls_validated=false`;
- `github_validated=partial`;
- scanner de secrets: `PASS`.

Bloqueios restantes:

- `SUPABASE_ACCESS_TOKEN` ausente;
- `SUPABASE_DB_URL` real ausente;
- publishable key retornou `401` no teste REST autenticado e deve ser conferida/rotacionada;
- migrations nao aplicadas;
- RLS nao validada no remoto.

Sistema legado original nao foi alterado.

## Cloud runtime validation e auto management - 2026-05-24

Foi executada validacao cloud real usando credenciais em variaveis de ambiente de processo, sem gravar tokens em arquivos versionados.

Resultados Supabase/PostgreSQL:

- access token validou acesso a lista de projetos;
- projeto `kwodkzfiuultdezanrjv` identificado como `Luci` e `ACTIVE_HEALTHY`;
- `supabase link` concluido;
- conexao direta IPv6 do host `db.kwodkzfiuultdezanrjv.supabase.co` falhou na rede atual;
- relink via pooler IPv4 concluido;
- migration principal aplicada;
- migration `202605240002_security_rpc_device_sync.sql` aplicada via `psql`;
- migration `202605240003_rls_completion.sql` criada e aplicada para completar RLS;
- historico de migrations reparado para `202605240001`, `202605240002`, `202605240003`;
- `supabase db push --dry-run` confirmou banco remoto atualizado;
- validacao RLS confirmou 33 tabelas publicas com `rowsecurity=true`;
- validacao PostgreSQL confirmou 33 tabelas, 55 indices e 69 triggers;
- calculos de licensing validados: 30 dias 350,00; 3 meses 997,50; 6 meses 1932,00; 12 meses 3780,00.

Resultados Mercado Pago:

- Rogerio validado por API com status `VALIDATED`;
- Luci validada por API com status `VALIDATED`;
- tokens_logged=false;
- sem tokens gravados em arquivos versionados.

Automacao criada:

- `D:\AtelieProd\MOD\apps\tools\auto_management_engine.py`;
- `D:\AtelieProd\docs\arquitetura\auto-management-engine.md`;
- `D:\AtelieProd\docs\arquitetura\roadmap-final-automacao-operacional.md`;
- `D:\AtelieProd\docs\arquitetura\relatorio-comunicacao-cloud-runtime.md`.

Readiness:

- scanner de secrets `PASS`;
- cloud database validado;
- RLS validado;
- Mercado Pago ownership validado;
- readiness final permanece `NO-GO`;
- ownership_allowed=false;
- shadow_go_allowed=false.

Pendencias:

- publishable key retornou `401` no teste REST e deve ser conferida/rotacionada;
- webhook Mercado Pago nao validado;
- pagamento teste nao executado;
- taxa/split/repasse nao validado;
- sync Windows/Linux runtime nao executado;
- appliance/Dell/Bluetooth/impressao fisica/replay/rollback/recovery ainda pendentes.

Sistema legado original nao foi alterado.

## Regra revisada de credenciais e Supabase completo - 2026-05-24

Foi revisada a regra de credenciais para homologacao/runtime controlado: credenciais podem existir em runtime, appliance, backend e ambiente operacional controlado, desde que protegidas, criptografadas/ofuscadas, mascaradas e nao versionadas.

Implementado:

- `D:\AtelieProd\MOD\apps\tools\credential_protection_engine.py`;
- `D:\AtelieProd\Atelie_Linux\config\secure-config-loader.sh`;
- `D:\AtelieProd\MOD\supabase\migrations\202605240004_domain_runtime_completion.sql`;
- `D:\AtelieProd\docs\seguranca\relatorio-seguranca-runtime.md`;
- `D:\AtelieProd\docs\seguranca\roadmap-hardening-futuro.md`;
- `D:\AtelieProd\docs\supabase\relatorio-tudo-refletido-no-supabase.md`;
- `D:\AtelieProd\docs\supabase\matriz-local-supabase.md`;
- relatorios de diff SQLite/runtime/financeiro/estoque/licensing/pagamentos/observabilidade vs Supabase.

Supabase atualizado:

- migration `202605240004_domain_runtime_completion.sql` aplicada;
- historico reparado como aplicado;
- `supabase db push --dry-run` confirmou remoto atualizado;
- tabelas publicas: 65;
- tabelas com RLS: 65;
- tabelas sem RLS: 0;
- criadas tabelas para licensing history/events/runtime, pagamentos eventos/webhooks/reconciliation, split_payments, historicos, estoque movimentos, OS, financeiro, SAT, relatorios, runtime_events, replay/rollback/recovery logs, sync_events, sync_reconciliation, appliance_events/runtime, observabilidade, divergencias e auditoria_eventos.

Status:

- schema cloud completo para as categorias conhecidas;
- dados historicos/ETL/diff/replay ainda pendentes;
- credential vault implementado, mas nao provisionado nesta sessao;
- scanner de secrets `PASS`;
- readiness permanece `NO-GO`;
- ownership_allowed=false;
- shadow_go_allowed=false.

Sistema legado original nao foi alterado.
