# Hipotese Tecnica do "Segredo" de Senhas Legado

## Importante

Esta analise foi feita somente na copia readonly em `MOD`. Nenhuma senha ou permissao do sistema original foi alterada.

## Tabelas analisadas

- `Ger\Dados\Usuarios.DB`
- `Ger\Dados\Senhas.DB`
- `Ger\Dados\UsuaSis.DB`
- `Ger\Dados\UsuaFil.DB`
- `Ger\Dados\UsuaGru.DB`
- `Ger\Dados\UsuaGruInt.DB`
- `Ger\Dados\GruUsuarios.DB`
- `Ger\Dados\Nivel.DB`

## Achados

### Usuarios

`Usuarios.DB` contem:

- `CodUsuario`
- `NomUsuario`
- `GruUsuario`
- `TipUsuario`
- `Senha`
- campos de cancelamento/auditoria

Na copia analisada, existe usuario `GABRIELA` com grupo `MASTE`.

### Sistemas liberados

`UsuaSis.DB` liga usuarios a sistemas como:

- `LAVSOFT`
- `ESTOQUE`
- `FINANCEIRO`
- `SAT`
- `SENHAS`
- `PCP`
- `PECLAV`
- `TRIAGEM`

### Permissoes granulares

`Nivel.DB` contem 438 registros, todos vinculados a `LAVSOFT` na copia analisada. A tabela possui:

- `CodUsuario`
- `CodFil`
- `CodSistema`
- `Rotina`
- `Op`
- `NivelI`
- `NivelA`
- `NivelE`
- `NivelT`

Hipotese: os campos `NivelI`, `NivelA`, `NivelE` e `NivelT` representam permissoes por acao, provavelmente inclusao, alteracao, exclusao e/ou total/tecnico, dependendo da rotina.

## Hipotese do segredo

Alterar somente o campo `Senha` em `Usuarios.DB` provavelmente nao basta para liberar acesso completo. O acesso depende tambem de:

1. usuario existir em `Usuarios.DB`;
2. usuario estar ativo;
3. usuario estar vinculado ao sistema em `UsuaSis.DB`;
4. grupo/tipo do usuario;
5. permissao por rotina em `Nivel.DB`;
6. possivelmente chamadas ao executavel `Senhas.exe` feitas por `LavSoft.exe` e `LavFacilLan.exe`.

Esse conjunto e o provavel "segredo" lembrado: senha, sistema, rotina e nivel precisam estar coerentes.

## Codificacao do campo Senha

Analise posterior do campo `Usuarios.Senha` indica uma hipotese forte de codificacao por deslocamento ASCII `+1`.

Exemplo tecnico: para senha digitada `12345`, o valor armazenado esperado seria `23456`.

Essa conclusao esta documentada em `analise-codificacao-senha-legado.md`.

## Arquivos gerados

- `amostra-legado-Usuarios.csv`
- `amostra-legado-UsuaSis.csv`
- `amostra-legado-Nivel.csv`
- `mapa-permissoes-legado-nivel.csv`

As senhas foram mascaradas nos CSVs gerados.
