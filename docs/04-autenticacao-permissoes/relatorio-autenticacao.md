# Relatorio de Autenticacao e Permissoes

## Legado

Foram encontrados arquivos e referencias associados a senhas, usuarios, licenca, registro, bloqueio e ativacao. A analise bruta foi movida para `logs\analysis\restricted\achados-autenticacao-licenca.txt` e deve ser tratada como sensivel.

## Nova autenticacao MOD

Foi criada API propria de homologacao com:

- Hash de senha PBKDF2-SHA256.
- Salt individual por usuario.
- Token local assinado com HMAC-SHA256.
- Expiracao de token em 8 horas.
- Auditoria JSONL em `logs\audit`.
- Usuario `gabriela` como administradora principal.

## Perfis iniciais

- `administrador`: acesso total.
- `operacional`: inventario legado e relatorios de leitura.
- `supervisor`: usuarios leitura, inventario e relatorios.
- `leitura`: leitura operacional.
- `auditoria`: auditoria e inventario.

## Permissoes iniciais

- `admin.read`
- `users.read`
- `users.write`
- `auth.audit.read`
- `legacy.inventory.read`
- `migration.plan.read`
- `reports.read`
- `reports.write`

## Pendencias

- Criar endpoint de troca de senha.
- Criar bloqueio por tentativas.
- Criar politica de senha.
- Migrar usuarios reais somente apos saneamento e autorizacao.
