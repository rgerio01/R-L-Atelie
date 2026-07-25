from __future__ import annotations

import argparse
import hashlib
import json
import os
import shutil
import subprocess
import sys
import tarfile
import tempfile
import urllib.error
import urllib.request
import zipfile
from datetime import datetime, timezone
from pathlib import Path


DEFAULT_REPO_API = "https://api.github.com/repos/rgerio01/R-L-Atelie/releases"
CRITICAL_LOCKS = [
    "sale.lock",
    "print.lock",
    "payment.lock",
    "sync-critical.lock",
    "migration.lock",
]


def now() -> str:
    return datetime.now(timezone.utc).isoformat()


def log(log_path: Path, event: str, **data: object) -> None:
    log_path.parent.mkdir(parents=True, exist_ok=True)
    payload = {"timestamp": now(), "event": event, **data}
    with log_path.open("a", encoding="utf-8") as handle:
        handle.write(json.dumps(payload, ensure_ascii=False) + "\n")


def sha256_file(path: Path) -> str:
    digest = hashlib.sha256()
    with path.open("rb") as handle:
        for chunk in iter(lambda: handle.read(1024 * 1024), b""):
            digest.update(chunk)
    return digest.hexdigest()


def _token() -> str:
    return os.environ.get("ATELIE_UPDATE_TOKEN", "").strip()


def _api_headers() -> dict[str, str]:
    headers = {"User-Agent": "AtelieNextGenUpdater/0.1", "Accept": "application/vnd.github+json"}
    if _token():
        headers["Authorization"] = f"Bearer {_token()}"
    return headers


def _asset_headers() -> dict[str, str]:
    headers = {"User-Agent": "AtelieNextGenUpdater/0.1", "Accept": "application/octet-stream"}
    if _token():
        headers["Authorization"] = f"Bearer {_token()}"
    return headers


def _http_get_json(url: str, headers: dict[str, str]) -> object:
    req = urllib.request.Request(url, headers=headers)
    with urllib.request.urlopen(req, timeout=30) as response:
        return json.loads(response.read().decode("utf-8"))


def _http_download(url: str, headers: dict[str, str], dest: Path) -> None:
    try:
        req = urllib.request.Request(url, headers=headers)
        with urllib.request.urlopen(req, timeout=120) as response, dest.open("wb") as handle:
            shutil.copyfileobj(response, handle)
    except urllib.error.HTTPError as exc:
        # Repositorio publico + token expirado/revogado: tenta de novo sem
        # autenticacao antes de desistir (mesma logica de fetch_private_release).
        if exc.code in (401, 403) and "Authorization" in headers:
            fallback = {k: v for k, v in headers.items() if k != "Authorization"}
            req = urllib.request.Request(url, headers=fallback)
            with urllib.request.urlopen(req, timeout=120) as response, dest.open("wb") as handle:
                shutil.copyfileobj(response, handle)
        else:
            raise


def read_json(path_or_url: str) -> dict:
    """Lê um manifesto de um caminho local ou de uma URL pública direta (sem
    autenticação) — usado só no modo --manifest (repositório público ou arquivo
    local de teste). Para repositório privado, use --github-repo em vez disso."""
    if path_or_url.startswith(("http://", "https://")):
        req = urllib.request.Request(path_or_url, headers={"User-Agent": "AtelieNextGenUpdater/0.1"})
        with urllib.request.urlopen(req, timeout=30) as response:
            return json.loads(response.read().decode("utf-8"))
    return json.loads(Path(path_or_url).read_text(encoding="utf-8"))


def download(url: str, dest: Path) -> None:
    """Baixa um asset de uma URL pública direta, sem autenticação (repositório
    público). Para repositório privado, use fetch_private_release()."""
    _http_download(url, {"User-Agent": "AtelieNextGenUpdater/0.1"}, dest)


class ManifestNotFound(Exception):
    pass


def fetch_private_release(repo: str, channel: str) -> tuple[dict, dict]:
    """Repositório privado no GitHub exige autenticação até pra baixar um asset de
    release — a URL de conveniência (.../releases/latest/download/arquivo) só
    funciona sem token em repositório público, então pra privado usamos a API
    do GitHub em duas etapas: 1) lista releases e acha o manifesto de cada uma
    até achar uma do canal certo; 2) os asset dela ficam disponíveis pra
    download autenticado via a própria API (não pela URL de conveniência).
    Token de leitura (só desse repositório) injetado via variável de ambiente
    ATELIE_UPDATE_TOKEN — nunca fica no código nem em texto puro no disco.
    """
    try:
        releases = _http_get_json(f"https://api.github.com/repos/{repo}/releases", _api_headers())
    except urllib.error.HTTPError as exc:
        # Repositorio publico + token expirado/revogado: em vez de travar a
        # checagem inteira, tenta de novo sem autenticacao antes de desistir.
        if exc.code in (401, 403) and _token():
            releases = _http_get_json(f"https://api.github.com/repos/{repo}/releases", {
                "User-Agent": "AtelieNextGenUpdater/0.1", "Accept": "application/vnd.github+json"
            })
        else:
            raise
    if not isinstance(releases, list):
        raise ManifestNotFound(f"Resposta inesperada da API para {repo}")

    for rel in releases:
        assets = {a["name"]: a for a in rel.get("assets", [])}
        manifest_asset = assets.get("update-manifest.json")
        if not manifest_asset:
            continue
        with tempfile.TemporaryDirectory(prefix="atelie-manifest-") as tmp:
            tmp_path = Path(tmp) / "update-manifest.json"
            _http_download(manifest_asset["url"], _asset_headers(), tmp_path)
            manifest = json.loads(tmp_path.read_text(encoding="utf-8"))
        if manifest.get("channel") == channel:
            return manifest, assets

    raise ManifestNotFound(f"Nenhuma release encontrada no canal '{channel}' em {repo}")


def verify_signature(asset: Path, signature_path: Path | None, public_key_path: Path | None) -> tuple[bool, str]:
    if not signature_path and not public_key_path:
        return True, "signature_not_configured"
    if not signature_path or not signature_path.exists():
        return False, "signature_file_missing"
    if not public_key_path or not public_key_path.exists():
        return False, "public_key_missing"

    # Prefer minisign for appliance/Windows portable signing. Keep this optional so
    # homologation can run checksum-only while release signing keys are provisioned.
    minisign = shutil.which("minisign")
    if minisign:
        proc = subprocess.run(
            [minisign, "-Vm", str(asset), "-x", str(signature_path), "-p", str(public_key_path)],
            text=True,
            stdout=subprocess.PIPE,
            stderr=subprocess.STDOUT,
            check=False,
        )
        return proc.returncode == 0, "minisign_ok" if proc.returncode == 0 else "minisign_failed"
    return False, "signature_tool_missing"


def version_tuple(value: str) -> tuple[int, int, int]:
    clean = value.strip().lstrip("v")
    parts = clean.split(".")
    if len(parts) != 3:
        raise ValueError(f"Invalid semantic version: {value}")
    return tuple(int(part) for part in parts)  # type: ignore[return-value]


def critical_operation_active(lock_dir: Path) -> list[str]:
    return [name for name in CRITICAL_LOCKS if (lock_dir / name).exists()]


def extract_asset(asset: Path, staging: Path) -> None:
    if asset.suffix.lower() == ".zip":
        with zipfile.ZipFile(asset) as archive:
            archive.extractall(staging)
        return
    if asset.name.endswith((".tar.gz", ".tgz")):
        with tarfile.open(asset, "r:gz") as archive:
            archive.extractall(staging)
        return
    staging.mkdir(parents=True, exist_ok=True)
    shutil.copy2(asset, staging / asset.name)


def copy_tree(src: Path, dst: Path) -> None:
    dst.mkdir(parents=True, exist_ok=True)
    for item in src.iterdir():
        target = dst / item.name
        if item.is_dir():
            if target.exists():
                shutil.rmtree(target)
            shutil.copytree(item, target)
        else:
            shutil.copy2(item, target)


def install(
    manifest: dict,
    app_dir: Path,
    state_dir: Path,
    public_key: Path | None,
    assets: dict | None = None,
) -> int:
    log_path = state_dir / "logs" / "update.log"
    lock_dir = state_dir / "locks"
    active = critical_operation_active(lock_dir)
    if active:
        log(log_path, "blocked_critical_operation", locks=active)
        return 20

    required = ["version", "channel", "platform", "asset_name", "sha256"]
    if not assets:
        required.append("release_url")
    missing = [key for key in required if not manifest.get(key)]
    if missing:
        log(log_path, "manifest_invalid", missing=missing)
        return 21

    state_dir.mkdir(parents=True, exist_ok=True)
    with tempfile.TemporaryDirectory(prefix="atelie-update-") as tmp:
        tmp_dir = Path(tmp)
        asset = tmp_dir / manifest["asset_name"]
        if assets is not None:
            # Repositório privado: baixa pela API (com autenticação), não pela
            # URL de conveniência — essa nem existe pra asset de repo privado.
            asset_meta = assets.get(manifest["asset_name"])
            if not asset_meta:
                log(log_path, "asset_not_found_in_release", asset=manifest["asset_name"])
                return 24
            _http_download(asset_meta["url"], _asset_headers(), asset)
        else:
            download(manifest["release_url"], asset)
        digest = sha256_file(asset)
        if digest.lower() != str(manifest["sha256"]).lower():
            log(log_path, "checksum_failed", expected="sha256_present", actual=digest)
            return 22

        signature_file = None
        if manifest.get("signature_url") or (assets and manifest.get("signature")):
            signature_file = tmp_dir / f"{manifest['asset_name']}.sig"
            if assets is not None:
                sig_meta = assets.get(manifest.get("signature", ""))
                if sig_meta:
                    _http_download(sig_meta["url"], _asset_headers(), signature_file)
                else:
                    signature_file = None
            else:
                download(manifest["signature_url"], signature_file)

        signature_ok, signature_status = verify_signature(asset, signature_file, public_key)
        if not signature_ok:
            log(log_path, "signature_failed", status=signature_status)
            return 23

        backup_dir = state_dir / "rollback" / "previous"
        staging = tmp_dir / "staging"
        if backup_dir.exists():
            shutil.rmtree(backup_dir)
        if app_dir.exists():
            shutil.copytree(app_dir, backup_dir)
        staging.mkdir(parents=True, exist_ok=True)
        extract_asset(asset, staging)
        copy_tree(staging, app_dir)

        (state_dir / "current-version.json").write_text(
            json.dumps(
                {
                    "version": manifest["version"],
                    "channel": manifest["channel"],
                    "platform": manifest["platform"],
                    "installed_at": now(),
                    "rollback_supported": bool(manifest.get("rollback_supported", True)),
                },
                indent=2,
            ),
            encoding="utf-8",
        )
        log(log_path, "installed", version=manifest["version"], channel=manifest["channel"])
    return 0


def rollback(app_dir: Path, state_dir: Path) -> int:
    log_path = state_dir / "logs" / "update.log"
    backup_dir = state_dir / "rollback" / "previous"
    if not backup_dir.exists():
        log(log_path, "rollback_missing")
        return 30
    if app_dir.exists():
        shutil.rmtree(app_dir)
    shutil.copytree(backup_dir, app_dir)
    log(log_path, "rollback_applied")
    return 0


def check(manifest: dict, current_version: str, platform: str, channel: str) -> int:
    if manifest.get("platform") != platform:
        print(json.dumps({"update_available": False, "reason": "platform_mismatch"}))
        return 0
    if manifest.get("channel") != channel:
        print(json.dumps({"update_available": False, "reason": "channel_mismatch"}))
        return 0
    available = version_tuple(manifest["version"]) > version_tuple(current_version)
    print(json.dumps({"update_available": available, "version": manifest["version"]}))
    return 0


def main() -> int:
    parser = argparse.ArgumentParser(description="Atelie NextGen secure updater")
    sub = parser.add_subparsers(dest="cmd", required=True)

    p_check = sub.add_parser("check")
    p_check.add_argument("--manifest", help="URL pública ou arquivo local (repositório público / teste)")
    p_check.add_argument("--github-repo", help="owner/repo — usa a API autenticada (repositório privado)")
    p_check.add_argument("--current-version", required=True)
    p_check.add_argument("--platform", required=True)
    p_check.add_argument("--channel", required=True)

    p_install = sub.add_parser("install")
    p_install.add_argument("--manifest", help="URL pública ou arquivo local (repositório público / teste)")
    p_install.add_argument("--github-repo", help="owner/repo — usa a API autenticada (repositório privado)")
    p_install.add_argument("--channel", default="appliance")
    p_install.add_argument("--app-dir", required=True)
    p_install.add_argument("--state-dir", required=True)
    p_install.add_argument("--public-key", default=os.environ.get("ATELIE_UPDATE_PUBLIC_KEY"))

    p_rollback = sub.add_parser("rollback")
    p_rollback.add_argument("--app-dir", required=True)
    p_rollback.add_argument("--state-dir", required=True)

    args = parser.parse_args()

    if args.cmd in ("check", "install") and not args.manifest and not args.github_repo:
        parser.error("informe --manifest ou --github-repo")

    if args.cmd == "check":
        if args.github_repo:
            manifest, _assets = fetch_private_release(args.github_repo, args.channel)
        else:
            manifest = read_json(args.manifest)
        return check(manifest, args.current_version, args.platform, args.channel)

    if args.cmd == "install":
        public_key = Path(args.public_key) if args.public_key else None
        if args.github_repo:
            manifest, assets = fetch_private_release(args.github_repo, args.channel)
        else:
            manifest, assets = read_json(args.manifest), None
        return install(manifest, Path(args.app_dir), Path(args.state_dir), public_key, assets)

    if args.cmd == "rollback":
        return rollback(Path(args.app_dir), Path(args.state_dir))
    return 1


if __name__ == "__main__":
    raise SystemExit(main())
