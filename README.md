# EquipeExe MOD

Ambiente de homologacao e modernizacao do sistema EquipeExe.

## Estado atual

- Sistema original preservado em `D:\AtelieProd\Equipexe`.
- Nova estrutura criada em `D:\AtelieProd\MOD`.
- Backup completo do original criado em `D:\AtelieProd\MOD\backups\original\Equipexe-original-20260523-111141.zip`.
- SHA-256 do backup: `26EFD94C7C18A12E0977C46B339006AA03ABE778EFE72AA93A7360991B89CC43`.
- API MOD inicial criada em `.NET 8` em `apps\backend\EquipeExe.Mod.Api`.
- API de homologacao em execucao local: `http://127.0.0.1:5058`.

## Acesso inicial da homologacao

- Usuario: `gabriela`
- Perfil: `administrador`
- Senha temporaria: `Trocar@PrimeiroAcesso2026!`
- Obrigatorio trocar a senha antes de qualquer uso operacional real.

## Regras de preservacao

- Nao alterar `D:\AtelieProd\Equipexe` sem backup e aprovacao.
- Nao usar a API MOD contra dados reais antes de migracao validada.
- Manter todo teste em `D:\AtelieProd\MOD\data\sandbox`.
- Registrar evidencias em `D:\AtelieProd\MOD\logs`.

## Execucao da API

```powershell
D:\AtelieProd\MOD\apps\tools\start-api.ps1
```

## Parada da API

```powershell
D:\AtelieProd\MOD\apps\tools\stop-api.ps1
```

## Endpoints iniciais

- `GET /health`
- `POST /auth/login`
- `GET /auth/me`
- `GET /admin/permissions`
- `GET /admin/users`
- `POST /admin/users`
- `POST /admin/users/{id}/roles`
- `GET /legacy/inventory`

## Bloqueio de atualizacao automatica

A versao MOD possui bloqueio local de `LiveUpdate.exe`.

Preparar/validar:

```powershell
D:\AtelieProd\MOD\apps\tools\prepare-mod-runtime-no-update.ps1
D:\AtelieProd\MOD\apps\tools\verify-update-block.ps1
```

Abrir LavSoft MOD sem atualizacao automatica:

```powershell
D:\AtelieProd\MOD\apps\tools\start-mod-lavsoft-no-update.ps1
```
