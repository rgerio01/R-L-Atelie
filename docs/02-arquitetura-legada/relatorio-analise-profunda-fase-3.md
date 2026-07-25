# Relatorio de Analise Profunda - Fase 3

Data: 2026-05-23

Escopo analisado:

- Executaveis e DLLs do legado em `D:\AtelieProd\Equipexe`.
- Superficie de imports PE dos modulos principais.
- Sinais estaticos de rede, atualizacao, autenticacao, licenca, hardware binding, memoria/processos e locks.
- Estrutura MOD de homologacao, sem alteracao no original.

## Regra de preservacao

Nenhum arquivo do sistema original foi alterado nesta etapa. Toda coleta foi feita por leitura e os artefatos foram gravados em `D:\AtelieProd\MOD`.

## Artefatos gerados

- `D:\AtelieProd\MOD\docs\02-arquitetura-legada\pe-imports\imports-executaveis-dlls.csv`
- `D:\AtelieProd\MOD\docs\02-arquitetura-legada\pe-imports\imports-resumo.csv`
- `D:\AtelieProd\MOD\docs\02-arquitetura-legada\sinais-profundos\hardware-binding.txt`
- `D:\AtelieProd\MOD\docs\02-arquitetura-legada\sinais-profundos\licenca-autenticacao.txt`
- `D:\AtelieProd\MOD\docs\02-arquitetura-legada\sinais-profundos\rede-http-socket.txt`
- `D:\AtelieProd\MOD\docs\02-arquitetura-legada\sinais-profundos\update-sync-cloud.txt`
- `D:\AtelieProd\MOD\docs\02-arquitetura-legada\sinais-profundos\memoria-processos.txt`
- `D:\AtelieProd\MOD\docs\02-arquitetura-legada\sinais-profundos\temporarios-locks.txt`

Observacao: os arquivos de sinais profundos sao evidencias brutas, grandes e ruidosas. Eles podem conter trechos binarios e dados sensiveis. O uso recomendado e consulta tecnica filtrada, nao leitura integral nem publicacao direta.

## Resultado dos imports PE

Foram identificados 1.122 imports em executaveis, DLLs e OCXs.

Distribuicao de arquitetura:

- `0x014C`: 1.119 imports, indicando predominancia de binarios Windows 32-bit.
- `0x8664`: 3 imports, indicando presenca pontual de binario 64-bit.

Conclusao: o legado depende majoritariamente de pilha 32-bit. A nova versao deve preservar compatibilidade operacional nesta fase, especialmente para BDE, Paradox, fiscais, impressao e componentes COM/OCX.

## Modulos principais

### LavSoft.exe

Imports relevantes:

- `BEMAFI32.DLL`
- `Mp20fi32.dll`
- `general32.dll`
- `mpr.dll`
- `winspool.drv`

Interpretacao:

- Forte ligacao com impressoras fiscais e/ou bibliotecas Bematech.
- Uso de impressao Windows.
- Possivel acesso a recursos de rede via `mpr.dll`.

### LavFacilLan.exe

Imports relevantes:

- `BEMAFI32.DLL`
- `Mp20fi32.dll`
- `general32.dll`
- `wininet.dll`
- `mpr.dll`
- `qtintf70.dll`
- `winspool.drv`

Interpretacao:

- E um dos modulos com maior superficie operacional.
- Possui comunicacao via `wininet.dll`.
- Possui dependencias fiscais/impressao.
- Pode atuar como ponto de entrada operacional ou modulo integrado com rede/atualizacao.

### Gerenciador.exe

Imports relevantes:

- `mscoree.dll`

Interpretacao:

- Indica componente .NET ou bootstrapper .NET dentro do conjunto legado.
- Deve ser analisado separadamente com ferramentas de metadados .NET na proxima fase.

### Financeiro.exe

Imports relevantes:

- `mpr.dll`
- `winspool.drv`

Interpretacao:

- Acesso a recursos de rede e impressao.
- Nao apareceu import direto de `wininet.dll` nesta coleta, mas pode depender de DLLs auxiliares.

### Estoque.exe

Imports relevantes:

- `wininet.dll`
- `winspool.drv`

Interpretacao:

- Possui capacidade direta de comunicacao HTTP/Internet via WinINet.
- Tambem usa impressao.

### NFE.exe

Imports relevantes:

- `wininet.dll`
- `lotenfse.dll`
- `winspool.drv`

Interpretacao:

- Superficie de comunicacao externa fiscal.
- Integra com componente de NFSe/lote.
- Deve ser tratado como modulo critico de integracao externa.

### SAT.exe

Imports relevantes:

- `Tapi32.dll`
- `qtintf70.dll`
- `winspool.drv`

Interpretacao:

- Modulo fiscal especializado.
- Requer validacao em ambiente isolado antes de qualquer modernizacao.

## Comunicacoes externas

Evidencias encontradas:

- `wininet.dll` em `LavFacilLan.exe`, `EquEstruAtu.exe`, `Estoque.exe`, `lotenfse.dll` e `NFE.exe`.
- `wsock32.dll` em `Conexoes.exe`, `EqEmail.exe`, `EquMail.exe`, `GDS32.DLL`, `qtintf.dll` e `qtintf70.dll`.
- `urlmon.dll` em bibliotecas Daruma.
- `LIBEAY32.dll` e `ssleay32.dll`, indicando OpenSSL legado.

Classificacao inicial:

- Essencial: NFE/NFSe/SAT e eventuais comunicacoes fiscais obrigatorias.
- Provavelmente essencial ou operacional: email, conexoes de sincronizacao, recursos em rede local.
- Deve permanecer bloqueado no MOD: atualizacao automatica do legado.
- Risco: bibliotecas OpenSSL antigas e conexoes diretas sem centralizacao de configuracao.

## Atualizacao automatica

Sinais encontrados:

- `LiveUpdate.exe`
- `EquEstruAtu.exe`
- diretorios/termos relacionados a `Sincroniza` e `Nuvem`
- chamadas potenciais de download/update nos artefatos estaticos

Status no MOD:

- O `LiveUpdate.exe` do runtime MOD foi substituido por um stub local seguro que registra tentativa e encerra sem rede.
- A politica local `update-policy.json` registra que a versao MOD nao deve buscar nem instalar atualizacoes da nuvem.
- O original permanece intacto.

Risco residual:

- Outros modulos podem chamar atualizacao ou sincronizacao sem passar por `LiveUpdate.exe`.
- Proxima fase deve executar captura dinamica em ambiente MOD com monitoramento de processo e rede.

## Autenticacao, licenca e senhas

Sinais encontrados:

- Modulo `Senhas.exe`.
- Tabelas legadas de usuarios e niveis ja identificadas em fase anterior.
- Hipotese forte de senha legada por deslocamento ASCII `+1`: texto `12345` gravado como `23456`.

Status no MOD:

- Autenticacao propria ja criada.
- Usuario `gabriela` definido como administradora principal.
- Senha no MOD definida como `12345`.
- Troca obrigatoria desativada conforme solicitacao administrativa.

Decisao tecnica:

- O mecanismo antigo deve ser mantido apenas como referencia de migracao.
- A nova autenticacao deve ser propria, auditavel e independente.

## Hardware binding

Sinais procurados:

- `MacAddress`
- `MachineName`
- `ComputerName`
- serial
- hardware
- maquina/estacao
- registro/ativacao

Interpretacao:

- Existem indicios suficientes para tratar hardware binding como area de risco.
- Ainda nao ha confirmacao completa do algoritmo de ativacao ou validacao por hardware.
- A proxima etapa deve separar evidencias por arquivo legivel, sem depender de varredura binaria ruidosa.

Diretriz para nova versao:

- Implementar device binding moderno somente no MOD.
- Usar multiplos identificadores, tolerancia a troca parcial de hardware e politica administrativa de revogacao/recuperacao.
- Futuramente integrar com Supabase para controle centralizado, mantendo fallback local.

## Memoria, processos e carregamento

Sinais encontrados:

- Uso amplo de APIs nativas do Windows pela pilha Delphi/Borland.
- Presenca de imports relacionados a processo, thread, DLL, interface grafica e impressao.

Interpretacao:

- A arquitetura carrega muitos modulos nativos separados.
- Ainda nao foi comprovado vazamento de memoria.
- Medicao real deve ser dinamica: abrir tela por tela no MOD, registrar uso de RAM/handles/threads e comparar antes/depois.

Diretriz para modernizacao:

- Preservar leveza.
- Evitar carregar todos os modulos na inicializacao.
- Usar carregamento sob demanda para telas, relatorios e integracoes fiscais.

## Temporarios, locks e BDE

Sinais encontrados:

- `PDOXUSRS`
- arquivos `.LCK`
- `_QS`
- rotinas de limpeza em scripts/termos legados

Interpretacao:

- O sistema depende de controle de locks do BDE/Paradox.
- Falhas de encerramento, rede instavel ou limpeza inadequada podem causar travas, corrupcao de indice ou inconsistencia operacional.

Diretriz:

- A base original deve permanecer preservada ate validacao completa.
- O MOD deve operar sobre copia/homologacao.
- Qualquer migracao deve ter backup, validacao de indices e rollback.

## Riscos principais

- Dependencia forte de binarios 32-bit.
- BDE/Paradox como ponto fragil operacional.
- Atualizacao automatica antiga, agora bloqueada apenas no MOD.
- Comunicacoes externas dispersas em varios modulos.
- Componentes fiscais e drivers legados.
- OpenSSL legado.
- Possivel hardware binding/licenciamento nao documentado.
- Extracao de layout/telas ainda parcial e ruidosa.

## Proximas acoes recomendadas

1. Refinar a extracao de layouts Delphi por executavel, priorizando `LavSoft.exe`, `LavFacilLan.exe`, `Gerenciador.exe`, `Financeiro.exe`, `Estoque.exe`, `NFE.exe` e `SAT.exe`.
2. Criar mapa tela/menu/relatorio com status por tela: identificado, parcialmente identificado, exige captura dinamica.
3. Rodar captura dinamica no runtime MOD para cada tela principal, sem acessar o original.
4. Monitorar processos, filhos, rede e memoria durante abertura/login/navegacao.
5. Extrair endpoints/hosts de forma redigida e classificar como fiscal, email, update, sincronizacao, telemetria ou desconhecido.
6. Analisar `Gerenciador.exe` como componente .NET.
7. Formalizar matriz de permissoes por modulo com base em `Nivel.DB` e telas confirmadas.

## Rollback

Como esta etapa nao alterou o original, o rollback operacional consiste em:

1. Ignorar os artefatos desta etapa.
2. Remover apenas arquivos gerados em `D:\AtelieProd\MOD\docs\02-arquitetura-legada\pe-imports` e `D:\AtelieProd\MOD\docs\02-arquitetura-legada\sinais-profundos`, se administrativamente desejado.
3. Manter `D:\AtelieProd\Equipexe` intacto.
