# Bloqueio de Atualizacao Automatica - MOD

## Objetivo

Impedir que a versao MOD busque, baixe ou instale atualizacoes da nuvem ao abrir o sistema.

## Regra

O sistema original em `D:\AtelieProd\Equipexe` nao foi alterado.

O bloqueio foi aplicado somente em:

`D:\AtelieProd\MOD`

## Pontos identificados

- `D:\AtelieProd\Equipexe\Exe\LiveUpdate.exe`
- `D:\AtelieProd\Equipexe\Exe\Gerenciador.exe`
- `D:\AtelieProd\Equipexe\Exe\Sincronizar.exe`
- `D:\AtelieProd\Equipexe\Sincroniza\Nuvem`
- metodos internos encontrados em strings: `VerificaAtualizacoes`, `TesteVerificaAtualizacoes`, `DownloadDados`, `DownloadFromSource`, `RegistraEstacao`

## Implementacao no MOD

Foi criada politica:

`D:\AtelieProd\MOD\config\env\update-policy.json`

Foi criado executavel substituto:

`D:\AtelieProd\MOD\apps\services\LiveUpdate.Disabled`

Foi preparado runtime de homologacao:

`D:\AtelieProd\MOD\apps\legacy-runtime\Equipexe`

Nesse runtime, `Exe\LiveUpdate.exe` nao e o atualizador original. Ele e um bloqueador local que:

- nao acessa rede;
- nao baixa arquivos;
- nao instala nada;
- registra log de bloqueio;
- retorna sucesso para evitar erro de abertura.

## Validacao

Comando:

```powershell
D:\AtelieProd\MOD\apps\tools\verify-update-block.ps1
```

Resultado validado em `2026-05-23`:

- `Verified = true`
- log gerado em `D:\AtelieProd\MOD\logs\communication\liveupdate-blocked-20260523.jsonl`

## Abertura segura da versao MOD

Usar:

```powershell
D:\AtelieProd\MOD\apps\tools\start-mod-lavsoft-no-update.ps1
```

Nao abrir o executavel original em `D:\AtelieProd\Equipexe` se a intencao for testar a versao sem atualizacao.

## Rollback

Como o original nao foi alterado, o rollback consiste em remover ou ignorar:

- `D:\AtelieProd\MOD\apps\legacy-runtime`
- `D:\AtelieProd\MOD\apps\services\LiveUpdate.Disabled`
- `D:\AtelieProd\MOD\config\env\update-policy.json`
