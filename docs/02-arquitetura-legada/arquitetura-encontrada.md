# Arquitetura Encontrada

## Classificacao inicial

O sistema atual aparenta ser uma aplicacao desktop Windows legada, com forte dependencia de componentes Borland/Delphi e BDE/Paradox.

## Evidencias

- DLLs Borland/BDE: `borlndmm.dll`, `midas.dll`, `dbd*.dll`, `Tutil32.dll`, `qtintf*.dll`.
- Banco local Paradox/BDE: arquivos `.DB`, `.PX`, `.XG*`, `.YG*`, `.MB`.
- Modulos separados por executaveis: financeiro, estoque, fiscal, SAT/NFE, senhas, parametros, backup, sincronizacao e atualizacao.
- Integracoes de hardware/fiscal: Bematech, Daruma, Argox, Nitgen, componentes NFSe.
- Rotina de limpeza de locks em `executalav.bat`, incluindo `P*.LCK`, `_QS*.*` e `PDOXUSRS.NET`.

## Riscos iniciais

- Tecnologia de dados local antiga e sensivel a locks/corrupcao.
- Dependencia de componentes nativos antigos de 32 bits.
- OpenSSL legado presente em DLLs `libeay32.dll` e `ssleay32.dll`.
- Possivel dependencia fiscal/hardware que precisa ser validada em ambiente controlado.
- Ausencia de codigo-fonte no escopo analisado ate o momento.

## Decisao tecnica inicial

Como nao foi localizado codigo-fonte Delphi nesta fase, a nova base foi iniciada em `.NET 8`, mantendo foco em Windows corporativo, operacao local controlada, autenticacao propria e possibilidade futura de API, banco moderno e integracao Supabase.
