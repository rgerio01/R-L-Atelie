from __future__ import annotations

import argparse
import hashlib
import json
from datetime import date
from pathlib import Path


def sha256_file(path: Path) -> str:
    digest = hashlib.sha256()
    with path.open("rb") as handle:
        for chunk in iter(lambda: handle.read(1024 * 1024), b""):
            digest.update(chunk)
    return digest.hexdigest()


def main() -> int:
    parser = argparse.ArgumentParser(description="Generate NextGen release manifest and checksums.")
    parser.add_argument("--version", required=True)
    parser.add_argument("--channel", required=True, choices=["stable", "beta", "homolog", "appliance"])
    parser.add_argument("--platform", required=True, choices=["windows", "linux", "appliance"])
    parser.add_argument("--asset", required=True)
    parser.add_argument("--release-url", required=True)
    parser.add_argument("--out-dir", default="release")
    parser.add_argument("--minimum-supported-version", default="v0.1.0")
    parser.add_argument("--migration-required", action="store_true")
    args = parser.parse_args()

    out_dir = Path(args.out_dir)
    asset = Path(args.asset)
    out_dir.mkdir(parents=True, exist_ok=True)
    digest = sha256_file(asset)

    # Só declara assinatura se o .sig realmente existir ao lado do asset (esta
    # função deve rodar DEPOIS do passo de assinatura no workflow). Declarar um
    # signature_url que não existe faz o updater tentar baixar um arquivo 404 e
    # falhar a instalação inteira mesmo sem chave de assinatura configurada.
    sig_path = asset.with_name(asset.name + ".sig")
    signed = sig_path.exists()

    manifest = {
        "version": args.version,
        "channel": args.channel,
        "platform": args.platform,
        "release_url": args.release_url,
        "asset_name": asset.name,
        "sha256": digest,
        "signature": f"{asset.name}.sig" if signed else None,
        "signature_url": f"{args.release_url}.sig" if signed else None,
        "minimum_supported_version": args.minimum_supported_version,
        "migration_required": bool(args.migration_required),
        "rollback_supported": True,
        "release_date": date.today().isoformat(),
    }
    (out_dir / "update-manifest.json").write_text(json.dumps(manifest, indent=2), encoding="utf-8")
    (out_dir / "latest.json").write_text(json.dumps(manifest, indent=2), encoding="utf-8")
    (out_dir / "checksums.txt").write_text(f"{digest}  {asset.name}\n", encoding="utf-8")
    if not (out_dir / "changelog.md").exists():
        (out_dir / "changelog.md").write_text(f"# Changelog\n\n## {args.version}\n\n- Release gerado pelo pipeline.\n", encoding="utf-8")
    if not (out_dir / "release-notes.md").exists():
        (out_dir / "release-notes.md").write_text(
            f"# {args.version}\n\nCanal: {args.channel}\n\nPlataforma: {args.platform}\n",
            encoding="utf-8",
        )
    print(json.dumps(manifest, indent=2))
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
