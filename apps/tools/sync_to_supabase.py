#!/usr/bin/env python3
"""Sincroniza o SQLite local (pdv.db) + auth-store.json para o Supabase.

Backup em um sentido so: SQLite continua sendo a fonte da verdade do
PDV (funciona 100% offline); isto so espelha os dados para a nuvem a
cada execucao (agendado a cada 3h no appliance via
atelie-supabase-sync.timer). Nunca deve travar o programa: qualquer
falha de rede/conexao e' capturada, logada, e o processo termina com
exit 0 mesmo assim.

Uso:
  python sync_to_supabase.py --data-dir /caminho/para/data/sandbox

Credenciais vem so de variavel de ambiente (SUPABASE_DB_URL) -- nunca
hardcoded, nunca logada. Se SUPABASE_DB_URL nao estiver no ambiente e
existir um .env no diretorio atual (ou --env-file), carrega de la
(uso local/dev; no appliance quem populao ambiente e' o
EnvironmentFile do systemd).
"""

from __future__ import annotations

import argparse
import json
import logging
import os
import sqlite3
import sys
from pathlib import Path

try:
    import psycopg2
    import psycopg2.extras
except ImportError:
    print("psycopg2 nao instalado. Rode: pip install psycopg2-binary", file=sys.stderr)
    sys.exit(1)


# Tabelas espelhadas do SQLite -> Postgres. Nomes de coluna sao os
# mesmos dos dois lados (ver Program.cs:1041-1400 e
# supabase/migrations/20260725182752_mirror_from_sqlite.sql). Colunas
# aqui excluem 'synced_at' (preenchida com now() pelo proprio upsert).
TABLES: dict[str, dict] = {
    "configuracoes": {
        "pk": ["chave"],
        "cols": ["chave", "valor", "updated_at"],
        "bool_cols": [],
    },
    "clientes": {
        "pk": ["id"],
        "cols": ["id", "nome", "documento", "telefone", "celular", "email", "logradouro",
                 "numero", "complemento", "bairro", "cidade", "estado", "cep", "observacoes",
                 "limite_credito", "desconto_percent", "ativo", "created_at", "updated_at",
                 "created_by", "legacy_codigo", "data_nascimento", "cartao_fidelidade", "contato",
                 "telefone3", "sexo", "grupo_cliente", "vendedor_codigo", "limite_faturamento"],
        "bool_cols": ["ativo"],
    },
    "clientes_historico": {
        "pk": ["id"],
        "cols": ["id", "cliente_id", "evento", "detalhe", "usuario", "created_at"],
        "bool_cols": [],
    },
    "clientes_credito": {
        "pk": ["cliente_id"],
        "cols": ["cliente_id", "saldo", "updated_at"],
        "bool_cols": [],
    },
    "clientes_credito_movimentos": {
        "pk": ["id"],
        "cols": ["id", "cliente_id", "tipo", "valor", "descricao", "referencia", "usuario",
                 "created_at"],
        "bool_cols": [],
    },
    "servicos": {
        "pk": ["id"],
        "cols": ["id", "codigo", "descricao", "categoria", "preco", "ativo", "created_at",
                 "updated_at"],
        "bool_cols": ["ativo"],
    },
    "ordens_servico": {
        "pk": ["id"],
        "cols": ["id", "numero", "cliente_id", "status", "data_entrada", "data_promessa",
                 "hora_promessa", "data_entrega", "data_pagamento", "valor_total", "desconto", "valor_final",
                 "valor_pago", "metodo_pagamento", "troco", "observacoes",
                 "motivo_cancelamento", "usuario_entrada", "usuario_entrega",
                 "usuario_pagamento", "created_at", "updated_at"],
        "bool_cols": [],
    },
    "os_itens": {
        "pk": ["id"],
        "cols": ["id", "os_id", "servico_id", "descricao", "tipo_tecido", "cor", "marca",
                 "defeito", "quantidade", "valor_unitario", "valor_total", "status",
                 "observacao", "created_at"],
        "bool_cols": [],
    },
    "os_historico": {
        "pk": ["id"],
        "cols": ["id", "os_id", "evento", "status_anterior", "status_novo", "detalhe",
                 "usuario", "created_at"],
        "bool_cols": [],
    },
    "pagamentos": {
        "pk": ["id"],
        "cols": ["id", "os_id", "metodo", "valor", "troco", "usuario", "created_at"],
        "bool_cols": [],
    },
    "caixa_sessoes": {
        "pk": ["id"],
        "cols": ["id", "data", "usuario", "valor_abertura", "valor_contado", "status",
                 "observacao_fechamento", "created_at", "fechado_em"],
        "bool_cols": [],
    },
    "caixa_movimentos": {
        "pk": ["id"],
        "cols": ["id", "sessao_id", "tipo", "valor", "descricao", "os_id", "usuario",
                 "created_at"],
        "bool_cols": [],
    },
    "financeiro": {
        "pk": ["id"],
        "cols": ["id", "cliente_id", "os_id", "tipo", "status", "valor", "vencimento",
                 "data_recebimento", "valor_recebido", "metodo_recebimento", "observacao",
                 "usuario", "created_at", "updated_at"],
        "bool_cols": [],
    },
    "legacy_records": {
        "pk": ["id"],
        "cols": ["id", "tabela", "legacy_pk", "payload", "imported_at"],
        "bool_cols": [],
    },
    "orcamentos": {
        "pk": ["id"],
        "cols": ["id", "numero", "cliente_id", "status", "data_entrada", "data_promessa",
                 "data_validade", "valor_total", "desconto", "valor_final", "observacoes",
                 "convertido_rol_id", "usuario_entrada", "created_at", "updated_at"],
        "bool_cols": [],
    },
    "orc_itens": {
        "pk": ["id"],
        "cols": ["id", "orc_id", "servico_id", "descricao", "tipo_tecido", "cor", "marca",
                 "quantidade", "valor_unitario", "valor_total", "observacao", "created_at"],
        "bool_cols": [],
    },
    "agenda": {
        "pk": ["id"],
        "cols": ["id", "rol_id", "orc_id", "cliente_id", "data_agendamento",
                 "hora_agendamento", "duracao_minutos", "tipo", "observacao", "status",
                 "usuario", "created_at"],
        "bool_cols": [],
    },
    "legacy_params": {
        "pk": ["id"],
        "cols": ["id", "fonte", "secao", "chave", "valor"],
        "bool_cols": [],
    },
    "legacy_coverage": {
        "pk": ["id"],
        "cols": ["id", "area", "item", "fonte", "status", "observacao", "updated_by",
                 "updated_at"],
        "bool_cols": [],
    },
    "catalogos": {
        "pk": ["id"],
        "cols": ["id", "tipo", "codigo", "descricao", "ativo", "created_at"],
        "bool_cols": ["ativo"],
    },
    "indenizacoes": {
        "pk": ["id"],
        "cols": ["id", "os_id", "cliente_id", "descricao", "valor", "status", "motivo",
                 "observacao", "usuario", "created_at", "updated_at"],
        "bool_cols": [],
    },
    "guardaroupa": {
        "pk": ["id"],
        "cols": ["id", "cliente_id", "descricao", "categoria", "cor", "marca", "quantidade",
                 "localizacao", "data_entrada", "data_saida", "status", "observacao",
                 "usuario", "created_at"],
        "bool_cols": [],
    },
    "terceirizacao": {
        "pk": ["id"],
        "cols": ["id", "os_id", "fornecedor", "descricao", "valor", "data_envio",
                 "data_retorno_prevista", "data_retorno", "status", "observacao", "usuario",
                 "created_at"],
        "bool_cols": [],
    },
    "fidelidade": {
        "pk": ["cliente_id"],
        "cols": ["cliente_id", "pontos", "updated_at"],
        "bool_cols": [],
    },
    "fidelidade_movimentos": {
        "pk": ["id"],
        "cols": ["id", "cliente_id", "pontos", "tipo", "referencia", "observacao", "usuario",
                 "created_at"],
        "bool_cols": [],
    },
    "doacoes": {
        "pk": ["id"],
        "cols": ["id", "os_id", "cliente_id", "descricao", "valor", "data_doacao", "status",
                 "motivo_cancelamento", "observacao", "usuario", "created_at"],
        "bool_cols": [],
    },
}


def load_env_file(path: Path, into: dict) -> None:
    if not path.exists():
        return
    for line in path.read_text(encoding="utf-8").splitlines():
        line = line.strip()
        if not line or line.startswith("#") or "=" not in line:
            continue
        k, v = line.split("=", 1)
        k = k.strip()
        if k and k not in into:
            into[k] = v.strip()


def sqlite_read_only(db_path: Path) -> sqlite3.Connection:
    uri = f"file:{db_path.as_posix()}?mode=ro"
    con = sqlite3.connect(uri, uri=True)
    con.row_factory = sqlite3.Row
    return con


def sync_table(sqlite_con: sqlite3.Connection, pg_con, table: str, spec: dict, log: logging.Logger) -> int:
    cols = spec["cols"]
    pk = spec["pk"]
    bool_cols = set(spec["bool_cols"])

    rows = sqlite_con.execute(f"SELECT {', '.join(cols)} FROM {table}").fetchall()
    if not rows:
        return 0

    update_cols = [c for c in cols if c not in pk]
    set_clause = ", ".join(f"{c} = EXCLUDED.{c}" for c in update_cols)
    set_clause = (set_clause + ", " if set_clause else "") + "synced_at = now()"

    sql = (
        f"INSERT INTO {table} ({', '.join(cols)}) VALUES %s "
        f"ON CONFLICT ({', '.join(pk)}) DO UPDATE SET {set_clause}"
    )

    values = []
    for row in rows:
        record = []
        for c in cols:
            v = row[c]
            if c in bool_cols and v is not None:
                v = bool(v)
            record.append(v)
        values.append(tuple(record))

    with pg_con.cursor() as cur:
        psycopg2.extras.execute_values(cur, sql, values, page_size=500)
    pg_con.commit()
    return len(rows)


def sync_usuarios_licenca(auth_store_path: Path, pg_con, log: logging.Logger) -> int:
    if not auth_store_path.exists():
        log.warning("auth-store.json nao encontrado em %s -- pulando", auth_store_path)
        return 0
    state = json.loads(auth_store_path.read_text(encoding="utf-8-sig"))
    users = state.get("Users") or state.get("users") or []
    if not users:
        return 0

    # Licenca e' da aplicacao (uma instalacao = uma licenca), nao por usuario --
    # o mesmo plano/vencimento vale para todo mundo (ver AuthState no backend).
    lic_plano = state.get("LicensePlano") or state.get("license_plano")
    lic_vence = state.get("LicenseVenceEm") or state.get("license_vence_em")
    lic_inicio = state.get("LicenseInicioEm") or state.get("license_inicio_em")

    # Conflito por lower(username) (nao por id): usuarios podem ser recriados
    # localmente com um id novo (ex: exclusao permanente + recriacao) mantendo
    # o mesmo username -- upsert por id sozinho batia na constraint unica de
    # username e falhava a tabela inteira.
    sql = """
INSERT INTO usuarios_licenca
  (id, username, display_name, roles, is_active, must_change_password,
   license_plano, license_vence_em, license_inicio_em, last_login_at)
VALUES %s
ON CONFLICT (lower(username)) DO UPDATE SET
  id = EXCLUDED.id,
  display_name = EXCLUDED.display_name,
  roles = EXCLUDED.roles,
  is_active = EXCLUDED.is_active,
  must_change_password = EXCLUDED.must_change_password,
  license_plano = EXCLUDED.license_plano,
  license_vence_em = EXCLUDED.license_vence_em,
  license_inicio_em = EXCLUDED.license_inicio_em,
  last_login_at = EXCLUDED.last_login_at,
  synced_at = now()
"""
    values = []
    for u in users:
        values.append((
            u.get("Id") or u.get("id"),
            u.get("Username") or u.get("username"),
            u.get("DisplayName") or u.get("display_name"),
            u.get("Roles") or u.get("roles") or [],
            bool(u.get("IsActive", u.get("is_active", True))),
            bool(u.get("MustChangePassword", u.get("must_change_password", False))),
            lic_plano,
            lic_vence,
            lic_inicio,
            u.get("LastLoginAt") or u.get("last_login_at"),
        ))

    with pg_con.cursor() as cur:
        psycopg2.extras.execute_values(cur, sql, values, page_size=500)
    pg_con.commit()
    return len(values)


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--data-dir", required=True, help="Diretorio com pdv.db e auth-store.json")
    parser.add_argument("--env-file", default=".env", help="Arquivo .env para dev local (padrao: .env)")
    args = parser.parse_args()

    logging.basicConfig(
        level=logging.INFO,
        format="%(asctime)s [sync-supabase] %(levelname)s %(message)s",
    )
    log = logging.getLogger("sync_to_supabase")

    env = dict(os.environ)
    if "SUPABASE_DB_URL" not in env:
        load_env_file(Path(args.env_file), env)

    db_url = env.get("SUPABASE_DB_URL")
    if not db_url:
        log.error("SUPABASE_DB_URL nao configurado -- nada a fazer, saindo sem erro (nunca bloqueia o app)")
        return 0

    data_dir = Path(args.data_dir)
    pdv_db = data_dir / "pdv.db"
    auth_store = data_dir / "auth-store.json"

    if not pdv_db.exists():
        log.error("pdv.db nao encontrado em %s", pdv_db)
        return 0

    try:
        pg_con = psycopg2.connect(db_url, connect_timeout=15)
    except Exception as exc:
        log.error("nao foi possivel conectar ao Supabase: %s -- seguindo sem sincronizar", exc)
        return 0

    total_rows = 0
    errors = 0
    try:
        sqlite_con = sqlite_read_only(pdv_db)
        try:
            for table, spec in TABLES.items():
                try:
                    n = sync_table(sqlite_con, pg_con, table, spec, log)
                    total_rows += n
                    log.info("tabela %-30s -> %d linhas", table, n)
                except Exception as exc:
                    errors += 1
                    pg_con.rollback()
                    log.error("falha ao sincronizar tabela %s: %s", table, exc)
            try:
                n = sync_usuarios_licenca(auth_store, pg_con, log)
                total_rows += n
                log.info("tabela %-30s -> %d linhas", "usuarios_licenca", n)
            except Exception as exc:
                errors += 1
                pg_con.rollback()
                log.error("falha ao sincronizar usuarios_licenca: %s", exc)
        finally:
            sqlite_con.close()
    finally:
        pg_con.close()

    log.info("sincronizacao concluida: %d linhas no total, %d tabela(s) com erro", total_rows, errors)
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
