# Arquitetura GitHub Releases NextGen

O GitHub passa a ser a central oficial de atualizacoes e releases do NextGen:

- fonte oficial de versionamento;
- canal de distribuicao Windows;
- canal futuro de distribuicao Linux appliance;
- controle de changelog;
- checksums SHA256;
- assinatura de assets;
- manifestos de atualizacao;
- rollback por versao anterior.

Arquivos operacionais versionados:

- `.github/workflows/nextgen-ci.yml`
- `.github/workflows/nextgen-release.yml`
- `apps/updater/release_updater.py`
- `apps/tools/generate_release_manifest.py`
- `apps/tools/release_gate.py`
- `release/update-manifest.json`
- `release/latest.json`
- `release/checksums.txt`
- `release/changelog.md`
- `release/release-notes.md`

O diretório externo `D:\AtelieProd\Atelie_Windows` e `D:\AtelieProd\Atelie_Linux` recebe wrappers operacionais, enquanto o core versionado permanece dentro do `MOD`.
