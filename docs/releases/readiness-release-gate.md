# Readiness release gate

Nenhuma release operacional pode ser publicada sem:

- testes OK;
- scanner de segredos OK;
- readiness minimo;
- build OK;
- checksums;
- assinatura;
- manifest;
- changelog;
- rollback preparado.

Status atual: `NO-GO` para publicacao operacional. O pipeline de release bloqueia quando o gate falha.
