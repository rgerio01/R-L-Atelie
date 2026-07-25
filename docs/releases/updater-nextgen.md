# Updater NextGen

Core versionado: `apps/updater/release_updater.py`.

Fluxo:

1. consultar manifesto;
2. comparar versao local;
3. validar canal e plataforma;
4. bloquear se houver operacao critica;
5. baixar asset;
6. validar SHA256;
7. validar assinatura quando configurada;
8. backup da versao atual;
9. aplicar em staging;
10. rollback se falhar.
