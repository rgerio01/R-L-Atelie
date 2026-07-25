# Relatorio Inicial de Comunicacoes

## Achados

Foram encontradas referencias a:

- Rotina de sincronizacao em `Sincroniza\Nuvem`.
- Modulos `Sincronizar`, `LiveUpdate`, `EnvEmailEquipe`, `EqEmail`, `EquMail`, `CEP`, `NFE`, `SAT`.
- Componentes antigos de internet/SMTP/FTP/HTTP registrados em arquivos de licenca e bibliotecas.
- Configuracoes de portas seriais e impressoras/fiscal.

A analise bruta foi movida para `logs\analysis\restricted\achados-urls-hosts.txt` por conter possiveis dados operacionais sensiveis.

## Classificacao inicial

- Essencial: comunicacoes fiscais, SAT/NFE/NFSe, se usadas pela operacao atual.
- Opcional: email, CEP web, sincronizacao em nuvem, atualizador automatico.
- Obsoleta/insegura: componentes antigos de rede, OpenSSL legado e atualizador sem validacao documentada.

## Regra para a nova versao

Toda comunicacao externa devera ser configuravel, auditada e desligavel. A operacao local deve ser priorizada sempre que legal e tecnicamente viavel.
