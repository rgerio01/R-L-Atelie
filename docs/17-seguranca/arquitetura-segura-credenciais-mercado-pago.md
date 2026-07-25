# Arquitetura Segura de Credenciais Mercado Pago

Data: 2026-05-23

## Regra central

Credenciais sensiveis nao devem ficar no executavel desktop, em arquivo `.ini`, `.json` local simples, banco local sem criptografia ou qualquer artefato distribuido ao cliente.

## Segredos

- `Access Token`
- `Client Secret`
- chaves de webhook
- chaves de assinatura de entitlement/licenca
- tokens OAuth de contas Rogerio/Luci

## Armazenamento recomendado

Ambiente local/homologacao:

- variaveis de ambiente protegidas;
- Windows Credential Manager/DPAPI;
- arquivo criptografado com chave fora do repositorio.

Ambiente cloud:

- secret manager do provedor;
- rotacao controlada;
- segregacao por ambiente: teste, homologacao, producao.

## Fluxo seguro

1. Desktop autentica usuario no backend.
2. Backend valida permissao e tenant.
3. Backend usa credencial da conta correta sem expor token ao desktop.
4. Backend chama Mercado Pago.
5. Desktop recebe somente dados operacionais: QR Code, status, terminal, identificador publico.
6. Webhook confirma eventos e assina payload interno.

## Controles

- TLS obrigatorio.
- Idempotencia.
- Rate limit.
- Registro de auditoria.
- Mascaramento de logs.
- Rotacao de credenciais.
- Separacao de credenciais Rogerio/Luci.
- Escopo minimo por conta.

## Proibido

- colocar `Access Token` no executavel;
- salvar token em `config.ini`;
- exibir token em logs;
- usar token de Rogerio para venda de Luci;
- usar token de Luci para licencas;
- misturar contas sem trilha contabil.

## Evidencia oficial

A documentacao oficial do Mercado Pago define o `Access Token` como chave privada da aplicacao, a ser usada no backend, e recomenda envio via header de autorizacao.
