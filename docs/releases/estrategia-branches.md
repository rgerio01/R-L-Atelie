# Estrategia de branches NextGen

- `main`: producao.
- `develop`: desenvolvimento integrado.
- `homolog`: homologacao operacional controlada.
- `release/*`: preparacao de versao.
- `hotfix/*`: correcoes urgentes.

Tags semanticas: `v1.0.0`, `v1.0.1`, `v1.1.0`.

Nenhuma branch deve receber credenciais em arquivo. Segredos ficam apenas em GitHub Secrets ou runtime seguro.
