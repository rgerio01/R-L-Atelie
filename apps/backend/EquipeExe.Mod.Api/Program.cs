using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Globalization;
using Microsoft.Data.Sqlite;

var builder = WebApplication.CreateBuilder(args);
// Frontend e API são sempre same-origin (o Kestrel serve o wwwroot/index.html
// que consome esta mesma API) — não há necessidade legítima de chamada
// cross-origin. Sem AllowAnyOrigin: nenhum outro site consegue usar um token
// roubado/vazado para chamar a API via fetch() do navegador do usuário.
builder.Services.AddCors(o => o.AddDefaultPolicy(p => p.AllowAnyHeader().AllowAnyMethod()));

var modRoot        = builder.Configuration["EquipeExe:ModRoot"]       ?? @"E:\Projeto Luci\MOD";
var dataDirectory  = builder.Configuration["EquipeExe:DataDirectory"]  ?? Path.Combine(modRoot, "data", "sandbox");
var auditDirectory = builder.Configuration["EquipeExe:AuditDirectory"] ?? Path.Combine(modRoot, "logs", "audit");

Directory.CreateDirectory(dataDirectory);
Directory.CreateDirectory(auditDirectory);

var auth = new AuthStore(dataDirectory, auditDirectory);
auth.EnsureSeeded();
auth.UpgradeRolePermissions();
auth.ImportLegacyUsers();

var db = new PdvDb(dataDirectory);
db.Initialize();

TrustedClock.StartBackgroundSync();

var app = builder.Build();
app.UseCors();
app.UseDefaultFiles();
app.UseStaticFiles();

// Cabeçalhos básicos de segurança — mitigam clickjacking, MIME-sniffing e
// vazamento de URL via Referer; custo zero, sem efeito colateral conhecido
// nesta aplicação (não embeda nem é embedada em iframes de terceiros).
app.Use(async (ctx, next) =>
{
    ctx.Response.Headers["X-Content-Type-Options"] = "nosniff";
    ctx.Response.Headers["X-Frame-Options"] = "DENY";
    ctx.Response.Headers["Referrer-Policy"] = "same-origin";
    await next();
});

app.Use(async (ctx, next) =>
{
    try { await next(); }
    catch (UnauthorizedAccessException ex)
    {
        ctx.Response.StatusCode = 403;
        await ctx.Response.WriteAsJsonAsync(new { error = ex.Message });
    }
    catch (InvalidOperationException ex)
    {
        ctx.Response.StatusCode = 400;
        await ctx.Response.WriteAsJsonAsync(new { error = ex.Message });
    }
    catch (KeyNotFoundException ex)
    {
        ctx.Response.StatusCode = 404;
        await ctx.Response.WriteAsJsonAsync(new { error = ex.Message });
    }
});

// ── Health ────────────────────────────────────────────────────────────────────
app.MapGet("/", () => Results.Redirect("/index.html"));
app.MapGet("/health", () => Results.Ok(new
{
    system    = "Atelie da Luci — PDV",
    status    = "ok",
    storage   = dataDirectory,
    audit     = auditDirectory,
    timestamp = DateTimeOffset.UtcNow
}));

// ── Auth ──────────────────────────────────────────────────────────────────────
app.MapPost("/auth/login", (LoginRequest req, HttpContext http) =>
{
    var result = auth.Login(req.Username, req.Password, http.Connection.RemoteIpAddress?.ToString());
    return result is null ? Results.Unauthorized() : Results.Ok(result);
});

app.MapGet("/auth/me", (HttpRequest req) =>
    Results.Ok(auth.DescribeSession(auth.RequireSession(req))));

app.MapPost("/auth/trocar-senha", (TrocarSenhaRequest req, HttpRequest http) =>
{
    var session = auth.RequireSession(http);
    auth.ChangePassword(session.UserId, req.SenhaAtual, req.SenhaNova);
    return Results.Ok(new { ok = true });
});

// ── Usuários (admin) ──────────────────────────────────────────────────────────
app.MapGet("/admin/usuarios", (HttpRequest req) =>
{
    auth.RequireAnyPermission(req, Perm.UsuariosRead, Perm.SenhaResetOutros);
    return Results.Ok(auth.ListUsers());
});

app.MapPost("/admin/usuarios", (CreateUserRequest req, HttpRequest http) =>
{
    var session = auth.RequirePermission(http, Perm.UsuariosWrite);
    var user = auth.CreateUser(req, session.Username);
    return Results.Created($"/admin/usuarios/{user.Id}", user);
});

app.MapPut("/admin/usuarios/{id:guid}", (Guid id, UpdateUserRequest req, HttpRequest http) =>
{
    var session = auth.RequirePermission(http, Perm.UsuariosWrite);
    var user = auth.UpdateUser(id, req, session.Username);
    return Results.Ok(user);
});

app.MapDelete("/admin/usuarios/{id:guid}", (Guid id, HttpRequest http) =>
{
    var session = auth.RequirePermission(http, Perm.UsuariosWrite);
    auth.DeactivateUser(id, session.Username);
    return Results.Ok(new { ok = true });
});

app.MapPost("/admin/usuarios/{id:guid}/ativar", (Guid id, HttpRequest http) =>
{
    var session = auth.RequirePermission(http, Perm.UsuariosWrite);
    auth.ReactivateUser(id, session.Username);
    return Results.Ok(new { ok = true });
});

app.MapDelete("/admin/usuarios/{id:guid}/permanente", (Guid id, HttpRequest http) =>
{
    var session = auth.RequirePermission(http, Perm.UsuariosWrite);
    auth.DeleteUserPermanently(id, session.Username);
    return Results.Ok(new { ok = true });
});

app.MapPost("/admin/usuarios/{id:guid}/reset-senha", (Guid id, ResetSenhaRequest req, HttpRequest http) =>
{
    var session = auth.RequireAnyPermission(http, Perm.UsuariosWrite, Perm.SenhaResetOutros);
    var podeTudo = session.Permissions.Contains("*") || session.Permissions.Contains(Perm.UsuariosWrite);
    var user = auth.ResetSenha(id, req.NovaSenha, podeTudo, session.Username);
    return Results.Ok(user);
});

app.MapPost("/admin/usuarios/{id:guid}/perfis", (Guid id, AssignRolesRequest req, HttpRequest http) =>
{
    var session = auth.RequirePermission(http, Perm.UsuariosWrite);
    return Results.Ok(auth.AssignRoles(id, req.Roles, session.Username));
});

app.MapGet("/admin/perfis", (HttpRequest req) =>
{
    auth.RequirePermission(req, Perm.UsuariosRead);
    return Results.Ok(auth.GetPermissionsMap());
});

// ── Licenciamento (mensal/trimestral/anual/vitalício) ─────────────────────────
app.MapGet("/admin/licencas/planos", (HttpRequest req) =>
{
    auth.RequirePermission(req, Perm.LicencasManage);
    return Results.Ok(LicencaPlanos.Catalogo());
});

app.MapPost("/admin/usuarios/{id:guid}/licenca/renovar", (Guid id, RenovarLicencaRequest req, HttpRequest http) =>
{
    var s = auth.RequirePermission(http, Perm.LicencasManage);
    return Results.Ok(auth.RenovarLicenca(id, req.Plano, s.Username));
});

// ── Configurações ─────────────────────────────────────────────────────────────
app.MapGet("/configuracoes", (HttpRequest req) =>
{
    auth.RequireSession(req);
    return Results.Ok(db.GetConfiguracoes());
});

app.MapPut("/configuracoes/{chave}", (string chave, ConfigRequest req, HttpRequest http) =>
{
    auth.RequirePermission(http, Perm.ConfigWrite);
    db.SetConfiguracao(chave, req.Valor);
    return Results.Ok(new { ok = true });
});

// ── Regra Nada Fica Para Tras / legado EquipeExe ─────────────────────────────
app.MapGet("/legacy/params", (HttpRequest req) =>
{
    auth.RequireSession(req);
    return Results.Ok(db.GetLegacyParams());
});

app.MapGet("/legacy/params/{secao}", (string secao, HttpRequest req) =>
{
    auth.RequireSession(req);
    return Results.Ok(db.GetLegacyParams(secao));
});

app.MapGet("/legacy/coverage", (HttpRequest req, string? area, string? status, int pg = 1, int tam = 100) =>
{
    auth.RequirePermission(req, Perm.LegadoRead);
    return Results.Ok(db.ListLegacyCoverage(area, status, pg, tam));
});

app.MapPost("/legacy/coverage/{id:int}/status", (int id, CoverageStatusRequest req, HttpRequest http) =>
{
    var s = auth.RequirePermission(http, Perm.ConfigWrite);
    db.UpdateLegacyCoverage(id, req.Status, req.Observacao, s.Username);
    return Results.Ok(new { ok = true });
});

// ── Clientes ──────────────────────────────────────────────────────────────────
app.MapGet("/clientes", (HttpRequest req, string? q, int pg = 1, int tam = 50) =>
{
    auth.RequireSession(req);
    return Results.Ok(db.ListarClientes(q, pg, tam));
});

app.MapGet("/clientes/{id}", (int id, HttpRequest req) =>
{
    auth.RequireSession(req);
    return Results.Ok(db.ObterCliente(id) ?? throw new KeyNotFoundException("Cliente não encontrado."));
});

app.MapPost("/clientes", (ClienteRequest req, HttpRequest http) =>
{
    var s = auth.RequireSession(http);
    var id = db.CriarCliente(req, s.Username);
    return Results.Created($"/clientes/{id}", db.ObterCliente(id));
});

app.MapPut("/clientes/{id}", (int id, ClienteRequest req, HttpRequest http) =>
{
    var s = auth.RequireSession(http);
    db.AtualizarCliente(id, req, s.Username);
    return Results.Ok(db.ObterCliente(id));
});

app.MapDelete("/clientes/{id}", (int id, HttpRequest http) =>
{
    var s = auth.RequireSession(http);
    db.InativarCliente(id, s.Username);
    return Results.Ok(new { ok = true });
});

app.MapGet("/clientes/{id}/historico", (int id, HttpRequest req) =>
{
    auth.RequireSession(req);
    return Results.Ok(db.HistoricoCliente(id));
});

// ── Serviços (tabela de preços) ───────────────────────────────────────────────
app.MapGet("/servicos", (HttpRequest req, string? q) =>
{
    auth.RequireSession(req);
    return Results.Ok(db.ListarServicos(q));
});

app.MapGet("/servicos/categorias", (HttpRequest req) =>
{
    auth.RequireSession(req);
    return Results.Ok(db.ListarCategorias());
});

app.MapPost("/servicos", (ServicoRequest req, HttpRequest http) =>
{
    auth.RequirePermission(http, Perm.PrecosWrite);
    var id = db.CriarServico(req);
    return Results.Created($"/servicos/{id}", db.ObterServico(id));
});

app.MapPut("/servicos/{id}", (int id, ServicoRequest req, HttpRequest http) =>
{
    auth.RequirePermission(http, Perm.PrecosWrite);
    db.AtualizarServico(id, req);
    return Results.Ok(db.ObterServico(id));
});

app.MapDelete("/servicos/{id}", (int id, HttpRequest http) =>
{
    auth.RequirePermission(http, Perm.PrecosWrite);
    db.InativarServico(id);
    return Results.Ok(new { ok = true });
});

app.MapPost("/servicos/ajustar-precos", (AjustarPrecosRequest req, HttpRequest http) =>
{
    auth.RequirePermission(http, Perm.PrecosWrite);
    var afetados = db.AjustarPrecos(req);
    return Results.Ok(new { ok = true, afetados });
});

// ── ROL (Ordens de Serviço) ───────────────────────────────────────────────────
app.MapGet("/rol", (HttpRequest req, string? status, int? clienteId, string? de, string? ate, string? q, int pg = 1, int tam = 50) =>
{
    auth.RequireSession(req);
    return Results.Ok(db.ListarRols(status, clienteId, de, ate, q, pg, tam));
});

app.MapGet("/rol/{id}", (int id, HttpRequest req) =>
{
    auth.RequireSession(req);
    return Results.Ok(db.ObterRol(id) ?? throw new KeyNotFoundException("ROL não encontrado."));
});

app.MapPost("/rol", (RolRequest req, HttpRequest http) =>
{
    var s = auth.RequireSession(http);
    var id = db.CriarRol(req, s.Username);
    return Results.Created($"/rol/{id}", db.ObterRol(id));
});

app.MapPut("/rol/{id}", (int id, RolRequest req, HttpRequest http) =>
{
    var s = auth.RequireSession(http);
    db.AtualizarRolCabecalho(id, req, s.Username);
    return Results.Ok(db.ObterRol(id));
});

app.MapPost("/rol/{id}/itens", (int id, RolItemRequest req, HttpRequest http) =>
{
    var s = auth.RequireSession(http);
    db.AdicionarItemRol(id, req, s.Username);
    return Results.Ok(db.ObterRol(id));
});

app.MapPut("/rol/{id}/itens/{itemId}", (int id, int itemId, RolItemRequest req, HttpRequest http) =>
{
    var s = auth.RequireSession(http);
    db.AtualizarItemRol(id, itemId, req, s.Username);
    return Results.Ok(db.ObterRol(id));
});

app.MapDelete("/rol/{id}/itens/{itemId}", (int id, int itemId, HttpRequest http) =>
{
    var s = auth.RequireSession(http);
    db.RemoverItemRol(id, itemId, s.Username);
    return Results.Ok(db.ObterRol(id));
});

app.MapPost("/rol/{id}/pronta", (int id, HttpRequest http) =>
{
    var s = auth.RequireSession(http);
    db.MarcarRolPronta(id, s.Username);
    return Results.Ok(db.ObterRol(id));
});

app.MapPost("/rol/{id}/entregar", (int id, EntregarRequest req, HttpRequest http) =>
{
    var s = auth.RequireSession(http);
    db.EntregarRol(id, req, s.Username);
    return Results.Ok(db.ObterRol(id));
});

app.MapPost("/rol/{id}/pagar", (int id, PagamentoRequest req, HttpRequest http) =>
{
    var s = auth.RequireSession(http);
    var resultado = db.PagarRol(id, req, s.Username);
    return Results.Ok(resultado);
});

app.MapPost("/rol/{id}/cancelar", (int id, CancelarRequest req, HttpRequest http) =>
{
    var s = auth.RequireSession(http);
    db.CancelarRol(id, req.Motivo, s.Username);
    return Results.Ok(db.ObterRol(id));
});

app.MapPost("/rol/{id}/estornar", (int id, EstornoRequest req, HttpRequest http) =>
{
    var s = auth.RequirePermission(http, Perm.CaixaAccess);
    return Results.Ok(db.EstornarRol(id, req.Motivo, s.Username));
});

app.MapGet("/rol/{id}/recibo", (int id, HttpRequest req) =>
{
    auth.RequireSession(req);
    return Results.Ok(db.GerarRecibo(id));
});

app.MapGet("/rol/{id}/impressao", (int id, HttpRequest req, string tipo = "rol") =>
{
    auth.RequireSession(req);
    return Results.Text(db.GerarImpressaoRolTexto(id, tipo), "text/plain; charset=utf-8");
});

app.MapGet("/rol/{id}/etiqueta-argox", (int id, HttpRequest req) =>
{
    auth.RequireSession(req);
    return Results.Text(db.GerarEtiquetaArgox(id), "text/plain; charset=utf-8");
});

// ── Caixa ─────────────────────────────────────────────────────────────────────
app.MapGet("/caixa/atual", (HttpRequest req) =>
{
    auth.RequirePermission(req, Perm.CaixaAccess);
    return Results.Ok(db.GetSessaoCaixaAtual());
});

app.MapGet("/caixa/historico", (HttpRequest req, int dias = 30) =>
{
    auth.RequirePermission(req, Perm.CaixaAccess);
    return Results.Ok(db.HistoricoCaixa(dias));
});

app.MapPost("/caixa/abrir", (AbrirCaixaRequest req, HttpRequest http) =>
{
    var s = auth.RequirePermission(http, Perm.CaixaAccess);
    var id = db.AbrirCaixa(req.ValorAbertura, s.Username);
    return Results.Ok(db.GetSessaoCaixa(id));
});

app.MapPost("/caixa/fechar", (FecharCaixaRequest req, HttpRequest http) =>
{
    var s = auth.RequirePermission(http, Perm.CaixaAccess);
    db.FecharCaixa(req.ValorContado, req.Observacao, s.Username);
    return Results.Ok(db.GetSessaoCaixaAtual());
});

app.MapPost("/caixa/suprimento", (MovCaixaRequest req, HttpRequest http) =>
{
    var s = auth.RequirePermission(http, Perm.CaixaAccess);
    db.SuprimentoCaixa(req.Valor, req.Descricao, s.Username);
    return Results.Ok(db.GetSessaoCaixaAtual());
});

app.MapPost("/caixa/sangria", (MovCaixaRequest req, HttpRequest http) =>
{
    var s = auth.RequirePermission(http, Perm.CaixaAccess);
    db.SangriaCaixa(req.Valor, req.Descricao, s.Username);
    return Results.Ok(db.GetSessaoCaixaAtual());
});

app.MapGet("/caixa/{id}/movimentos", (int id, HttpRequest req) =>
{
    auth.RequirePermission(req, Perm.CaixaAccess);
    return Results.Ok(db.MovimentosCaixa(id));
});

// ── Financeiro / Cobrança ─────────────────────────────────────────────────────
app.MapGet("/financeiro", (HttpRequest req, string? status, int? clienteId, string? de, string? ate, int pg = 1, int tam = 50) =>
{
    auth.RequirePermission(req, Perm.FinanceiroRead);
    return Results.Ok(db.ListarFinanceiro(status, clienteId, de, ate, pg, tam));
});

app.MapGet("/financeiro/resumo", (HttpRequest req) =>
{
    auth.RequirePermission(req, Perm.FinanceiroRead);
    return Results.Ok(db.ResumoFinanceiro());
});

app.MapPost("/financeiro/{id}/receber", (int id, ReceberRequest req, HttpRequest http) =>
{
    var s = auth.RequirePermission(http, Perm.FinanceiroRead);
    db.ReceberDuplicata(id, req, s.Username);
    return Results.Ok(new { ok = true });
});

// ── Relatórios ────────────────────────────────────────────────────────────────
app.MapGet("/relatorios/movimento-dia", (HttpRequest req, string? data) =>
{
    var s = auth.RequireAnyPermission(req, Perm.RelatoriosRead, Perm.DashboardRead);
    var podeVerQualquerDia = s.Permissions.Contains("*") || s.Permissions.Contains(Perm.RelatoriosRead);
    var hoje = TrustedClock.UtcNow.ToString("yyyy-MM-dd");
    var dia = podeVerQualquerDia ? (data ?? hoje) : hoje;
    return Results.Ok(db.RelMovimentoDia(dia));
});

app.MapGet("/relatorios/movimento-periodo", (HttpRequest req, string de, string ate) =>
{
    auth.RequirePermission(req, Perm.RelatoriosRead);
    return Results.Ok(db.RelMovimentoPeriodo(de, ate));
});

app.MapGet("/relatorios/rol-abertos", (HttpRequest req) =>
{
    auth.RequirePermission(req, Perm.RelatoriosRead);
    return Results.Ok(db.RelRolAbertos());
});

app.MapGet("/relatorios/rol-entrega", (HttpRequest req, string? data) =>
{
    var s = auth.RequireAnyPermission(req, Perm.RelatoriosRead, Perm.DashboardRead);
    var podeVerQualquerDia = s.Permissions.Contains("*") || s.Permissions.Contains(Perm.RelatoriosRead);
    var dia = podeVerQualquerDia ? data : TrustedClock.UtcNow.ToString("yyyy-MM-dd");
    return Results.Ok(db.RelRolEntrega(dia));
});

app.MapGet("/relatorios/caixa-dia", (HttpRequest req, string? data) =>
{
    auth.RequirePermission(req, Perm.RelatoriosRead);
    var dia = data ?? DateTime.Today.ToString("yyyy-MM-dd");
    return Results.Ok(db.RelCaixaDia(dia));
});

app.MapGet("/relatorios/clientes-debito", (HttpRequest req) =>
{
    auth.RequirePermission(req, Perm.RelatoriosRead);
    return Results.Ok(db.RelClientesDebito());
});

app.MapGet("/relatorios/servicos-periodo", (HttpRequest req, string de, string ate) =>
{
    auth.RequirePermission(req, Perm.RelatoriosRead);
    return Results.Ok(db.RelServicosPeriodo(de, ate));
});

app.MapGet("/relatorios/frequencia-clientes", (HttpRequest req, string de, string ate) =>
{
    auth.RequirePermission(req, Perm.RelatoriosRead);
    return Results.Ok(db.RelFrequenciaClientes(de, ate));
});

// ── Crédito de clientes ───────────────────────────────────────────────────────
app.MapGet("/clientes/{id}/credito", (int id, HttpRequest req) =>
{
    auth.RequireSession(req);
    return Results.Ok(db.GetCreditoCliente(id));
});

app.MapPost("/clientes/{id}/credito", (int id, CreditoRequest req, HttpRequest http) =>
{
    var s = auth.RequireSession(http);
    db.LancarCreditoCliente(id, req, s.Username);
    return Results.Ok(db.GetCreditoCliente(id));
});

// ── Orçamentos ────────────────────────────────────────────────────────────────
app.MapGet("/orcamentos", (HttpRequest req, string? status, int? clienteId, string? q, int pg = 1, int tam = 50) =>
{
    auth.RequireSession(req);
    return Results.Ok(db.ListarOrcamentos(status, clienteId, q, pg, tam));
});

app.MapGet("/orcamentos/{id}", (int id, HttpRequest req) =>
{
    auth.RequireSession(req);
    return Results.Ok(db.ObterOrcamento(id) ?? throw new KeyNotFoundException("Orçamento não encontrado."));
});

app.MapPost("/orcamentos", (OrcamentoRequest req, HttpRequest http) =>
{
    var s = auth.RequireSession(http);
    var id = db.CriarOrcamento(req, s.Username);
    return Results.Created($"/orcamentos/{id}", db.ObterOrcamento(id));
});

app.MapPut("/orcamentos/{id}", (int id, OrcamentoRequest req, HttpRequest http) =>
{
    var s = auth.RequireSession(http);
    db.AtualizarOrcamento(id, req, s.Username);
    return Results.Ok(db.ObterOrcamento(id));
});

app.MapPost("/orcamentos/{id}/itens", (int id, RolItemRequest req, HttpRequest http) =>
{
    var s = auth.RequireSession(http);
    db.AdicionarItemOrcamento(id, req, s.Username);
    return Results.Ok(db.ObterOrcamento(id));
});

app.MapPut("/orcamentos/{id}/itens/{itemId}", (int id, int itemId, RolItemRequest req, HttpRequest http) =>
{
    var s = auth.RequireSession(http);
    db.AtualizarItemOrcamento(id, itemId, req);
    return Results.Ok(db.ObterOrcamento(id));
});

app.MapDelete("/orcamentos/{id}/itens/{itemId}", (int id, int itemId, HttpRequest http) =>
{
    var s = auth.RequireSession(http);
    db.RemoverItemOrcamento(id, itemId);
    return Results.Ok(db.ObterOrcamento(id));
});

app.MapPost("/orcamentos/{id}/converter-rol", (int id, HttpRequest http) =>
{
    var s = auth.RequireSession(http);
    var rolId = db.ConverterOrcamentoEmRol(id, s.Username);
    return Results.Ok(db.ObterRol(rolId));
});

app.MapPost("/orcamentos/{id}/cancelar", (int id, CancelarRequest req, HttpRequest http) =>
{
    var s = auth.RequireSession(http);
    db.CancelarOrcamento(id, req.Motivo, s.Username);
    return Results.Ok(db.ObterOrcamento(id));
});

// ── Agenda ────────────────────────────────────────────────────────────────────
app.MapGet("/agenda", (HttpRequest req, string? data, string? de, string? ate, int? clienteId) =>
{
    auth.RequireSession(req);
    return Results.Ok(db.ListarAgenda(data, de, ate, clienteId));
});

app.MapPost("/agenda", (AgendaRequest req, HttpRequest http) =>
{
    var s = auth.RequireSession(http);
    var id = db.CriarAgendamento(req, s.Username);
    return Results.Created($"/agenda/{id}", db.ObterAgendamento(id));
});

app.MapGet("/agenda/{id}", (int id, HttpRequest req) =>
{
    auth.RequireSession(req);
    return Results.Ok(db.ObterAgendamento(id) ?? throw new KeyNotFoundException("Agendamento não encontrado."));
});

app.MapPut("/agenda/{id}", (int id, AgendaRequest req, HttpRequest http) =>
{
    var s = auth.RequireSession(http);
    db.AtualizarAgendamento(id, req, s.Username);
    return Results.Ok(db.ObterAgendamento(id));
});

app.MapDelete("/agenda/{id}", (int id, HttpRequest http) =>
{
    auth.RequireSession(http);
    db.CancelarAgendamento(id);
    return Results.Ok(new { ok = true });
});

// ── Catálogos ─────────────────────────────────────────────────────────────────
app.MapGet("/catalogos", (HttpRequest req, string? tipo, string? q) =>
{
    auth.RequireSession(req);
    return Results.Ok(db.ListarCatalogos(tipo, q));
});

app.MapPost("/catalogos", (CatalogoRequest req, HttpRequest http) =>
{
    auth.RequirePermission(http, Perm.CatalogosWrite);
    var id = db.CriarCatalogo(req);
    return Results.Created($"/catalogos/{id}", new { id });
});

app.MapPut("/catalogos/{id}", (int id, CatalogoRequest req, HttpRequest http) =>
{
    auth.RequirePermission(http, Perm.CatalogosWrite);
    db.AtualizarCatalogo(id, req);
    return Results.Ok(new { ok = true });
});

app.MapDelete("/catalogos/{id}", (int id, HttpRequest http) =>
{
    auth.RequirePermission(http, Perm.CatalogosWrite);
    db.InativarCatalogo(id);
    return Results.Ok(new { ok = true });
});

// ── Indenizações ──────────────────────────────────────────────────────────────
app.MapGet("/indenizacoes", (HttpRequest req, int? clienteId, int? osId, string? status) =>
{
    auth.RequireSession(req);
    return Results.Ok(db.ListarIndenizacoes(clienteId, osId, status));
});

app.MapPost("/indenizacoes", (IndenizacaoRequest req, HttpRequest http) =>
{
    var s = auth.RequireSession(http);
    var id = db.CriarIndenizacao(req, s.Username);
    return Results.Created($"/indenizacoes/{id}", new { id });
});

app.MapPut("/indenizacoes/{id}", (int id, IndenizacaoUpdateRequest req, HttpRequest http) =>
{
    var s = auth.RequireSession(http);
    db.AtualizarIndenizacao(id, req, s.Username);
    return Results.Ok(new { ok = true });
});

// ── Guarda-roupa ──────────────────────────────────────────────────────────────
app.MapGet("/guardaroupa", (HttpRequest req, int? clienteId, string? status) =>
{
    auth.RequireSession(req);
    return Results.Ok(db.ListarGuardaroupa(clienteId, status));
});

app.MapPost("/guardaroupa", (GuardaroupaRequest req, HttpRequest http) =>
{
    var s = auth.RequireSession(http);
    var id = db.CriarGuardaroupa(req, s.Username);
    return Results.Created($"/guardaroupa/{id}", new { id });
});

app.MapPost("/guardaroupa/{id}/retirar", (int id, HttpRequest http) =>
{
    var s = auth.RequireSession(http);
    db.RetirarGuardaroupa(id, s.Username);
    return Results.Ok(new { ok = true });
});

// ── Terceirização ─────────────────────────────────────────────────────────────
app.MapGet("/terceirizacao", (HttpRequest req, int? osId, string? status) =>
{
    auth.RequireSession(req);
    return Results.Ok(db.ListarTerceirizacao(osId, status));
});

app.MapPost("/terceirizacao", (TerceirizacaoRequest req, HttpRequest http) =>
{
    var s = auth.RequireSession(http);
    var id = db.CriarTerceirizacao(req, s.Username);
    return Results.Created($"/terceirizacao/{id}", new { id });
});

app.MapPost("/terceirizacao/{id}/receber", (int id, HttpRequest http) =>
{
    var s = auth.RequireSession(http);
    db.ReceberTerceirizacao(id, s.Username);
    return Results.Ok(new { ok = true });
});

// ── Fidelidade / Pontos ───────────────────────────────────────────────────────
app.MapGet("/clientes/{id}/fidelidade", (int id, HttpRequest req) =>
{
    auth.RequireSession(req);
    return Results.Ok(db.GetFidelidade(id));
});

app.MapPost("/clientes/{id}/fidelidade/pontos", (int id, PontosRequest req, HttpRequest http) =>
{
    var s = auth.RequirePermission(http, Perm.FidelidadeManage);
    db.LancarPontos(id, req.Pontos, req.Tipo, req.Referencia, req.Observacao, s.Username);
    return Results.Ok(db.GetFidelidade(id));
});

// ── Regras de pontos por categoria (quantos pontos cada venda vale) ───────────
app.MapGet("/fidelidade/regras", (HttpRequest req) =>
{
    auth.RequireSession(req);
    return Results.Ok(db.GetFidelidadeRegras());
});

app.MapPut("/fidelidade/regras/{categoria}", (string categoria, RegraFidelidadeRequest req, HttpRequest http) =>
{
    var s = auth.RequirePermission(http, Perm.FidelidadeManage);
    db.SetFidelidadeRegra(categoria, req.PontosPorVenda, s.Username);
    return Results.Ok(db.GetFidelidadeRegras());
});

// ── Doações (peças não retiradas) ─────────────────────────────────────────────
app.MapGet("/doacoes", (HttpRequest req, int? clienteId, int? osId, string? status) =>
{
    auth.RequireSession(req);
    return Results.Ok(db.ListarDoacoes(clienteId, osId, status));
});

app.MapPost("/doacoes", (DoacaoRequest req, HttpRequest http) =>
{
    var s = auth.RequireSession(http);
    var id = db.CriarDoacao(req, s.Username);
    return Results.Created($"/doacoes/{id}", new { id });
});

app.MapPost("/doacoes/{id}/confirmar", (int id, HttpRequest http) =>
{
    var s = auth.RequireSession(http);
    db.ConfirmarDoacao(id, s.Username);
    return Results.Ok(new { ok = true });
});

app.MapPost("/doacoes/{id}/cancelar", (int id, CancelarRequest req, HttpRequest http) =>
{
    var s = auth.RequireSession(http);
    db.CancelarDoacao(id, req.Motivo, s.Username);
    return Results.Ok(new { ok = true });
});

// ── Importação de créditos legados (endpoint manual para reprocessar) ─────────
app.MapPost("/admin/reimportar-creditos", (HttpRequest http) =>
{
    var s = auth.RequirePermission(http, Perm.ConfigWrite);
    using var con = db.OpenPublic();
    var n = db.ReimportarCreditos(con);
    return Results.Ok(new { importados = n });
});

// ── Mercado Pago PIX ────────────────────────────────────────────────────────────
// Duas contas separadas: "venda" usa o token da Luci (operadora, cobra o cliente
// final no PDV) e "licenca" usa o token do Rogerio (fornecedor, cobra a licença
// de uso do sistema dos operadores). O contexto decide qual token é usado.
app.MapPost("/pagamentos/pix/criar", async (PixCriarRequest req, HttpRequest http) =>
{
    var contexto = (req.Contexto ?? "venda").ToLowerInvariant();
    if (contexto == "licenca") auth.RequirePermission(http, Perm.LicencasManage);
    else auth.RequireSession(http);

    var chaveToken = contexto == "licenca" ? "mp_rogerio_access_token" : "mp_luci_access_token";
    var accessToken = db.GetConfiguracao(chaveToken);
    if (string.IsNullOrWhiteSpace(accessToken))
        throw new InvalidOperationException(contexto == "licenca"
            ? "Access token do Mercado Pago do Rogerio (licenciamento) não configurado. Acesse Configurações → Mercado Pago."
            : "Access token do Mercado Pago da Luci (vendas) não configurado. Acesse Configurações → Mercado Pago.");

    var descricao = req.Descricao ?? $"Ateliê da Luci — ROL #{req.RolId}";

    using var http2 = new System.Net.Http.HttpClient();
    http2.DefaultRequestHeaders.Authorization =
        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
    http2.DefaultRequestHeaders.Add("X-Idempotency-Key", Guid.NewGuid().ToString());

    var payload = JsonSerializer.Serialize(new
    {
        transaction_amount = Math.Round(req.Valor, 2),
        description = descricao,
        payment_method_id = "pix",
        payer = new { email = "pagamento@atelie.com.br" }
    });

    var response = await http2.PostAsync(
        "https://api.mercadopago.com/v1/payments",
        new System.Net.Http.StringContent(payload, System.Text.Encoding.UTF8, "application/json"));

    var body = await response.Content.ReadFromJsonAsync<JsonElement>();

    if (!response.IsSuccessStatusCode)
    {
        var msg = body.TryGetProperty("message", out var m) ? m.GetString() : response.StatusCode.ToString();
        throw new InvalidOperationException($"Erro Mercado Pago: {msg}");
    }

    var txData = body.GetProperty("point_of_interaction").GetProperty("transaction_data");
    return Results.Ok(new
    {
        mpPaymentId = body.GetProperty("id").GetInt64(),
        qrCode      = txData.GetProperty("qr_code").GetString(),
        qrCodeBase64 = txData.GetProperty("qr_code_base64").GetString(),
        valor       = req.Valor
    });
});

app.MapGet("/pagamentos/pix/{mpPaymentId:long}/status", async (long mpPaymentId, HttpRequest http, string contexto = "venda") =>
{
    contexto = contexto.ToLowerInvariant();
    if (contexto == "licenca") auth.RequirePermission(http, Perm.LicencasManage);
    else auth.RequireSession(http);

    var chaveToken = contexto == "licenca" ? "mp_rogerio_access_token" : "mp_luci_access_token";
    var accessToken = db.GetConfiguracao(chaveToken);
    if (string.IsNullOrWhiteSpace(accessToken))
        return Results.Ok(new { status = "error", message = "Token não configurado" });

    using var http2 = new System.Net.Http.HttpClient();
    http2.DefaultRequestHeaders.Authorization =
        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

    var response = await http2.GetAsync($"https://api.mercadopago.com/v1/payments/{mpPaymentId}");
    if (!response.IsSuccessStatusCode)
        return Results.Ok(new { status = "error" });

    var body = await response.Content.ReadFromJsonAsync<JsonElement>();
    return Results.Ok(new
    {
        status       = body.GetProperty("status").GetString(),
        statusDetail = body.TryGetProperty("status_detail", out var sd) ? sd.GetString() : null
    });
});

// ── Autoatendimento de licença ────────────────────────────────────────────────
// Diferente de /admin/licencas/* e /pagamentos/pix (contexto=licenca), que exigem
// Perm.LicencasManage (um admin regularizando em nome de outro usuário), estes
// endpoints servem o próprio usuário travado por licença vencida: ele acabou de
// logar (tem sessão válida) mas não tem — nem precisa ter — permissão de admin.
app.MapGet("/minha-licenca/planos", (HttpRequest req) =>
{
    auth.RequireSession(req);
    return Results.Ok(LicencaPlanos.Catalogo());
});

app.MapPost("/minha-licenca/pix", async (MinhaLicencaPixRequest body, HttpRequest http) =>
{
    var s = auth.RequireSession(http);
    var plano = (body.Plano ?? "").ToLowerInvariant();
    var valor = LicencaPlanos.ValorFixo(plano)
        ?? throw new InvalidOperationException("Este plano não tem pagamento automático — entre em contato com o fornecedor.");

    var accessToken = db.GetConfiguracao("mp_rogerio_access_token");
    if (string.IsNullOrWhiteSpace(accessToken))
        throw new InvalidOperationException("Access token do Mercado Pago do fornecedor não configurado.");

    using var http2 = new System.Net.Http.HttpClient();
    http2.DefaultRequestHeaders.Authorization =
        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
    http2.DefaultRequestHeaders.Add("X-Idempotency-Key", Guid.NewGuid().ToString());

    var payload = JsonSerializer.Serialize(new
    {
        transaction_amount = Math.Round(valor, 2),
        description = $"Licença Ateliê da Luci — {plano} — {s.Username}",
        payment_method_id = "pix",
        payer = new { email = "licenca@atelie.com.br" }
    });

    var response = await http2.PostAsync(
        "https://api.mercadopago.com/v1/payments",
        new System.Net.Http.StringContent(payload, System.Text.Encoding.UTF8, "application/json"));

    var mpBody = await response.Content.ReadFromJsonAsync<JsonElement>();
    if (!response.IsSuccessStatusCode)
    {
        var msg = mpBody.TryGetProperty("message", out var m) ? m.GetString() : response.StatusCode.ToString();
        throw new InvalidOperationException($"Erro Mercado Pago: {msg}");
    }

    var mpPaymentId = mpBody.GetProperty("id").GetInt64();
    var txData = mpBody.GetProperty("point_of_interaction").GetProperty("transaction_data");
    auth.RegistrarPagamentoLicencaPendente(mpPaymentId, s.UserId, plano);

    return Results.Ok(new
    {
        mpPaymentId,
        qrCode       = txData.GetProperty("qr_code").GetString(),
        qrCodeBase64 = txData.GetProperty("qr_code_base64").GetString(),
        valor,
        plano
    });
});

app.MapGet("/minha-licenca/status/{mpPaymentId:long}", async (long mpPaymentId, HttpRequest http) =>
{
    var s = auth.RequireSession(http);
    var accessToken = db.GetConfiguracao("mp_rogerio_access_token");
    if (string.IsNullOrWhiteSpace(accessToken))
        return Results.Ok(new { status = "error", message = "Token não configurado" });

    using var http2 = new System.Net.Http.HttpClient();
    http2.DefaultRequestHeaders.Authorization =
        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

    var response = await http2.GetAsync($"https://api.mercadopago.com/v1/payments/{mpPaymentId}");
    if (!response.IsSuccessStatusCode)
        return Results.Ok(new { status = "error" });

    var mpBody = await response.Content.ReadFromJsonAsync<JsonElement>();
    var status = mpBody.GetProperty("status").GetString();

    if (status == "approved" && auth.TentarConcluirPagamentoLicenca(mpPaymentId, s.UserId, out var resultado))
        return Results.Ok(new { status, renewed = true, resultado });

    return Results.Ok(new { status, renewed = false });
});

// Botão de simulação de pagamento — SOMENTE para o usuário "teste", usado pra
// validar o fluxo de liberação de licença (mensal/trimestral/anual) sem precisar
// pagar um PIX de verdade a cada teste. Checagem de username no backend também
// (não só escondido no frontend), pra não virar um bypass de pagamento real.
app.MapPost("/minha-licenca/simular-pagamento", (MinhaLicencaPixRequest body, HttpRequest http) =>
{
    var s = auth.RequireSession(http);
    if (!string.Equals(s.Username, "teste", StringComparison.OrdinalIgnoreCase))
        throw new UnauthorizedAccessException("Simulação de pagamento disponível somente para o usuário de teste.");

    var plano = (body.Plano ?? "").ToLowerInvariant();
    _ = LicencaPlanos.ValorFixo(plano); // valida que o plano existe
    var resultado = auth.RenovarLicenca(s.UserId, plano, "simulacao-teste");
    return Results.Ok(new { status = "approved", renewed = true, resultado });
});

// ── Controle de energia (reboot / desligar) ───────────────────────────────────
app.MapPost("/admin/reboot", (HttpRequest http) =>
{
    // Reiniciar/desligar o equipamento físico é permitido a qualquer usuário logado
    // (é a própria máquina em que ele está sentado, não uma ação administrativa remota).
    auth.RequireSession(http);
    Task.Delay(1500).ContinueWith(_ =>
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = "/bin/sudo",
            Arguments = "systemctl reboot",
            UseShellExecute = false,
            CreateNoWindow = true
        }));
    return Results.Ok(new { ok = true, msg = "Reiniciando o sistema em instantes…" });
});

app.MapPost("/admin/desligar", (HttpRequest http) =>
{
    auth.RequireSession(http);
    Task.Delay(1500).ContinueWith(_ =>
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = "/bin/sudo",
            Arguments = "systemctl poweroff",
            UseShellExecute = false,
            CreateNoWindow = true
        }));
    return Results.Ok(new { ok = true, msg = "Desligando o sistema em instantes…" });
});

// ── Rede (Wi-Fi / Cabo) ────────────────────────────────────────────────────────
app.MapGet("/sistema/rede", async (HttpRequest req) =>
{
    auth.RequireSession(req);
    return Results.Ok(await SystemCommands.RedeStatus());
});

app.MapGet("/sistema/rede/wifi", async (HttpRequest req) =>
{
    auth.RequireSession(req);
    return Results.Ok(await SystemCommands.WifiListar());
});

app.MapPost("/sistema/rede/wifi/conectar", async (WifiConectarRequest body, HttpRequest http) =>
{
    auth.RequirePermission(http, Perm.ConfigWrite);
    if (string.IsNullOrWhiteSpace(body.Ssid)) throw new InvalidOperationException("Informe o SSID da rede");
    var (ok, saida) = await SystemCommands.WifiConectar(body.Ssid, body.Senha ?? "");
    return ok ? Results.Ok(new { ok = true, msg = saida }) : Results.BadRequest(new { error = saida });
});

app.MapPost("/sistema/rede/cabo/renovar", async (HttpRequest http) =>
{
    auth.RequirePermission(http, Perm.ConfigWrite);
    var conexao = db.GetConfiguracao("rede_conexao_cabo") ?? "Wired connection 1";
    var (ok, saida) = await SystemCommands.CaboRenovar(conexao);
    return ok ? Results.Ok(new { ok = true, msg = saida }) : Results.BadRequest(new { error = saida });
});

// ── Impressoras (A4 / Térmica) ─────────────────────────────────────────────────
app.MapGet("/sistema/impressoras", async (HttpRequest req) =>
{
    auth.RequireSession(req);
    var (disponivel, erro, instaladas) = await SystemCommands.ImpressorasListar();
    return Results.Ok(new
    {
        disponivel,
        erro,
        instaladas,
        impressoraA4      = db.GetConfiguracao("impressora_a4") ?? "",
        impressoraTermica = db.GetConfiguracao("impressora_termica") ?? "",
        larguraTermica    = db.GetConfiguracao("impressora_termica_largura") ?? "42"
    });
});

app.MapPut("/sistema/impressoras/config", (ImpressorasConfigRequest body, HttpRequest http) =>
{
    auth.RequirePermission(http, Perm.ConfigWrite);
    db.SetConfiguracao("impressora_a4", body.ImpressoraA4 ?? "");
    db.SetConfiguracao("impressora_termica", body.ImpressoraTermica ?? "");
    if (!string.IsNullOrWhiteSpace(body.LarguraTermica))
    {
        var largura = int.TryParse(body.LarguraTermica, out var l) ? Math.Clamp(l, 32, 64) : 42;
        db.SetConfiguracao("impressora_termica_largura", largura.ToString());
    }
    return Results.Ok(new { ok = true });
});

app.MapPost("/sistema/impressoras/{nome}/testar", async (string nome, HttpRequest http, string tipo) =>
{
    auth.RequirePermission(http, Perm.ConfigWrite);
    var (ok, saida) = await SystemCommands.ImpressoraTestar(nome, tipo);
    return ok ? Results.Ok(new { ok = true, msg = saida }) : Results.BadRequest(new { error = saida });
});

// ── Navegador auxiliar (e-mail / consultas) ───────────────────────────────────
app.MapPost("/sistema/navegador/abrir", async (NavegadorAbrirRequest body, HttpRequest http) =>
{
    auth.RequireSession(http);
    var url = string.IsNullOrWhiteSpace(body.Url) ? (db.GetConfiguracao("email_url") ?? "https://mail.google.com") : body.Url!;
    var (ok, saida) = await SystemCommands.AbrirNavegador(url);
    return ok ? Results.Ok(new { ok = true, msg = saida }) : Results.BadRequest(new { error = saida });
});

app.Run();

// ═════════════════════════════════════════════════════════════════════════════
// PdvDb — camada SQLite
// ═════════════════════════════════════════════════════════════════════════════
sealed class PdvDb
{
    private readonly string _connStr;

    // Formatação pt-BR construída manualmente (sem depender de CultureInfo("pt-BR")):
    // o appliance roda com DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1 (economiza RAM/disco
    // não carregando dados ICU) — sob invariant mode, CultureInfo("pt-BR") lança
    // CultureNotFoundException. Um NumberFormatInfo próprio não toca em ICU e funciona
    // igual nos dois modos.
    private static readonly NumberFormatInfo PtBr = new()
    {
        NumberDecimalSeparator = ",",
        NumberGroupSeparator = ".",
        CurrencyDecimalSeparator = ",",
        CurrencyGroupSeparator = ".",
        CurrencySymbol = "R$",
        CurrencyPositivePattern = 2, // "R$ n"
        CurrencyNegativePattern = 9, // "-R$ n"
    };

    public PdvDb(string dataDir)
    {
        var path = Path.Combine(dataDir, "pdv.db");
        _connStr = $"Data Source={path}";
    }

    public void Initialize()
    {
        using var con = Open();
        con.Execute(@"
PRAGMA journal_mode=WAL;
PRAGMA foreign_keys=ON;
PRAGMA synchronous=NORMAL;

CREATE TABLE IF NOT EXISTS configuracoes (
  chave TEXT PRIMARY KEY,
  valor TEXT NOT NULL,
  updated_at TEXT NOT NULL DEFAULT (datetime('now'))
);

CREATE TABLE IF NOT EXISTS clientes (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  nome TEXT NOT NULL,
  documento TEXT,
  telefone TEXT,
  celular TEXT,
  email TEXT,
  logradouro TEXT,
  numero TEXT,
  bairro TEXT,
  cidade TEXT,
  estado TEXT,
  cep TEXT,
  observacoes TEXT,
  limite_credito REAL NOT NULL DEFAULT 0,
  desconto_percent REAL NOT NULL DEFAULT 0,
  ativo INTEGER NOT NULL DEFAULT 1,
  created_at TEXT NOT NULL DEFAULT (datetime('now')),
  updated_at TEXT NOT NULL DEFAULT (datetime('now')),
  created_by TEXT NOT NULL DEFAULT ''
);

CREATE TABLE IF NOT EXISTS clientes_historico (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  cliente_id INTEGER NOT NULL REFERENCES clientes(id),
  evento TEXT NOT NULL,
  detalhe TEXT,
  usuario TEXT NOT NULL,
  created_at TEXT NOT NULL DEFAULT (datetime('now'))
);

CREATE TABLE IF NOT EXISTS clientes_credito (
  cliente_id INTEGER PRIMARY KEY REFERENCES clientes(id),
  saldo REAL NOT NULL DEFAULT 0,
  updated_at TEXT NOT NULL DEFAULT (datetime('now'))
);

CREATE TABLE IF NOT EXISTS clientes_credito_movimentos (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  cliente_id INTEGER NOT NULL REFERENCES clientes(id),
  tipo TEXT NOT NULL,
  valor REAL NOT NULL,
  descricao TEXT,
  referencia TEXT,
  usuario TEXT NOT NULL,
  created_at TEXT NOT NULL DEFAULT (datetime('now'))
);

CREATE TABLE IF NOT EXISTS servicos (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  codigo TEXT NOT NULL UNIQUE,
  descricao TEXT NOT NULL,
  categoria TEXT NOT NULL DEFAULT '',
  preco REAL NOT NULL DEFAULT 0,
  ativo INTEGER NOT NULL DEFAULT 1,
  created_at TEXT NOT NULL DEFAULT (datetime('now')),
  updated_at TEXT NOT NULL DEFAULT (datetime('now'))
);

CREATE TABLE IF NOT EXISTS ordens_servico (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  numero TEXT NOT NULL UNIQUE,
  cliente_id INTEGER NOT NULL REFERENCES clientes(id),
  status TEXT NOT NULL DEFAULT 'aberta',
  data_entrada TEXT NOT NULL DEFAULT (date('now')),
  data_promessa TEXT,
  data_entrega TEXT,
  data_pagamento TEXT,
  valor_total REAL NOT NULL DEFAULT 0,
  desconto REAL NOT NULL DEFAULT 0,
  valor_final REAL NOT NULL DEFAULT 0,
  valor_pago REAL NOT NULL DEFAULT 0,
  metodo_pagamento TEXT,
  troco REAL NOT NULL DEFAULT 0,
  observacoes TEXT,
  motivo_cancelamento TEXT,
  usuario_entrada TEXT NOT NULL DEFAULT '',
  usuario_entrega TEXT,
  usuario_pagamento TEXT,
  created_at TEXT NOT NULL DEFAULT (datetime('now')),
  updated_at TEXT NOT NULL DEFAULT (datetime('now'))
);

CREATE TABLE IF NOT EXISTS os_itens (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  os_id INTEGER NOT NULL REFERENCES ordens_servico(id),
  servico_id INTEGER REFERENCES servicos(id),
  descricao TEXT NOT NULL,
  tipo_tecido TEXT,
  cor TEXT,
  marca TEXT,
  defeito TEXT,
  quantidade REAL NOT NULL DEFAULT 1,
  valor_unitario REAL NOT NULL DEFAULT 0,
  valor_total REAL NOT NULL DEFAULT 0,
  status TEXT NOT NULL DEFAULT 'pendente',
  observacao TEXT,
  created_at TEXT NOT NULL DEFAULT (datetime('now'))
);

CREATE TABLE IF NOT EXISTS os_historico (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  os_id INTEGER NOT NULL REFERENCES ordens_servico(id),
  evento TEXT NOT NULL,
  status_anterior TEXT,
  status_novo TEXT,
  detalhe TEXT,
  usuario TEXT NOT NULL,
  created_at TEXT NOT NULL DEFAULT (datetime('now'))
);

CREATE TABLE IF NOT EXISTS pagamentos (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  os_id INTEGER NOT NULL REFERENCES ordens_servico(id),
  metodo TEXT NOT NULL,
  valor REAL NOT NULL,
  troco REAL NOT NULL DEFAULT 0,
  usuario TEXT NOT NULL,
  created_at TEXT NOT NULL DEFAULT (datetime('now'))
);

CREATE TABLE IF NOT EXISTS caixa_sessoes (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  data TEXT NOT NULL,
  usuario TEXT NOT NULL,
  valor_abertura REAL NOT NULL DEFAULT 0,
  valor_contado REAL,
  status TEXT NOT NULL DEFAULT 'aberta',
  observacao_fechamento TEXT,
  created_at TEXT NOT NULL DEFAULT (datetime('now')),
  fechado_em TEXT
);

CREATE TABLE IF NOT EXISTS caixa_movimentos (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  sessao_id INTEGER NOT NULL REFERENCES caixa_sessoes(id),
  tipo TEXT NOT NULL,
  valor REAL NOT NULL,
  descricao TEXT,
  os_id INTEGER,
  usuario TEXT NOT NULL,
  created_at TEXT NOT NULL DEFAULT (datetime('now'))
);

CREATE TABLE IF NOT EXISTS financeiro (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  cliente_id INTEGER NOT NULL REFERENCES clientes(id),
  os_id INTEGER REFERENCES ordens_servico(id),
  tipo TEXT NOT NULL DEFAULT 'a_receber',
  status TEXT NOT NULL DEFAULT 'aberto',
  valor REAL NOT NULL,
  vencimento TEXT,
  data_recebimento TEXT,
  valor_recebido REAL,
  metodo_recebimento TEXT,
  observacao TEXT,
  usuario TEXT NOT NULL,
  created_at TEXT NOT NULL DEFAULT (datetime('now')),
  updated_at TEXT NOT NULL DEFAULT (datetime('now'))
);

CREATE TABLE IF NOT EXISTS legacy_records (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  tabela TEXT NOT NULL,
  legacy_pk TEXT,
  payload TEXT NOT NULL,
  imported_at TEXT NOT NULL DEFAULT (datetime('now')),
  UNIQUE(tabela, legacy_pk)
);

CREATE INDEX IF NOT EXISTS ix_os_cliente    ON ordens_servico(cliente_id);
CREATE INDEX IF NOT EXISTS ix_os_status     ON ordens_servico(status);
CREATE INDEX IF NOT EXISTS ix_os_data       ON ordens_servico(data_entrada);
CREATE INDEX IF NOT EXISTS ix_itens_os      ON os_itens(os_id);
CREATE INDEX IF NOT EXISTS ix_fin_cliente   ON financeiro(cliente_id);
CREATE INDEX IF NOT EXISTS ix_fin_status    ON financeiro(status);
CREATE INDEX IF NOT EXISTS ix_caixa_status  ON caixa_sessoes(status);

CREATE TABLE IF NOT EXISTS orcamentos (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  numero TEXT NOT NULL UNIQUE,
  cliente_id INTEGER NOT NULL REFERENCES clientes(id),
  status TEXT NOT NULL DEFAULT 'aberto',
  data_entrada TEXT NOT NULL DEFAULT (date('now')),
  data_promessa TEXT,
  data_validade TEXT,
  valor_total REAL NOT NULL DEFAULT 0,
  desconto REAL NOT NULL DEFAULT 0,
  valor_final REAL NOT NULL DEFAULT 0,
  observacoes TEXT,
  convertido_rol_id INTEGER REFERENCES ordens_servico(id),
  usuario_entrada TEXT NOT NULL DEFAULT '',
  created_at TEXT NOT NULL DEFAULT (datetime('now')),
  updated_at TEXT NOT NULL DEFAULT (datetime('now'))
);

CREATE TABLE IF NOT EXISTS orc_itens (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  orc_id INTEGER NOT NULL REFERENCES orcamentos(id),
  servico_id INTEGER REFERENCES servicos(id),
  descricao TEXT NOT NULL,
  tipo_tecido TEXT,
  cor TEXT,
  marca TEXT,
  quantidade REAL NOT NULL DEFAULT 1,
  valor_unitario REAL NOT NULL DEFAULT 0,
  valor_total REAL NOT NULL DEFAULT 0,
  observacao TEXT,
  created_at TEXT NOT NULL DEFAULT (datetime('now'))
);

CREATE TABLE IF NOT EXISTS agenda (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  rol_id INTEGER REFERENCES ordens_servico(id),
  orc_id INTEGER REFERENCES orcamentos(id),
  cliente_id INTEGER NOT NULL REFERENCES clientes(id),
  data_agendamento TEXT NOT NULL,
  hora_agendamento TEXT NOT NULL DEFAULT '09:00',
  duracao_minutos INTEGER NOT NULL DEFAULT 30,
  tipo TEXT NOT NULL DEFAULT 'entrega',
  observacao TEXT,
  status TEXT NOT NULL DEFAULT 'agendado',
  usuario TEXT NOT NULL,
  created_at TEXT NOT NULL DEFAULT (datetime('now'))
);

CREATE TABLE IF NOT EXISTS legacy_params (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  fonte TEXT NOT NULL,
  secao TEXT NOT NULL,
  chave TEXT NOT NULL,
  valor TEXT NOT NULL,
  UNIQUE(fonte, secao, chave)
);

CREATE TABLE IF NOT EXISTS legacy_coverage (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  area TEXT NOT NULL,
  item TEXT NOT NULL,
  fonte TEXT NOT NULL,
  status TEXT NOT NULL DEFAULT 'pendente',
  observacao TEXT,
  updated_by TEXT,
  updated_at TEXT NOT NULL DEFAULT (datetime('now')),
  UNIQUE(area, item, fonte)
);

CREATE INDEX IF NOT EXISTS ix_orc_cliente ON orcamentos(cliente_id);
CREATE INDEX IF NOT EXISTS ix_orc_status  ON orcamentos(status);
CREATE INDEX IF NOT EXISTS ix_agenda_data ON agenda(data_agendamento);
CREATE INDEX IF NOT EXISTS ix_legacy_coverage_area ON legacy_coverage(area);
CREATE INDEX IF NOT EXISTS ix_legacy_coverage_status ON legacy_coverage(status);

CREATE TABLE IF NOT EXISTS catalogos (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  tipo TEXT NOT NULL,
  codigo TEXT NOT NULL,
  descricao TEXT NOT NULL,
  ativo INTEGER NOT NULL DEFAULT 1,
  created_at TEXT NOT NULL DEFAULT (datetime('now'))
);
CREATE INDEX IF NOT EXISTS ix_catalogos_tipo ON catalogos(tipo);

CREATE TABLE IF NOT EXISTS indenizacoes (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  os_id INTEGER REFERENCES ordens_servico(id),
  cliente_id INTEGER NOT NULL REFERENCES clientes(id),
  descricao TEXT NOT NULL,
  valor REAL NOT NULL DEFAULT 0,
  status TEXT NOT NULL DEFAULT 'aberta',
  motivo TEXT,
  observacao TEXT,
  usuario TEXT NOT NULL,
  created_at TEXT NOT NULL DEFAULT (datetime('now')),
  updated_at TEXT NOT NULL DEFAULT (datetime('now'))
);
CREATE INDEX IF NOT EXISTS ix_inden_cliente ON indenizacoes(cliente_id);
CREATE INDEX IF NOT EXISTS ix_inden_status ON indenizacoes(status);

CREATE TABLE IF NOT EXISTS guardaroupa (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  cliente_id INTEGER NOT NULL REFERENCES clientes(id),
  descricao TEXT NOT NULL,
  categoria TEXT,
  cor TEXT,
  marca TEXT,
  quantidade INTEGER NOT NULL DEFAULT 1,
  localizacao TEXT,
  data_entrada TEXT NOT NULL DEFAULT (date('now')),
  data_saida TEXT,
  status TEXT NOT NULL DEFAULT 'guardado',
  observacao TEXT,
  usuario TEXT NOT NULL,
  created_at TEXT NOT NULL DEFAULT (datetime('now'))
);
CREATE INDEX IF NOT EXISTS ix_guardaroupa_cliente ON guardaroupa(cliente_id);
CREATE INDEX IF NOT EXISTS ix_guardaroupa_status ON guardaroupa(status);

CREATE TABLE IF NOT EXISTS terceirizacao (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  os_id INTEGER REFERENCES ordens_servico(id),
  fornecedor TEXT NOT NULL,
  descricao TEXT NOT NULL,
  valor REAL NOT NULL DEFAULT 0,
  data_envio TEXT NOT NULL DEFAULT (date('now')),
  data_retorno_prevista TEXT,
  data_retorno TEXT,
  status TEXT NOT NULL DEFAULT 'enviado',
  observacao TEXT,
  usuario TEXT NOT NULL,
  created_at TEXT NOT NULL DEFAULT (datetime('now'))
);
CREATE INDEX IF NOT EXISTS ix_terc_status ON terceirizacao(status);
CREATE INDEX IF NOT EXISTS ix_terc_os ON terceirizacao(os_id);

CREATE TABLE IF NOT EXISTS fidelidade (
  cliente_id INTEGER PRIMARY KEY REFERENCES clientes(id),
  pontos INTEGER NOT NULL DEFAULT 0,
  updated_at TEXT NOT NULL DEFAULT (datetime('now'))
);

CREATE TABLE IF NOT EXISTS fidelidade_regras (
  categoria TEXT PRIMARY KEY,
  pontos_por_venda INTEGER NOT NULL DEFAULT 0,
  updated_by TEXT,
  updated_at TEXT NOT NULL DEFAULT (datetime('now'))
);

CREATE TABLE IF NOT EXISTS fidelidade_movimentos (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  cliente_id INTEGER NOT NULL REFERENCES clientes(id),
  pontos INTEGER NOT NULL,
  tipo TEXT NOT NULL,
  referencia TEXT,
  observacao TEXT,
  usuario TEXT NOT NULL,
  created_at TEXT NOT NULL DEFAULT (datetime('now'))
);
CREATE INDEX IF NOT EXISTS ix_fidel_cli ON fidelidade_movimentos(cliente_id);

CREATE TABLE IF NOT EXISTS doacoes (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  os_id INTEGER REFERENCES ordens_servico(id),
  cliente_id INTEGER NOT NULL REFERENCES clientes(id),
  descricao TEXT NOT NULL,
  valor REAL NOT NULL DEFAULT 0,
  data_doacao TEXT NOT NULL DEFAULT (date('now')),
  status TEXT NOT NULL DEFAULT 'pendente',
  motivo_cancelamento TEXT,
  observacao TEXT,
  usuario TEXT NOT NULL,
  created_at TEXT NOT NULL DEFAULT (datetime('now'))
);
CREATE INDEX IF NOT EXISTS ix_doacoes_cliente ON doacoes(cliente_id);
CREATE INDEX IF NOT EXISTS ix_doacoes_status ON doacoes(status);
");

        EnsureLegacyColumns(con);
        con.Execute("BEGIN IMMEDIATE");
        try
        {
            ImportLegacyParams(con);
            ImportLegacyCoverage(con);

            // Zera os pontos de fidelidade herdados do legado, uma única vez —
            // ponto passa a contar só a partir de novas vendas com regra por
            // categoria configurada (fidelidade_regras). Marcador em
            // 'configuracoes' evita repetir a zeragem em reinicializações
            // futuras (não pode ficar zerando pontos ganhos depois disso).
            using var chkZerado = con.CreateCommand();
            chkZerado.CommandText = "SELECT COUNT(*) FROM configuracoes WHERE chave='fidelidade_zerada_legado'";
            if ((long)(chkZerado.ExecuteScalar() ?? 0L) == 0)
            {
                con.Execute("UPDATE fidelidade SET pontos=0, updated_at=datetime('now')");
                con.Execute(@"INSERT OR IGNORE INTO configuracoes(chave,valor) VALUES('fidelidade_zerada_legado', datetime('now'))");
            }

            // Migração: instalações antigas guardavam o token de vendas em 'mp_access_token'.
            // Copia uma única vez para a chave nova (mp_luci_access_token), antes do seed abaixo
            // criar 'mp_luci_access_token' vazio (senão o OR IGNORE do seed venceria a corrida e
            // a migração nunca copiaria o valor real).
            using var migMp = con.CreateCommand();
            migMp.CommandText = @"
INSERT OR IGNORE INTO configuracoes(chave, valor)
SELECT 'mp_luci_access_token', valor FROM configuracoes WHERE chave='mp_access_token' AND valor<>''";
            migMp.ExecuteNonQuery();

            // Seed configuracoes — usa dados do INI legado quando disponíveis
            using var cmd = con.CreateCommand();
            cmd.CommandText = @"
INSERT OR IGNORE INTO configuracoes(chave, valor) VALUES
 ('empresa_nome',      COALESCE((SELECT valor FROM legacy_params WHERE secao='INDUSTRIAL' AND chave='TitRol'      LIMIT 1), 'Ateliê da Luci')),
 ('empresa_telefone',  COALESCE((SELECT valor FROM legacy_params WHERE secao='INDUSTRIAL' AND chave='CabRolFone'  LIMIT 1), '')),
 ('empresa_endereco',  COALESCE((SELECT valor FROM legacy_params WHERE secao='INDUSTRIAL' AND chave='TitRol2'     LIMIT 1), '')),
 ('empresa_cidade',    COALESCE((SELECT valor FROM legacy_params WHERE secao='INDUSTRIAL' AND chave='CabRolEnd'   LIMIT 1), 'São Paulo')),
 ('empresa_cep',       COALESCE((SELECT valor FROM legacy_params WHERE secao='INDUSTRIAL' AND chave='CabRolBai'   LIMIT 1), '')),
 ('empresa_cnpj',      ''),
 ('empresa_iss_percent', COALESCE((SELECT valor FROM legacy_params WHERE secao='NOTAFISCAL' AND chave='PorISS'   LIMIT 1), '5')),
 ('prazo_entrega_dias','3'),
 ('moeda',             'R$'),
 ('versao',            '1.0.0'),
 ('email_url',         'https://mail.google.com'),
 ('impressora_a4',     ''),
 ('impressora_termica',''),
 ('rede_conexao_cabo', 'Wired connection 1'),
 ('mp_luci_access_token',    ''),
 ('mp_rogerio_access_token', '');";
            cmd.ExecuteNonQuery();

            ImportLegacyClientes(con);
            ImportLegacyRols(con);
            ImportLegacyItens(con);
            ImportLegacyFinanceiro(con);
            ImportLegacyFileCatalog(con);
            ImportLegacyCredito(con);
            ImportLegacyPontos(con);
            BackfillLegacyNFe(con);
            BackfillLegacyObs2(con);
            BackfillValorTerceiro(con);
            BackfillHoraPagamento(con);
            BackfillMovCabExtras(con);
            BackfillMotivoDesconto(con);
            BackfillItemDescricoes(con);

            // Seed categorias e serviços padrão ou tabela de preços migrada
            SeedServicos(con);
            SeedCatalogos(con);
            con.Execute("COMMIT");
        }
        catch
        {
            con.Execute("ROLLBACK");
            throw;
        }
    }

    private static void EnsureLegacyColumns(SqliteConnection con)
    {
        EnsureColumn(con, "clientes", "legacy_codigo", "TEXT");
        EnsureColumn(con, "clientes", "data_nascimento", "TEXT");
        EnsureColumn(con, "clientes", "cartao_fidelidade", "TEXT");
        EnsureColumn(con, "clientes", "contato", "TEXT");
        EnsureColumn(con, "ordens_servico", "legacy_rol", "TEXT");
        EnsureColumn(con, "ordens_servico", "legacy_payload", "TEXT");
        EnsureColumn(con, "os_itens", "legacy_payload", "TEXT");
        EnsureColumn(con, "financeiro", "legacy_payload", "TEXT");
        EnsureColumn(con, "os_itens", "peso", "REAL");
        EnsureColumn(con, "os_itens", "identificacao", "TEXT");
        EnsureColumn(con, "os_itens", "localizacao", "TEXT");
        EnsureColumn(con, "os_itens", "valor_terceiro", "REAL");
        EnsureColumn(con, "os_itens", "obs2", "TEXT");
        EnsureColumn(con, "ordens_servico", "numero_nota_fiscal", "TEXT");
        EnsureColumn(con, "ordens_servico", "hora_pagamento", "TEXT");
        EnsureColumn(con, "ordens_servico", "nota_numero", "TEXT");
        EnsureColumn(con, "financeiro", "legacy_numfat", "TEXT");
        EnsureColumn(con, "financeiro", "legacy_numnot", "TEXT");
        EnsureColumn(con, "ordens_servico", "total_pecas", "INTEGER");
        EnsureColumn(con, "ordens_servico", "localizacao_rol", "TEXT");
        EnsureColumn(con, "ordens_servico", "data_cancelamento", "TEXT");
        EnsureColumn(con, "ordens_servico", "motivo_desconto", "TEXT");
        con.Execute("CREATE UNIQUE INDEX IF NOT EXISTS ux_clientes_legacy_codigo ON clientes(legacy_codigo) WHERE legacy_codigo IS NOT NULL");
        con.Execute("CREATE UNIQUE INDEX IF NOT EXISTS ux_ordens_legacy_rol ON ordens_servico(legacy_rol) WHERE legacy_rol IS NOT NULL");
    }

    private static void EnsureColumn(SqliteConnection con, string table, string column, string definition)
    {
        using var check = con.CreateCommand();
        check.CommandText = $"PRAGMA table_info({table})";
        using var r = check.ExecuteReader();
        while (r.Read())
            if (r.GetString(1).Equals(column, StringComparison.OrdinalIgnoreCase))
                return;
        r.Close();
        using var alter = con.CreateCommand();
        alter.CommandText = $"ALTER TABLE {table} ADD COLUMN {column} {definition}";
        alter.ExecuteNonQuery();
    }

    private static void SeedServicos(SqliteConnection con)
    {
        using var chk = con.CreateCommand();
        chk.CommandText = "SELECT COUNT(*) FROM servicos";
        var count = (long)(chk.ExecuteScalar() ?? 0L);
        if (count > 0) return;

        if (ImportLegacyServicos(con)) return;

        var servicos = new[]
        {
            ("ALM01","ALMOFADA - FEITIO COM BARRADO","ALMOFADA",60.00m),
            ("ALM02","ALMOFADA - FEITIO SIMPLES","ALMOFADA",45.00m),
            ("BAR01","BARRA CALCA JEANS","CALCA",20.00m),
            ("BAR02","BARRA CALCA SOCIAL","CALCA",25.00m),
            ("BAR03","BARRA VESTIDO","VESTIDO",30.00m),
            ("BAR04","BARRA SAIA","SAIA",25.00m),
            ("AJU01","AJUSTE CINTO/CINTURA","AJUSTE",35.00m),
            ("AJU02","AJUSTE LATERAL","AJUSTE",30.00m),
            ("AJU03","AJUSTE OMBRO","AJUSTE",40.00m),
            ("AJU04","AJUSTE MANGA","AJUSTE",35.00m),
            ("BOT01","TROCA BOTAO","CONSERTO",10.00m),
            ("BOT02","COLOCAR ZIPER","CONSERTO",35.00m),
            ("BOT03","CONSERTO GERAL","CONSERTO",25.00m),
            ("BOR01","BORDADO SIMPLES","BORDADO",50.00m),
            ("BOR02","BORDADO ELABORADO","BORDADO",80.00m),
            ("LAV01","LAVAGEM SIMPLES","LAVAGEM",20.00m),
            ("LAV02","LAVAGEM ESPECIAL","LAVAGEM",40.00m),
            ("PAS01","PASSADORIA","PASSADORIA",15.00m),
            ("PAS02","PASSADORIA ESPECIAL","PASSADORIA",25.00m),
            ("COS01","COSTURA GERAL","COSTURA",35.00m),
            ("COS02","FORRO","COSTURA",60.00m),
            ("FEI01","FEITIO BLUSA","FEITIO",120.00m),
            ("FEI02","FEITIO CALCA","FEITIO",150.00m),
            ("FEI03","FEITIO VESTIDO","FEITIO",200.00m),
            ("FEI04","FEITIO SAIA","FEITIO",100.00m),
        };

        foreach (var (cod, desc, cat, preco) in servicos)
        {
            using var ins = con.CreateCommand();
            ins.CommandText = "INSERT OR IGNORE INTO servicos(codigo,descricao,categoria,preco) VALUES($c,$d,$g,$p)";
            ins.Parameters.AddWithValue("$c", cod);
            ins.Parameters.AddWithValue("$d", desc);
            ins.Parameters.AddWithValue("$g", cat);
            ins.Parameters.AddWithValue("$p", (double)preco);
            ins.ExecuteNonQuery();
        }
    }

    private static void SeedCatalogos(SqliteConnection con)
    {
        using var chk = con.CreateCommand();
        chk.CommandText = "SELECT COUNT(*) FROM catalogos";
        if ((long)(chk.ExecuteScalar() ?? 0L) > 0) return;

        var seed = new[]
        {
            ("defeito","D01","RASGADO"),("defeito","D02","MANCHADO"),("defeito","D03","FURO"),
            ("defeito","D04","BOTAO FALTANDO"),("defeito","D05","ZIPER QUEBRADO"),
            ("defeito","D06","DESFEIADO"),("defeito","D07","ENCOLHIDO"),
            ("defeito","D08","DESFIADO"),("defeito","D09","DESCOSTURADO"),
            ("defeito","D10","DESBOTADO"),("defeito","D11","ESTICADO"),
            ("cor","C01","PRETO"),("cor","C02","BRANCO"),("cor","C03","CINZA"),
            ("cor","C04","AZUL"),("cor","C05","AZUL MARINHO"),("cor","C06","VERMELHO"),
            ("cor","C07","VERDE"),("cor","C08","AMARELO"),("cor","C09","MARROM"),
            ("cor","C10","ROSA"),("cor","C11","LARANJA"),("cor","C12","LILAS"),
            ("cor","C13","BEGE"),("cor","C14","COLORIDO"),("cor","C15","ESTAMPADO"),
            ("tipo_tecido","T01","ALGODAO"),("tipo_tecido","T02","POLIESTER"),
            ("tipo_tecido","T03","LINHO"),("tipo_tecido","T04","SEDA"),
            ("tipo_tecido","T05","MALHA"),("tipo_tecido","T06","JEANS"),
            ("tipo_tecido","T07","TRICOT"),("tipo_tecido","T08","NYLON"),
            ("tipo_tecido","T09","VISCOSE"),("tipo_tecido","T10","CREPE"),
            ("tipo_tecido","T11","CETIM"),("tipo_tecido","T12","OXFORD"),
            ("localizacao","L01","PRATELEIRA-A1"),("localizacao","L02","PRATELEIRA-A2"),
            ("localizacao","L03","PRATELEIRA-B1"),("localizacao","L04","PRATELEIRA-B2"),
            ("localizacao","L05","CABIDE-1"),("localizacao","L06","CABIDE-2"),
            ("localizacao","L07","CAIXA-1"),("localizacao","L08","CAIXA-2"),
            ("localizacao","L09","DEPOSITO"),("localizacao","L10","BALCAO"),
            ("processo","P01","LAVAGEM"),("processo","P02","PASSADORIA"),
            ("processo","P03","TINGIMENTO"),("processo","P04","CLAREAMENTO"),
            ("processo","P05","IMPERMEABILIZACAO"),
            ("fornecedor","F01","TERCEIRO GERAL"),
            ("tingimento_servico","TS01","TINGIMENTO BASICO"),
            ("tingimento_servico","TS02","TINGIMENTO ESPECIAL"),
            ("tingimento_servico","TS03","TIE-DYE"),
            ("tingimento_servico","TS04","CLAREAMENTO/ALVEJAMENTO"),
            ("tingimento_servico","TS05","TINGIMENTO DEGRADÊ"),
            ("tingimento_tipo","TT01","TINTA DIRETA"),
            ("tingimento_tipo","TT02","TINTA REATIVA"),
            ("tingimento_tipo","TT03","TINTA DISPERSA"),
            ("tingimento_tipo","TT04","TINTA ÁCIDA"),
            ("tingimento_tipo","TT05","TINTA BÁSICA")
        };
        foreach (var (tipo, cod, desc) in seed)
        {
            using var cmd = con.CreateCommand();
            cmd.CommandText = "INSERT OR IGNORE INTO catalogos(tipo,codigo,descricao) VALUES($t,$c,$d)";
            cmd.Parameters.AddWithValue("$t", tipo);
            cmd.Parameters.AddWithValue("$c", cod);
            cmd.Parameters.AddWithValue("$d", desc);
            cmd.ExecuteNonQuery();
        }
    }

    private static void ImportLegacyParams(SqliteConnection con)
    {
        using var chk = con.CreateCommand();
        chk.CommandText = "SELECT COUNT(*) FROM legacy_params";
        if ((long)(chk.ExecuteScalar() ?? 0L) > 0) return;

        foreach (var file in new[] { "EquLav00001.ini", "Argox.ini" })
        {
            var path = FindImportFile(file);
            if (path is null) continue;
            var section = "GERAL";
            foreach (var raw in File.ReadLines(path, Encoding.Default))
            {
                var line = raw.Trim();
                if (line.Length == 0 || line.StartsWith(";") || line.StartsWith("#")) continue;
                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    section = line.Trim('[', ']').Trim();
                    continue;
                }
                var ix = line.IndexOf('=');
                if (ix <= 0) continue;
                var key = line[..ix].Trim();
                var value = line[(ix + 1)..].Trim();
                using var cmd = con.CreateCommand();
                cmd.CommandText = @"INSERT OR IGNORE INTO legacy_params(fonte,secao,chave,valor)
VALUES($f,$s,$k,$v)";
                cmd.Parameters.AddWithValue("$f", file);
                cmd.Parameters.AddWithValue("$s", section);
                cmd.Parameters.AddWithValue("$k", key);
                cmd.Parameters.AddWithValue("$v", value);
                cmd.ExecuteNonQuery();
            }
        }

        var critical = new[]
        {
            ("regra", "Atalho F7 pagamento", "EquLav00001.ini/HabF7Pagto", "implementado"),
            ("regra", "Alteracao de preco F4", "EquLav00001.ini/F4AlteraPreco", "pendente"),
            ("regra", "Menu lancamento F2", "EquLav00001.ini/ChkBox_F2MenLan", "implementado"),
            ("regra", "Motivo obrigatorio desconto", "EquLav00001.ini/ObrigMotDesc", "implementado"),
            ("impressao", "ROL 2 vias", "EquLav00001.ini/QdeViasRol", "implementado_parcial"),
            ("impressao", "Etiqueta Argox PPLB", "Argox.ini/ModeloFitas", "implementado_parcial"),
            ("permissao", "Permissao por rotina/menu", "Nivel.DB/mapa-permissoes", "pendente"),
            ("ui", "Paridade visual tela a tela", "ui-parity-evidence.json", "pendente"),
            ("fiscal", "SAT/NFE/RPS", "SAT.exe/NFE.exe/EquLav00001.ini", "pendente")
        };
        foreach (var (area, item, source, status) in critical)
            UpsertCoverage(con, area, item, source, status, "Item critico P0/P1 rastreado automaticamente.");
    }

    private static void ImportLegacyCoverage(SqliteConnection con)
    {
        var files = new[]
        {
            ("menu", "mapa-menus-submenus-acoes-consolidado.csv", "MenuOuAcao"),
            ("permissao", "mapa-permissoes-ui-consolidado.csv", "OperacaoPermissao"),
            ("relatorio", "matriz-relatorios.csv", "RelatorioOuImpressao")
        };

        foreach (var (area, file, itemColumn) in files)
        {
            var path = FindImportFile(file);
            if (path is null) continue;
            var lines = File.ReadLines(path, Encoding.UTF8).Take(2500).ToArray();
            if (lines.Length < 2) continue;
            var header = SplitCsv(lines[0]);
            var itemIx = Array.FindIndex(header, h => h.Equals(itemColumn, StringComparison.OrdinalIgnoreCase));
            if (itemIx < 0) itemIx = Math.Min(2, header.Length - 1);
            foreach (var line in lines.Skip(1))
            {
                var cols = SplitCsv(line);
                if (cols.Length <= itemIx) continue;
                var item = Clean(cols[itemIx]);
                if (item.Length < 3) continue;
                UpsertCoverage(con, area, item, file, InferCoverageStatus(area, item), null);
            }
        }
    }

    private static string[] SplitCsv(string line)
    {
        var parts = new List<string>();
        var sb = new StringBuilder();
        var quoted = false;
        for (var i = 0; i < line.Length; i++)
        {
            var c = line[i];
            if (c == '"') { quoted = !quoted; continue; }
            if (c == ',' && !quoted) { parts.Add(sb.ToString()); sb.Clear(); continue; }
            sb.Append(c);
        }
        parts.Add(sb.ToString());
        return parts.ToArray();
    }

    private static readonly Dictionary<string, string> _opStatus = new(StringComparer.OrdinalIgnoreCase)
    {
        // ── IMPLEMENTADO ────────────────────────────────────────────────
        ["AlteraCliente1"]               = "implementado",
        ["CaixaDiaDia1"]                 = "implementado",
        ["Cancelamento2"]                = "implementado",
        ["CancPag"]                      = "implementado",
        ["CFAbertura1"]                  = "implementado",
        ["CFFechamento1"]                = "implementado",
        ["CFFechamentoParcial1"]         = "implementado",
        ["CFFundodeCaixa1"]              = "implementado",
        ["CFSangriaCaixa1"]              = "implementado",
        ["ClientesCad1"]                 = "implementado",
        ["CobrancaDiversas"]             = "implementado",
        ["ConIntEntrega1"]               = "implementado",
        ["ControledeCaixa1"]             = "implementado",
        ["ControledeLavagem1"]           = "implementado",
        ["Cores1"]                       = "implementado",
        ["Crditos2"]                     = "implementado",
        ["CreditosemAberto1"]            = "implementado",
        ["Defeitos1"]                    = "implementado",
        ["DescontoPorc1"]                = "implementado",
        ["DescontoporValor1"]            = "implementado",
        ["DescontosnoRol1"]              = "implementado",
        ["Ed_ValPag"]                    = "implementado",
        ["Encerramento1"]                = "implementado",
        ["EncerramentoParcial1"]         = "implementado",
        ["EntradaRol1"]                  = "implementado",
        ["EntregaporPecas1"]             = "implementado",
        ["EntregaRol"]                   = "implementado",
        ["EntregaVariosRol1"]            = "implementado",
        ["EntregaVariosRol2"]            = "implementado",
        ["Etiquetas1"]                   = "implementado",
        ["ExtratoCliente1"]              = "implementado",
        ["FechamentodeCaixa1"]           = "implementado",
        ["FormadePagamento1"]            = "implementado",
        ["Inclusao1"]                    = "implementado",
        ["LancamentoRol1"]               = "implementado",
        ["Listagem1"]                    = "implementado",
        ["Localizacao1"]                 = "implementado",
        ["MarcaEntrega"]                 = "implementado",
        ["Marcas1"]                      = "implementado",
        ["MenuEntradaRol1"]              = "implementado",
        ["MenuLancamentoRol1"]           = "implementado",
        ["MovimentoAnalitico1"]          = "implementado",
        ["MovimentoPorServicosExecutados1"] = "implementado",
        ["MovimentoPorServicoSintetico1"] = "implementado",
        ["MovimentoServicoMensal1"]      = "implementado",
        ["MovimentoSintetico1"]          = "implementado",
        ["Pagamento"]                    = "implementado",
        ["PagamentodeVariosRols1"]       = "implementado",
        ["PagamentoRol1"]                = "implementado",
        ["Parametros1"]                  = "implementado",
        ["PrevisoEntrega1"]              = "implementado",
        ["ReciboCaixa1"]                 = "implementado",
        ["Reemisso2"]                    = "implementado",
        ["RelatConfRol1"]                = "implementado",
        ["RelatorioparaEntrega1"]        = "implementado",
        ["RelControlePontos1"]           = "implementado",
        ["RelFatPrecos1"]                = "implementado",
        ["RelFreCli1"]                   = "implementado",
        ["Resumida1"]                    = "implementado",
        ["RolsCancelados1"]              = "implementado",
        ["ROLsemAberto1"]                = "implementado",
        ["ROlsPagos1"]                   = "implementado",
        ["SitRol"]                       = "implementado",
        ["SituacaodoRols1"]              = "implementado",
        ["TabeladePreco1"]               = "implementado",
        ["Terceirizacao1"]               = "implementado",
        ["TiposdeServico1"]              = "implementado",
        ["TiposdeTecido1"]               = "implementado",
        ["Usuarios1"]                    = "implementado",
        ["VerificaLocalizaoporPeas1"]    = "implementado",
        // ── IMPLEMENTADO_PARCIAL ─────────────────────────────────────────
        ["AnaMovDia1"]                   = "implementado_parcial",
        ["CartadeCobranca1"]             = "implementado_parcial",
        ["MovimentoporDiadeSemana1"]     = "implementado_parcial",
        ["MovProAna1"]                   = "implementado_parcial",
        ["MovProAnaSer1"]                = "implementado_parcial",
        ["MovProdServ1"]                 = "implementado_parcial",
        ["MovProSin1"]                   = "implementado_parcial",
        ["PontosEntregues1"]             = "implementado_parcial",
        ["ResFatDiaDia1"]                = "implementado_parcial",
        ["ResumoPorEmpree1"]             = "implementado_parcial",
        ["RSL1"]                         = "implementado_parcial",
        // ── NAO_APLICAVEL ────────────────────────────────────────────────
        ["Alterao1"]                     = "nao_aplicavel",
        ["Anotacoes1"]                   = "nao_aplicavel",
        ["AtuEstrutura1"]                = "nao_aplicavel",
        ["CancAdi"]                      = "nao_aplicavel",
        ["CancelamentodaNota1"]          = "nao_aplicavel",
        ["CancelamentodeNotasIntervalo1"]= "nao_aplicavel",
        ["CancelamentoNotaFiscal1"]      = "nao_aplicavel",
        ["Caractersticas1"]              = "nao_aplicavel",
        ["CFDestravaImpressoraFiscal1"]  = "nao_aplicavel",
        ["CFHorarioVerao1"]              = "nao_aplicavel",
        ["CFLeituraMemriaFiscal1"]       = "nao_aplicavel",
        ["CFLeituraX1"]                  = "nao_aplicavel",
        ["ComissaoPagamento1"]           = "nao_aplicavel",
        ["ComissoRol1"]                  = "nao_aplicavel",
        ["CondicaodePagamento1"]         = "nao_aplicavel",
        ["ContasRapidas1"]               = "nao_aplicavel",
        ["ControledeMetas1"]             = "nao_aplicavel",
        ["Criticas1"]                    = "nao_aplicavel",
        ["Custos1"]                      = "nao_aplicavel",
        ["Delivery1"]                    = "nao_aplicavel",
        ["Devoluo1"]                     = "nao_aplicavel",
        ["DevoluoPagamento1"]            = "nao_aplicavel",
        ["Ed_PorcDescontoPag"]           = "nao_aplicavel",
        ["EmissaodeCartao1"]             = "nao_aplicavel",
        ["EmitirNotaFiscal1"]            = "nao_aplicavel",
        ["EntradaEstoque1"]              = "nao_aplicavel",
        ["EnviaEmail1"]                  = "nao_aplicavel",
        ["ExcluiConfiguraoColunas1"]     = "nao_aplicavel",
        ["Exportacao1"]                  = "nao_aplicavel",
        ["Faturamento1"]                 = "nao_aplicavel",
        ["Feriados1"]                    = "nao_aplicavel",
        ["FiliaisMatriz1"]               = "nao_aplicavel",
        ["Filialmatriz1"]                = "nao_aplicavel",
        ["GravaColunas1"]                = "nao_aplicavel",
        ["GrupoEntradas1"]               = "nao_aplicavel",
        ["Importacao1"]                  = "nao_aplicavel",
        ["InformaRolnoEstoque1"]         = "nao_aplicavel",
        ["Lanamentos1"]                  = "nao_aplicavel",
        ["LogSistema1"]                  = "nao_aplicavel",
        ["MalaDiretaRemetente1"]         = "nao_aplicavel",
        ["Manutencao1"]                  = "nao_aplicavel",
        ["MenuRetiradas1"]               = "nao_aplicavel",
        ["MovGrupoClientes1"]            = "nao_aplicavel",
        ["MovimentoAnaliticoEntrada1"]   = "nao_aplicavel",
        ["MovimentoEntradaSaidaSinttico1"]= "nao_aplicavel",
        ["MovimentoPorServicosPeso1"]    = "nao_aplicavel",
        ["MovimentoProduto1"]            = "nao_aplicavel",
        ["MovimentoProdutoMensal1"]      = "nao_aplicavel",
        ["MovimentoResumidoNotaFiscal1"] = "nao_aplicavel",
        ["MovimentoSintticoEntrada1"]    = "nao_aplicavel",
        ["NotaFiscal1"]                  = "nao_aplicavel",
        ["Passadoria1"]                  = "nao_aplicavel",
        ["PgtosporTabela1"]              = "nao_aplicavel",
        ["RelEstoquenoCli1"]             = "nao_aplicavel",
        ["ResFatNotFis1"]                = "nao_aplicavel",
        ["ResTipTabEmp1"]                = "nao_aplicavel",
        ["ResTipTabGru1"]                = "nao_aplicavel",
        ["SaldoControleEntrada1"]        = "nao_aplicavel",
        ["SB_MudaValBase"]               = "nao_aplicavel",
        ["TiposdeDescontos1"]            = "nao_aplicavel",
        ["TiposdeEntrada1"]              = "nao_aplicavel",
        ["TransporteporNota1"]           = "nao_aplicavel",
        ["VoltarRolaProduo1"]            = "nao_aplicavel",
    };

    private static bool IsGarbageItem(string item)
    {
        if (string.IsNullOrWhiteSpace(item) || item.Length < 4) return true;
        var c0 = item[0];
        // Começa com símbolo/pontuação (artefatos do binário Delphi)
        if ("!#$%()\\:*+[]{}@&^~|<>".Contains(c0)) return true;
        var x = item.ToLowerInvariant();
        // Referências de campo/banco do Firebird/Paradox
        if (x.Contains(".db.") || x.Contains("rdb$") || x.Contains(".displaytext") ||
            x.Contains(".displayvalue") || x.Contains(".displaylabel")) return true;
        // Caminhos de arquivo e URLs
        if (x.Contains("s:\\") || x.Contains("\\equipexe\\") || x.Contains("http://") ||
            x.Contains(".rav") || x.Contains(".bmp") || x.Contains(".ini") ||
            x.Contains(".exe") || x.Contains(".dll")) return true;
        // Strings de erro/parser do Delphi
        if (x.Contains("eparser") || x.Contains("_token_err") || x.Contains("_name_err") ||
            x.Contains("duplicate_notation")) return true;
        // Handlers de click duplicados (AlteraCliente1Click', AlteraCliente18, etc.)
        if (System.Text.RegularExpressions.Regex.IsMatch(item, @"\d{2,}Click|Click['""$@#]")) return true;
        // Strings truncadas com reticências ou apenas símbolos
        if (item.EndsWith("...") || item.StartsWith("---") || item.All(ch => !char.IsLetterOrDigit(ch))) return true;
        return false;
    }

    private static string InferCoverageStatus(string area, string item)
    {
        // Lookup direto no dicionário de operações conhecidas
        if (_opStatus.TryGetValue(item, out var mapped)) return mapped;

        // Lixo extraído do binário → não aplicável
        if (IsGarbageItem(item)) return "nao_aplicavel";

        var x = item.ToLowerInvariant();

        if (area == "relatorio")
        {
            // Relatórios implementados (palavras-chave nos nomes extraídos do binário)
            if (x.Contains("movimento") && (x.Contains("dia") || x.Contains("periodo") || x.Contains("período") || x.Contains("sintet")))
                return "implementado";
            if (x.Contains("rol") && (x.Contains("aberto") || x.Contains("entrega") || x.Contains("pago") || x.Contains("cancelad") || x.Contains("situac")))
                return "implementado";
            if (x.Contains("caixa") && (x.Contains("dia") || x.Contains("fecha") || x.Contains("resumo") || x.Contains("fundo") || x.Contains("sangria")))
                return "implementado";
            if (x.Contains("debito") || x.Contains("débito") || (x.Contains("cliente") && x.Contains("deve")))
                return "implementado";
            if (x.Contains("frequen") || x.Contains("frequent"))
                return "implementado";
            if ((x.Contains("servico") || x.Contains("serviço")) && (x.Contains("period") || x.Contains("preco") || x.Contains("preço") || x.Contains("execut")))
                return "implementado";
            if (x.Contains("entrega") && (x.Contains("relat") || x.Contains("prev")))
                return "implementado";
            if (x.Contains("fidelidade") || x.Contains("ponto"))
                return "implementado";
            // Relatórios fiscais/estoque — não aplicáveis
            if (x.Contains("fiscal") || x.Contains("nota") || x.Contains("estoque") ||
                x.Contains("faturamento") || x.Contains("comissao") || x.Contains("comissão") ||
                x.Contains("produto") || x.Contains("passadoria") || x.Contains("centro de custo"))
                return "nao_aplicavel";
            // Strings sem contexto de relatório real → não aplicável (ruído do binário)
            return "nao_aplicavel";
        }

        if (area == "menu" || area == "permissao")
        {
            // Domínios inteiramente não aplicáveis
            if (x.Contains("fiscal") || x.Contains("nota fiscal") || x.Contains("estoque") ||
                x.Contains("delivery") || x.Contains("comiss") || x.Contains("filial") ||
                x.Contains("envio de email") || x.Contains("envia email") || x.Contains("feriado") ||
                x.Contains("passadoria") || x.Contains("faturamento") || x.Contains("mala direta") ||
                x.Contains("importacao") || x.Contains("importação") || x.Contains("exportacao") ||
                x.Contains("exportação") || x.Contains("horario de verao") || x.Contains("horário de verão") ||
                x.Contains("manutencao") || x.Contains("manutenção") || x.Contains("centro de custo") ||
                x.Contains("delivery") || x.Contains("sat") || x.Contains("nfe") || x.Contains("nf-e"))
                return "nao_aplicavel";
            // Operações de click duplicadas (Delphi event handlers)
            if (item.EndsWith("Click") || item.EndsWith("1Click") || item.EndsWith("4"))
                return "nao_aplicavel";
            // Módulos implementados (por domínio/palavra-chave)
            if (x.Contains("cliente") || x.Contains("rol") || x.Contains("caixa") ||
                x.Contains("servico") || x.Contains("serviço") || x.Contains("usuario") || x.Contains("usuário") ||
                x.Contains("config") || x.Contains("entrega") || x.Contains("ponto") ||
                x.Contains("fidelidade") || x.Contains("terc") || x.Contains("catalo") ||
                x.Contains("indeniz") || x.Contains("guarda") || x.Contains("doacao") || x.Contains("doação") ||
                x.Contains("orcamento") || x.Contains("orçamento") || x.Contains("agenda") ||
                x.Contains("relatorio") || x.Contains("relatório") || x.Contains("desconto") ||
                x.Contains("pagamento") || x.Contains("abertura") || x.Contains("fechamento") ||
                x.Contains("sangria") || x.Contains("suprimento") || x.Contains("financeiro") ||
                x.Contains("duplicata") || x.Contains("credito") || x.Contains("crédito") ||
                x.Contains("cancelamento") || x.Contains("cancelar") || x.Contains("altera") ||
                x.Contains("cadastro") || x.Contains("localizacao") || x.Contains("localização") ||
                x.Contains("recibo") || x.Contains("etiqueta") || x.Contains("impressao") || x.Contains("impressão"))
                return "implementado_parcial";
            return "nao_aplicavel";
        }

        return "nao_aplicavel";
    }

    private static void UpsertCoverage(SqliteConnection con, string area, string item, string fonte, string status, string? obs)
    {
        using var cmd = con.CreateCommand();
        // Atualiza status/observacao apenas para itens sem override manual (updated_by IS NULL)
        cmd.CommandText = @"INSERT INTO legacy_coverage(area,item,fonte,status,observacao)
VALUES($a,$i,$f,$s,$o)
ON CONFLICT(area,item,fonte) DO UPDATE SET
  status     = excluded.status,
  observacao = excluded.observacao
WHERE updated_by IS NULL";
        cmd.Parameters.AddWithValue("$a", area);
        cmd.Parameters.AddWithValue("$i", item.Length > 220 ? item[..220] : item);
        cmd.Parameters.AddWithValue("$f", fonte);
        cmd.Parameters.AddWithValue("$s", status);
        cmd.Parameters.AddWithValue("$o", obs ?? (object)DBNull.Value);
        cmd.ExecuteNonQuery();
    }

    private static void ImportLegacyClientes(SqliteConnection con)
    {
        using var chk = con.CreateCommand();
        chk.CommandText = "SELECT COUNT(*) FROM clientes";
        if ((long)(chk.ExecuteScalar() ?? 0L) > 0) return;

        var paradox = FindImportFile(@"legacy\Clientes.csv");
        if (paradox is not null)
        {
            foreach (var row in ReadCsvDict(paradox))
            {
                var cod = V(row, "CodCli");
                var nome = V(row, "NomCli", "RazSocCli");
                if (string.IsNullOrWhiteSpace(cod) || string.IsNullOrWhiteSpace(nome)) continue;
                using var cmd = con.CreateCommand();
                cmd.CommandText = @"INSERT OR IGNORE INTO clientes(
legacy_codigo,nome,documento,telefone,celular,email,logradouro,bairro,cidade,estado,cep,observacoes,limite_credito,desconto_percent,ativo,created_by,data_nascimento,cartao_fidelidade,contato)
VALUES($legacy,$nome,$doc,$tel,$cel,$email,$log,$bairro,$cidade,$uf,$cep,$obs,$lim,$desc,$ativo,'legacy-paradox',$datnas,$cartfid,$contato)";
                cmd.Parameters.AddWithValue("$legacy", cod);
                cmd.Parameters.AddWithValue("$nome", nome);
                cmd.Parameters.AddWithValue("$doc", Db(FirstNonEmpty(V(row, "CPFCli"), V(row, "CgcCli"))));
                cmd.Parameters.AddWithValue("$tel", Db(V(row, "TelCli")));
                cmd.Parameters.AddWithValue("$cel", Db(FirstNonEmpty(V(row, "TelCli2"), V(row, "TelCli3"))));
                cmd.Parameters.AddWithValue("$email", Db(V(row, "email")));
                cmd.Parameters.AddWithValue("$log", Db(V(row, "EndCli")));
                cmd.Parameters.AddWithValue("$bairro", Db(V(row, "BaiCli")));
                cmd.Parameters.AddWithValue("$cidade", Db(V(row, "CidCli")));
                cmd.Parameters.AddWithValue("$uf", Db(V(row, "EstCli")));
                cmd.Parameters.AddWithValue("$cep", Db(V(row, "CepCli")));
                cmd.Parameters.AddWithValue("$obs", Db(FirstNonEmpty(V(row, "ObsCli"), V(row, "ObsCli2"), V(row, "ObsCli3"), V(row, "ObsCli4"))));
                cmd.Parameters.AddWithValue("$lim", ParseDouble(V(row, "LimCred", "VlrFatLim")));
                cmd.Parameters.AddWithValue("$desc", ParseDouble(V(row, "Desconto")));
                cmd.Parameters.AddWithValue("$ativo", IsTrue(V(row, "Desativado")) ? 0 : 1);
                cmd.Parameters.AddWithValue("$datnas", DbDate(V(row, "DatNas")));
                cmd.Parameters.AddWithValue("$cartfid", Db(V(row, "CartaoFid")));
                cmd.Parameters.AddWithValue("$contato", Db(V(row, "Contato")));
                cmd.ExecuteNonQuery();
            }
            return;
        }

        var path = FindImportFile("CLIENTES.csv");
        if (path is null) return;

        foreach (var line in File.ReadLines(path, Encoding.UTF8).Skip(1))
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            var p = line.Split(';');
            if (p.Length < 12 || string.IsNullOrWhiteSpace(p[0])) continue;
            using var cmd = con.CreateCommand();
            cmd.CommandText = @"INSERT INTO clientes(nome,documento,logradouro,numero,bairro,cidade,estado,cep,telefone,celular,email,created_by)
VALUES($nome,$doc,$log,$num,$bairro,$cidade,$uf,$cep,$tel,$cel,$email,'legacy-csv')";
            cmd.Parameters.AddWithValue("$nome", Clean(p[0]));
            cmd.Parameters.AddWithValue("$doc", Db(Clean(p[1])));
            cmd.Parameters.AddWithValue("$log", Db(Clean(p[2])));
            cmd.Parameters.AddWithValue("$num", Db(Clean(p[3])));
            cmd.Parameters.AddWithValue("$bairro", Db(Clean(p[4])));
            cmd.Parameters.AddWithValue("$cidade", Db(Clean(p[5])));
            cmd.Parameters.AddWithValue("$uf", Db(Clean(p[6])));
            cmd.Parameters.AddWithValue("$cep", Db(Clean(p[7])));
            cmd.Parameters.AddWithValue("$tel", Db(Clean(p[9])));
            cmd.Parameters.AddWithValue("$cel", Db(Clean(p[10])));
            cmd.Parameters.AddWithValue("$email", Db(Clean(p[11])));
            cmd.ExecuteNonQuery();
        }
    }

    private static void ImportLegacyRols(SqliteConnection con)
    {
        using var chk = con.CreateCommand();
        chk.CommandText = "SELECT COUNT(*) FROM ordens_servico WHERE legacy_rol IS NOT NULL";
        if ((long)(chk.ExecuteScalar() ?? 0L) > 0) return;

        var path = FindImportFile(@"legacy\MovCab.csv");
        if (path is null) return;

        foreach (var row in ReadCsvDict(path))
        {
            var rol = V(row, "ROL");
            if (string.IsNullOrWhiteSpace(rol)) continue;
            var clienteId = GetOrCreateLegacyCliente(con, V(row, "CodCli"));
            var status = MapLegacyRolStatus(V(row, "Posicao"), V(row, "Cancelado"));
            var valor = ParseDouble(V(row, "ValTot"));
            var desconto = ParseDouble(V(row, "DescontoValor", "DescontoROL"));
            var payload = JsonSerializer.Serialize(row);
            using var cmd = con.CreateCommand();
            cmd.CommandText = @"INSERT OR IGNORE INTO ordens_servico(
numero,legacy_rol,cliente_id,status,data_entrada,data_promessa,data_entrega,valor_total,desconto,valor_final,valor_pago,
metodo_pagamento,observacoes,motivo_cancelamento,data_cancelamento,total_pecas,localizacao_rol,
usuario_entrada,usuario_entrega,usuario_pagamento,numero_nota_fiscal,legacy_payload)
VALUES($num,$legacy,$cli,$status,$entrada,$promessa,$entrega,$total,$desc,$final,$pago,$metodo,$obs,$motivo,$dataCanc,$pecas,$loc,$uEntrada,$uEntrega,$uPag,$nfe,$payload)";
            cmd.Parameters.AddWithValue("$num", $"ROL{long.Parse(rol):D6}");
            cmd.Parameters.AddWithValue("$legacy", rol);
            cmd.Parameters.AddWithValue("$cli", clienteId);
            cmd.Parameters.AddWithValue("$status", status);
            cmd.Parameters.AddWithValue("$entrada", DbDateOrToday(V(row, "DatLan")));
            cmd.Parameters.AddWithValue("$promessa", DbDate(V(row, "DatEnt")));
            cmd.Parameters.AddWithValue("$entrega", DbDate(V(row, "DatEntRol", "DatEntLoja")));
            cmd.Parameters.AddWithValue("$total", valor);
            cmd.Parameters.AddWithValue("$desc", desconto);
            cmd.Parameters.AddWithValue("$final", Math.Max(0, valor - desconto));
            cmd.Parameters.AddWithValue("$pago", status == "paga" ? Math.Max(0, valor - desconto) : 0);
            cmd.Parameters.AddWithValue("$metodo", status == "paga" ? "legado" : DBNull.Value);
            cmd.Parameters.AddWithValue("$obs", Db(V(row, "ObsROL", "Pedidos")));
            cmd.Parameters.AddWithValue("$motivo", Db(V(row, "MotivoCanc")));
            cmd.Parameters.AddWithValue("$dataCanc", DbDate(V(row, "DataCanc")));
            var pecas = (int)ParseDouble(V(row, "TotPecas"));
            cmd.Parameters.AddWithValue("$pecas", pecas > 0 ? pecas : DBNull.Value);
            cmd.Parameters.AddWithValue("$loc", Db(V(row, "CodLoc")));
            cmd.Parameters.AddWithValue("$uEntrada", DbUser(V(row, "CodUsuario", "CodVen")));
            cmd.Parameters.AddWithValue("$uEntrega", DbUserOrNull(V(row, "CodVenEnt")));
            cmd.Parameters.AddWithValue("$uPag", status == "paga" ? DbUser(V(row, "CodUsuario", "CodVen")) : DBNull.Value);
            cmd.Parameters.AddWithValue("$nfe", Db(V(row, "NumNotFis", "NumNotFisF")));
            cmd.Parameters.AddWithValue("$payload", payload);
            cmd.ExecuteNonQuery();
        }
    }

    private static void ImportLegacyItens(SqliteConnection con)
    {
        using var chk = con.CreateCommand();
        chk.CommandText = "SELECT COUNT(*) FROM os_itens WHERE legacy_payload IS NOT NULL";
        if ((long)(chk.ExecuteScalar() ?? 0L) > 0) return;

        ImportLegacyItemFile(con, @"legacy\MovItem.csv", false);
        ImportLegacyItemFile(con, @"legacy\MovItemSer.csv", true);
    }

    private static Dictionary<string, string> LoadLegacyCatalog(string csvFile, string codeCol, string nameCol)
    {
        var path = FindImportFile(Path.Combine("legacy", csvFile));
        var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        if (path is null) return dict;
        foreach (var row in ReadCsvDict(path))
        {
            var cod = V(row, codeCol).Trim();
            var des = V(row, nameCol).Trim();
            if (!string.IsNullOrEmpty(cod) && !string.IsNullOrEmpty(des))
                dict[cod] = des;
        }
        return dict;
    }

    private static object CatalogDb(Dictionary<string, string> catalog, string code)
    {
        if (string.IsNullOrWhiteSpace(code)) return DBNull.Value;
        if (!catalog.TryGetValue(code.Trim(), out var name)) return code;
        if (name.StartsWith("SEM ", StringComparison.OrdinalIgnoreCase)) return DBNull.Value;
        return name;
    }

    private static void ImportLegacyItemFile(SqliteConnection con, string file, bool serviceOnly)
    {
        var path = FindImportFile(file);
        if (path is null) return;
        var produtos = LoadLegacyCatalog("TabPro.csv", "CodPro", "DesPro");
        var servicos = LoadLegacyCatalog("SerLav.csv", "CodSerLav", "DesSerLav");
        var cores    = LoadLegacyCatalog("Cores.csv",   "CodCor",    "DesCor");
        var defeitos = LoadLegacyCatalog("Defeitos.csv","CodDef",    "DesDef");
        var tecidos  = LoadLegacyCatalog("TipTec.csv",  "CodTipTec", "DesTipTec");
        var rolMap = GetLegacyRolMap(con);
        using var cmd = con.CreateCommand();
        cmd.CommandText = @"INSERT INTO os_itens(os_id,descricao,tipo_tecido,cor,marca,defeito,quantidade,valor_unitario,valor_total,observacao,obs2,peso,identificacao,localizacao,valor_terceiro,legacy_payload)
VALUES($os,$desc,$tec,$cor,$marca,$def,$qtd,$unit,$total,$obs,$obs2,$peso,$ident,$loc,$vterc,$payload)";
        var pOs = cmd.Parameters.Add("$os", SqliteType.Integer);
        var pDesc = cmd.Parameters.Add("$desc", SqliteType.Text);
        var pTec = cmd.Parameters.Add("$tec", SqliteType.Text);
        var pCor = cmd.Parameters.Add("$cor", SqliteType.Text);
        var pMarca = cmd.Parameters.Add("$marca", SqliteType.Text);
        var pDef = cmd.Parameters.Add("$def", SqliteType.Text);
        var pQtd = cmd.Parameters.Add("$qtd", SqliteType.Real);
        var pUnit = cmd.Parameters.Add("$unit", SqliteType.Real);
        var pTotal = cmd.Parameters.Add("$total", SqliteType.Real);
        var pObs = cmd.Parameters.Add("$obs", SqliteType.Text);
        var pObs2 = cmd.Parameters.Add("$obs2", SqliteType.Text);
        var pPeso = cmd.Parameters.Add("$peso", SqliteType.Real);
        var pIdent = cmd.Parameters.Add("$ident", SqliteType.Text);
        var pLoc = cmd.Parameters.Add("$loc", SqliteType.Text);
        var pVTerc = cmd.Parameters.Add("$vterc", SqliteType.Real);
        var pPayload = cmd.Parameters.Add("$payload", SqliteType.Text);

        foreach (var row in ReadCsvDict(path))
        {
            var rol = V(row, "ROL", "Rol");
            if (string.IsNullOrWhiteSpace(rol)) continue;
            if (!rolMap.TryGetValue(rol, out var osId)) continue;
            var codPro = V(row, "CodPro").Trim();
            var codSer = V(row, "CodSerLav").Trim();
            var descricao = serviceOnly
                ? (servicos.GetValueOrDefault(codSer, $"SERVICO {codSer}"))
                : (produtos.GetValueOrDefault(codPro, $"PRODUTO {codPro}"));
            var obs = V(row, "Obs");
            var obs2 = V(row, "Obs2");
            var quantidade = ParseDouble(V(row, "Quantidade"));
            if (quantidade <= 0) quantidade = 1;
            var unitario = ParseDouble(V(row, serviceOnly ? "PreUniSer" : "PreUniVen"));
            var total = ParseDouble(V(row, serviceOnly ? "PreFinalSer" : "PreFinal"));
            if (total == 0 && unitario > 0) total = quantidade * unitario;
            var peso = ParseDouble(V(row, "Peso", "PesoLiq"));
            var ident = V(row, "Identif", "Identificacao", "Ident", "NumIdent");
            var loc = V(row, "Localizacao", "Loc", "CodLoc");

            pOs.Value = osId;
            pDesc.Value = descricao;
            pTec.Value = CatalogDb(tecidos, V(row, "CodTipTec"));
            pCor.Value = CatalogDb(cores,   V(row, "CodCor"));
            pMarca.Value = Db(V(row, "Marca"));
            pDef.Value = CatalogDb(defeitos, V(row, "CodDef"));
            pQtd.Value = quantidade;
            pUnit.Value = unitario;
            pTotal.Value = total;
            var vterc = ParseDouble(V(row, "ValorTerc"));
            pObs.Value = Db(obs);
            pObs2.Value = Db(obs2);
            pPeso.Value = peso > 0 ? peso : DBNull.Value;
            pIdent.Value = Db(ident);
            pLoc.Value = Db(loc);
            pVTerc.Value = vterc > 0 ? vterc : DBNull.Value;
            pPayload.Value = JsonSerializer.Serialize(row);
            cmd.ExecuteNonQuery();
        }
    }

    private static void ImportLegacyFinanceiro(SqliteConnection con)
    {
        using var chk = con.CreateCommand();
        chk.CommandText = "SELECT COUNT(*) FROM financeiro WHERE legacy_payload IS NOT NULL";
        if ((long)(chk.ExecuteScalar() ?? 0L) > 0) return;

        var path = FindImportFile(@"legacy\Duplicat.csv");
        if (path is null) return;
        foreach (var row in ReadCsvDict(path))
        {
            var numFat = V(row, "NumFat");
            var numDup = V(row, "NumDup");
            if (string.IsNullOrWhiteSpace(numFat) && string.IsNullOrWhiteSpace(numDup)) continue;
            var clienteId = GetOrCreateLegacyCliente(con, V(row, "CodCli"));
            var baixada = IsTrue(V(row, "Baixa"));
            var valor = ParseDouble(FirstNonEmpty(V(row, "ValDup"), V(row, "ValFat"), V(row, "ValDev")));
            using var cmd = con.CreateCommand();
            var numNot = V(row, "NumNot");
            cmd.CommandText = @"INSERT INTO financeiro(cliente_id,tipo,status,valor,vencimento,data_recebimento,valor_recebido,metodo_recebimento,observacao,usuario,legacy_numfat,legacy_numnot,legacy_payload)
VALUES($cli,'a_receber',$status,$valor,$ven,$pag,$valorPago,$metodo,$obs,$usuario,$nfat,$nnot,$payload)";
            cmd.Parameters.AddWithValue("$cli", clienteId);
            cmd.Parameters.AddWithValue("$status", baixada ? "recebido" : "aberto");
            cmd.Parameters.AddWithValue("$valor", valor);
            cmd.Parameters.AddWithValue("$ven", DbDate(V(row, "DatVen", "DatVenNot")));
            cmd.Parameters.AddWithValue("$pag", baixada ? DbDate(V(row, "DatPag")) : DBNull.Value);
            cmd.Parameters.AddWithValue("$valorPago", baixada ? ParseDouble(V(row, "ValDupPag", "ValFat")) : DBNull.Value);
            cmd.Parameters.AddWithValue("$metodo", baixada ? Db(V(row, "CodFpg")) : DBNull.Value);
            cmd.Parameters.AddWithValue("$obs", Db(FirstNonEmpty(V(row, "Obs"), V(row, "Obs2"))));
            cmd.Parameters.AddWithValue("$usuario", DbUser(V(row, "Usuario", "CodVen")));
            cmd.Parameters.AddWithValue("$nfat", Db(numFat));
            cmd.Parameters.AddWithValue("$nnot", Db(numNot));
            cmd.Parameters.AddWithValue("$payload", JsonSerializer.Serialize(row));
            cmd.ExecuteNonQuery();
        }
    }

    private static void ImportLegacyFileCatalog(SqliteConnection con)
    {
        using var chk = con.CreateCommand();
        chk.CommandText = "SELECT COUNT(*) FROM legacy_records";
        if ((long)(chk.ExecuteScalar() ?? 0L) > 0) return;

        var files = new[] { "Usuarios", "Clientes", "MovCab", "MovItem", "MovItemSer", "Notas", "Duplicat", "CliCredito" };
        foreach (var table in files)
        {
            var path = FindImportFile($@"legacy\{table}.csv");
            if (path is null) continue;
            var totalRows = Math.Max(0, File.ReadLines(path).LongCount() - 1);
            using var cmd = con.CreateCommand();
            cmd.CommandText = @"INSERT OR IGNORE INTO legacy_records(tabela,legacy_pk,payload) VALUES($t,$pk,$payload)";
            cmd.Parameters.AddWithValue("$t", table);
            cmd.Parameters.AddWithValue("$pk", $"arquivo:{table}");
            cmd.Parameters.AddWithValue("$payload", JsonSerializer.Serialize(new
            {
                arquivo = Path.GetFileName(path),
                caminho = path,
                linhas = totalRows,
                observacao = "Arquivo CSV completo empacotado na ISO em import/legacy; nenhum registro bruto foi descartado."
            }));
            cmd.ExecuteNonQuery();
        }
    }

    private static void ImportLegacyCredito(SqliteConnection con)
    {
        using var chk = con.CreateCommand();
        chk.CommandText = "SELECT COUNT(*) FROM clientes_credito WHERE saldo > 0";
        if ((long)(chk.ExecuteScalar() ?? 0L) > 0) return;

        var path = FindImportFile(@"legacy\CliCredito.csv");
        if (path is null) return;
        foreach (var row in ReadCsvDict(path))
        {
            var cancelado = IsTrue(V(row, "Cancelado"));
            var sit = V(row, "Sit").Trim().ToUpperInvariant();
            if (cancelado || sit is "B" or "C") continue;

            var codCli = V(row, "CodCli");
            if (string.IsNullOrWhiteSpace(codCli)) continue;
            var valor = ParseDouble(V(row, "ValCre"));
            if (valor <= 0) continue;

            using var findCli = con.CreateCommand();
            findCli.CommandText = "SELECT id FROM clientes WHERE legacy_codigo=$c LIMIT 1";
            findCli.Parameters.AddWithValue("$c", codCli);
            var cliObj = findCli.ExecuteScalar();
            if (cliObj is null) continue;
            var cliId = Convert.ToInt32(cliObj, CultureInfo.InvariantCulture);

            con.Execute(@"INSERT INTO clientes_credito(cliente_id,saldo,updated_at)
VALUES($cli,$saldo,datetime('now'))
ON CONFLICT(cliente_id) DO UPDATE SET saldo=saldo+$saldo, updated_at=datetime('now')",
                ("$cli", cliId), ("$saldo", valor));

            var dat = V(row, "DatCre");
            var numNot = V(row, "NumNot");
            con.Execute(@"INSERT INTO clientes_credito_movimentos(cliente_id,tipo,valor,descricao,referencia,usuario,created_at)
VALUES($cli,'credito_legado',$val,$desc,$ref,'LEGADO',COALESCE($dat,datetime('now')))",
                ("$cli", cliId), ("$val", valor),
                ("$desc", $"Saldo de crédito legado — Nota {numNot}"),
                ("$ref", numNot), ("$dat", Db(dat)));
        }
    }

    private static void BackfillLegacyNFe(SqliteConnection con)
    {
        using var chk = con.CreateCommand();
        chk.CommandText = "SELECT COUNT(*) FROM ordens_servico WHERE numero_nota_fiscal IS NOT NULL";
        if ((long)(chk.ExecuteScalar() ?? 0L) > 0) return;

        using var cmd = con.CreateCommand();
        cmd.CommandText = "SELECT id, legacy_payload FROM ordens_servico WHERE legacy_payload IS NOT NULL AND numero_nota_fiscal IS NULL";
        using var r = cmd.ExecuteReader();
        var updates = new List<(int id, string nfe)>();
        while (r.Read())
        {
            try
            {
                var payload = r.GetString(1);
                using var doc = JsonDocument.Parse(payload);
                string? nfe = null;
                if (doc.RootElement.TryGetProperty("NumNotFis", out var p1) && p1.ValueKind == JsonValueKind.String)
                    nfe = p1.GetString();
                else if (doc.RootElement.TryGetProperty("NumNotFisF", out var p2) && p2.ValueKind == JsonValueKind.String)
                    nfe = p2.GetString();
                if (!string.IsNullOrWhiteSpace(nfe))
                    updates.Add((r.GetInt32(0), nfe));
            }
            catch { /* skip malformed payload */ }
        }
        r.Close();
        foreach (var (id, nfe) in updates)
            con.Execute("UPDATE ordens_servico SET numero_nota_fiscal=$nfe WHERE id=$id",
                ("$nfe", nfe), ("$id", id));
    }

    private static void BackfillLegacyObs2(SqliteConnection con)
    {
        using var chk = con.CreateCommand();
        chk.CommandText = "SELECT COUNT(*) FROM os_itens WHERE obs2 IS NOT NULL";
        if ((long)(chk.ExecuteScalar() ?? 0L) > 0) return;

        using var cmd = con.CreateCommand();
        cmd.CommandText = "SELECT id, legacy_payload FROM os_itens WHERE legacy_payload IS NOT NULL AND obs2 IS NULL";
        using var r = cmd.ExecuteReader();
        var updates = new List<(int id, string obs2)>();
        while (r.Read())
        {
            try
            {
                var payload = r.GetString(1);
                using var doc = JsonDocument.Parse(payload);
                if (doc.RootElement.TryGetProperty("Obs2", out var p) && p.ValueKind == JsonValueKind.String)
                {
                    var obs2 = p.GetString();
                    if (!string.IsNullOrWhiteSpace(obs2))
                        updates.Add((r.GetInt32(0), obs2));
                }
            }
            catch { /* skip */ }
        }
        r.Close();
        foreach (var (id, obs2) in updates)
            con.Execute("UPDATE os_itens SET obs2=$obs2 WHERE id=$id",
                ("$obs2", obs2), ("$id", id));
    }

    private static void BackfillValorTerceiro(SqliteConnection con)
    {
        using var chk = con.CreateCommand();
        chk.CommandText = "SELECT COUNT(*) FROM os_itens WHERE valor_terceiro IS NOT NULL AND valor_terceiro > 0";
        if ((long)(chk.ExecuteScalar() ?? 0L) > 0) return;

        using var cmd = con.CreateCommand();
        cmd.CommandText = "SELECT id, legacy_payload FROM os_itens WHERE legacy_payload IS NOT NULL AND valor_terceiro IS NULL";
        using var r = cmd.ExecuteReader();
        var updates = new List<(int id, double v)>();
        while (r.Read())
        {
            try
            {
                var payload = r.GetString(1);
                using var doc = JsonDocument.Parse(payload);
                if (doc.RootElement.TryGetProperty("ValorTerc", out var p) && p.ValueKind == JsonValueKind.String)
                {
                    var v = ParseDouble(p.GetString() ?? "");
                    if (v > 0) updates.Add((r.GetInt32(0), v));
                }
            }
            catch { /* skip */ }
        }
        r.Close();
        foreach (var (id, v) in updates)
            con.Execute("UPDATE os_itens SET valor_terceiro=$v WHERE id=$id", ("$v", v), ("$id", id));
    }

    private static void BackfillHoraPagamento(SqliteConnection con)
    {
        using var chk = con.CreateCommand();
        chk.CommandText = "SELECT COUNT(*) FROM ordens_servico WHERE hora_pagamento IS NOT NULL";
        if ((long)(chk.ExecuteScalar() ?? 0L) > 0) return;

        var path = FindImportFile(@"legacy\Notas.csv");
        if (path is null) return;

        // Build map: RolPrincip → (HorPag, NumNot) — prefer TipNota=A
        var horMap = new Dictionary<string, (string hor, string numNot)>(StringComparer.OrdinalIgnoreCase);
        foreach (var row in ReadCsvDict(path))
        {
            var rol = V(row, "RolPrincip");
            if (string.IsNullOrWhiteSpace(rol) || rol == "0") continue;
            var hor = V(row, "HorPag");
            if (string.IsNullOrWhiteSpace(hor)) continue;
            // Normalize "12:24:30.515000" → "12:24:30"
            if (hor.Length > 8) hor = hor[..8];
            var numNot = V(row, "NumNot");
            var tipNota = V(row, "TipNota").Trim().ToUpperInvariant();
            // Prefer TipNota=A (pagamento); don't overwrite A with B
            if (!horMap.TryGetValue(rol, out var existing) || tipNota == "A")
                horMap[rol] = (hor, numNot);
        }

        if (horMap.Count == 0) return;

        // Batch update ordens_servico
        using var upd = con.CreateCommand();
        upd.CommandText = "UPDATE ordens_servico SET hora_pagamento=$hor, nota_numero=$nn WHERE legacy_rol=$rol AND hora_pagamento IS NULL";
        var pHor = upd.Parameters.Add("$hor", SqliteType.Text);
        var pNn  = upd.Parameters.Add("$nn",  SqliteType.Text);
        var pRol = upd.Parameters.Add("$rol", SqliteType.Text);
        foreach (var (rol, entry) in horMap)
        {
            pHor.Value = entry.hor;
            pNn.Value  = string.IsNullOrWhiteSpace(entry.numNot) ? DBNull.Value : (object)entry.numNot;
            pRol.Value = rol;
            upd.ExecuteNonQuery();
        }
    }

    private static void BackfillMotivoDesconto(SqliteConnection con)
    {
        using var chk = con.CreateCommand();
        chk.CommandText = "SELECT COUNT(*) FROM ordens_servico WHERE motivo_desconto IS NOT NULL";
        if ((long)(chk.ExecuteScalar() ?? 0L) > 0) return;

        var path = FindImportFile(@"legacy\Notas.csv");
        if (path is null) return;

        using var upd = con.CreateCommand();
        upd.CommandText = @"UPDATE ordens_servico SET motivo_desconto=$motivo
WHERE legacy_rol=$rol AND motivo_desconto IS NULL AND $motivo IS NOT NULL";
        var pMotivo = upd.Parameters.Add("$motivo", SqliteType.Text);
        var pRol    = upd.Parameters.Add("$rol",    SqliteType.Text);

        foreach (var row in ReadCsvDict(path))
        {
            var motivo = V(row, "MotivoDesc");
            if (string.IsNullOrWhiteSpace(motivo)) continue;
            var rol = V(row, "RolPrincip");
            if (string.IsNullOrWhiteSpace(rol) || rol == "0") continue;
            pMotivo.Value = motivo;
            pRol.Value    = rol;
            upd.ExecuteNonQuery();
        }
    }

    private static void BackfillItemDescricoes(SqliteConnection con)
    {
        using var chk = con.CreateCommand();
        chk.CommandText = "SELECT COUNT(*) FROM os_itens WHERE descricao LIKE 'PRODUTO %' OR descricao LIKE 'SERVICO %'";
        if ((long)(chk.ExecuteScalar() ?? 0L) == 0) return;

        var produtos = LoadLegacyCatalog("TabPro.csv", "CodPro", "DesPro");
        var servicos = LoadLegacyCatalog("SerLav.csv", "CodSerLav", "DesSerLav");
        var cores    = LoadLegacyCatalog("Cores.csv",   "CodCor",    "DesCor");
        var defeitos = LoadLegacyCatalog("Defeitos.csv","CodDef",    "DesDef");
        var tecidos  = LoadLegacyCatalog("TipTec.csv",  "CodTipTec", "DesTipTec");

        // Collect IDs and payload in memory-efficient chunks, apply in one transaction
        var rows = new List<(int id, string desc, string codCor, string codTec, string codDef)>();
        using (var cmd = con.CreateCommand())
        {
            cmd.CommandText = "SELECT id, descricao, legacy_payload FROM os_itens WHERE legacy_payload IS NOT NULL AND (descricao LIKE 'PRODUTO %' OR descricao LIKE 'SERVICO %')";
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                try
                {
                    var id = r.GetInt32(0);
                    var descAtual = r.GetString(1);
                    var payload = r.IsDBNull(2) ? null : r.GetString(2);
                    if (payload == null) continue;
                    using var doc = JsonDocument.Parse(payload);
                    var root = doc.RootElement;
                    string GetJ(string key) => root.TryGetProperty(key, out var el) && el.ValueKind == JsonValueKind.String ? (el.GetString() ?? "").Trim() : "";
                    var codPro = GetJ("CodPro");
                    var codSer = GetJ("CodSerLav");
                    var newDesc = descAtual.StartsWith("SERVICO ")
                        ? servicos.GetValueOrDefault(codSer, descAtual)
                        : produtos.GetValueOrDefault(codPro, descAtual);
                    rows.Add((id, newDesc, GetJ("CodCor"), GetJ("CodTipTec"), GetJ("CodDef")));
                }
                catch { /* skip bad payload */ }
            }
        }

        if (rows.Count == 0) return;

        // Run inside Initialize()'s already-open BEGIN IMMEDIATE — no nested transaction needed
        using var upd = con.CreateCommand();
        upd.CommandText = "UPDATE os_itens SET descricao=$d,cor=$cor,tipo_tecido=$tec,defeito=$def WHERE id=$id";
        var pId  = upd.Parameters.Add("$id",  SqliteType.Integer);
        var pD   = upd.Parameters.Add("$d",   SqliteType.Text);
        var pCor = upd.Parameters.Add("$cor", SqliteType.Text);
        var pTec = upd.Parameters.Add("$tec", SqliteType.Text);
        var pDef = upd.Parameters.Add("$def", SqliteType.Text);
        foreach (var (id, desc, codCor, codTec, codDef) in rows)
        {
            pId.Value  = id;
            pD.Value   = desc;
            pCor.Value = CatalogDb(cores,    codCor);
            pTec.Value = CatalogDb(tecidos,  codTec);
            pDef.Value = CatalogDb(defeitos, codDef);
            upd.ExecuteNonQuery();
        }
    }

    private static void BackfillMovCabExtras(SqliteConnection con)
    {
        // Guard: skip if already backfilled
        using var chk = con.CreateCommand();
        chk.CommandText = "SELECT COUNT(*) FROM ordens_servico WHERE total_pecas IS NOT NULL";
        if ((long)(chk.ExecuteScalar() ?? 0L) > 0) return;

        var path = FindImportFile(@"legacy\MovCab.csv");
        if (path is null) return;

        using var upd = con.CreateCommand();
        upd.CommandText = @"UPDATE ordens_servico
SET total_pecas=$pecas, localizacao_rol=$loc, data_cancelamento=$dataCanc
WHERE legacy_rol=$rol AND total_pecas IS NULL";
        var pPecas    = upd.Parameters.Add("$pecas",    SqliteType.Integer);
        var pLoc      = upd.Parameters.Add("$loc",      SqliteType.Text);
        var pDataCanc = upd.Parameters.Add("$dataCanc", SqliteType.Text);
        var pRol      = upd.Parameters.Add("$rol",      SqliteType.Text);

        foreach (var row in ReadCsvDict(path))
        {
            var rol = V(row, "ROL");
            if (string.IsNullOrWhiteSpace(rol)) continue;
            var pecas = (int)ParseDouble(V(row, "TotPecas"));
            pPecas.Value    = pecas > 0 ? pecas : DBNull.Value;
            pLoc.Value      = Db(V(row, "CodLoc"));
            pDataCanc.Value = DbDate(V(row, "DataCanc"));
            pRol.Value      = rol;
            upd.ExecuteNonQuery();
        }
    }

    private static void ImportLegacyPontos(SqliteConnection con)
    {
        using var chk = con.CreateCommand();
        chk.CommandText = "SELECT COUNT(*) FROM fidelidade";
        if ((long)(chk.ExecuteScalar() ?? 0L) > 0) return;

        var path = FindImportFile(@"legacy\Clientes.csv");
        if (path is null) return;

        foreach (var row in ReadCsvDict(path))
        {
            var cod = V(row, "CodCli");
            var pontosStr = V(row, "Pontos");
            if (string.IsNullOrWhiteSpace(cod) || string.IsNullOrWhiteSpace(pontosStr)) continue;
            var pontos = (int)ParseDouble(pontosStr);
            if (pontos <= 0) continue;

            using var sel = con.CreateCommand();
            sel.CommandText = "SELECT id FROM clientes WHERE legacy_codigo=$cod LIMIT 1";
            sel.Parameters.AddWithValue("$cod", cod);
            var clienteId = sel.ExecuteScalar();
            if (clienteId is null or DBNull) continue;
            var cid = Convert.ToInt64(clienteId);

            using var ups = con.CreateCommand();
            ups.CommandText = @"INSERT INTO fidelidade(cliente_id,pontos,updated_at)
VALUES($cid,$pts,datetime('now'))
ON CONFLICT(cliente_id) DO UPDATE SET pontos=excluded.pontos, updated_at=excluded.updated_at";
            ups.Parameters.AddWithValue("$cid", cid);
            ups.Parameters.AddWithValue("$pts", pontos);
            ups.ExecuteNonQuery();

            using var mov = con.CreateCommand();
            mov.CommandText = @"INSERT INTO fidelidade_movimentos(cliente_id,pontos,tipo,referencia,observacao,usuario,created_at)
VALUES($cid,$pts,'credito','legacy','Saldo importado do EquipeExe','legacy-paradox',datetime('now'))";
            mov.Parameters.AddWithValue("$cid", cid);
            mov.Parameters.AddWithValue("$pts", pontos);
            mov.ExecuteNonQuery();
        }
    }

    public int ReimportarCreditos(SqliteConnection con) { ImportLegacyCredito(con); return 1; }
    public SqliteConnection OpenPublic() => Open();

    public List<object> ListarDoacoes(int? clienteId, int? osId, string? status)
    {
        using var con = Open();
        var where = new List<string>();
        if (clienteId.HasValue) where.Add("d.cliente_id=$cli");
        if (osId.HasValue) where.Add("d.os_id=$os");
        if (!string.IsNullOrWhiteSpace(status)) where.Add("d.status=$status");
        var w = where.Count > 0 ? "WHERE " + string.Join(" AND ", where) : "";
        using var cmd = con.CreateCommand();
        cmd.CommandText = $@"
SELECT d.*, c.nome as cliente_nome, os.numero as os_numero
FROM doacoes d
JOIN clientes c ON c.id=d.cliente_id
LEFT JOIN ordens_servico os ON os.id=d.os_id
{w} ORDER BY d.id DESC";
        if (clienteId.HasValue) cmd.Parameters.AddWithValue("$cli", clienteId.Value);
        if (osId.HasValue) cmd.Parameters.AddWithValue("$os", osId.Value);
        if (!string.IsNullOrWhiteSpace(status)) cmd.Parameters.AddWithValue("$status", status);
        using var r = cmd.ExecuteReader();
        var list = new List<object>();
        while (r.Read())
        {
            list.Add(new
            {
                id = r.GetInt32(r.GetOrdinal("id")),
                osId = r.IsDBNull(r.GetOrdinal("os_id")) ? (int?)null : r.GetInt32(r.GetOrdinal("os_id")),
                clienteId = r.GetInt32(r.GetOrdinal("cliente_id")),
                descricao = r.GetString(r.GetOrdinal("descricao")),
                valor = r.GetDouble(r.GetOrdinal("valor")),
                dataDoacao = r.GetString(r.GetOrdinal("data_doacao")),
                status = r.GetString(r.GetOrdinal("status")),
                motivoCancelamento = GetStringByName(r, "motivo_cancelamento"),
                observacao = GetStringByName(r, "observacao"),
                usuario = r.GetString(r.GetOrdinal("usuario")),
                createdAt = r.GetString(r.GetOrdinal("created_at")),
                clienteNome = GetStringByName(r, "cliente_nome"),
                osNumero = GetStringByName(r, "os_numero")
            });
        }
        return list;
    }

    public int CriarDoacao(DoacaoRequest req, string usuario)
    {
        using var con = Open();
        using var cmd = con.CreateCommand();
        cmd.CommandText = @"
INSERT INTO doacoes(cliente_id,os_id,descricao,valor,observacao,usuario)
VALUES($cli,$os,$desc,$val,$obs,$usr);
SELECT last_insert_rowid();";
        cmd.Parameters.AddWithValue("$cli", req.ClienteId);
        cmd.Parameters.AddWithValue("$os", req.OsId.HasValue ? (object)req.OsId.Value : DBNull.Value);
        cmd.Parameters.AddWithValue("$desc", req.Descricao);
        cmd.Parameters.AddWithValue("$val", req.Valor);
        cmd.Parameters.AddWithValue("$obs", req.Observacao ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("$usr", usuario);
        return (int)(long)(cmd.ExecuteScalar() ?? 0L);
    }

    public void ConfirmarDoacao(int id, string usuario)
    {
        using var con = Open();
        con.Execute("UPDATE doacoes SET status='doado', data_doacao=date('now') WHERE id=$id AND status='pendente'",
            ("$id", id));
    }

    public void CancelarDoacao(int id, string? motivo, string usuario)
    {
        using var con = Open();
        con.Execute("UPDATE doacoes SET status='cancelado', motivo_cancelamento=$mot WHERE id=$id",
            ("$mot", motivo ?? (object)DBNull.Value), ("$id", id));
    }

    private static bool ImportLegacyServicos(SqliteConnection con)
    {
        var path = FindImportFile("Exportar-Servicos.csv");
        if (path is null) return false;
        var n = 1;
        foreach (var line in File.ReadLines(path, Encoding.UTF8).Skip(1))
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            var p = line.Split(';');
            if (p.Length < 3 || string.IsNullOrWhiteSpace(p[1])) continue;
            using var cmd = con.CreateCommand();
            cmd.CommandText = "INSERT OR IGNORE INTO servicos(codigo,descricao,categoria,preco) VALUES($cod,$desc,$cat,$preco)";
            cmd.Parameters.AddWithValue("$cod", $"LEG{n:D5}");
            cmd.Parameters.AddWithValue("$desc", Clean(p[1]).ToUpperInvariant());
            cmd.Parameters.AddWithValue("$cat", Clean(p[0]).ToUpperInvariant());
            cmd.Parameters.AddWithValue("$preco", ParseMoney(p[2]));
            cmd.ExecuteNonQuery();
            n++;
        }
        return n > 1;
    }

    private static string? FindImportFile(string name)
    {
        var candidates = new[]
        {
            Path.Combine(AppContext.BaseDirectory, "import", name),
            Path.Combine(Directory.GetCurrentDirectory(), "import", name),
            Path.Combine(@"D:\Projetos Dev", name),
            Path.Combine(@"E:\Projeto Luci\MOD\apps\backend\EquipeExe.Mod.Api\import", name)
        };
        return candidates.FirstOrDefault(File.Exists);
    }

    private static IEnumerable<Dictionary<string, string>> ReadCsvDict(string path)
    {
        using var reader = new StreamReader(path, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
        var headerLine = reader.ReadLine();
        if (headerLine is null) yield break;
        var header = SplitCsv(headerLine).Select(h => h.Trim().TrimStart('\uFEFF')).ToArray();
        string? line;
        while ((line = reader.ReadLine()) is not null)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            var cols = SplitCsv(line);
            var row = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            for (var i = 0; i < header.Length; i++)
                row[header[i]] = i < cols.Length ? Clean(cols[i]) : "";
            yield return row;
        }
    }

    private static string V(Dictionary<string, string> row, params string[] keys)
    {
        foreach (var key in keys)
            if (row.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value))
                return value.Trim();
        return "";
    }

    private static string FirstNonEmpty(params string[] values) =>
        values.FirstOrDefault(v => !string.IsNullOrWhiteSpace(v)) ?? "";

    private static double ParseDouble(string value)
    {
        var cleaned = Clean(value).Replace("R$", "", StringComparison.OrdinalIgnoreCase).Trim();
        if (double.TryParse(cleaned, NumberStyles.Any, CultureInfo.InvariantCulture, out var inv)) return inv;
        if (double.TryParse(cleaned, NumberStyles.Any, PtBr, out var br)) return br;
        return 0;
    }

    private static object DbDate(string value) =>
        string.IsNullOrWhiteSpace(value) ? DBNull.Value : value.Length >= 10 ? value[..10] : value;

    private static string DbDateOrToday(string value) =>
        string.IsNullOrWhiteSpace(value) ? DateTime.Today.ToString("yyyy-MM-dd") : value.Length >= 10 ? value[..10] : value;

    private static bool IsTrue(string value)
    {
        var v = Clean(value).ToUpperInvariant();
        return v is "S" or "SIM" or "TRUE" or "1" or "T";
    }

    private static object DbUserOrNull(string value) =>
        string.IsNullOrWhiteSpace(value) ? DBNull.Value : DbUser(value);

    private static string DbUser(params string[] values)
    {
        var user = FirstNonEmpty(values).Trim();
        return string.IsNullOrWhiteSpace(user) ? "LEGADO" : user.ToUpperInvariant();
    }

    private static int GetOrCreateLegacyCliente(SqliteConnection con, string legacyCode)
    {
        var code = string.IsNullOrWhiteSpace(legacyCode) ? "SEM_CODIGO" : legacyCode.Trim();
        using var find = con.CreateCommand();
        find.CommandText = "SELECT id FROM clientes WHERE legacy_codigo=$c LIMIT 1";
        find.Parameters.AddWithValue("$c", code);
        var found = find.ExecuteScalar();
        if (found is not null) return Convert.ToInt32(found, CultureInfo.InvariantCulture);

        using var ins = con.CreateCommand();
        ins.CommandText = "INSERT INTO clientes(legacy_codigo,nome,created_by) VALUES($c,$n,'legacy-placeholder'); SELECT last_insert_rowid();";
        ins.Parameters.AddWithValue("$c", code);
        ins.Parameters.AddWithValue("$n", $"CLIENTE LEGADO {code}");
        return Convert.ToInt32(ins.ExecuteScalar(), CultureInfo.InvariantCulture);
    }

    private static int? GetLegacyRolId(SqliteConnection con, string legacyRol)
    {
        using var cmd = con.CreateCommand();
        cmd.CommandText = "SELECT id FROM ordens_servico WHERE legacy_rol=$rol LIMIT 1";
        cmd.Parameters.AddWithValue("$rol", legacyRol);
        var value = cmd.ExecuteScalar();
        return value is null ? null : Convert.ToInt32(value, CultureInfo.InvariantCulture);
    }

    private static Dictionary<string, int> GetLegacyRolMap(SqliteConnection con)
    {
        using var cmd = con.CreateCommand();
        cmd.CommandText = "SELECT legacy_rol, id FROM ordens_servico WHERE legacy_rol IS NOT NULL";
        using var r = cmd.ExecuteReader();
        var map = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        while (r.Read())
            if (!r.IsDBNull(0))
                map[r.GetString(0)] = r.GetInt32(1);
        return map;
    }

    private static string MapLegacyRolStatus(string posicao, string cancelado)
    {
        if (IsTrue(cancelado)) return "cancelada";
        return Clean(posicao).ToUpperInvariant() switch
        {
            "E" => "entregue",
            "P" => "pronta",
            "S" => "paga",
            "C" => "cancelada",
            _ => "aberta"
        };
    }

    private static string LegacyPk(string table, Dictionary<string, string> row) => table switch
    {
        "Usuarios" => V(row, "CodUsuario"),
        "Clientes" => V(row, "CodCli"),
        "MovCab" => V(row, "ROL"),
        "MovItem" => $"{V(row, "ROL")}-{V(row, "CodPro")}-{V(row, "SeqPro")}-{V(row, "CodCor")}",
        "MovItemSer" => $"{V(row, "Rol", "ROL")}-{V(row, "CodPro")}-{V(row, "SeqPro")}-{V(row, "CodSerLav")}",
        "Notas" => $"{V(row, "NumNot")}-{V(row, "CodCli")}-{V(row, "DatEmi")}",
        "Duplicat" => $"{V(row, "NumFat")}-{V(row, "NumDup")}-{V(row, "CodCli")}",
        "CliCredito" => V(row, "CodCli"),
        _ => Guid.NewGuid().ToString("N")
    };

    private static string Clean(string? s) => (s ?? "").Trim().Trim('"');
    private static object Db(string s) => string.IsNullOrWhiteSpace(s) ? DBNull.Value : s;
    private static double ParseMoney(string s)
    {
        var cleaned = Clean(s).Replace("R$", "", StringComparison.OrdinalIgnoreCase).Trim();
        return double.TryParse(cleaned, NumberStyles.Any, PtBr, out var v) ? v : 0;
    }

    // ── Configurações ──────────────────────────────────────────────────────────
    public Dictionary<string, string> GetConfiguracoes()
    {
        using var con = Open();
        using var cmd = con.CreateCommand();
        cmd.CommandText = "SELECT chave, valor FROM configuracoes ORDER BY chave";
        using var r = cmd.ExecuteReader();
        var d = new Dictionary<string, string>();
        while (r.Read()) d[r.GetString(0)] = r.GetString(1);
        return d;
    }

    public string? GetConfiguracao(string chave)
    {
        using var con = Open();
        using var cmd = con.CreateCommand();
        cmd.CommandText = "SELECT valor FROM configuracoes WHERE chave=$k";
        cmd.Parameters.AddWithValue("$k", chave);
        return cmd.ExecuteScalar() as string;
    }

    public void SetConfiguracao(string chave, string valor)
    {
        using var con = Open();
        using var cmd = con.CreateCommand();
        cmd.CommandText = "INSERT INTO configuracoes(chave,valor) VALUES($k,$v) ON CONFLICT(chave) DO UPDATE SET valor=$v, updated_at=datetime('now')";
        cmd.Parameters.AddWithValue("$k", chave);
        cmd.Parameters.AddWithValue("$v", valor);
        cmd.ExecuteNonQuery();
    }

    public object GetLegacyParams(string? secao = null)
    {
        using var con = Open();
        using var cmd = con.CreateCommand();
        cmd.CommandText = secao is null
            ? "SELECT fonte,secao,chave,valor FROM legacy_params ORDER BY fonte,secao,chave"
            : "SELECT fonte,secao,chave,valor FROM legacy_params WHERE secao=$s ORDER BY fonte,chave";
        if (secao is not null) cmd.Parameters.AddWithValue("$s", secao);
        using var r = cmd.ExecuteReader();
        var rows = new List<object>();
        while (r.Read()) rows.Add(new
        {
            fonte = r.GetString(0),
            secao = r.GetString(1),
            chave = r.GetString(2),
            valor = r.GetString(3)
        });
        return rows;
    }

    public object ListLegacyCoverage(string? area, string? status, int pg, int tam)
    {
        using var con = Open();
        var conds = new List<string>();
        if (!string.IsNullOrWhiteSpace(area)) conds.Add("area=$area");
        if (!string.IsNullOrWhiteSpace(status)) conds.Add("status=$status");
        var where = conds.Count == 0 ? "" : "WHERE " + string.Join(" AND ", conds);

        using var total = con.CreateCommand();
        total.CommandText = $"SELECT COUNT(*) FROM legacy_coverage {where}";
        if (!string.IsNullOrWhiteSpace(area)) total.Parameters.AddWithValue("$area", area);
        if (!string.IsNullOrWhiteSpace(status)) total.Parameters.AddWithValue("$status", status);
        var count = (long)(total.ExecuteScalar() ?? 0L);

        using var cmd = con.CreateCommand();
        cmd.CommandText = $"SELECT id,area,item,fonte,status,observacao,updated_by,updated_at FROM legacy_coverage {where} ORDER BY CASE status WHEN 'pendente' THEN 0 WHEN 'implementado_parcial' THEN 1 WHEN 'implementado' THEN 2 WHEN 'nao_aplicavel' THEN 3 ELSE 4 END, area, item LIMIT $lim OFFSET $off";
        if (!string.IsNullOrWhiteSpace(area)) cmd.Parameters.AddWithValue("$area", area);
        if (!string.IsNullOrWhiteSpace(status)) cmd.Parameters.AddWithValue("$status", status);
        cmd.Parameters.AddWithValue("$lim", tam);
        cmd.Parameters.AddWithValue("$off", Math.Max(0, pg - 1) * tam);
        using var r = cmd.ExecuteReader();
        var items = new List<object>();
        while (r.Read()) items.Add(new
        {
            id = r.GetInt32(0), area = r.GetString(1), item = r.GetString(2), fonte = r.GetString(3),
            status = r.GetString(4), observacao = r.IsDBNull(5) ? null : r.GetString(5),
            updatedBy = r.IsDBNull(6) ? null : r.GetString(6), updatedAt = r.GetString(7)
        });

        using var sum = con.CreateCommand();
        sum.CommandText = "SELECT status, COUNT(*) FROM legacy_coverage GROUP BY status ORDER BY status";
        using var rs = sum.ExecuteReader();
        var resumo = new Dictionary<string, long>();
        while (rs.Read()) resumo[rs.GetString(0)] = rs.GetInt64(1);
        return new { total = count, pagina = pg, tamanho = tam, resumo, items };
    }

    public void UpdateLegacyCoverage(int id, string status, string? observacao, string usuario)
    {
        var allowed = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "pendente", "implementado", "implementado_parcial", "nao_aplicavel", "substituido", "descartado_validado"
        };
        if (!allowed.Contains(status)) throw new InvalidOperationException("Status de cobertura invalido.");
        using var con = Open();
        con.Execute(@"UPDATE legacy_coverage SET status=$s, observacao=$o, updated_by=$u, updated_at=datetime('now') WHERE id=$id",
            ("$s", status), ("$o", observacao ?? (object)DBNull.Value), ("$u", usuario), ("$id", id));
    }

    // ── Clientes ───────────────────────────────────────────────────────────────
    public object ListarClientes(string? q, int pg, int tam)
    {
        using var con = Open();
        var where = q is { Length: > 0 } ? "AND (nome LIKE $q OR documento LIKE $q OR telefone LIKE $q OR celular LIKE $q)" : "";
        using var total = con.CreateCommand();
        total.CommandText = $"SELECT COUNT(*) FROM clientes WHERE ativo=1 {where}";
        if (q != null) total.Parameters.AddWithValue("$q", $"%{q}%");
        var tot = (long)(total.ExecuteScalar() ?? 0L);

        using var cmd = con.CreateCommand();
        cmd.CommandText = $"SELECT * FROM clientes WHERE ativo=1 {where} ORDER BY nome LIMIT $lim OFFSET $off";
        if (q != null) cmd.Parameters.AddWithValue("$q", $"%{q}%");
        cmd.Parameters.AddWithValue("$lim", tam);
        cmd.Parameters.AddWithValue("$off", (pg - 1) * tam);
        using var r = cmd.ExecuteReader();
        var items = ReadClientes(r).ToList();
        return new { total = tot, pagina = pg, tamanho = tam, items };
    }

    public object? ObterCliente(int id)
    {
        using var con = Open();
        using var cmd = con.CreateCommand();
        cmd.CommandText = "SELECT * FROM clientes WHERE id=$id";
        cmd.Parameters.AddWithValue("$id", id);
        using var r = cmd.ExecuteReader();
        return ReadClientes(r).FirstOrDefault();
    }

    public int CriarCliente(ClienteRequest req, string usuario)
    {
        using var con = Open();
        using var cmd = con.CreateCommand();
        cmd.CommandText = @"
INSERT INTO clientes(nome,documento,telefone,celular,email,logradouro,numero,bairro,cidade,estado,cep,observacoes,limite_credito,desconto_percent,created_by,data_nascimento,cartao_fidelidade,contato)
VALUES($nm,$doc,$tel,$cel,$eml,$log,$num,$bai,$cid,$est,$cep,$obs,$lim,$desc,$usr,$datnas,$cartfid,$contato);
SELECT last_insert_rowid();";
        SetClienteParams(cmd, req, usuario);
        var id = (int)(long)(cmd.ExecuteScalar() ?? 0L);
        RegistrarHistoricoCliente(con, id, "criado", null, usuario);
        return id;
    }

    public void AtualizarCliente(int id, ClienteRequest req, string usuario)
    {
        using var con = Open();
        using var cmd = con.CreateCommand();
        cmd.CommandText = @"
UPDATE clientes SET nome=$nm,documento=$doc,telefone=$tel,celular=$cel,email=$eml,
  logradouro=$log,numero=$num,bairro=$bai,cidade=$cid,estado=$est,cep=$cep,
  observacoes=$obs,limite_credito=$lim,desconto_percent=$desc,updated_at=datetime('now'),
  data_nascimento=$datnas,cartao_fidelidade=$cartfid,contato=$contato
WHERE id=$id";
        SetClienteParams(cmd, req, usuario);
        cmd.Parameters.AddWithValue("$id", id);
        cmd.ExecuteNonQuery();
        RegistrarHistoricoCliente(con, id, "atualizado", null, usuario);
    }

    public void InativarCliente(int id, string usuario)
    {
        using var con = Open();
        con.Execute("UPDATE clientes SET ativo=0, updated_at=datetime('now') WHERE id=$id",
            ("$id", id));
        RegistrarHistoricoCliente(con, id, "inativado", null, usuario);
    }

    public IEnumerable<object> HistoricoCliente(int id)
    {
        using var con = Open();
        using var cmd = con.CreateCommand();
        cmd.CommandText = "SELECT * FROM clientes_historico WHERE cliente_id=$id ORDER BY created_at DESC LIMIT 100";
        cmd.Parameters.AddWithValue("$id", id);
        using var r = cmd.ExecuteReader();
        var list = new List<object>();
        while (r.Read()) list.Add(new
        {
            id = r.GetInt32(0), clienteId = r.GetInt32(1),
            evento = r.GetString(2), detalhe = r.IsDBNull(3) ? null : r.GetString(3),
            usuario = r.GetString(4), createdAt = r.GetString(5)
        });
        return list;
    }

    public object GetCreditoCliente(int clienteId)
    {
        using var con = Open();
        using var cmd = con.CreateCommand();
        cmd.CommandText = "SELECT saldo, updated_at FROM clientes_credito WHERE cliente_id=$id";
        cmd.Parameters.AddWithValue("$id", clienteId);
        using var r = cmd.ExecuteReader();
        if (!r.Read()) return new { clienteId, saldo = 0.0, movimentos = new object[0] };
        var saldo = r.GetDouble(0);
        r.Close();
        using var m = con.CreateCommand();
        m.CommandText = "SELECT * FROM clientes_credito_movimentos WHERE cliente_id=$id ORDER BY created_at DESC LIMIT 30";
        m.Parameters.AddWithValue("$id", clienteId);
        using var r2 = m.ExecuteReader();
        var movs = new List<object>();
        while (r2.Read()) movs.Add(new
        {
            id = r2.GetInt32(0), tipo = r2.GetString(2), valor = r2.GetDouble(3),
            descricao = r2.IsDBNull(4) ? null : r2.GetString(4),
            referencia = r2.IsDBNull(5) ? null : r2.GetString(5),
            usuario = r2.GetString(6), createdAt = r2.GetString(7)
        });
        return new { clienteId, saldo, movimentos = movs };
    }

    public void LancarCreditoCliente(int clienteId, CreditoRequest req, string usuario)
    {
        using var con = Open();
        con.Execute(@"INSERT OR IGNORE INTO clientes_credito(cliente_id,saldo) VALUES($id,0)",
            ("$id", clienteId));
        var delta = req.Tipo == "credito" ? req.Valor : -req.Valor;
        con.Execute("UPDATE clientes_credito SET saldo=saldo+$d, updated_at=datetime('now') WHERE cliente_id=$id",
            ("$d", delta), ("$id", clienteId));
        using var cmd = con.CreateCommand();
        cmd.CommandText = "INSERT INTO clientes_credito_movimentos(cliente_id,tipo,valor,descricao,referencia,usuario) VALUES($id,$t,$v,$d,$r,$u)";
        cmd.Parameters.AddWithValue("$id", clienteId);
        cmd.Parameters.AddWithValue("$t", req.Tipo);
        cmd.Parameters.AddWithValue("$v", req.Valor);
        cmd.Parameters.AddWithValue("$d", req.Descricao ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("$r", req.Referencia ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("$u", usuario);
        cmd.ExecuteNonQuery();
    }

    // ── Serviços ───────────────────────────────────────────────────────────────
    public IEnumerable<object> ListarServicos(string? q)
    {
        using var con = Open();
        var where = q is { Length: > 0 } ? "AND (descricao LIKE $q OR codigo LIKE $q OR categoria LIKE $q)" : "";
        using var cmd = con.CreateCommand();
        cmd.CommandText = $"SELECT * FROM servicos WHERE ativo=1 {where} ORDER BY categoria, descricao";
        if (q != null) cmd.Parameters.AddWithValue("$q", $"%{q}%");
        using var r = cmd.ExecuteReader();
        return ReadServicos(r).ToList();
    }

    public IEnumerable<string> ListarCategorias()
    {
        using var con = Open();
        using var cmd = con.CreateCommand();
        cmd.CommandText = "SELECT DISTINCT categoria FROM servicos WHERE ativo=1 ORDER BY categoria";
        using var r = cmd.ExecuteReader();
        var cats = new List<string>();
        while (r.Read()) cats.Add(r.GetString(0));
        return cats;
    }

    public object? ObterServico(int id)
    {
        using var con = Open();
        using var cmd = con.CreateCommand();
        cmd.CommandText = "SELECT * FROM servicos WHERE id=$id";
        cmd.Parameters.AddWithValue("$id", id);
        using var r = cmd.ExecuteReader();
        return ReadServicos(r).FirstOrDefault();
    }

    public int CriarServico(ServicoRequest req)
    {
        using var con = Open();
        using var cmd = con.CreateCommand();
        cmd.CommandText = "INSERT INTO servicos(codigo,descricao,categoria,preco) VALUES($c,$d,$g,$p); SELECT last_insert_rowid();";
        cmd.Parameters.AddWithValue("$c", req.Codigo.Trim().ToUpper());
        cmd.Parameters.AddWithValue("$d", req.Descricao.Trim().ToUpper());
        cmd.Parameters.AddWithValue("$g", req.Categoria.Trim().ToUpper());
        cmd.Parameters.AddWithValue("$p", req.Preco);
        return (int)(long)(cmd.ExecuteScalar() ?? 0L);
    }

    public void AtualizarServico(int id, ServicoRequest req)
    {
        using var con = Open();
        con.Execute("UPDATE servicos SET codigo=$c,descricao=$d,categoria=$g,preco=$p,updated_at=datetime('now') WHERE id=$id",
            ("$c", req.Codigo.Trim().ToUpper()), ("$d", req.Descricao.Trim().ToUpper()),
            ("$g", req.Categoria.Trim().ToUpper()), ("$p", req.Preco), ("$id", id));
    }

    /// Reajuste em massa de preços: por lista de ids (seleção manual), por categoria inteira,
    /// ou por todas as categorias de uma vez (TodasCategorias=true), aumentando ou diminuindo
    /// por valor fixo (R$) ou percentual. Nunca deixa o preço negativo.
    public int AjustarPrecos(AjustarPrecosRequest req)
    {
        var temIds = req.Ids is { Count: > 0 };
        var temCategoria = !string.IsNullOrWhiteSpace(req.Categoria);
        var todasCategorias = req.TodasCategorias == true;
        if (!temIds && !temCategoria && !todasCategorias)
            throw new InvalidOperationException("Selecione ao menos um serviço, uma categoria, ou marque \"todas as categorias\".");
        if (req.Valor <= 0)
            throw new InvalidOperationException("Informe um valor de ajuste maior que zero.");

        var sinal = req.Tipo.Equals("diminuir", StringComparison.OrdinalIgnoreCase) ? -1 : 1;
        var ajuste = req.Modo.Equals("percentual", StringComparison.OrdinalIgnoreCase)
            ? "preco * (1 + ($sinal * $valor / 100.0))"
            : "preco + ($sinal * $valor)";

        using var con = Open();
        using var cmd = con.CreateCommand();
        cmd.Parameters.AddWithValue("$sinal", sinal);
        cmd.Parameters.AddWithValue("$valor", req.Valor);
        var where = new List<string> { "ativo=1" };
        if (temCategoria)
        {
            where.Add("categoria=$cat");
            cmd.Parameters.AddWithValue("$cat", req.Categoria!.Trim().ToUpper());
        }
        if (temIds)
        {
            var placeholders = req.Ids!.Select((id, i) => $"$id{i}").ToList();
            for (var i = 0; i < req.Ids!.Count; i++) cmd.Parameters.AddWithValue($"$id{i}", req.Ids[i]);
            where.Add($"id IN ({string.Join(",", placeholders)})");
        }
        cmd.CommandText = $"UPDATE servicos SET preco = MAX(0, {ajuste}), updated_at=datetime('now') WHERE {string.Join(" AND ", where)}";
        return cmd.ExecuteNonQuery();
    }

    public void InativarServico(int id)
    {
        using var con = Open();
        con.Execute("UPDATE servicos SET ativo=0, updated_at=datetime('now') WHERE id=$id", ("$id", id));
    }

    // ── ROL ───────────────────────────────────────────────────────────────────
    public object ListarRols(string? status, int? clienteId, string? de, string? ate, string? q, int pg, int tam)
    {
        using var con = Open();
        var conds = new List<string>();
        if (status != null) conds.Add("os.status=$status");
        if (clienteId.HasValue) conds.Add("os.cliente_id=$cliId");
        if (de != null) conds.Add("os.data_entrada>=$de");
        if (ate != null) conds.Add("os.data_entrada<=$ate");
        if (q != null) conds.Add("(os.numero LIKE $q OR c.nome LIKE $q)");
        var where = conds.Count > 0 ? "WHERE " + string.Join(" AND ", conds) : "";

        var sql = $@"
SELECT os.*, c.nome as cliente_nome, c.telefone as cliente_tel,
       (SELECT COUNT(*) FROM os_itens i WHERE i.os_id=os.id) as total_itens
FROM ordens_servico os JOIN clientes c ON c.id=os.cliente_id
{where} ORDER BY os.id DESC LIMIT $lim OFFSET $off";

        using var cnt = con.CreateCommand();
        cnt.CommandText = $"SELECT COUNT(*) FROM ordens_servico os JOIN clientes c ON c.id=os.cliente_id {where}";
        var cntSql = cnt;
        SetRolFilterParams(cntSql, status, clienteId, de, ate, q);
        var tot = (long)(cnt.ExecuteScalar() ?? 0L);

        using var cmd = con.CreateCommand();
        cmd.CommandText = sql;
        SetRolFilterParams(cmd, status, clienteId, de, ate, q);
        cmd.Parameters.AddWithValue("$lim", tam);
        cmd.Parameters.AddWithValue("$off", (pg - 1) * tam);
        using var r = cmd.ExecuteReader();
        var items = ReadRolsResumo(r).ToList();
        return new { total = tot, pagina = pg, tamanho = tam, items };
    }

    public object? ObterRol(int id)
    {
        using var con = Open();
        using var cmd = con.CreateCommand();
        cmd.CommandText = @"
SELECT os.*, c.nome as cliente_nome, c.telefone as cliente_tel, c.celular as cliente_cel,
       c.limite_credito, c.desconto_percent
FROM ordens_servico os JOIN clientes c ON c.id=os.cliente_id
WHERE os.id=$id";
        cmd.Parameters.AddWithValue("$id", id);
        using var r = cmd.ExecuteReader();
        if (!r.Read()) return null;
        var rol = ReadRolCompleto(r);
        r.Close();

        using var i = con.CreateCommand();
        i.CommandText = "SELECT oi.*, s.codigo as srv_codigo, s.categoria as srv_cat FROM os_itens oi LEFT JOIN servicos s ON s.id=oi.servico_id WHERE oi.os_id=$id ORDER BY oi.id";
        i.Parameters.AddWithValue("$id", id);
        using var ri = i.ExecuteReader();
        var itens = ReadItens(ri).ToList();
        return new { rol, itens };
    }

    public int CriarRol(RolRequest req, string usuario)
    {
        using var con = Open();
        using var tx = con.BeginTransaction();
        // Numero derivado do id real (não de COUNT(*)) — evita colisão em concorrência ou após exclusões.
        using var cmd = con.CreateCommand();
        cmd.Transaction = tx;
        cmd.CommandText = @"
INSERT INTO ordens_servico(numero,cliente_id,data_entrada,data_promessa,observacoes,usuario_entrada)
VALUES('',$cli,$de,$pr,$obs,$usr);
SELECT last_insert_rowid();";
        cmd.Parameters.AddWithValue("$cli", req.ClienteId);
        cmd.Parameters.AddWithValue("$de", req.DataEntrada ?? DateTime.Today.ToString("yyyy-MM-dd"));
        cmd.Parameters.AddWithValue("$pr", req.DataPromessa ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("$obs", req.Observacoes ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("$usr", usuario);
        var id = (int)(long)(cmd.ExecuteScalar() ?? 0L);

        using var upd = con.CreateCommand();
        upd.Transaction = tx;
        upd.CommandText = "UPDATE ordens_servico SET numero=$num WHERE id=$id";
        upd.Parameters.AddWithValue("$num", $"ROL{id:D6}");
        upd.Parameters.AddWithValue("$id", id);
        upd.ExecuteNonQuery();

        RegistrarHistoricoOs(con, id, "criado", null, "aberta", null, usuario);
        tx.Commit();
        return id;
    }

    public void AtualizarRolCabecalho(int id, RolRequest req, string usuario)
    {
        EnsureRolEditavel(id);
        using var con = Open();
        con.Execute(@"UPDATE ordens_servico SET cliente_id=$cli,data_promessa=$pr,observacoes=$obs,updated_at=datetime('now') WHERE id=$id",
            ("$cli", req.ClienteId), ("$pr", req.DataPromessa ?? (object)DBNull.Value),
            ("$obs", req.Observacoes ?? (object)DBNull.Value), ("$id", id));
    }

    public void AdicionarItemRol(int osId, RolItemRequest req, string usuario)
    {
        EnsureRolEditavel(osId);
        using var con = Open();
        var total = req.Quantidade * req.ValorUnitario;
        using var cmd = con.CreateCommand();
        cmd.CommandText = @"
INSERT INTO os_itens(os_id,servico_id,descricao,tipo_tecido,cor,marca,defeito,quantidade,valor_unitario,valor_total,observacao,obs2,peso,identificacao,localizacao,valor_terceiro)
VALUES($os,$srv,$desc,$tec,$cor,$mar,$def,$qty,$vu,$vt,$obs,$obs2,$peso,$ident,$loc,$vterc)";
        cmd.Parameters.AddWithValue("$os", osId);
        cmd.Parameters.AddWithValue("$srv", req.ServicoId ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("$desc", req.Descricao);
        cmd.Parameters.AddWithValue("$tec", req.TipoTecido ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("$cor", req.Cor ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("$mar", req.Marca ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("$def", req.Defeito ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("$qty", req.Quantidade);
        cmd.Parameters.AddWithValue("$vu", req.ValorUnitario);
        cmd.Parameters.AddWithValue("$vt", total);
        cmd.Parameters.AddWithValue("$obs", req.Observacao ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("$obs2", req.Obs2 ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("$peso", req.Peso.HasValue ? (object)req.Peso.Value : DBNull.Value);
        cmd.Parameters.AddWithValue("$ident", req.Identificacao ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("$loc", req.Localizacao ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("$vterc", req.ValorTerceiro.HasValue ? (object)req.ValorTerceiro.Value : DBNull.Value);
        cmd.ExecuteNonQuery();
        RecalcularTotalRol(con, osId);
    }

    public void AtualizarItemRol(int osId, int itemId, RolItemRequest req, string usuario)
    {
        EnsureRolEditavel(osId);
        var total = req.Quantidade * req.ValorUnitario;
        using var con = Open();
        con.Execute(@"UPDATE os_itens SET servico_id=$srv,descricao=$desc,tipo_tecido=$tec,cor=$cor,marca=$mar,defeito=$def,quantidade=$qty,valor_unitario=$vu,valor_total=$vt,observacao=$obs,obs2=$obs2,peso=$peso,identificacao=$ident,localizacao=$loc,valor_terceiro=$vterc WHERE id=$id AND os_id=$os",
            ("$srv", req.ServicoId ?? (object)DBNull.Value), ("$desc", req.Descricao),
            ("$tec", req.TipoTecido ?? (object)DBNull.Value), ("$cor", req.Cor ?? (object)DBNull.Value),
            ("$mar", req.Marca ?? (object)DBNull.Value), ("$def", req.Defeito ?? (object)DBNull.Value),
            ("$qty", req.Quantidade), ("$vu", req.ValorUnitario), ("$vt", total),
            ("$obs", req.Observacao ?? (object)DBNull.Value),
            ("$obs2", req.Obs2 ?? (object)DBNull.Value),
            ("$peso", req.Peso.HasValue ? (object)req.Peso.Value : DBNull.Value),
            ("$ident", req.Identificacao ?? (object)DBNull.Value),
            ("$loc", req.Localizacao ?? (object)DBNull.Value),
            ("$vterc", req.ValorTerceiro.HasValue ? (object)req.ValorTerceiro.Value : DBNull.Value),
            ("$id", itemId), ("$os", osId));
        RecalcularTotalRol(con, osId);
    }

    public void RemoverItemRol(int osId, int itemId, string usuario)
    {
        EnsureRolEditavel(osId);
        using var con = Open();
        con.Execute("DELETE FROM os_itens WHERE id=$id AND os_id=$os", ("$id", itemId), ("$os", osId));
        RecalcularTotalRol(con, osId);
    }

    public void MarcarRolPronta(int id, string usuario)
    {
        using var con = Open();
        var status = GetRolStatus(con, id);
        if (status != "aberta") throw new InvalidOperationException($"ROL não pode ser marcada como pronta no status '{status}'.");
        con.Execute("UPDATE ordens_servico SET status='pronta', updated_at=datetime('now') WHERE id=$id", ("$id", id));
        RegistrarHistoricoOs(con, id, "marcada_pronta", "aberta", "pronta", null, usuario);
    }

    public void EntregarRol(int id, EntregarRequest req, string usuario)
    {
        using var con = Open();
        var status = GetRolStatus(con, id);
        if (status != "pronta" && status != "aberta")
            throw new InvalidOperationException($"ROL não pode ser entregue no status '{status}'.");
        con.Execute(@"UPDATE ordens_servico SET status='entregue', data_entrega=$de, usuario_entrega=$usr, updated_at=datetime('now') WHERE id=$id",
            ("$de", req.DataEntrega ?? DateTime.Today.ToString("yyyy-MM-dd")), ("$usr", usuario), ("$id", id));
        RegistrarHistoricoOs(con, id, "entregue", status, "entregue", req.Observacao, usuario);
    }

    public object PagarRol(int id, PagamentoRequest req, string usuario)
    {
        using var con = Open();
        var status = GetRolStatus(con, id);
        if (status == "paga" || status == "cancelada" || status == "estornada")
            throw new InvalidOperationException($"ROL já está '{status}'.");

        // Sem caixa aberto o recebimento não pode ser lançado em caixa_movimentos (RegistrarMovimentoCaixa
        // simplesmente ignorava o lançamento antes desta checagem), causando divergência entre o financeiro
        // e o fechamento de caixa do dia. Exige caixa aberto antes de confirmar o pagamento.
        using var chkCaixa = con.CreateCommand();
        chkCaixa.CommandText = "SELECT COUNT(*) FROM caixa_sessoes WHERE status='aberta'";
        if ((long)(chkCaixa.ExecuteScalar() ?? 0L) == 0)
            throw new InvalidOperationException("Abra o caixa antes de registrar pagamentos.");

        // Pagamento dividido: usa Linhas se vier preenchido; senão cai no formato antigo
        // (um único MetodoPagamento/ValorPago), mantendo compatibilidade.
        var linhas = (req.Linhas is { Count: > 0 })
            ? req.Linhas
            : [new PagamentoLinha(req.MetodoPagamento ?? throw new InvalidOperationException("Informe a forma de pagamento."), req.ValorPago ?? 0)];

        var valorPagoTotal = linhas.Sum(l => l.Valor);
        var metodoResumo = linhas.Count == 1 ? linhas[0].Metodo : "misto(" + string.Join("+", linhas.Select(l => l.Metodo).Distinct()) + ")";

        // Verifica valor
        using var vCmd = con.CreateCommand();
        vCmd.CommandText = "SELECT valor_final FROM ordens_servico WHERE id=$id";
        vCmd.Parameters.AddWithValue("$id", id);
        var valorFinal = (double)(vCmd.ExecuteScalar() ?? 0.0);
        if (req.Desconto.HasValue) valorFinal -= req.Desconto.Value;
        var troco = valorPagoTotal - valorFinal;

        // Credito do cliente? — valida só a parcela paga com crédito, não o total da venda.
        var valorCreditoCliente = linhas.Where(l => l.Metodo == "credito_cliente").Sum(l => l.Valor);
        if (valorCreditoCliente > 0)
        {
            using var credCmd = con.CreateCommand();
            credCmd.CommandText = "SELECT saldo FROM clientes_credito cc JOIN ordens_servico os ON os.cliente_id=cc.cliente_id WHERE os.id=$id";
            credCmd.Parameters.AddWithValue("$id", id);
            var saldo = (double)(credCmd.ExecuteScalar() ?? 0.0);
            if (saldo < valorCreditoCliente)
                throw new InvalidOperationException($"Saldo em crédito insuficiente (R$ {saldo:F2}).");
        }

        using var cmd = con.CreateCommand();
        cmd.CommandText = @"UPDATE ordens_servico SET
  status='paga', data_pagamento=datetime('now'), valor_pago=$vp,
  metodo_pagamento=$met, troco=$troco, desconto=COALESCE($desc, desconto),
  valor_final=COALESCE($vf, valor_final), usuario_pagamento=$usr, updated_at=datetime('now')
  WHERE id=$id";
        cmd.Parameters.AddWithValue("$vp", valorPagoTotal);
        cmd.Parameters.AddWithValue("$met", metodoResumo);
        cmd.Parameters.AddWithValue("$troco", troco);
        cmd.Parameters.AddWithValue("$desc", req.Desconto.HasValue ? (object)req.Desconto.Value : DBNull.Value);
        cmd.Parameters.AddWithValue("$vf", req.Desconto.HasValue ? valorFinal : DBNull.Value);
        cmd.Parameters.AddWithValue("$usr", usuario);
        cmd.Parameters.AddWithValue("$id", id);
        cmd.ExecuteNonQuery();

        // Uma linha por forma de pagamento — registro em pagamentos, caixa e crédito do cliente
        // ficam quebrados por método, não somados num lançamento só.
        for (var i = 0; i < linhas.Count; i++)
        {
            var linha = linhas[i];
            var trocoLinha = (i == linhas.Count - 1) ? troco : 0; // troco só sai na última linha (normalmente a de dinheiro)

            using var pag = con.CreateCommand();
            pag.CommandText = "INSERT INTO pagamentos(os_id,metodo,valor,troco,usuario) VALUES($os,$met,$v,$t,$u)";
            pag.Parameters.AddWithValue("$os", id);
            pag.Parameters.AddWithValue("$met", linha.Metodo);
            pag.Parameters.AddWithValue("$v", linha.Valor);
            pag.Parameters.AddWithValue("$t", trocoLinha > 0 ? trocoLinha : 0);
            pag.Parameters.AddWithValue("$u", usuario);
            pag.ExecuteNonQuery();

            RegistrarMovimentoCaixa(con, id, linha.Metodo, linha.Valor, usuario);

            if (linha.Metodo == "credito_cliente")
            {
                using var cli = con.CreateCommand();
                cli.CommandText = "SELECT cliente_id FROM ordens_servico WHERE id=$id";
                cli.Parameters.AddWithValue("$id", id);
                var cliId = (int)(long)(cli.ExecuteScalar() ?? 0L);
                LancarCreditoCliente(cliId, new CreditoRequest("debito", linha.Valor, "Pagamento ROL", id.ToString()), usuario);
            }
        }

        RegistrarHistoricoOs(con, id, "paga", status, "paga", metodoResumo, usuario);
        CreditarPontosPorVenda(con, id, usuario);
        return ObterRol(id)!;
    }

    /// Reverte uma ROL já paga: registra o estorno no caixa (se houver sessão aberta) e
    /// muda o status para 'estornada' — distinto de 'cancelada' (que só existe pré-pagamento),
    /// pra manter claro no histórico que houve dinheiro envolvido e devolvido.
    public object EstornarRol(int id, string motivo, string usuario)
    {
        if (string.IsNullOrWhiteSpace(motivo)) throw new InvalidOperationException("Motivo do estorno é obrigatório.");
        using var con = Open();
        var status = GetRolStatus(con, id);
        if (status != "paga") throw new InvalidOperationException($"Só é possível estornar uma ROL paga (status atual: '{status}').");

        using var vCmd = con.CreateCommand();
        vCmd.CommandText = "SELECT valor_pago FROM ordens_servico WHERE id=$id";
        vCmd.Parameters.AddWithValue("$id", id);
        var valorPago = (double)(vCmd.ExecuteScalar() ?? 0.0);

        using var chkCaixa = con.CreateCommand();
        chkCaixa.CommandText = "SELECT id FROM caixa_sessoes WHERE status='aberta' ORDER BY id DESC LIMIT 1";
        var sessaoId = chkCaixa.ExecuteScalar();
        if (sessaoId is not null && valorPago > 0)
        {
            using var mov = con.CreateCommand();
            mov.CommandText = "INSERT INTO caixa_movimentos(sessao_id,tipo,valor,descricao,os_id,usuario) VALUES($s,'sangria',$v,$d,$os,$u)";
            mov.Parameters.AddWithValue("$s", (long)sessaoId);
            mov.Parameters.AddWithValue("$v", valorPago);
            mov.Parameters.AddWithValue("$d", $"Estorno ROL — {motivo}");
            mov.Parameters.AddWithValue("$os", id);
            mov.Parameters.AddWithValue("$u", usuario);
            mov.ExecuteNonQuery();
        }

        con.Execute("UPDATE ordens_servico SET status='estornada', motivo_cancelamento=$mot, data_cancelamento=date('now'), updated_at=datetime('now') WHERE id=$id",
            ("$mot", motivo), ("$id", id));
        RegistrarHistoricoOs(con, id, "estornada", status, "estornada", motivo, usuario);
        return ObterRol(id)!;
    }

    public void CancelarRol(int id, string? motivo, string usuario)
    {
        using var con = Open();
        var status = GetRolStatus(con, id);
        if (status == "paga") throw new InvalidOperationException("ROL já paga não pode ser cancelada — use estornar.");
        if (status == "cancelada") throw new InvalidOperationException("ROL já cancelada.");
        if (status == "estornada") throw new InvalidOperationException("ROL já estornada.");
        con.Execute("UPDATE ordens_servico SET status='cancelada', motivo_cancelamento=$mot, data_cancelamento=date('now'), updated_at=datetime('now') WHERE id=$id",
            ("$mot", motivo ?? (object)DBNull.Value), ("$id", id));
        RegistrarHistoricoOs(con, id, "cancelada", status, "cancelada", motivo, usuario);
    }

    public object GerarRecibo(int id)
    {
        var rol = ObterRol(id) ?? throw new KeyNotFoundException("ROL não encontrado.");
        var cfg = GetConfiguracoes();
        return new
        {
            empresa = new { nome = cfg.GetValueOrDefault("empresa_nome"), tel = cfg.GetValueOrDefault("empresa_telefone"), end = cfg.GetValueOrDefault("empresa_endereco"), cidade = cfg.GetValueOrDefault("empresa_cidade"), cnpj = cfg.GetValueOrDefault("empresa_cnpj") },
            rol,
            dataImpressao = DateTime.Now.ToString("dd/MM/yyyy HH:mm")
        };
    }

    public string GerarImpressaoRolTexto(int id, string tipo)
    {
        using var con = Open();
        var (numero, cliente, telefone, status, total, dataEntrada, dataPromessa) = GetRolPrintHeader(con, id);
        var itens = GetRolPrintItems(con, id);
        var cfg = GetConfiguracoes();
        var vias = GetLegacyParamInt("INDUSTRIAL", tipo.Equals("pagamento", StringComparison.OrdinalIgnoreCase) ? "QdeViasPag" : "QdeViasRol", 1);
        // impressora_termica_largura (configurável na tela de Impressoras) tem prioridade
        // sobre o parâmetro legado do Paradox — só cai no legado se nunca foi configurado.
        var width = int.TryParse(cfg.GetValueOrDefault("impressora_termica_largura"), out var w)
            ? w
            : GetLegacyParamInt("INDUSTRIAL", "LarguraRol", 42);
        width = Math.Clamp(width, 32, 64);

        var sb = new StringBuilder();
        for (var via = 1; via <= vias; via++)
        {
            if (via > 1) sb.AppendLine().AppendLine(new string('-', width)).AppendLine();
            sb.AppendLine(Center(cfg.GetValueOrDefault("empresa_nome", "ATELIE DA LUCI"), width));
            var cfgEnd = cfg.GetValueOrDefault("empresa_endereco", "");
            var cfgTel = cfg.GetValueOrDefault("empresa_telefone", "");
            if (!string.IsNullOrWhiteSpace(cfgEnd)) sb.AppendLine(Center(cfgEnd, width));
            if (!string.IsNullOrWhiteSpace(cfgTel)) sb.AppendLine(Center(cfgTel, width));
            sb.AppendLine(new string('-', width));
            var tipoLabel = tipo.Equals("pagamento", StringComparison.OrdinalIgnoreCase) ? "PAGAMENTO" : "VENDA";
            sb.AppendLine($"{tipoLabel} VIA {via}/{vias}");
            // numero é gravado como "ROL000123" (histórico) — no recibo mostramos só o número.
            var numeroExibicao = numero.StartsWith("ROL", StringComparison.OrdinalIgnoreCase) ? numero[3..] : numero;
            sb.AppendLine($"VENDA: {numeroExibicao}   STATUS: {status}");
            sb.AppendLine($"CLIENTE: {cliente}");
            if (!string.IsNullOrWhiteSpace(telefone)) sb.AppendLine($"FONE: {telefone}");
            sb.AppendLine($"ENTRADA: {dataEntrada}  PROMESSA: {dataPromessa}");
            sb.AppendLine(new string('-', width));
            foreach (var item in itens)
            {
                var desc = item.Descricao.Length > width - 10 ? item.Descricao[..(width - 10)] : item.Descricao;
                sb.AppendLine($"{item.Quantidade:0.##}x {desc}");
                sb.AppendLine($"  {Money(item.ValorUnitario)}  TOTAL {Money(item.ValorTotal)}");
                var det = string.Join(" / ", new[] { item.Cor, item.Marca, item.Defeito, item.Observacao }.Where(x => !string.IsNullOrWhiteSpace(x)));
                if (det.Length > 0) sb.AppendLine($"  {det}");
            }
            sb.AppendLine(new string('-', width));
            sb.AppendLine($"TOTAL: {Money(total)}");
            sb.AppendLine($"IMPRESSO: {DateTime.Now:dd/MM/yyyy HH:mm}");
            sb.AppendLine(Center("Obrigado pela preferencia", width));
        }
        return sb.ToString();
    }

    public string GerarEtiquetaArgox(int id)
    {
        using var con = Open();
        var (numero, cliente, _, status, total, _, dataPromessa) = GetRolPrintHeader(con, id);
        var widthDots = GetLegacyParamInt("INICIO", "Linha2", 300);
        var sb = new StringBuilder();
        sb.AppendLine("N");
        sb.AppendLine($"q{widthDots}");
        sb.AppendLine(GetLegacyParam("INICIO", "Linha4", "D7"));
        sb.AppendLine(GetLegacyParam("INICIO", "Linha5", "S2"));
        sb.AppendLine(GetLegacyParam("INICIO", "Linha6", "OC1"));
        sb.AppendLine($"A20,20,0,2,1,1,N,\"ROL {SanitizePplb(numero)}\"");
        sb.AppendLine($"A20,48,0,2,1,1,N,\"{SanitizePplb(cliente, 24)}\"");
        sb.AppendLine($"A20,76,0,2,1,1,N,\"ENT {SanitizePplb(dataPromessa ?? "")}\"");
        sb.AppendLine($"A20,104,0,2,1,1,N,\"{SanitizePplb(status)} {SanitizePplb(Money(total))}\"");
        sb.AppendLine($"B20,132,0,1,2,4,50,N,\"{SanitizePplb(numero)}\"");
        sb.AppendLine("P1");
        return sb.ToString();
    }

    private (string Numero, string Cliente, string? Telefone, string Status, double Total, string DataEntrada, string? DataPromessa) GetRolPrintHeader(SqliteConnection con, int id)
    {
        using var cmd = con.CreateCommand();
        cmd.CommandText = @"SELECT os.numero,c.nome,COALESCE(c.telefone,c.celular,''),os.status,os.valor_final,os.data_entrada,os.data_promessa
FROM ordens_servico os JOIN clientes c ON c.id=os.cliente_id WHERE os.id=$id";
        cmd.Parameters.AddWithValue("$id", id);
        using var r = cmd.ExecuteReader();
        if (!r.Read()) throw new KeyNotFoundException("ROL nao encontrada.");
        return (r.GetString(0), r.GetString(1), r.IsDBNull(2) ? null : r.GetString(2), r.GetString(3), r.GetDouble(4), r.GetString(5), r.IsDBNull(6) ? null : r.GetString(6));
    }

    private List<PrintItem> GetRolPrintItems(SqliteConnection con, int id)
    {
        using var cmd = con.CreateCommand();
        cmd.CommandText = "SELECT descricao,quantidade,valor_unitario,valor_total,cor,marca,defeito,observacao FROM os_itens WHERE os_id=$id ORDER BY id";
        cmd.Parameters.AddWithValue("$id", id);
        using var r = cmd.ExecuteReader();
        var list = new List<PrintItem>();
        while (r.Read()) list.Add(new(
            r.GetString(0), r.GetDouble(1), r.GetDouble(2), r.GetDouble(3),
            r.IsDBNull(4) ? null : r.GetString(4), r.IsDBNull(5) ? null : r.GetString(5),
            r.IsDBNull(6) ? null : r.GetString(6), r.IsDBNull(7) ? null : r.GetString(7)));
        return list;
    }

    private string GetLegacyParam(string secao, string chave, string fallback)
    {
        using var con = Open();
        using var cmd = con.CreateCommand();
        cmd.CommandText = "SELECT valor FROM legacy_params WHERE secao=$s AND chave=$k ORDER BY id DESC LIMIT 1";
        cmd.Parameters.AddWithValue("$s", secao);
        cmd.Parameters.AddWithValue("$k", chave);
        return cmd.ExecuteScalar() as string ?? fallback;
    }

    private int GetLegacyParamInt(string secao, string chave, int fallback)
    {
        var raw = GetLegacyParam(secao, chave, fallback.ToString(CultureInfo.InvariantCulture));
        var digits = new string(raw.TakeWhile(c => char.IsDigit(c) || c == '-').ToArray());
        return int.TryParse(digits, out var v) ? v : fallback;
    }

    private static string Center(string text, int width)
    {
        text = text.Trim();
        if (text.Length >= width) return text[..width];
        return new string(' ', (width - text.Length) / 2) + text;
    }

    private static string Money(double value) => value.ToString("C", PtBr);
    private static string SanitizePplb(string value, int max = 32) =>
        new string((value ?? "").Normalize(NormalizationForm.FormD).Where(c => c < 128 && c != '"').Take(max).ToArray());

    // ── Caixa ──────────────────────────────────────────────────────────────────
    public object? GetSessaoCaixaAtual()
    {
        using var con = Open();
        using var cmd = con.CreateCommand();
        cmd.CommandText = "SELECT * FROM caixa_sessoes WHERE status='aberta' ORDER BY id DESC LIMIT 1";
        using var r = cmd.ExecuteReader();
        if (!r.Read()) return null;
        var sessao = ReadSessaoCaixa(r);
        r.Close();
        return EnriquecerSessao(con, sessao);
    }

    public object? GetSessaoCaixa(int id)
    {
        using var con = Open();
        using var cmd = con.CreateCommand();
        cmd.CommandText = "SELECT * FROM caixa_sessoes WHERE id=$id";
        cmd.Parameters.AddWithValue("$id", id);
        using var r = cmd.ExecuteReader();
        if (!r.Read()) return null;
        var sessao = ReadSessaoCaixa(r);
        r.Close();
        return EnriquecerSessao(con, sessao);
    }

    public IEnumerable<object> HistoricoCaixa(int dias)
    {
        using var con = Open();
        using var cmd = con.CreateCommand();
        cmd.CommandText = "SELECT * FROM caixa_sessoes WHERE data>=date('now',-$d||' days') ORDER BY id DESC";
        cmd.Parameters.AddWithValue("$d", dias);
        using var r = cmd.ExecuteReader();
        return ReadSessoesCaixa(r).ToList();
    }

    public int AbrirCaixa(double valorAbertura, string usuario)
    {
        using var con = Open();
        // Verifica se já tem sessão aberta
        using var chk = con.CreateCommand();
        chk.CommandText = "SELECT COUNT(*) FROM caixa_sessoes WHERE status='aberta'";
        var aberta = (long)(chk.ExecuteScalar() ?? 0L);
        if (aberta > 0) throw new InvalidOperationException("Já existe um caixa aberto.");

        using var cmd = con.CreateCommand();
        cmd.CommandText = "INSERT INTO caixa_sessoes(data,usuario,valor_abertura) VALUES(date('now'),$usr,$val); SELECT last_insert_rowid();";
        cmd.Parameters.AddWithValue("$usr", usuario);
        cmd.Parameters.AddWithValue("$val", valorAbertura);
        var id = (int)(long)(cmd.ExecuteScalar() ?? 0L);

        using var mov = con.CreateCommand();
        mov.CommandText = "INSERT INTO caixa_movimentos(sessao_id,tipo,valor,descricao,usuario) VALUES($s,'abertura',$v,'Abertura de caixa',$u)";
        mov.Parameters.AddWithValue("$s", id);
        mov.Parameters.AddWithValue("$v", valorAbertura);
        mov.Parameters.AddWithValue("$u", usuario);
        mov.ExecuteNonQuery();
        return id;
    }

    public void FecharCaixa(double valorContado, string? observacao, string usuario)
    {
        using var con = Open();
        using var cmd = con.CreateCommand();
        cmd.CommandText = "SELECT id FROM caixa_sessoes WHERE status='aberta' ORDER BY id DESC LIMIT 1";
        var sessaoId = (int?)(long?)(cmd.ExecuteScalar());
        if (sessaoId is null) throw new InvalidOperationException("Nenhum caixa aberto.");

        con.Execute("UPDATE caixa_sessoes SET status='fechada', valor_contado=$vc, observacao_fechamento=$obs, fechado_em=datetime('now') WHERE id=$id",
            ("$vc", valorContado), ("$obs", observacao ?? (object)DBNull.Value), ("$id", sessaoId.Value));

        using var mov = con.CreateCommand();
        mov.CommandText = "INSERT INTO caixa_movimentos(sessao_id,tipo,valor,descricao,usuario) VALUES($s,'fechamento',$v,'Fechamento de caixa',$u)";
        mov.Parameters.AddWithValue("$s", sessaoId.Value);
        mov.Parameters.AddWithValue("$v", valorContado);
        mov.Parameters.AddWithValue("$u", usuario);
        mov.ExecuteNonQuery();
    }

    public void SuprimentoCaixa(double valor, string? descricao, string usuario)
    {
        var sessaoId = GetSessaoAbertaId();
        using var con = Open();
        using var cmd = con.CreateCommand();
        cmd.CommandText = "INSERT INTO caixa_movimentos(sessao_id,tipo,valor,descricao,usuario) VALUES($s,'suprimento',$v,$d,$u)";
        cmd.Parameters.AddWithValue("$s", sessaoId);
        cmd.Parameters.AddWithValue("$v", valor);
        cmd.Parameters.AddWithValue("$d", descricao ?? "Suprimento");
        cmd.Parameters.AddWithValue("$u", usuario);
        cmd.ExecuteNonQuery();
    }

    public void SangriaCaixa(double valor, string? descricao, string usuario)
    {
        var sessaoId = GetSessaoAbertaId();
        using var con = Open();
        using var cmd = con.CreateCommand();
        cmd.CommandText = "INSERT INTO caixa_movimentos(sessao_id,tipo,valor,descricao,usuario) VALUES($s,'sangria',$v,$d,$u)";
        cmd.Parameters.AddWithValue("$s", sessaoId);
        cmd.Parameters.AddWithValue("$v", -valor);
        cmd.Parameters.AddWithValue("$d", descricao ?? "Sangria");
        cmd.Parameters.AddWithValue("$u", usuario);
        cmd.ExecuteNonQuery();
    }

    public IEnumerable<object> MovimentosCaixa(int sessaoId)
    {
        using var con = Open();
        using var cmd = con.CreateCommand();
        cmd.CommandText = "SELECT * FROM caixa_movimentos WHERE sessao_id=$id ORDER BY id";
        cmd.Parameters.AddWithValue("$id", sessaoId);
        using var r = cmd.ExecuteReader();
        var list = new List<object>();
        while (r.Read()) list.Add(new
        {
            id = r.GetInt32(0), sessaoId = r.GetInt32(1), tipo = r.GetString(2),
            valor = r.GetDouble(3), descricao = r.IsDBNull(4) ? null : r.GetString(4),
            osId = r.IsDBNull(5) ? (int?)null : r.GetInt32(5),
            usuario = r.GetString(6), createdAt = r.GetString(7)
        });
        return list;
    }

    // ── Financeiro ─────────────────────────────────────────────────────────────
    public object ListarFinanceiro(string? status, int? clienteId, string? de, string? ate, int pg, int tam)
    {
        using var con = Open();
        var conds = new List<string>();
        if (status != null) conds.Add("f.status=$status");
        if (clienteId.HasValue) conds.Add("f.cliente_id=$cliId");
        if (de != null) conds.Add("f.vencimento>=$de");
        if (ate != null) conds.Add("f.vencimento<=$ate");
        var where = conds.Count > 0 ? "WHERE " + string.Join(" AND ", conds) : "";

        using var total = con.CreateCommand();
        total.CommandText = $"SELECT COUNT(*) FROM financeiro f {where}";
        SetFinFilterParams(total, status, clienteId, de, ate);
        var tot = (long)(total.ExecuteScalar() ?? 0L);

        using var cmd = con.CreateCommand();
        cmd.CommandText = $@"SELECT f.*, c.nome as cliente_nome, os.numero as os_numero
FROM financeiro f JOIN clientes c ON c.id=f.cliente_id
LEFT JOIN ordens_servico os ON os.id=f.os_id {where}
ORDER BY f.vencimento LIMIT $lim OFFSET $off";
        SetFinFilterParams(cmd, status, clienteId, de, ate);
        cmd.Parameters.AddWithValue("$lim", tam);
        cmd.Parameters.AddWithValue("$off", (pg - 1) * tam);
        using var r = cmd.ExecuteReader();
        var items = ReadFinanceiro(r).ToList();
        return new { total = tot, pagina = pg, tamanho = tam, items };
    }

    public object ResumoFinanceiro()
    {
        using var con = Open();
        using var cmd = con.CreateCommand();
        cmd.CommandText = @"
SELECT
  SUM(CASE WHEN status='aberto' THEN valor ELSE 0 END) as total_aberto,
  SUM(CASE WHEN status='recebido' THEN valor ELSE 0 END) as total_recebido,
  COUNT(CASE WHEN status='aberto' AND vencimento < date('now') THEN 1 END) as vencidos,
  COUNT(CASE WHEN status='aberto' THEN 1 END) as total_em_aberto
FROM financeiro";
        using var r = cmd.ExecuteReader();
        if (!r.Read()) return new { };
        return new
        {
            totalAberto   = r.IsDBNull(0) ? 0 : r.GetDouble(0),
            totalRecebido = r.IsDBNull(1) ? 0 : r.GetDouble(1),
            vencidos      = r.IsDBNull(2) ? 0 : r.GetInt32(2),
            totalEmAberto = r.IsDBNull(3) ? 0 : r.GetInt32(3)
        };
    }

    public void ReceberDuplicata(int id, ReceberRequest req, string usuario)
    {
        using var con = Open();
        con.Execute(@"UPDATE financeiro SET status='recebido', data_recebimento=date('now'),
  valor_recebido=$vr, metodo_recebimento=$met, observacao=$obs, updated_at=datetime('now')
  WHERE id=$id",
            ("$vr", req.ValorRecebido), ("$met", req.Metodo),
            ("$obs", req.Observacao ?? (object)DBNull.Value), ("$id", id));
        RegistrarMovimentoCaixaSimples(con, req.Metodo, req.ValorRecebido, $"Receb. duplicata #{id}", usuario);
    }

    // ── Relatórios ─────────────────────────────────────────────────────────────
    // Cada status usa a data de domínio que de fato representa "aconteceu hoje" —
    // updated_at NÃO serve pra isso (é tocado por reimportações/migrações antigas e
    // acabava contando o histórico inteiro como se fosse tudo de hoje).
    private const string MovimentoDoDiaWhere = @"
   (os.status IN ('aberta','pronta') AND date(os.data_entrada)=$dia)
OR (os.status='entregue' AND date(os.data_entrega)=$dia)
OR (os.status='paga' AND date(os.data_pagamento)=$dia)
OR (os.status IN ('cancelada','estornada') AND date(os.data_cancelamento)=$dia)";

    public object RelMovimentoDia(string dia)
    {
        using var con = Open();
        using var cmd = con.CreateCommand();
        cmd.CommandText = $@"
SELECT os.status,
  COUNT(*) as qtd,
  SUM(os.valor_final) as valor_total
FROM ordens_servico os
WHERE {MovimentoDoDiaWhere}
GROUP BY os.status";
        cmd.Parameters.AddWithValue("$dia", dia);
        using var r = cmd.ExecuteReader();
        var por_status = new List<object>();
        while (r.Read()) por_status.Add(new { status = r.GetString(0), qtd = r.GetInt32(1), valorTotal = r.GetDouble(2) });

        using var rols = con.CreateCommand();
        rols.CommandText = $@"
SELECT os.*, c.nome as cliente_nome FROM ordens_servico os
JOIN clientes c ON c.id=os.cliente_id
WHERE {MovimentoDoDiaWhere} ORDER BY os.id DESC";
        rols.Parameters.AddWithValue("$dia", dia);
        using var r2 = rols.ExecuteReader();
        var ordens = ReadRolsResumo(r2).ToList();
        return new { dia, porStatus = por_status, ordens };
    }

    public object RelMovimentoPeriodo(string de, string ate)
    {
        using var con = Open();
        using var cmd = con.CreateCommand();
        cmd.CommandText = @"
SELECT
  date(data_entrada) as dia,
  COUNT(*) as total_rols,
  SUM(CASE WHEN status='paga' THEN valor_final ELSE 0 END) as total_pago,
  SUM(CASE WHEN status='cancelada' THEN 1 ELSE 0 END) as canceladas,
  SUM(CASE WHEN status NOT IN ('paga','cancelada') THEN 1 ELSE 0 END) as em_aberto
FROM ordens_servico WHERE data_entrada BETWEEN $de AND $ate
GROUP BY date(data_entrada) ORDER BY dia";
        cmd.Parameters.AddWithValue("$de", de);
        cmd.Parameters.AddWithValue("$ate", ate);
        using var r = cmd.ExecuteReader();
        var dias = new List<object>();
        double totalGeral = 0;
        int totalRols = 0;
        while (r.Read())
        {
            var tp = r.GetDouble(2);
            totalGeral += tp;
            totalRols += r.GetInt32(1);
            dias.Add(new { dia = r.GetString(0), totalRols = r.GetInt32(1), totalPago = tp, canceladas = r.GetInt32(3), emAberto = r.GetInt32(4) });
        }
        return new { de, ate, totalGeral, totalRols, dias };
    }

    public object RelRolAbertos()
    {
        using var con = Open();
        using var cmd = con.CreateCommand();
        cmd.CommandText = @"
SELECT os.*, c.nome as cliente_nome, c.telefone as cliente_tel,
       (SELECT COUNT(*) FROM os_itens i WHERE i.os_id=os.id) as total_itens,
       julianday('now') - julianday(os.data_entrada) as dias_em_aberto
FROM ordens_servico os JOIN clientes c ON c.id=os.cliente_id
WHERE os.status IN ('aberta','pronta')
ORDER BY os.data_entrada";
        using var r = cmd.ExecuteReader();
        return ReadRolsResumo(r).ToList();
    }

    public object RelRolEntrega(string? data)
    {
        var dia = data ?? DateTime.Today.AddDays(1).ToString("yyyy-MM-dd");
        using var con = Open();
        using var cmd = con.CreateCommand();
        cmd.CommandText = @"
SELECT os.*, c.nome as cliente_nome, c.telefone as cliente_tel, c.celular as cliente_cel,
       (SELECT COUNT(*) FROM os_itens i WHERE i.os_id=os.id) as total_itens
FROM ordens_servico os JOIN clientes c ON c.id=os.cliente_id
WHERE os.data_promessa=$dia AND os.status IN ('aberta','pronta')
ORDER BY c.nome";
        cmd.Parameters.AddWithValue("$dia", dia);
        using var r = cmd.ExecuteReader();
        return new { data = dia, ordens = ReadRolsResumo(r).ToList() };
    }

    public object RelCaixaDia(string dia)
    {
        using var con = Open();
        using var cmd = con.CreateCommand();
        cmd.CommandText = @"
SELECT cm.tipo, SUM(cm.valor) as total, COUNT(*) as qtd
FROM caixa_movimentos cm
JOIN caixa_sessoes cs ON cs.id=cm.sessao_id
WHERE cs.data=$dia
GROUP BY cm.tipo ORDER BY cm.tipo";
        cmd.Parameters.AddWithValue("$dia", dia);
        using var r = cmd.ExecuteReader();
        var movimentos = new List<object>();
        double totalEntradas = 0, totalSaidas = 0;
        while (r.Read())
        {
            var tipo = r.GetString(0);
            var total = r.GetDouble(1);
            var qtd = r.GetInt32(2);
            movimentos.Add(new { tipo, total, qtd });
            if (tipo is "venda" or "suprimento" or "abertura") totalEntradas += total;
            else if (tipo is "sangria" or "fechamento") totalSaidas += Math.Abs(total);
        }
        return new { dia, movimentos, totalEntradas, totalSaidas, saldo = totalEntradas - totalSaidas };
    }

    public object RelClientesDebito()
    {
        using var con = Open();
        using var cmd = con.CreateCommand();
        cmd.CommandText = @"
SELECT c.id, c.nome, c.telefone, c.celular,
  SUM(f.valor) as total_devendo,
  MIN(f.vencimento) as mais_antigo,
  COUNT(*) as qtd_duplicatas
FROM financeiro f JOIN clientes c ON c.id=f.cliente_id
WHERE f.status='aberto'
GROUP BY c.id ORDER BY total_devendo DESC";
        using var r = cmd.ExecuteReader();
        var list = new List<object>();
        while (r.Read()) list.Add(new
        {
            id = r.GetInt32(0), nome = r.GetString(1),
            telefone = r.IsDBNull(2) ? null : r.GetString(2),
            celular = r.IsDBNull(3) ? null : r.GetString(3),
            totalDevendo = r.GetDouble(4),
            maisAntigo = r.IsDBNull(5) ? null : r.GetString(5),
            qtdDuplicatas = r.GetInt32(6)
        });
        return list;
    }

    public object RelServicosPeriodo(string de, string ate)
    {
        using var con = Open();
        using var cmd = con.CreateCommand();
        cmd.CommandText = @"
SELECT oi.descricao, COUNT(*) as qtd, SUM(oi.valor_total) as total
FROM os_itens oi JOIN ordens_servico os ON os.id=oi.os_id
WHERE os.status NOT IN ('cancelada') AND date(os.data_entrada) BETWEEN $de AND $ate
GROUP BY oi.descricao ORDER BY total DESC";
        cmd.Parameters.AddWithValue("$de", de);
        cmd.Parameters.AddWithValue("$ate", ate);
        using var r = cmd.ExecuteReader();
        var list = new List<object>();
        while (r.Read()) list.Add(new { servico = r.GetString(0), qtd = r.GetInt32(1), total = r.GetDouble(2) });
        return new { de, ate, servicos = list };
    }

    public object RelFrequenciaClientes(string de, string ate)
    {
        using var con = Open();
        using var cmd = con.CreateCommand();
        cmd.CommandText = @"
SELECT c.id, c.nome, COUNT(os.id) as total_rols,
  SUM(CASE WHEN os.status='paga' THEN os.valor_final ELSE 0 END) as total_pago,
  MAX(os.data_entrada) as ultima_visita
FROM clientes c JOIN ordens_servico os ON os.cliente_id=c.id
WHERE date(os.data_entrada) BETWEEN $de AND $ate
GROUP BY c.id ORDER BY total_rols DESC LIMIT 100";
        cmd.Parameters.AddWithValue("$de", de);
        cmd.Parameters.AddWithValue("$ate", ate);
        using var r = cmd.ExecuteReader();
        var list = new List<object>();
        while (r.Read()) list.Add(new
        {
            id = r.GetInt32(0), nome = r.GetString(1), totalRols = r.GetInt32(2),
            totalPago = r.GetDouble(3), ultimaVisita = r.IsDBNull(4) ? null : r.GetString(4)
        });
        return new { de, ate, clientes = list };
    }

    // ── Orçamentos ─────────────────────────────────────────────────────────────
    public object ListarOrcamentos(string? status, int? clienteId, string? q, int pg, int tam)
    {
        using var con = Open();
        var conds = new List<string>();
        if (status != null) conds.Add("o.status=$status");
        if (clienteId.HasValue) conds.Add("o.cliente_id=$cliId");
        if (q != null) conds.Add("(o.numero LIKE $q OR c.nome LIKE $q)");
        var where = conds.Count > 0 ? "WHERE " + string.Join(" AND ", conds) : "";

        using var cnt = con.CreateCommand();
        cnt.CommandText = $"SELECT COUNT(*) FROM orcamentos o JOIN clientes c ON c.id=o.cliente_id {where}";
        if (status != null) cnt.Parameters.AddWithValue("$status", status);
        if (clienteId.HasValue) cnt.Parameters.AddWithValue("$cliId", clienteId.Value);
        if (q != null) cnt.Parameters.AddWithValue("$q", $"%{q}%");
        var tot = (long)(cnt.ExecuteScalar() ?? 0L);

        using var cmd = con.CreateCommand();
        cmd.CommandText = $@"SELECT o.*, c.nome as cliente_nome, c.telefone as cliente_tel,
          (SELECT COUNT(*) FROM orc_itens i WHERE i.orc_id=o.id) as total_itens
        FROM orcamentos o JOIN clientes c ON c.id=o.cliente_id {where}
        ORDER BY o.id DESC LIMIT $lim OFFSET $off";
        if (status != null) cmd.Parameters.AddWithValue("$status", status);
        if (clienteId.HasValue) cmd.Parameters.AddWithValue("$cliId", clienteId.Value);
        if (q != null) cmd.Parameters.AddWithValue("$q", $"%{q}%");
        cmd.Parameters.AddWithValue("$lim", tam);
        cmd.Parameters.AddWithValue("$off", (pg - 1) * tam);
        using var r = cmd.ExecuteReader();
        return new { total = tot, pagina = pg, tamanho = tam, items = ReadOrcamentosResumo(r).ToList() };
    }

    public object? ObterOrcamento(int id)
    {
        using var con = Open();
        using var cmd = con.CreateCommand();
        cmd.CommandText = @"SELECT o.*, c.nome as cliente_nome, c.telefone as cliente_tel, c.celular as cliente_cel
        FROM orcamentos o JOIN clientes c ON c.id=o.cliente_id WHERE o.id=$id";
        cmd.Parameters.AddWithValue("$id", id);
        using var r = cmd.ExecuteReader();
        if (!r.Read()) return null;
        var orc = ReadOrcamentoCompleto(r);
        r.Close();
        using var i = con.CreateCommand();
        i.CommandText = "SELECT oi.*, s.codigo as srv_codigo, s.categoria as srv_cat FROM orc_itens oi LEFT JOIN servicos s ON s.id=oi.servico_id WHERE oi.orc_id=$id ORDER BY oi.id";
        i.Parameters.AddWithValue("$id", id);
        using var ri = i.ExecuteReader();
        var itens = ReadOrcItens(ri).ToList();
        return new { orcamento = orc, itens };
    }

    public int CriarOrcamento(OrcamentoRequest req, string usuario)
    {
        using var con = Open();
        using var tx = con.BeginTransaction();
        // Numero derivado do id real (não de COUNT(*)) — evita colisão em concorrência ou após exclusões.
        using var ins = con.CreateCommand();
        ins.Transaction = tx;
        ins.CommandText = @"INSERT INTO orcamentos(numero,cliente_id,data_entrada,data_promessa,data_validade,observacoes,usuario_entrada)
        VALUES('',$cli,$de,$pr,$val,$obs,$usr); SELECT last_insert_rowid();";
        ins.Parameters.AddWithValue("$cli", req.ClienteId);
        ins.Parameters.AddWithValue("$de", req.DataEntrada ?? DateTime.Today.ToString("yyyy-MM-dd"));
        ins.Parameters.AddWithValue("$pr", req.DataPromessa ?? (object)DBNull.Value);
        ins.Parameters.AddWithValue("$val", req.DataValidade ?? (object)DBNull.Value);
        ins.Parameters.AddWithValue("$obs", req.Observacoes ?? (object)DBNull.Value);
        ins.Parameters.AddWithValue("$usr", usuario);
        var id = (int)(long)(ins.ExecuteScalar() ?? 0L);

        using var upd = con.CreateCommand();
        upd.Transaction = tx;
        upd.CommandText = "UPDATE orcamentos SET numero=$num WHERE id=$id";
        upd.Parameters.AddWithValue("$num", $"ORC{id:D6}");
        upd.Parameters.AddWithValue("$id", id);
        upd.ExecuteNonQuery();

        tx.Commit();
        return id;
    }

    public void AtualizarOrcamento(int id, OrcamentoRequest req, string usuario)
    {
        using var con = Open();
        con.Execute(@"UPDATE orcamentos SET cliente_id=$cli,data_promessa=$pr,data_validade=$val,observacoes=$obs,updated_at=datetime('now') WHERE id=$id",
            ("$cli", req.ClienteId), ("$pr", req.DataPromessa ?? (object)DBNull.Value),
            ("$val", req.DataValidade ?? (object)DBNull.Value),
            ("$obs", req.Observacoes ?? (object)DBNull.Value), ("$id", id));
    }

    public void AdicionarItemOrcamento(int orcId, RolItemRequest req, string usuario)
    {
        using var con = Open();
        var total = req.Quantidade * req.ValorUnitario;
        using var cmd = con.CreateCommand();
        cmd.CommandText = @"INSERT INTO orc_itens(orc_id,servico_id,descricao,tipo_tecido,cor,marca,quantidade,valor_unitario,valor_total,observacao)
        VALUES($orc,$srv,$desc,$tec,$cor,$mar,$qty,$vu,$vt,$obs)";
        cmd.Parameters.AddWithValue("$orc", orcId);
        cmd.Parameters.AddWithValue("$srv", req.ServicoId ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("$desc", req.Descricao);
        cmd.Parameters.AddWithValue("$tec", req.TipoTecido ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("$cor", req.Cor ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("$mar", req.Marca ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("$qty", req.Quantidade);
        cmd.Parameters.AddWithValue("$vu", req.ValorUnitario);
        cmd.Parameters.AddWithValue("$vt", total);
        cmd.Parameters.AddWithValue("$obs", req.Observacao ?? (object)DBNull.Value);
        cmd.ExecuteNonQuery();
        RecalcularTotalOrcamento(con, orcId);
    }

    public void AtualizarItemOrcamento(int orcId, int itemId, RolItemRequest req)
    {
        var total = req.Quantidade * req.ValorUnitario;
        using var con = Open();
        con.Execute(@"UPDATE orc_itens SET servico_id=$srv,descricao=$desc,tipo_tecido=$tec,cor=$cor,marca=$mar,quantidade=$qty,valor_unitario=$vu,valor_total=$vt,observacao=$obs WHERE id=$id AND orc_id=$orc",
            ("$srv", req.ServicoId ?? (object)DBNull.Value), ("$desc", req.Descricao),
            ("$tec", req.TipoTecido ?? (object)DBNull.Value), ("$cor", req.Cor ?? (object)DBNull.Value),
            ("$mar", req.Marca ?? (object)DBNull.Value), ("$qty", req.Quantidade),
            ("$vu", req.ValorUnitario), ("$vt", total),
            ("$obs", req.Observacao ?? (object)DBNull.Value), ("$id", itemId), ("$orc", orcId));
        RecalcularTotalOrcamento(con, orcId);
    }

    public void RemoverItemOrcamento(int orcId, int itemId)
    {
        using var con = Open();
        con.Execute("DELETE FROM orc_itens WHERE id=$id AND orc_id=$orc", ("$id", itemId), ("$orc", orcId));
        RecalcularTotalOrcamento(con, orcId);
    }

    public int ConverterOrcamentoEmRol(int orcId, string usuario)
    {
        using var con = Open();
        using var chk = con.CreateCommand();
        chk.CommandText = "SELECT status, cliente_id, data_promessa, observacoes FROM orcamentos WHERE id=$id";
        chk.Parameters.AddWithValue("$id", orcId);
        using var r = chk.ExecuteReader();
        if (!r.Read()) throw new KeyNotFoundException("Orçamento não encontrado.");
        var status = r.GetString(0);
        if (status != "aberto") throw new InvalidOperationException($"Orçamento não pode ser convertido no status '{status}'.");
        var cliId = r.GetInt32(1);
        var dataProm = r.IsDBNull(2) ? (string?)null : r.GetString(2);
        var obs = r.IsDBNull(3) ? (string?)null : r.GetString(3);
        r.Close();

        var rolReq = new RolRequest(cliId, DateTime.Today.ToString("yyyy-MM-dd"), dataProm, obs);
        var rolId = CriarRol(rolReq, usuario);

        // Copiar itens
        using var itens = con.CreateCommand();
        itens.CommandText = "SELECT * FROM orc_itens WHERE orc_id=$id";
        itens.Parameters.AddWithValue("$id", orcId);
        using var ri = itens.ExecuteReader();
        while (ri.Read())
        {
            AdicionarItemRol(rolId, new RolItemRequest(
                ri.IsDBNull(2) ? null : (int?)ri.GetInt32(2),
                ri.GetString(3), ri.IsDBNull(4) ? null : ri.GetString(4),
                ri.IsDBNull(5) ? null : ri.GetString(5), ri.IsDBNull(6) ? null : ri.GetString(6),
                null, ri.GetDouble(7), ri.GetDouble(8), ri.IsDBNull(10) ? null : ri.GetString(10)), usuario);
        }
        ri.Close();

        // Marcar orçamento como convertido
        con.Execute("UPDATE orcamentos SET status='convertido', convertido_rol_id=$rol, updated_at=datetime('now') WHERE id=$id",
            ("$rol", rolId), ("$id", orcId));
        return rolId;
    }

    public void CancelarOrcamento(int id, string? motivo, string usuario)
    {
        using var con = Open();
        con.Execute("UPDATE orcamentos SET status='cancelado', updated_at=datetime('now') WHERE id=$id", ("$id", id));
    }

    private static void RecalcularTotalOrcamento(SqliteConnection con, int orcId)
    {
        using var cmd = con.CreateCommand();
        cmd.CommandText = @"UPDATE orcamentos SET
          valor_total=(SELECT COALESCE(SUM(valor_total),0) FROM orc_itens WHERE orc_id=$id),
          valor_final=(SELECT COALESCE(SUM(valor_total),0) FROM orc_itens WHERE orc_id=$id) - desconto,
          updated_at=datetime('now') WHERE id=$id";
        cmd.Parameters.AddWithValue("$id", orcId);
        cmd.ExecuteNonQuery();
    }

    private static IEnumerable<object> ReadOrcamentosResumo(SqliteDataReader r)
    {
        while (r.Read()) yield return new
        {
            id = r.GetInt32(0), numero = r.GetString(1), clienteId = r.GetInt32(2),
            status = r.GetString(3), dataEntrada = r.GetString(4),
            dataPromessa = r.IsDBNull(5) ? null : r.GetString(5),
            dataValidade = r.IsDBNull(6) ? null : r.GetString(6),
            valorTotal = r.GetDouble(7), desconto = r.GetDouble(8), valorFinal = r.GetDouble(9),
            observacoes = r.IsDBNull(10) ? null : r.GetString(10),
            convertidoRolId = r.IsDBNull(11) ? (int?)null : r.GetInt32(11),
            usuarioEntrada = r.GetString(12), createdAt = r.GetString(13), updatedAt = r.GetString(14),
            clienteNome = r.IsDBNull(15) ? null : r.GetString(15),
            clienteTel  = r.IsDBNull(16) ? null : r.GetString(16),
            totalItens = r.IsDBNull(17) ? 0 : r.GetInt32(17)
        };
    }

    private static object ReadOrcamentoCompleto(SqliteDataReader r) => new
    {
        id = r.GetInt32(0), numero = r.GetString(1), clienteId = r.GetInt32(2),
        status = r.GetString(3), dataEntrada = r.GetString(4),
        dataPromessa = r.IsDBNull(5) ? null : r.GetString(5),
        dataValidade = r.IsDBNull(6) ? null : r.GetString(6),
        valorTotal = r.GetDouble(7), desconto = r.GetDouble(8), valorFinal = r.GetDouble(9),
        observacoes = r.IsDBNull(10) ? null : r.GetString(10),
        convertidoRolId = r.IsDBNull(11) ? (int?)null : r.GetInt32(11),
        usuarioEntrada = r.GetString(12), createdAt = r.GetString(13), updatedAt = r.GetString(14),
        clienteNome = r.IsDBNull(15) ? null : r.GetString(15),
        clienteTel  = r.IsDBNull(16) ? null : r.GetString(16),
        clienteCel  = r.IsDBNull(17) ? null : r.GetString(17)
    };

    private static IEnumerable<object> ReadOrcItens(SqliteDataReader r)
    {
        while (r.Read()) yield return new
        {
            id = r.GetInt32(0), orcId = r.GetInt32(1),
            servicoId = r.IsDBNull(2) ? (int?)null : r.GetInt32(2),
            descricao = r.GetString(3),
            tipoTecido = r.IsDBNull(4) ? null : r.GetString(4),
            cor = r.IsDBNull(5) ? null : r.GetString(5),
            marca = r.IsDBNull(6) ? null : r.GetString(6),
            quantidade = r.GetDouble(7), valorUnitario = r.GetDouble(8), valorTotal = r.GetDouble(9),
            observacao = r.IsDBNull(10) ? null : r.GetString(10), createdAt = r.GetString(11),
            srvCodigo = r.IsDBNull(12) ? null : r.GetString(12),
            srvCat = r.IsDBNull(13) ? null : r.GetString(13)
        };
    }

    // ── Agenda ─────────────────────────────────────────────────────────────────
    public IEnumerable<object> ListarAgenda(string? data, string? de, string? ate, int? clienteId)
    {
        using var con = Open();
        var conds = new List<string> { "a.status='agendado'" };
        if (data != null) conds.Add("a.data_agendamento=$data");
        else { if (de != null) conds.Add("a.data_agendamento>=$de"); if (ate != null) conds.Add("a.data_agendamento<=$ate"); }
        if (clienteId.HasValue) conds.Add("a.cliente_id=$cliId");
        var where = "WHERE " + string.Join(" AND ", conds);
        using var cmd = con.CreateCommand();
        cmd.CommandText = $@"SELECT a.*, c.nome as cliente_nome, c.telefone as cliente_tel,
          os.numero as rol_numero, o.numero as orc_numero
        FROM agenda a JOIN clientes c ON c.id=a.cliente_id
        LEFT JOIN ordens_servico os ON os.id=a.rol_id
        LEFT JOIN orcamentos o ON o.id=a.orc_id
        {where} ORDER BY a.data_agendamento, a.hora_agendamento";
        if (data != null) cmd.Parameters.AddWithValue("$data", data);
        else { if (de != null) cmd.Parameters.AddWithValue("$de", de); if (ate != null) cmd.Parameters.AddWithValue("$ate", ate); }
        if (clienteId.HasValue) cmd.Parameters.AddWithValue("$cliId", clienteId.Value);
        using var r = cmd.ExecuteReader();
        return ReadAgenda(r).ToList();
    }

    public int CriarAgendamento(AgendaRequest req, string usuario)
    {
        using var con = Open();
        using var cmd = con.CreateCommand();
        cmd.CommandText = @"INSERT INTO agenda(rol_id,orc_id,cliente_id,data_agendamento,hora_agendamento,duracao_minutos,tipo,observacao,usuario)
        VALUES($rol,$orc,$cli,$data,$hora,$dur,$tipo,$obs,$usr); SELECT last_insert_rowid();";
        cmd.Parameters.AddWithValue("$rol", req.RolId ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("$orc", req.OrcId ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("$cli", req.ClienteId);
        cmd.Parameters.AddWithValue("$data", req.DataAgendamento);
        cmd.Parameters.AddWithValue("$hora", req.HoraAgendamento ?? "09:00");
        cmd.Parameters.AddWithValue("$dur", req.DuracaoMinutos ?? 30);
        cmd.Parameters.AddWithValue("$tipo", req.Tipo ?? "entrega");
        cmd.Parameters.AddWithValue("$obs", req.Observacao ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("$usr", usuario);
        return (int)(long)(cmd.ExecuteScalar() ?? 0L);
    }

    public object? ObterAgendamento(int id)
    {
        using var con = Open();
        using var cmd = con.CreateCommand();
        cmd.CommandText = @"SELECT a.*, c.nome as cliente_nome, c.telefone as cliente_tel,
          os.numero as rol_numero, o.numero as orc_numero
        FROM agenda a JOIN clientes c ON c.id=a.cliente_id
        LEFT JOIN ordens_servico os ON os.id=a.rol_id
        LEFT JOIN orcamentos o ON o.id=a.orc_id WHERE a.id=$id";
        cmd.Parameters.AddWithValue("$id", id);
        using var r = cmd.ExecuteReader();
        return ReadAgenda(r).FirstOrDefault();
    }

    public void AtualizarAgendamento(int id, AgendaRequest req, string usuario)
    {
        using var con = Open();
        con.Execute(@"UPDATE agenda SET rol_id=$rol,orc_id=$orc,cliente_id=$cli,data_agendamento=$data,hora_agendamento=$hora,
          duracao_minutos=$dur,tipo=$tipo,observacao=$obs WHERE id=$id",
            ("$rol", req.RolId ?? (object)DBNull.Value), ("$orc", req.OrcId ?? (object)DBNull.Value),
            ("$cli", req.ClienteId), ("$data", req.DataAgendamento),
            ("$hora", req.HoraAgendamento ?? "09:00"), ("$dur", req.DuracaoMinutos ?? 30),
            ("$tipo", req.Tipo ?? "entrega"), ("$obs", req.Observacao ?? (object)DBNull.Value), ("$id", id));
    }

    public void CancelarAgendamento(int id)
    {
        using var con = Open();
        con.Execute("UPDATE agenda SET status='cancelado' WHERE id=$id", ("$id", id));
    }

    // ── Catálogos ──────────────────────────────────────────────────────────────
    public IEnumerable<object> ListarCatalogos(string? tipo, string? q)
    {
        using var con = Open();
        var conds = new List<string> { "ativo=1" };
        if (!string.IsNullOrWhiteSpace(tipo)) conds.Add("tipo=$tipo");
        if (!string.IsNullOrWhiteSpace(q)) conds.Add("(descricao LIKE $q OR codigo LIKE $q)");
        var where = "WHERE " + string.Join(" AND ", conds);
        using var cmd = con.CreateCommand();
        cmd.CommandText = $"SELECT id,tipo,codigo,descricao,ativo,created_at FROM catalogos {where} ORDER BY tipo,descricao";
        if (!string.IsNullOrWhiteSpace(tipo)) cmd.Parameters.AddWithValue("$tipo", tipo);
        if (!string.IsNullOrWhiteSpace(q)) cmd.Parameters.AddWithValue("$q", $"%{q}%");
        using var r = cmd.ExecuteReader();
        var list = new List<object>();
        while (r.Read()) list.Add(new { id = r.GetInt32(0), tipo = r.GetString(1), codigo = r.GetString(2), descricao = r.GetString(3), ativo = r.GetInt32(4) == 1, createdAt = r.GetString(5) });
        return list;
    }

    public int CriarCatalogo(CatalogoRequest req)
    {
        using var con = Open();
        using var cmd = con.CreateCommand();
        cmd.CommandText = "INSERT INTO catalogos(tipo,codigo,descricao) VALUES($t,$c,$d); SELECT last_insert_rowid();";
        cmd.Parameters.AddWithValue("$t", req.Tipo.Trim().ToLower());
        cmd.Parameters.AddWithValue("$c", req.Codigo.Trim().ToUpper());
        cmd.Parameters.AddWithValue("$d", req.Descricao.Trim().ToUpper());
        return (int)(long)(cmd.ExecuteScalar() ?? 0L);
    }

    public void AtualizarCatalogo(int id, CatalogoRequest req)
    {
        using var con = Open();
        con.Execute("UPDATE catalogos SET tipo=$t,codigo=$c,descricao=$d WHERE id=$id",
            ("$t", req.Tipo.Trim().ToLower()), ("$c", req.Codigo.Trim().ToUpper()),
            ("$d", req.Descricao.Trim().ToUpper()), ("$id", id));
    }

    public void InativarCatalogo(int id)
    {
        using var con = Open();
        con.Execute("UPDATE catalogos SET ativo=0 WHERE id=$id", ("$id", id));
    }

    // ── Indenizações ───────────────────────────────────────────────────────────
    public IEnumerable<object> ListarIndenizacoes(int? clienteId, int? osId, string? status)
    {
        using var con = Open();
        var conds = new List<string>();
        if (clienteId.HasValue) conds.Add("i.cliente_id=$cliId");
        if (osId.HasValue) conds.Add("i.os_id=$osId");
        if (!string.IsNullOrWhiteSpace(status)) conds.Add("i.status=$status");
        var where = conds.Count > 0 ? "WHERE " + string.Join(" AND ", conds) : "";
        using var cmd = con.CreateCommand();
        cmd.CommandText = $@"SELECT i.id,i.os_id,i.cliente_id,i.descricao,i.valor,i.status,i.motivo,i.observacao,i.usuario,i.created_at,i.updated_at,c.nome as cliente_nome,os.numero as rol_numero
        FROM indenizacoes i
        JOIN clientes c ON c.id=i.cliente_id
        LEFT JOIN ordens_servico os ON os.id=i.os_id
        {where} ORDER BY i.id DESC LIMIT 200";
        if (clienteId.HasValue) cmd.Parameters.AddWithValue("$cliId", clienteId.Value);
        if (osId.HasValue) cmd.Parameters.AddWithValue("$osId", osId.Value);
        if (!string.IsNullOrWhiteSpace(status)) cmd.Parameters.AddWithValue("$status", status);
        using var r = cmd.ExecuteReader();
        var list = new List<object>();
        while (r.Read()) list.Add(new
        {
            id = r.GetInt32(0), osId = r.IsDBNull(1) ? (int?)null : r.GetInt32(1), clienteId = r.GetInt32(2),
            descricao = r.GetString(3), valor = r.GetDouble(4), status = r.GetString(5),
            motivo = r.IsDBNull(6) ? null : r.GetString(6), observacao = r.IsDBNull(7) ? null : r.GetString(7),
            usuario = r.GetString(8), createdAt = r.GetString(9), updatedAt = r.GetString(10),
            clienteNome = r.IsDBNull(11) ? null : r.GetString(11),
            rolNumero = r.IsDBNull(12) ? null : r.GetString(12)
        });
        return list;
    }

    public int CriarIndenizacao(IndenizacaoRequest req, string usuario)
    {
        using var con = Open();
        using var cmd = con.CreateCommand();
        cmd.CommandText = @"INSERT INTO indenizacoes(os_id,cliente_id,descricao,valor,motivo,observacao,usuario)
        VALUES($os,$cli,$desc,$val,$mot,$obs,$usr); SELECT last_insert_rowid();";
        cmd.Parameters.AddWithValue("$os", req.OsId ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("$cli", req.ClienteId);
        cmd.Parameters.AddWithValue("$desc", req.Descricao);
        cmd.Parameters.AddWithValue("$val", req.Valor);
        cmd.Parameters.AddWithValue("$mot", req.Motivo ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("$obs", req.Observacao ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("$usr", usuario);
        return (int)(long)(cmd.ExecuteScalar() ?? 0L);
    }

    public void AtualizarIndenizacao(int id, IndenizacaoUpdateRequest req, string usuario)
    {
        using var con = Open();
        con.Execute("UPDATE indenizacoes SET status=$s,motivo=$m,observacao=$o,updated_at=datetime('now') WHERE id=$id",
            ("$s", req.Status), ("$m", req.Motivo ?? (object)DBNull.Value),
            ("$o", req.Observacao ?? (object)DBNull.Value), ("$id", id));
    }

    // ── Guarda-roupa ───────────────────────────────────────────────────────────
    public IEnumerable<object> ListarGuardaroupa(int? clienteId, string? status)
    {
        using var con = Open();
        var conds = new List<string>();
        if (clienteId.HasValue) conds.Add("g.cliente_id=$cliId");
        conds.Add(!string.IsNullOrWhiteSpace(status) ? "g.status=$status" : "g.status='guardado'");
        var where = "WHERE " + string.Join(" AND ", conds);
        using var cmd = con.CreateCommand();
        cmd.CommandText = $@"SELECT g.id,g.cliente_id,g.descricao,g.categoria,g.cor,g.marca,g.quantidade,g.localizacao,g.data_entrada,g.data_saida,g.status,g.observacao,g.usuario,g.created_at,c.nome as cliente_nome,c.telefone as cliente_tel
        FROM guardaroupa g JOIN clientes c ON c.id=g.cliente_id
        {where} ORDER BY g.data_entrada DESC LIMIT 200";
        if (clienteId.HasValue) cmd.Parameters.AddWithValue("$cliId", clienteId.Value);
        if (!string.IsNullOrWhiteSpace(status)) cmd.Parameters.AddWithValue("$status", status);
        using var r = cmd.ExecuteReader();
        var list = new List<object>();
        while (r.Read()) list.Add(new
        {
            id = r.GetInt32(0), clienteId = r.GetInt32(1),
            descricao = r.GetString(2), categoria = r.IsDBNull(3) ? null : r.GetString(3),
            cor = r.IsDBNull(4) ? null : r.GetString(4), marca = r.IsDBNull(5) ? null : r.GetString(5),
            quantidade = r.GetInt32(6), localizacao = r.IsDBNull(7) ? null : r.GetString(7),
            dataEntrada = r.GetString(8), dataSaida = r.IsDBNull(9) ? null : r.GetString(9),
            status = r.GetString(10), observacao = r.IsDBNull(11) ? null : r.GetString(11),
            usuario = r.GetString(12), createdAt = r.GetString(13),
            clienteNome = r.IsDBNull(14) ? null : r.GetString(14),
            clienteTel = r.IsDBNull(15) ? null : r.GetString(15)
        });
        return list;
    }

    public int CriarGuardaroupa(GuardaroupaRequest req, string usuario)
    {
        using var con = Open();
        using var cmd = con.CreateCommand();
        cmd.CommandText = @"INSERT INTO guardaroupa(cliente_id,descricao,categoria,cor,marca,quantidade,localizacao,observacao,usuario)
        VALUES($cli,$desc,$cat,$cor,$mar,$qty,$loc,$obs,$usr); SELECT last_insert_rowid();";
        cmd.Parameters.AddWithValue("$cli", req.ClienteId);
        cmd.Parameters.AddWithValue("$desc", req.Descricao);
        cmd.Parameters.AddWithValue("$cat", req.Categoria ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("$cor", req.Cor ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("$mar", req.Marca ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("$qty", req.Quantidade > 0 ? req.Quantidade : 1);
        cmd.Parameters.AddWithValue("$loc", req.Localizacao ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("$obs", req.Observacao ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("$usr", usuario);
        return (int)(long)(cmd.ExecuteScalar() ?? 0L);
    }

    public void RetirarGuardaroupa(int id, string usuario)
    {
        using var con = Open();
        con.Execute("UPDATE guardaroupa SET status='retirado', data_saida=date('now') WHERE id=$id", ("$id", id));
    }

    // ── Terceirização ──────────────────────────────────────────────────────────
    public IEnumerable<object> ListarTerceirizacao(int? osId, string? status)
    {
        using var con = Open();
        var conds = new List<string>();
        if (osId.HasValue) conds.Add("t.os_id=$osId");
        if (!string.IsNullOrWhiteSpace(status)) conds.Add("t.status=$status");
        var where = conds.Count > 0 ? "WHERE " + string.Join(" AND ", conds) : "";
        using var cmd = con.CreateCommand();
        cmd.CommandText = $@"SELECT t.id,t.os_id,t.fornecedor,t.descricao,t.valor,t.data_envio,t.data_retorno_prevista,t.data_retorno,t.status,t.observacao,t.usuario,t.created_at,os.numero as rol_numero
        FROM terceirizacao t LEFT JOIN ordens_servico os ON os.id=t.os_id
        {where} ORDER BY t.id DESC LIMIT 200";
        if (osId.HasValue) cmd.Parameters.AddWithValue("$osId", osId.Value);
        if (!string.IsNullOrWhiteSpace(status)) cmd.Parameters.AddWithValue("$status", status);
        using var r = cmd.ExecuteReader();
        var list = new List<object>();
        while (r.Read()) list.Add(new
        {
            id = r.GetInt32(0), osId = r.IsDBNull(1) ? (int?)null : r.GetInt32(1),
            fornecedor = r.GetString(2), descricao = r.GetString(3), valor = r.GetDouble(4),
            dataEnvio = r.GetString(5), dataRetornoPrevista = r.IsDBNull(6) ? null : r.GetString(6),
            dataRetorno = r.IsDBNull(7) ? null : r.GetString(7), status = r.GetString(8),
            observacao = r.IsDBNull(9) ? null : r.GetString(9), usuario = r.GetString(10),
            createdAt = r.GetString(11), rolNumero = r.IsDBNull(12) ? null : r.GetString(12)
        });
        return list;
    }

    public int CriarTerceirizacao(TerceirizacaoRequest req, string usuario)
    {
        using var con = Open();
        using var cmd = con.CreateCommand();
        cmd.CommandText = @"INSERT INTO terceirizacao(os_id,fornecedor,descricao,valor,data_envio,data_retorno_prevista,observacao,usuario)
        VALUES($os,$forn,$desc,$val,$env,$ret,$obs,$usr); SELECT last_insert_rowid();";
        cmd.Parameters.AddWithValue("$os", req.OsId ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("$forn", req.Fornecedor);
        cmd.Parameters.AddWithValue("$desc", req.Descricao);
        cmd.Parameters.AddWithValue("$val", req.Valor);
        cmd.Parameters.AddWithValue("$env", DateTime.Today.ToString("yyyy-MM-dd"));
        cmd.Parameters.AddWithValue("$ret", req.DataRetornoPrevista ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("$obs", req.Observacao ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("$usr", usuario);
        return (int)(long)(cmd.ExecuteScalar() ?? 0L);
    }

    public void ReceberTerceirizacao(int id, string usuario)
    {
        using var con = Open();
        con.Execute("UPDATE terceirizacao SET status='recebido', data_retorno=date('now') WHERE id=$id", ("$id", id));
    }

    // ── Fidelidade / Pontos ────────────────────────────────────────────────────
    public object GetFidelidade(int clienteId)
    {
        using var con = Open();
        using var cmd = con.CreateCommand();
        cmd.CommandText = "SELECT pontos, updated_at FROM fidelidade WHERE cliente_id=$id";
        cmd.Parameters.AddWithValue("$id", clienteId);
        using var r = cmd.ExecuteReader();
        var pontos = r.Read() ? r.GetInt32(0) : 0;
        r.Close();
        using var m = con.CreateCommand();
        m.CommandText = "SELECT id,pontos,tipo,referencia,observacao,usuario,created_at FROM fidelidade_movimentos WHERE cliente_id=$id ORDER BY id DESC LIMIT 30";
        m.Parameters.AddWithValue("$id", clienteId);
        using var r2 = m.ExecuteReader();
        var movs = new List<object>();
        while (r2.Read()) movs.Add(new
        {
            id = r2.GetInt32(0), pontos = r2.GetInt32(1), tipo = r2.GetString(2),
            referencia = r2.IsDBNull(3) ? null : r2.GetString(3),
            observacao = r2.IsDBNull(4) ? null : r2.GetString(4),
            usuario = r2.GetString(5), createdAt = r2.GetString(6)
        });
        return new { clienteId, pontos, movimentos = movs };
    }

    public void LancarPontos(int clienteId, int pontos, string tipo, string? referencia, string? observacao, string usuario)
    {
        using var con = Open();
        con.Execute("INSERT OR IGNORE INTO fidelidade(cliente_id,pontos) VALUES($id,0)", ("$id", clienteId));
        var delta = tipo == "resgate" ? -pontos : pontos;
        con.Execute("UPDATE fidelidade SET pontos=MAX(0,pontos+$d),updated_at=datetime('now') WHERE cliente_id=$id",
            ("$d", delta), ("$id", clienteId));
        using var cmd = con.CreateCommand();
        cmd.CommandText = "INSERT INTO fidelidade_movimentos(cliente_id,pontos,tipo,referencia,observacao,usuario) VALUES($id,$p,$t,$r,$o,$u)";
        cmd.Parameters.AddWithValue("$id", clienteId);
        cmd.Parameters.AddWithValue("$p", pontos);
        cmd.Parameters.AddWithValue("$t", tipo);
        cmd.Parameters.AddWithValue("$r", referencia ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("$o", observacao ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("$u", usuario);
        cmd.ExecuteNonQuery();
    }

    public List<object> GetFidelidadeRegras()
    {
        using var con = Open();
        using var cmd = con.CreateCommand();
        cmd.CommandText = "SELECT categoria, pontos_por_venda FROM fidelidade_regras ORDER BY categoria";
        using var r = cmd.ExecuteReader();
        var list = new List<object>();
        while (r.Read()) list.Add(new { categoria = r.GetString(0), pontosPorVenda = r.GetInt32(1) });
        return list;
    }

    public void SetFidelidadeRegra(string categoria, int pontosPorVenda, string usuario)
    {
        using var con = Open();
        con.Execute(@"INSERT INTO fidelidade_regras(categoria,pontos_por_venda,updated_by,updated_at) VALUES($c,$p,$u,datetime('now'))
ON CONFLICT(categoria) DO UPDATE SET pontos_por_venda=$p, updated_by=$u, updated_at=datetime('now')",
            ("$c", categoria), ("$p", pontosPorVenda), ("$u", usuario));
    }

    /// Pontos de fidelidade ganhos automaticamente ao pagar uma venda: soma,
    /// por item, os pontos configurados para a categoria do serviço vendido
    /// (fidelidade_regras). Categorias sem regra configurada valem 0.
    private void CreditarPontosPorVenda(SqliteConnection con, int osId, string usuario)
    {
        using var cli = con.CreateCommand();
        cli.CommandText = "SELECT cliente_id FROM ordens_servico WHERE id=$id";
        cli.Parameters.AddWithValue("$id", osId);
        var clienteIdObj = cli.ExecuteScalar();
        if (clienteIdObj is null) return;
        var clienteId = (int)(long)clienteIdObj;

        using var cmd = con.CreateCommand();
        cmd.CommandText = @"
SELECT COALESCE(SUM(fr.pontos_por_venda), 0)
FROM os_itens oi
JOIN servicos s ON s.id = oi.servico_id
JOIN fidelidade_regras fr ON fr.categoria = s.categoria
WHERE oi.os_id = $id";
        cmd.Parameters.AddWithValue("$id", osId);
        var pontos = (int)(long)(cmd.ExecuteScalar() ?? 0L);
        if (pontos <= 0) return;

        con.Execute("INSERT OR IGNORE INTO fidelidade(cliente_id,pontos) VALUES($id,0)", ("$id", clienteId));
        con.Execute("UPDATE fidelidade SET pontos=pontos+$d,updated_at=datetime('now') WHERE cliente_id=$id",
            ("$d", pontos), ("$id", clienteId));
        con.Execute("INSERT INTO fidelidade_movimentos(cliente_id,pontos,tipo,referencia,observacao,usuario) VALUES($cid,$p,'ganho',$ref,'Pontos automáticos da venda',$u)",
            ("$cid", clienteId), ("$p", pontos), ("$ref", osId.ToString()), ("$u", usuario));
    }

    private static IEnumerable<object> ReadAgenda(SqliteDataReader r)
    {
        while (r.Read()) yield return new
        {
            id = r.GetInt32(0), rolId = r.IsDBNull(1) ? (int?)null : r.GetInt32(1),
            orcId = r.IsDBNull(2) ? (int?)null : r.GetInt32(2), clienteId = r.GetInt32(3),
            dataAgendamento = r.GetString(4), horaAgendamento = r.GetString(5),
            duracaoMinutos = r.GetInt32(6), tipo = r.GetString(7),
            observacao = r.IsDBNull(8) ? null : r.GetString(8),
            status = r.GetString(9), usuario = r.GetString(10), createdAt = r.GetString(11),
            clienteNome = r.IsDBNull(12) ? null : r.GetString(12),
            clienteTel  = r.IsDBNull(13) ? null : r.GetString(13),
            rolNumero  = r.IsDBNull(14) ? null : r.GetString(14),
            orcNumero  = r.IsDBNull(15) ? null : r.GetString(15)
        };
    }

    // ── Helpers privados ───────────────────────────────────────────────────────
    private SqliteConnection Open()
    {
        var con = new SqliteConnection(_connStr);
        con.Open();
        return con;
    }


    private static string? GetRolStatus(SqliteConnection con, int id)
    {
        using var cmd = con.CreateCommand();
        cmd.CommandText = "SELECT status FROM ordens_servico WHERE id=$id";
        cmd.Parameters.AddWithValue("$id", id);
        return cmd.ExecuteScalar() as string ?? throw new KeyNotFoundException($"ROL {id} não encontrado.");
    }

    private void EnsureRolEditavel(int id)
    {
        using var con = Open();
        var status = GetRolStatus(con, id);
        if (status is "paga" or "cancelada" or "estornada")
            throw new InvalidOperationException($"ROL não pode ser editada no status '{status}'.");
    }

    private static void RecalcularTotalRol(SqliteConnection con, int osId)
    {
        using var cmd = con.CreateCommand();
        cmd.CommandText = @"UPDATE ordens_servico SET
  valor_total=(SELECT COALESCE(SUM(valor_total),0) FROM os_itens WHERE os_id=$id),
  valor_final=(SELECT COALESCE(SUM(valor_total),0) FROM os_itens WHERE os_id=$id) - desconto,
  updated_at=datetime('now') WHERE id=$id";
        cmd.Parameters.AddWithValue("$id", osId);
        cmd.ExecuteNonQuery();
    }

    private static void RegistrarHistoricoOs(SqliteConnection con, int osId, string evento, string? anterior, string? novo, string? detalhe, string usuario)
    {
        using var cmd = con.CreateCommand();
        cmd.CommandText = "INSERT INTO os_historico(os_id,evento,status_anterior,status_novo,detalhe,usuario) VALUES($os,$ev,$sa,$sn,$d,$u)";
        cmd.Parameters.AddWithValue("$os", osId);
        cmd.Parameters.AddWithValue("$ev", evento);
        cmd.Parameters.AddWithValue("$sa", anterior ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("$sn", novo ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("$d", detalhe ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("$u", usuario);
        cmd.ExecuteNonQuery();
    }

    private static void RegistrarHistoricoCliente(SqliteConnection con, int id, string evento, string? detalhe, string usuario)
    {
        using var cmd = con.CreateCommand();
        cmd.CommandText = "INSERT INTO clientes_historico(cliente_id,evento,detalhe,usuario) VALUES($id,$ev,$d,$u)";
        cmd.Parameters.AddWithValue("$id", id);
        cmd.Parameters.AddWithValue("$ev", evento);
        cmd.Parameters.AddWithValue("$d", detalhe ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("$u", usuario);
        cmd.ExecuteNonQuery();
    }

    private int GetSessaoAbertaId()
    {
        using var con = Open();
        using var cmd = con.CreateCommand();
        cmd.CommandText = "SELECT id FROM caixa_sessoes WHERE status='aberta' ORDER BY id DESC LIMIT 1";
        var r = cmd.ExecuteScalar();
        if (r is null) throw new InvalidOperationException("Caixa não está aberto.");
        return (int)(long)r;
    }

    private static void RegistrarMovimentoCaixa(SqliteConnection con, int osId, string metodo, double valor, string usuario)
    {
        using var chk = con.CreateCommand();
        chk.CommandText = "SELECT id FROM caixa_sessoes WHERE status='aberta' ORDER BY id DESC LIMIT 1";
        var sessaoId = chk.ExecuteScalar();
        if (sessaoId is null) return;

        using var cmd = con.CreateCommand();
        cmd.CommandText = "INSERT INTO caixa_movimentos(sessao_id,tipo,valor,descricao,os_id,usuario) VALUES($s,'venda',$v,$d,$os,$u)";
        cmd.Parameters.AddWithValue("$s", (long)sessaoId);
        cmd.Parameters.AddWithValue("$v", valor);
        cmd.Parameters.AddWithValue("$d", $"ROL - {metodo}");
        cmd.Parameters.AddWithValue("$os", osId);
        cmd.Parameters.AddWithValue("$u", usuario);
        cmd.ExecuteNonQuery();
    }

    private void RegistrarMovimentoCaixaSimples(SqliteConnection con, string metodo, double valor, string descricao, string usuario)
    {
        using var chk = con.CreateCommand();
        chk.CommandText = "SELECT id FROM caixa_sessoes WHERE status='aberta' ORDER BY id DESC LIMIT 1";
        var sessaoId = chk.ExecuteScalar();
        if (sessaoId is null) return;

        using var cmd = con.CreateCommand();
        cmd.CommandText = "INSERT INTO caixa_movimentos(sessao_id,tipo,valor,descricao,usuario) VALUES($s,'recebimento',$v,$d,$u)";
        cmd.Parameters.AddWithValue("$s", (long)sessaoId);
        cmd.Parameters.AddWithValue("$v", valor);
        cmd.Parameters.AddWithValue("$d", descricao);
        cmd.Parameters.AddWithValue("$u", usuario);
        cmd.ExecuteNonQuery();
    }

    private static object EnriquecerSessao(SqliteConnection con, dynamic sessao)
    {
        using var cmd = con.CreateCommand();
        cmd.CommandText = @"SELECT
  SUM(CASE WHEN tipo IN ('venda','suprimento','abertura','recebimento') THEN valor ELSE 0 END) as entradas,
  SUM(CASE WHEN tipo='sangria' THEN ABS(valor) ELSE 0 END) as saidas,
  SUM(CASE WHEN tipo='venda' THEN valor ELSE 0 END) as total_vendas,
  COUNT(CASE WHEN tipo='venda' THEN 1 END) as qtd_vendas
FROM caixa_movimentos WHERE sessao_id=$id";
        cmd.Parameters.AddWithValue("$id", sessao.id);
        using var r = cmd.ExecuteReader();
        if (!r.Read()) return sessao;
        return new
        {
            sessao.id, sessao.data, sessao.usuario, sessao.valorAbertura,
            sessao.valorContado, sessao.status,
            entradas = r.IsDBNull(0) ? 0 : r.GetDouble(0),
            saidas = r.IsDBNull(1) ? 0 : r.GetDouble(1),
            totalVendas = r.IsDBNull(2) ? 0 : r.GetDouble(2),
            qtdVendas = r.IsDBNull(3) ? 0 : r.GetInt32(3),
            // "entradas" já inclui o movimento 'abertura' (ver AbrirCaixa) — somar valorAbertura de novo duplicava o valor no saldo.
            saldoAtual = (r.IsDBNull(0) ? 0 : r.GetDouble(0)) - (r.IsDBNull(1) ? 0 : r.GetDouble(1))
        };
    }

    private static void SetClienteParams(SqliteCommand cmd, ClienteRequest req, string usuario)
    {
        cmd.Parameters.AddWithValue("$nm", req.Nome.Trim());
        cmd.Parameters.AddWithValue("$doc", req.Documento?.Trim() ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("$tel", req.Telefone?.Trim() ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("$cel", req.Celular?.Trim() ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("$eml", req.Email?.Trim() ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("$log", req.Logradouro?.Trim() ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("$num", req.Numero?.Trim() ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("$bai", req.Bairro?.Trim() ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("$cid", req.Cidade?.Trim() ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("$est", req.Estado?.Trim() ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("$cep", req.Cep?.Trim() ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("$obs", req.Observacoes?.Trim() ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("$lim", req.LimiteCredito);
        cmd.Parameters.AddWithValue("$desc", req.DescontoPercent);
        cmd.Parameters.AddWithValue("$usr", usuario);
        cmd.Parameters.AddWithValue("$datnas", string.IsNullOrWhiteSpace(req.DataNascimento) ? DBNull.Value : (object)req.DataNascimento.Trim());
        cmd.Parameters.AddWithValue("$cartfid", string.IsNullOrWhiteSpace(req.CartaoFidelidade) ? DBNull.Value : (object)req.CartaoFidelidade.Trim());
        cmd.Parameters.AddWithValue("$contato", string.IsNullOrWhiteSpace(req.Contato) ? DBNull.Value : (object)req.Contato.Trim());
    }

    private static void SetRolFilterParams(SqliteCommand cmd, string? status, int? clienteId, string? de, string? ate, string? q)
    {
        if (status != null) cmd.Parameters.AddWithValue("$status", status);
        if (clienteId.HasValue) cmd.Parameters.AddWithValue("$cliId", clienteId.Value);
        if (de != null) cmd.Parameters.AddWithValue("$de", de);
        if (ate != null) cmd.Parameters.AddWithValue("$ate", ate);
        if (q != null) cmd.Parameters.AddWithValue("$q", $"%{q}%");
    }

    private static void SetFinFilterParams(SqliteCommand cmd, string? status, int? clienteId, string? de, string? ate)
    {
        if (status != null) cmd.Parameters.AddWithValue("$status", status);
        if (clienteId.HasValue) cmd.Parameters.AddWithValue("$cliId", clienteId.Value);
        if (de != null) cmd.Parameters.AddWithValue("$de", de);
        if (ate != null) cmd.Parameters.AddWithValue("$ate", ate);
    }

    private static IEnumerable<object> ReadClientes(SqliteDataReader r)
    {
        while (r.Read()) yield return new
        {
            id = r.GetInt32(0), nome = r.GetString(1),
            documento = r.IsDBNull(2) ? null : r.GetString(2),
            telefone = r.IsDBNull(3) ? null : r.GetString(3),
            celular = r.IsDBNull(4) ? null : r.GetString(4),
            email = r.IsDBNull(5) ? null : r.GetString(5),
            logradouro = r.IsDBNull(6) ? null : r.GetString(6),
            numero = r.IsDBNull(7) ? null : r.GetString(7),
            bairro = r.IsDBNull(8) ? null : r.GetString(8),
            cidade = r.IsDBNull(9) ? null : r.GetString(9),
            estado = r.IsDBNull(10) ? null : r.GetString(10),
            cep = r.IsDBNull(11) ? null : r.GetString(11),
            observacoes = r.IsDBNull(12) ? null : r.GetString(12),
            limiteCredito = r.GetDouble(13),
            descontoPercent = r.GetDouble(14),
            ativo = r.GetInt32(15) == 1,
            createdAt = r.GetString(16), updatedAt = r.GetString(17),
            dataNascimento = GetStringByNameSafe(r, "data_nascimento"),
            cartaoFidelidade = GetStringByNameSafe(r, "cartao_fidelidade"),
            contato = GetStringByNameSafe(r, "contato")
        };
    }

    private static IEnumerable<object> ReadServicos(SqliteDataReader r)
    {
        while (r.Read()) yield return new
        {
            id = r.GetInt32(0), codigo = r.GetString(1), descricao = r.GetString(2),
            categoria = r.GetString(3), preco = r.GetDouble(4),
            ativo = r.GetInt32(5) == 1, createdAt = r.GetString(6)
        };
    }

    private static IEnumerable<object> ReadRolsResumo(SqliteDataReader r)
    {
        while (r.Read())
        {
            var cols = Enumerable.Range(0, r.FieldCount).Select(i => r.GetName(i)).ToArray();
            yield return new
            {
                id = r.GetInt32(0), numero = r.GetString(1), clienteId = r.GetInt32(2),
                status = r.GetString(3), dataEntrada = r.GetString(4),
                dataPromessa = r.IsDBNull(5) ? null : r.GetString(5),
                dataEntrega = r.IsDBNull(6) ? null : r.GetString(6),
                dataPagamento = r.IsDBNull(7) ? null : r.GetString(7),
                valorTotal = r.GetDouble(8), desconto = r.GetDouble(9), valorFinal = r.GetDouble(10),
                valorPago = r.GetDouble(11),
                metodoPagamento = r.IsDBNull(12) ? null : r.GetString(12),
                troco = r.GetDouble(13),
                observacoes = r.IsDBNull(14) ? null : r.GetString(14),
                motivoCancelamento = r.IsDBNull(15) ? null : r.GetString(15),
                usuarioEntrada = r.GetString(16),
                usuarioEntrega = r.IsDBNull(17) ? null : r.GetString(17),
                usuarioPagamento = r.IsDBNull(18) ? null : r.GetString(18),
                createdAt = r.GetString(19), updatedAt = r.GetString(20),
                clienteNome = cols.Contains("cliente_nome") ? (r.IsDBNull(r.GetOrdinal("cliente_nome")) ? null : r.GetString(r.GetOrdinal("cliente_nome"))) : null,
                clienteTel = cols.Contains("cliente_tel") ? (r.IsDBNull(r.GetOrdinal("cliente_tel")) ? null : r.GetString(r.GetOrdinal("cliente_tel"))) : null,
                totalItens = cols.Contains("total_itens") ? r.GetInt32(r.GetOrdinal("total_itens")) : 0,
                totalPecas = GetIntByNameSafe(r, "total_pecas", cols),
                localizacaoRol = GetStringByNameSafe2(r, "localizacao_rol", cols),
                dataCancelamento = GetStringByNameSafe2(r, "data_cancelamento", cols)
            };
        }
    }

    private static object ReadRolCompleto(SqliteDataReader r) => new
    {
        id = r.GetInt32(0), numero = r.GetString(1), clienteId = r.GetInt32(2),
        status = r.GetString(3), dataEntrada = r.GetString(4),
        dataPromessa = r.IsDBNull(5) ? null : r.GetString(5),
        dataEntrega = r.IsDBNull(6) ? null : r.GetString(6),
        dataPagamento = r.IsDBNull(7) ? null : r.GetString(7),
        valorTotal = r.GetDouble(8), desconto = r.GetDouble(9), valorFinal = r.GetDouble(10),
        valorPago = r.GetDouble(11),
        metodoPagamento = r.IsDBNull(12) ? null : r.GetString(12),
        troco = r.GetDouble(13),
        observacoes = r.IsDBNull(14) ? null : r.GetString(14),
        motivoCancelamento = r.IsDBNull(15) ? null : r.GetString(15),
        usuarioEntrada = r.GetString(16),
        usuarioEntrega = r.IsDBNull(17) ? null : r.GetString(17),
        usuarioPagamento = r.IsDBNull(18) ? null : r.GetString(18),
        createdAt = r.GetString(19), updatedAt = r.GetString(20),
        clienteNome = GetStringByName(r, "cliente_nome"),
        clienteTel = GetStringByName(r, "cliente_tel"),
        clienteCel = GetStringByName(r, "cliente_cel"),
        limiteCredito = GetDoubleByName(r, "limite_credito"),
        descontoPercent = GetDoubleByName(r, "desconto_percent"),
        numeroNotaFiscal = GetStringByNameSafe(r, "numero_nota_fiscal"),
        horaPagamento = GetStringByNameSafe(r, "hora_pagamento"),
        notaNumero = GetStringByNameSafe(r, "nota_numero"),
        totalPecas = GetStringByNameSafe(r, "total_pecas"),
        localizacaoRol = GetStringByNameSafe(r, "localizacao_rol"),
        dataCancelamento = GetStringByNameSafe(r, "data_cancelamento"),
        motivoDesconto = GetStringByNameSafe(r, "motivo_desconto")
    };

    private static IEnumerable<object> ReadItens(SqliteDataReader r)
    {
        while (r.Read()) yield return new
        {
            id = r.GetInt32(0), osId = r.GetInt32(1),
            servicoId = r.IsDBNull(2) ? (int?)null : r.GetInt32(2),
            descricao = r.GetString(3),
            tipoTecido = r.IsDBNull(4) ? null : r.GetString(4),
            cor = r.IsDBNull(5) ? null : r.GetString(5),
            marca = r.IsDBNull(6) ? null : r.GetString(6),
            defeito = r.IsDBNull(7) ? null : r.GetString(7),
            quantidade = r.GetDouble(8), valorUnitario = r.GetDouble(9), valorTotal = r.GetDouble(10),
            status = r.GetString(11),
            observacao = r.IsDBNull(12) ? null : r.GetString(12),
            createdAt = r.GetString(13),
            srvCodigo = GetStringByNameSafe(r, "srv_codigo"),
            srvCat = GetStringByNameSafe(r, "srv_cat"),
            peso = GetDoubleNullByName(r, "peso"),
            identificacao = GetStringByNameSafe(r, "identificacao"),
            localizacao = GetStringByNameSafe(r, "localizacao"),
            valorTerceiro = GetDoubleNullByName(r, "valor_terceiro"),
            obs2 = GetStringByNameSafe(r, "obs2")
        };
    }

    private static object ReadSessaoCaixa(SqliteDataReader r) => new
    {
        id = r.GetInt32(0), data = r.GetString(1), usuario = r.GetString(2),
        valorAbertura = r.GetDouble(3),
        valorContado = r.IsDBNull(4) ? (double?)null : r.GetDouble(4),
        status = r.GetString(5),
        observacaoFechamento = r.IsDBNull(6) ? null : r.GetString(6),
        createdAt = r.GetString(7),
        fechadoEm = r.IsDBNull(8) ? null : r.GetString(8)
    };

    private static IEnumerable<object> ReadSessoesCaixa(SqliteDataReader r)
    {
        while (r.Read()) yield return ReadSessaoCaixa(r);
    }

    private static IEnumerable<object> ReadFinanceiro(SqliteDataReader r)
    {
        while (r.Read()) yield return new
        {
            id = r.GetInt32(r.GetOrdinal("id")),
            clienteId = r.GetInt32(r.GetOrdinal("cliente_id")),
            osId = r.IsDBNull(r.GetOrdinal("os_id")) ? (int?)null : r.GetInt32(r.GetOrdinal("os_id")),
            tipo = r.GetString(r.GetOrdinal("tipo")),
            status = r.GetString(r.GetOrdinal("status")),
            valor = r.GetDouble(r.GetOrdinal("valor")),
            vencimento = GetStringByName(r, "vencimento"),
            dataRecebimento = GetStringByName(r, "data_recebimento"),
            valorRecebido = r.IsDBNull(r.GetOrdinal("valor_recebido")) ? (double?)null : r.GetDouble(r.GetOrdinal("valor_recebido")),
            metodoRecebimento = GetStringByName(r, "metodo_recebimento"),
            observacao = GetStringByName(r, "observacao"),
            usuario = r.GetString(r.GetOrdinal("usuario")),
            createdAt = r.GetString(r.GetOrdinal("created_at")),
            updatedAt = r.GetString(r.GetOrdinal("updated_at")),
            clienteNome = GetStringByName(r, "cliente_nome"),
            osNumero = GetStringByName(r, "os_numero")
        };
    }

    private static string? GetStringByName(SqliteDataReader r, string name)
    {
        var ix = r.GetOrdinal(name);
        return r.IsDBNull(ix) ? null : r.GetString(ix);
    }

    private static string? GetStringByNameSafe(SqliteDataReader r, string name)
    {
        try { var ix = r.GetOrdinal(name); return r.IsDBNull(ix) ? null : r.GetString(ix); }
        catch { return null; }
    }

    private static string? GetStringByNameSafe2(SqliteDataReader r, string name, string[] cols)
    {
        if (!cols.Contains(name)) return null;
        var ix = r.GetOrdinal(name);
        return r.IsDBNull(ix) ? null : r.GetString(ix);
    }

    private static int? GetIntByNameSafe(SqliteDataReader r, string name, string[] cols)
    {
        if (!cols.Contains(name)) return null;
        var ix = r.GetOrdinal(name);
        return r.IsDBNull(ix) ? null : r.GetInt32(ix);
    }

    private static double? GetDoubleNullByName(SqliteDataReader r, string name)
    {
        try { var ix = r.GetOrdinal(name); return r.IsDBNull(ix) ? (double?)null : r.GetDouble(ix); }
        catch { return null; }
    }

    private static double GetDoubleByName(SqliteDataReader r, string name)
    {
        var ix = r.GetOrdinal(name);
        return r.IsDBNull(ix) ? 0 : r.GetDouble(ix);
    }
}

// Extensão helper para SQLite
static class SqliteExt
{
    public static void Execute(this SqliteConnection con, string sql, params (string, object)[] parms)
    {
        using var cmd = con.CreateCommand();
        cmd.CommandText = sql;
        foreach (var (k, v) in parms) cmd.Parameters.AddWithValue(k, v ?? DBNull.Value);
        cmd.ExecuteNonQuery();
    }
}

// ═════════════════════════════════════════════════════════════════════════════
// Permissões
// ═════════════════════════════════════════════════════════════════════════════
static class Perm
{
    public const string AdminRead       = "admin.read";
    public const string UsuariosRead    = "usuarios.read";
    public const string UsuariosWrite   = "usuarios.write";
    public const string CadastroWrite   = "cadastro.write";
    public const string RelatoriosRead  = "relatorios.read";
    public const string ConfigWrite     = "config.write";
    public const string CaixaAccess     = "caixa.access";
    public const string FinanceiroRead  = "financeiro.read";
    public const string LegadoRead      = "legado.read";
    public const string LicencasManage  = "licencas.manage";
    public const string PrecosWrite     = "precos.write";
    public const string CatalogosWrite  = "catalogos.write";
    public const string FidelidadeManage = "fidelidade.manage";
    public const string DashboardRead    = "dashboard.read";
    public const string SenhaResetOutros = "senha.reset-outros";
}

// ═════════════════════════════════════════════════════════════════════════════
// SystemCommands — rede (Wi-Fi/cabo), impressoras (A4/térmica) e navegador
// Executa utilitários de sistema já presentes no appliance (nmcli, lpstat, lp).
// Ausência dos binários (ex.: ambiente de desenvolvimento) é tratada como erro
// recuperável — nunca derruba a API.
// ═════════════════════════════════════════════════════════════════════════════
static class SystemCommands
{
    static async Task<(bool ok, string output)> RunAsync(string file, string[] args, int timeoutMs = 15000)
    {
        try
        {
            var psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = file,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };
            foreach (var a in args) psi.ArgumentList.Add(a);

            using var p = System.Diagnostics.Process.Start(psi);
            if (p is null) return (false, $"Não foi possível iniciar '{file}'.");

            var outTask = p.StandardOutput.ReadToEndAsync();
            var errTask = p.StandardError.ReadToEndAsync();
            var exited  = await Task.WhenAny(p.WaitForExitAsync(), Task.Delay(timeoutMs)) != Task.Delay(timeoutMs);
            if (!exited)
            {
                try { p.Kill(true); } catch { /* processo já pode ter terminado */ }
                return (false, $"Tempo esgotado executando '{file}'.");
            }
            var stdout = (await outTask).Trim();
            var stderr = (await errTask).Trim();
            return p.ExitCode == 0 ? (true, stdout) : (false, string.IsNullOrEmpty(stderr) ? stdout : stderr);
        }
        catch (System.ComponentModel.Win32Exception)
        {
            return (false, $"Comando '{file}' não encontrado neste sistema.");
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    // ── Rede ────────────────────────────────────────────────────────────────
    public static async Task<object> RedeStatus()
    {
        var (ok, saida) = await RunAsync("nmcli", new[] { "-t", "-f", "DEVICE,TYPE,STATE,CONNECTION", "device", "status" });
        if (!ok) return new { disponivel = false, erro = saida, dispositivos = Array.Empty<object>() };

        var linhas = saida.Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(l => l.Split(':'))
            .Where(p => p.Length >= 3 && (p[1] == "ethernet" || p[1] == "wifi"))
            .ToList();

        var dispositivos = new List<object>();
        foreach (var p in linhas)
        {
            var ip = "";
            var (ipOk, ipSaida) = await RunAsync("nmcli", new[] { "-g", "IP4.ADDRESS", "device", "show", p[0] });
            if (ipOk && !string.IsNullOrWhiteSpace(ipSaida))
                ip = ipSaida.Split('\n', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault()?.Split('/')[0] ?? "";

            dispositivos.Add(new
            {
                dispositivo = p[0],
                tipo        = p[1],
                estado      = p[2],
                conexao     = p.Length > 3 ? p[3] : "",
                ip
            });
        }
        return new { disponivel = true, dispositivos };
    }

    public static async Task<object> WifiListar()
    {
        var (ok, saida) = await RunAsync("nmcli", new[] { "-t", "-f", "SSID,SIGNAL,SECURITY,IN-USE", "device", "wifi", "list", "--rescan", "yes" }, timeoutMs: 20000);
        if (!ok) return new { disponivel = false, erro = saida, redes = Array.Empty<object>() };

        var redes = saida.Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(l => l.Split(':'))
            .Where(p => p.Length >= 1 && !string.IsNullOrWhiteSpace(p[0]))
            .Select(p => new
            {
                ssid       = p[0],
                sinal      = p.Length > 1 ? p[1] : "",
                seguranca  = p.Length > 2 ? p[2] : "",
                conectada  = p.Length > 3 && p[3] == "*"
            })
            .GroupBy(r => r.ssid).Select(g => g.OrderByDescending(x => x.conectada).First())
            .OrderByDescending(r => int.TryParse(r.sinal, out var s) ? s : 0)
            .ToList();
        return new { disponivel = true, redes };
    }

    public static async Task<(bool ok, string output)> WifiConectar(string ssid, string senha)
    {
        // Requer privilégio (cria/ativa conexão de sistema) — mesmo modelo de sudo já usado em /admin/reboot.
        var args = string.IsNullOrEmpty(senha)
            ? new[] { "nmcli", "device", "wifi", "connect", ssid }
            : new[] { "nmcli", "device", "wifi", "connect", ssid, "password", senha };
        var (ok, saida) = await RunAsync("sudo", args, timeoutMs: 30000);
        return (ok, ok ? $"Conectado à rede \"{ssid}\"." : saida);
    }

    public static async Task<(bool ok, string output)> CaboRenovar(string conexao)
    {
        await RunAsync("sudo", new[] { "nmcli", "connection", "down", conexao });
        var (ok, saida) = await RunAsync("sudo", new[] { "nmcli", "connection", "up", conexao }, timeoutMs: 20000);
        return (ok, ok ? "Conexão via cabo de rede reestabelecida." : saida);
    }

    // ── Impressoras ─────────────────────────────────────────────────────────
    public static async Task<(bool disponivel, string? erro, List<object> lista)> ImpressorasListar()
    {
        var (ok, saida) = await RunAsync("lpstat", new[] { "-p" });
        if (!ok) return (false, saida, new List<object>());

        var lista = saida.Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Where(l => l.StartsWith("printer "))
            .Select(l =>
            {
                var partes = l.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var nome = partes.Length > 1 ? partes[1] : "";
                var ativa = l.Contains("idle") || l.Contains("printing");
                return (object)new { nome, ativa };
            }).ToList();
        return (true, null, lista);
    }

    public static async Task<(bool ok, string output)> ImpressoraTestar(string nome, string tipo)
    {
        var tmp = Path.GetTempFileName();
        try
        {
            if (string.Equals(tipo, "termica", StringComparison.OrdinalIgnoreCase))
            {
                var esc = "\x1b@Ateliê da Luci\nTeste de impressao termica\n" + DateTime.Now.ToString("g") + "\n\n\n\x1dV\x01";
                await File.WriteAllTextAsync(tmp, esc);
                return await RunAsync("lp", new[] { "-d", nome, "-o", "raw", tmp });
            }
            var texto = $"Ateliê da Luci\nTeste de impressão A4\n{DateTime.Now:g}\n";
            await File.WriteAllTextAsync(tmp, texto);
            return await RunAsync("lp", new[] { "-d", nome, tmp });
        }
        finally
        {
            try { File.Delete(tmp); } catch { /* arquivo temporário, sem problema se falhar */ }
        }
    }

    // ── Navegador auxiliar (e-mail / consultas gerais) ─────────────────────
    // Usa um perfil (--user-data-dir) separado do Chromium do kiosk: sem isso, o
    // Chromium trata "--new-window" como uma nova janela do MESMO processo já em
    // --kiosk, que herda o modo kiosk inteiro (sem barra de endereço/voltar/fechar)
    // — o usuário ficava preso em qualquer página que abrisse (ex.: login do Google).
    public static Task<(bool ok, string output)> AbrirNavegador(string url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) || (uri.Scheme != "http" && uri.Scheme != "https"))
            return Task.FromResult((false, "URL inválida."));

        var profileDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "atelie-navegador-auxiliar");

        foreach (var bin in new[] { "chromium", "chromium-browser", "google-chrome-stable", "google-chrome" })
        {
            try
            {
                var psi = new System.Diagnostics.ProcessStartInfo { FileName = bin, UseShellExecute = false, CreateNoWindow = true };
                psi.ArgumentList.Add($"--user-data-dir={profileDir}");
                psi.ArgumentList.Add("--new-window");
                psi.ArgumentList.Add("--no-first-run");
                psi.ArgumentList.Add(uri.ToString());
                var p = System.Diagnostics.Process.Start(psi);
                if (p is not null) return Task.FromResult((true, $"Abrindo navegador ({bin})…"));
            }
            catch (System.ComponentModel.Win32Exception) { /* binário ausente, tenta o próximo candidato */ }
        }
        return Task.FromResult((false, "Nenhum navegador Chromium encontrado no sistema."));
    }
}

// ═════════════════════════════════════════════════════════════════════════════
// AuthStore (autenticação baseada em arquivo JSON)
// ═════════════════════════════════════════════════════════════════════════════
sealed class AuthStore
{
    private static readonly JsonSerializerOptions JsonOpts = new() { WriteIndented = true };
    private readonly string _statePath;
    private readonly string _auditDir;

    // Pagamentos PIX de autoatendimento de licença ainda não confirmados — em
    // memória (perdido se a API reiniciar; nesse caso o usuário só reconsulta o
    // status e o processo de renovação roda de novo, sem risco de duplicar).
    private static readonly System.Collections.Concurrent.ConcurrentDictionary<long, (Guid UserId, string Plano)> _pendingLicensePagamentos = new();

    public void RegistrarPagamentoLicencaPendente(long mpPaymentId, Guid userId, string plano) =>
        _pendingLicensePagamentos[mpPaymentId] = (userId, plano);

    /// Confirma e aplica um pagamento de licença aprovado — só uma vez (remove do
    /// dicionário ao processar) e só pro mesmo usuário que gerou a cobrança.
    public bool TentarConcluirPagamentoLicenca(long mpPaymentId, Guid userId, out object? resultado)
    {
        resultado = null;
        if (!_pendingLicensePagamentos.TryRemove(mpPaymentId, out var pendente)) return false;
        if (pendente.UserId != userId) return false;
        resultado = RenovarLicenca(pendente.UserId, pendente.Plano, "auto-pix-autoatendimento");
        return true;
    }

    public AuthStore(string dataDir, string auditDir)
    {
        Directory.CreateDirectory(dataDir);
        Directory.CreateDirectory(auditDir);
        _statePath = Path.Combine(dataDir, "auth-store.json");
        _auditDir  = auditDir;
    }

    // Perfis padrão do sistema — nome -> permissões. Fonte única usada tanto na
    // primeira instalação (EnsureSeeded) quanto para atualizar instalações já
    // existentes (UpgradeRolePermissions), para que uma mudança de modelo de
    // acesso chegue a bancos já em produção sem precisar recriar usuários.
    //
    // administrador: acesso total (fornecedor/dono). operacional: só operação
    // (vendas, ROL, clientes, estoque/serviços) — sem caixa, financeiro,
    // relatórios, usuários, configurações ou legado. supervisor/caixa: acesso
    // intermediário. leitura: só relatórios.
    private static readonly (string Name, string DisplayName, string[] Permissions)[] RolesCanonicos =
    [
        ("administrador", "Administrador", ["*"]),
        ("operacional",   "Operacional",   [Perm.CadastroWrite, Perm.DashboardRead]),
        // usuarios.read e config.write ficam fora de propósito: só o
        // administrador acessa as telas de Usuários e Configurações (tokens
        // do Mercado Pago, etc.) — supervisor é acesso operacional avançado,
        // não administrativo.
        ("supervisor",    "Supervisor",    [Perm.RelatoriosRead, Perm.CadastroWrite, Perm.CaixaAccess, Perm.FinanceiroRead, Perm.LegadoRead, Perm.PrecosWrite, Perm.CatalogosWrite, Perm.FidelidadeManage, Perm.SenhaResetOutros]),
        ("caixa",         "Caixa",         [Perm.CadastroWrite, Perm.CaixaAccess, Perm.FinanceiroRead, Perm.RelatoriosRead]),
        ("leitura",       "Leitura",       [Perm.RelatoriosRead]),
    ];

    /// Sincroniza as permissões dos perfis padrão com o modelo atual, mesmo em
    /// um auth-store.json já existente. Não mexe em usuários nem em perfis
    /// customizados que não estejam na lista canônica.
    public void UpgradeRolePermissions()
    {
        if (!File.Exists(_statePath)) return;
        var state = Load();
        var changed = false;
        foreach (var (name, displayName, perms) in RolesCanonicos)
        {
            var idx = state.Roles.FindIndex(r => r.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (idx < 0)
            {
                state.Roles.Add(new(name, displayName, perms.ToList()));
                changed = true;
            }
            else if (!state.Roles[idx].Permissions.SequenceEqual(perms))
            {
                state.Roles[idx] = state.Roles[idx] with { Permissions = perms.ToList() };
                changed = true;
            }
        }
        if (changed) Save(state);
    }

    public void EnsureSeeded()
    {
        if (File.Exists(_statePath)) return;

        var salt = Rnd64();
        var state = new AuthState
        {
            Roles = RolesCanonicos.Select(r => new RoleDefinition(r.Name, r.DisplayName, r.Permissions.ToList())).ToList(),
            Users =
            [
                new() { Id = Guid.NewGuid(), Username = "gabriela", DisplayName = "Gabriela",
                    PasswordSalt = salt, PasswordHash = HashPwd("12345", salt),
                    Roles = ["administrador"], IsActive = true, MustChangePassword = false,
                    CreatedAt = DateTimeOffset.UtcNow, CreatedBy = "bootstrap" }
            ],
            SigningKey = Rnd64()
        };

        Save(state);
        Audit("bootstrap", "auth.seed", "Gabriela criada como administradora.");
    }

    public void ImportLegacyUsers()
    {
        var path = FindImportFile(@"legacy\Usuarios.csv");
        if (path is null) return;

        var state = Load();
        var imported = 0;
        foreach (var row in ReadCsvDict(path))
        {
            var username = V(row, "CodUsuario").Trim().ToLowerInvariant();
            var display = V(row, "NomUsuario");
            if (string.IsNullOrWhiteSpace(username)) continue;

            var roles = ResolveLegacyRoles(V(row, "GruUsuario"), V(row, "TipUsuario"));
            var existing = state.Users.FirstOrDefault(x => x.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            if (existing is not null)
            {
                existing.DisplayName = string.IsNullOrWhiteSpace(display) ? existing.DisplayName : display;
                existing.Roles = roles;
                existing.IsActive = !IsTrue(V(row, "Cancelado"));
                continue;
            }

            var salt = Rnd64();
            state.Users.Add(new UserAccount
            {
                Id = Guid.NewGuid(),
                Username = username,
                DisplayName = string.IsNullOrWhiteSpace(display) ? username.ToUpperInvariant() : display,
                PasswordSalt = salt,
                PasswordHash = HashPwd("12345", salt),
                Roles = roles,
                IsActive = !IsTrue(V(row, "Cancelado")),
                MustChangePassword = true,
                CreatedAt = DateTimeOffset.UtcNow,
                CreatedBy = "legacy-Usuarios.DB"
            });
            imported++;
        }

        Save(state);
        if (imported > 0)
            Audit("legacy-import", "users.import", $"{imported} usuarios importados de Usuarios.DB com senha temporaria 12345.");
    }

    // Proteção contra força bruta: 5 tentativas erradas por usuário em 15min
    // bloqueiam novas tentativas por 15min. Em memória (por processo) — reinicia
    // sozinho a cada restart da API, o que é aceitável para um kiosk local.
    private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, (int Fails, DateTimeOffset WindowStart, DateTimeOffset? LockedUntil)> _loginAttempts = new();
    private const int MaxLoginFails = 5;
    private static readonly TimeSpan LoginLockDuration = TimeSpan.FromMinutes(15);
    private static readonly TimeSpan LoginWindowDuration = TimeSpan.FromMinutes(15);

    public LoginResponse? Login(string username, string password, string? ip)
    {
        var key = username.Trim().ToLowerInvariant();
        var now = DateTimeOffset.UtcNow;
        if (_loginAttempts.TryGetValue(key, out var throttle) && throttle.LockedUntil is { } until && until > now)
        {
            Audit(username, "auth.login.blocked_lockout", $"ip={ip}");
            throw new InvalidOperationException("Muitas tentativas de login. Tente novamente em alguns minutos.");
        }

        var state = Load();
        var user = state.Users.FirstOrDefault(x => x.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        if (user is null || !user.IsActive || !FixedEq(user.PasswordHash, HashPwd(password, user.PasswordSalt)))
        {
            _loginAttempts.AddOrUpdate(key,
                _ => (1, now, null),
                (_, old) =>
                {
                    if (now - old.WindowStart > LoginWindowDuration) return (1, now, null);
                    var fails = old.Fails + 1;
                    DateTimeOffset? locked = fails >= MaxLoginFails ? now.Add(LoginLockDuration) : null;
                    return (fails, old.WindowStart, locked);
                });
            Audit(username, "auth.login.failed", $"ip={ip}");
            return null;
        }
        _loginAttempts.TryRemove(key, out _);

        var lic = AvaliarLicenca(user, state);
        // Licença vencida não bloqueia mais o login em si — o usuário entra
        // normalmente, mas o frontend mostra uma tela de regularização travando
        // o resto do sistema (ver user.LicenseVencida em UserSummary/Describe).
        if (lic.bloqueada)
            Audit(user.Username, "auth.login.license_locked", $"ip={ip}");

        user.LastLoginAt = DateTimeOffset.UtcNow;
        Save(state);
        Audit(user.Username, "auth.login.ok", $"ip={ip}");
        return new(MkToken(state, user), DateTimeOffset.UtcNow.AddHours(8), user.MustChangePassword, Describe(user, state), lic.aviso);
    }

    public void ChangePassword(Guid userId, string senhaAtual, string senhaNova)
    {
        var state = Load();
        var user = state.Users.FirstOrDefault(x => x.Id == userId) ?? throw new InvalidOperationException("Usuário não encontrado.");
        if (!FixedEq(user.PasswordHash, HashPwd(senhaAtual, user.PasswordSalt)))
            throw new InvalidOperationException("Senha atual incorreta.");
        user.PasswordSalt = Rnd64();
        user.PasswordHash = HashPwd(senhaNova, user.PasswordSalt);
        user.MustChangePassword = false;
        Save(state);
        Audit(user.Username, "auth.password.changed", "");
    }

    public Session RequireSession(HttpRequest request)
    {
        var hdr = request.Headers.Authorization.ToString();
        if (!hdr.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            throw new UnauthorizedAccessException("Token ausente.");

        var state = Load();
        var token = hdr["Bearer ".Length..].Trim();
        var parts = token.Split('.');
        if (parts.Length != 2) throw new UnauthorizedAccessException("Token inválido.");

        var payloadJson = Encoding.UTF8.GetString(B64Dec(parts[0]));
        var sig = B64Enc(Sign(state.SigningKey, parts[0]));
        if (!FixedEq(sig, parts[1])) throw new UnauthorizedAccessException("Assinatura inválida.");

        var payload = JsonSerializer.Deserialize<TokenPayload>(payloadJson) ?? throw new UnauthorizedAccessException("Token vazio.");
        if (payload.ExpiresAt < DateTimeOffset.UtcNow) throw new UnauthorizedAccessException("Token expirado.");

        var user = state.Users.FirstOrDefault(x => x.Id == payload.UserId && x.IsActive)
            ?? throw new UnauthorizedAccessException("Usuário inativo.");

        return new(user.Id, user.Username, user.DisplayName, user.Roles, ResolvePerm(user, state));
    }

    public Session RequirePermission(HttpRequest request, string perm)
    {
        var s = RequireSession(request);
        if (!s.Permissions.Contains("*") && !s.Permissions.Contains(perm))
            throw new UnauthorizedAccessException($"Permissão necessária: {perm}");
        return s;
    }

    public Session RequireAnyPermission(HttpRequest request, params string[] perms)
    {
        var s = RequireSession(request);
        if (!s.Permissions.Contains("*") && !perms.Any(s.Permissions.Contains))
            throw new UnauthorizedAccessException($"Permissão necessária: {string.Join(" ou ", perms)}");
        return s;
    }

    public object DescribeSession(Session s) => new { s.UserId, s.Username, s.DisplayName, s.Roles, s.Permissions };

    public IEnumerable<object> ListUsers()
    {
        var state = Load();
        return state.Users.Select(u => Describe(u, state));
    }

    public UserSummary CreateUser(CreateUserRequest req, string by)
    {
        if (string.IsNullOrWhiteSpace(req.Username)) throw new InvalidOperationException("Login é obrigatório.");
        if (string.IsNullOrWhiteSpace(req.DisplayName)) throw new InvalidOperationException("Nome de exibição é obrigatório.");
        if (string.IsNullOrWhiteSpace(req.TemporaryPassword)) throw new InvalidOperationException("Senha temporária é obrigatória.");
        var state = Load();
        if (state.Users.Any(x => x.Username.Equals(req.Username, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException("Usuário já existe.");
        ValidRoles(req.Roles, state);
        var salt = Rnd64();
        var user = new UserAccount
        {
            Id = Guid.NewGuid(), Username = req.Username.Trim().ToLowerInvariant(),
            DisplayName = req.DisplayName.Trim(), PasswordSalt = salt,
            PasswordHash = HashPwd(req.TemporaryPassword, salt),
            Roles = req.Roles.Distinct(StringComparer.OrdinalIgnoreCase).ToList(),
            IsActive = true, MustChangePassword = true,
            CreatedAt = DateTimeOffset.UtcNow, CreatedBy = by
        };
        state.Users.Add(user);
        Save(state);
        Audit(by, "users.create", user.Username);
        return Describe(user, state);
    }

    public UserSummary UpdateUser(Guid id, UpdateUserRequest req, string by)
    {
        var state = Load();
        var user = state.Users.FirstOrDefault(x => x.Id == id) ?? throw new KeyNotFoundException("Usuário não encontrado.");
        user.DisplayName = req.DisplayName.Trim();
        if (req.TemporaryPassword is { Length: > 0 })
        {
            user.PasswordSalt = Rnd64();
            user.PasswordHash = HashPwd(req.TemporaryPassword, user.PasswordSalt);
            user.MustChangePassword = true;
        }
        Save(state);
        Audit(by, "users.update", user.Username);
        return Describe(user, state);
    }

    public void DeactivateUser(Guid id, string by)
    {
        var state = Load();
        var user = state.Users.FirstOrDefault(x => x.Id == id) ?? throw new KeyNotFoundException("Usuário não encontrado.");
        user.IsActive = false;
        Save(state);
        Audit(by, "users.deactivate", user.Username);
    }

    public void ReactivateUser(Guid id, string by)
    {
        var state = Load();
        var user = state.Users.FirstOrDefault(x => x.Id == id) ?? throw new KeyNotFoundException("Usuário não encontrado.");
        user.IsActive = true;
        Save(state);
        Audit(by, "users.reactivate", user.Username);
    }

    /// Reset de senha por terceiros. Quem só tem senha.reset-outros (supervisor)
    /// nunca pode mexer numa conta com o perfil administrador.
    public UserSummary ResetSenha(Guid id, string novaSenha, bool podeTudo, string by)
    {
        var state = Load();
        var user = state.Users.FirstOrDefault(x => x.Id == id) ?? throw new KeyNotFoundException("Usuário não encontrado.");
        if (!podeTudo && user.Roles.Contains("administrador", StringComparer.OrdinalIgnoreCase))
            throw new UnauthorizedAccessException("Sem permissão para redefinir a senha do administrador.");
        if (string.IsNullOrWhiteSpace(novaSenha)) throw new InvalidOperationException("Nova senha é obrigatória.");
        user.PasswordSalt = Rnd64();
        user.PasswordHash = HashPwd(novaSenha, user.PasswordSalt);
        user.MustChangePassword = true;
        Save(state);
        Audit(by, "users.reset-senha", user.Username);
        return Describe(user, state);
    }

    /// Remove o registro do usuário definitivamente. Só permitido para contas já
    /// inativas — histórico operacional (caixa, vendas, auditoria) referencia o
    /// usuário por nome de texto, não por Id, então não quebra nada existente.
    public void DeleteUserPermanently(Guid id, string by)
    {
        var state = Load();
        var user = state.Users.FirstOrDefault(x => x.Id == id) ?? throw new KeyNotFoundException("Usuário não encontrado.");
        if (user.IsActive) throw new InvalidOperationException("Desative o usuário antes de excluir permanentemente.");
        state.Users.Remove(user);
        Save(state);
        Audit(by, "users.delete-permanent", user.Username);
    }

    public UserSummary AssignRoles(Guid id, IEnumerable<string> roles, string by)
    {
        var state = Load();
        ValidRoles(roles, state);
        var user = state.Users.FirstOrDefault(x => x.Id == id) ?? throw new KeyNotFoundException("Usuário não encontrado.");
        user.Roles = roles.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        Save(state);
        Audit(by, "users.roles", $"{user.Username}: {string.Join(",", user.Roles)}");
        return Describe(user, state);
    }

    public object GetPermissionsMap()
    {
        var state = Load();
        return state.Roles.Select(r => new { r.Name, r.DisplayName, r.Permissions });
    }

    private static void ValidRoles(IEnumerable<string> roles, AuthState state)
    {
        var known = state.Roles.Select(x => x.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var bad = roles.Where(x => !known.Contains(x)).ToArray();
        if (bad.Length > 0) throw new InvalidOperationException($"Perfil inválido: {string.Join(", ", bad)}");
    }

    private UserSummary Describe(UserAccount u, AuthState s)
    {
        var lic = AvaliarLicenca(u, s);
        return new(u.Id, u.Username, u.DisplayName, u.Roles, ResolvePerm(u, s), u.IsActive, u.MustChangePassword, u.LastLoginAt,
            s.LicensePlano, s.LicenseVenceEm, lic.isenta, lic.bloqueada);
    }

    private static List<string> ResolvePerm(UserAccount u, AuthState s) =>
        s.Roles.Where(r => u.Roles.Contains(r.Name, StringComparer.OrdinalIgnoreCase))
               .SelectMany(r => r.Permissions).Distinct(StringComparer.OrdinalIgnoreCase).Order().ToList();

    /// Licença é da aplicação (uma instalação = uma licença), não por usuário —
    /// se qualquer pessoa da loja paga, libera o acesso para todo mundo daquela
    /// instalação. Administradores são sempre isentos (acesso do fornecedor).
    /// Sem plano configurado (LicenseVenceEm nulo e não vitalício) é tratado
    /// como vencida — bloqueia todo mundo, menos admin, até alguém pagar.
    private static (bool isenta, bool bloqueada, int? diasParaVencer, string? aviso) AvaliarLicenca(UserAccount u, AuthState s)
    {
        if (u.Roles.Contains("administrador", StringComparer.OrdinalIgnoreCase))
            return (true, false, null, null);
        if (string.Equals(s.LicensePlano, "vitalicio", StringComparison.OrdinalIgnoreCase))
            return (true, false, null, null);
        if (s.LicenseVenceEm is null)
            return (false, true, null, null);

        var dias = (int)Math.Ceiling((s.LicenseVenceEm.Value - TrustedClock.UtcNow).TotalDays);
        if (dias < 0) return (false, true, dias, null);
        var aviso = dias <= 10
            ? $"A licença do sistema vence em {dias} dia(s), em {s.LicenseVenceEm.Value:dd/MM/yyyy}. Renove para evitar bloqueio de acesso."
            : null;
        return (false, false, dias, aviso);
    }

    /// Renova/atribui a licença da aplicação (compartilhada por todos os usuários
    /// da instalação). Se ainda faltam dias para o vencimento atual, eles são
    /// preservados: a nova data parte do vencimento atual (não de hoje), depois
    /// soma os meses do plano. Ex.: vence dia 10, renova dia 02 → fica valendo
    /// até dia 10 do mês seguinte (dias que faltavam + o novo ciclo inteiro).
    /// userId identifica só quem disparou a renovação, para auditoria.
    public object RenovarLicenca(Guid userId, string plano, string by)
    {
        var state = Load();
        var user = state.Users.FirstOrDefault(x => x.Id == userId) ?? throw new KeyNotFoundException("Usuário não encontrado.");
        var meses = LicencaPlanos.MesesDoPlano(plano);
        var valor = LicencaPlanos.ValorFixo(plano);

        state.LicenseInicioEm ??= TrustedClock.UtcNow;
        if (meses is null)
        {
            state.LicensePlano = "vitalicio";
            state.LicenseVenceEm = null;
        }
        else
        {
            var baseData = (state.LicenseVenceEm.HasValue && state.LicenseVenceEm.Value > TrustedClock.UtcNow)
                ? state.LicenseVenceEm.Value
                : TrustedClock.UtcNow;
            state.LicensePlano = plano.ToLowerInvariant();
            state.LicenseVenceEm = baseData.AddMonths(meses.Value);
        }
        Save(state);
        Audit(by, "licenca.renovada", $"disparadoPor={user.Username};plano={state.LicensePlano};valor={valor};venceEm={state.LicenseVenceEm:O}");
        return new { plano = state.LicensePlano, venceEm = state.LicenseVenceEm, valor };
    }

    private string MkToken(AuthState s, UserAccount u)
    {
        var payload = B64Enc(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(
            new TokenPayload(u.Id, DateTimeOffset.UtcNow.AddHours(8), Guid.NewGuid().ToString("N")))));
        return $"{payload}.{B64Enc(Sign(s.SigningKey, payload))}";
    }

    private AuthState Load() => JsonSerializer.Deserialize<AuthState>(File.ReadAllText(_statePath)) ?? new();
    private void Save(AuthState s) { var t = _statePath + ".tmp"; File.WriteAllText(t, JsonSerializer.Serialize(s, JsonOpts), Encoding.UTF8); File.Move(t, _statePath, true); }
    private void Audit(string actor, string action, string detail) => File.AppendAllText(Path.Combine(_auditDir, $"audit-{DateTime.UtcNow:yyyyMMdd}.jsonl"), JsonSerializer.Serialize(new { ts = DateTimeOffset.UtcNow, actor, action, detail }) + "\n", Encoding.UTF8);
    private static List<string> ResolveLegacyRoles(string grupo, string tipo)
    {
        if (grupo.Equals("MASTE", StringComparison.OrdinalIgnoreCase)) return ["administrador"];
        if (tipo.Equals("S", StringComparison.OrdinalIgnoreCase)) return ["supervisor"];
        return ["operacional"];
    }
    private static string? FindImportFile(string name)
    {
        var candidates = new[]
        {
            Path.Combine(AppContext.BaseDirectory, "import", name),
            Path.Combine(Directory.GetCurrentDirectory(), "import", name),
            Path.Combine(@"E:\Projeto Luci\MOD\apps\backend\EquipeExe.Mod.Api\import", name)
        };
        return candidates.FirstOrDefault(File.Exists);
    }
    private static IEnumerable<Dictionary<string, string>> ReadCsvDict(string path)
    {
        using var reader = new StreamReader(path, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
        var headerLine = reader.ReadLine();
        if (headerLine is null) yield break;
        var header = SplitCsv(headerLine).Select(h => h.Trim().TrimStart('\uFEFF')).ToArray();
        string? line;
        while ((line = reader.ReadLine()) is not null)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            var cols = SplitCsv(line);
            var row = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            for (var i = 0; i < header.Length; i++)
                row[header[i]] = i < cols.Length ? cols[i].Trim().Trim('"') : "";
            yield return row;
        }
    }
    private static string[] SplitCsv(string line)
    {
        var parts = new List<string>();
        var sb = new StringBuilder();
        var quoted = false;
        for (var i = 0; i < line.Length; i++)
        {
            var c = line[i];
            if (c == '"') { quoted = !quoted; continue; }
            if (c == ',' && !quoted) { parts.Add(sb.ToString()); sb.Clear(); continue; }
            sb.Append(c);
        }
        parts.Add(sb.ToString());
        return parts.ToArray();
    }
    private static string V(Dictionary<string, string> row, string key) =>
        row.TryGetValue(key, out var value) ? value : "";
    private static bool IsTrue(string value)
    {
        var v = value.Trim().ToUpperInvariant();
        return v is "S" or "SIM" or "TRUE" or "1" or "T";
    }
    private static string HashPwd(string pw, string salt) => Convert.ToBase64String(Rfc2898DeriveBytes.Pbkdf2(pw, Convert.FromBase64String(salt), 210_000, HashAlgorithmName.SHA256, 32));
    private static byte[] Sign(string key, string payload) { using var h = new HMACSHA256(Convert.FromBase64String(key)); return h.ComputeHash(Encoding.UTF8.GetBytes(payload)); }
    private static bool FixedEq(string a, string b) => CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(a), Encoding.UTF8.GetBytes(b));
    private static string B64Enc(byte[] v) => Convert.ToBase64String(v).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    private static byte[] B64Dec(string v) { var p = v.Replace('-', '+').Replace('_', '/').PadRight(v.Length + (4 - v.Length % 4) % 4, '='); return Convert.FromBase64String(p); }
    private static string Rnd64() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
}

// ═════════════════════════════════════════════════════════════════════════════
// Records / DTOs
// ═════════════════════════════════════════════════════════════════════════════
record LoginRequest(string Username, string Password);
record LoginResponse(string AccessToken, DateTimeOffset ExpiresAt, bool MustChangePassword, UserSummary User, string? LicenseWarning);
record TrocarSenhaRequest(string SenhaAtual, string SenhaNova);
record CreateUserRequest(string Username, string DisplayName, string TemporaryPassword, List<string> Roles);
record RenovarLicencaRequest(string Plano);
record UpdateUserRequest(string DisplayName, string? TemporaryPassword);
record ResetSenhaRequest(string NovaSenha);
record AssignRolesRequest(List<string> Roles);
record TokenPayload(Guid UserId, DateTimeOffset ExpiresAt, string Nonce);
record RoleDefinition(string Name, string DisplayName, List<string> Permissions);
record UserSummary(Guid Id, string Username, string DisplayName, List<string> Roles, List<string> Permissions, bool IsActive, bool MustChangePassword, DateTimeOffset? LastLoginAt,
    string? LicensePlano, DateTimeOffset? LicenseVenceEm, bool LicenseIsenta, bool LicenseVencida);
record Session(Guid UserId, string Username, string DisplayName, List<string> Roles, List<string> Permissions);

record ClienteRequest(
    string Nome, string? Documento, string? Telefone, string? Celular, string? Email,
    string? Logradouro, string? Numero, string? Bairro, string? Cidade, string? Estado, string? Cep,
    string? Observacoes, double LimiteCredito, double DescontoPercent,
    string? DataNascimento = null, string? CartaoFidelidade = null, string? Contato = null);

record ServicoRequest(string Codigo, string Descricao, string Categoria, double Preco);
record AjustarPrecosRequest(List<int>? Ids, string? Categoria, string Tipo, string Modo, double Valor, bool? TodasCategorias);

record RolRequest(int ClienteId, string? DataEntrada, string? DataPromessa, string? Observacoes);

record RolItemRequest(
    int? ServicoId, string Descricao, string? TipoTecido, string? Cor,
    string? Marca, string? Defeito, double Quantidade, double ValorUnitario, string? Observacao,
    double? Peso = null, string? Identificacao = null, string? Localizacao = null, double? ValorTerceiro = null, string? Obs2 = null);

record PagamentoRequest(string? MetodoPagamento, double? ValorPago, double? Desconto, List<PagamentoLinha>? Linhas);
record PagamentoLinha(string Metodo, double Valor);
record EstornoRequest(string Motivo);
record EntregarRequest(string? DataEntrega, string? Observacao);
record CancelarRequest(string? Motivo);

record AbrirCaixaRequest(double ValorAbertura);
record FecharCaixaRequest(double ValorContado, string? Observacao);
record MovCaixaRequest(double Valor, string? Descricao);

record ReceberRequest(double ValorRecebido, string Metodo, string? Observacao);
record ConfigRequest(string Valor);
record WifiConectarRequest(string Ssid, string? Senha);
record ImpressorasConfigRequest(string? ImpressoraA4, string? ImpressoraTermica, string? LarguraTermica);
record NavegadorAbrirRequest(string? Url);
record CreditoRequest(string Tipo, double Valor, string? Descricao, string? Referencia);
record CoverageStatusRequest(string Status, string? Observacao);
record PrintItem(string Descricao, double Quantidade, double ValorUnitario, double ValorTotal, string? Cor, string? Marca, string? Defeito, string? Observacao);
record OrcamentoRequest(int ClienteId, string? DataEntrada, string? DataPromessa, string? DataValidade, string? Observacoes);
record AgendaRequest(int ClienteId, string DataAgendamento, string? HoraAgendamento, int? DuracaoMinutos, string? Tipo, string? Observacao, int? RolId, int? OrcId);
record CatalogoRequest(string Tipo, string Codigo, string Descricao);
record IndenizacaoRequest(int ClienteId, int? OsId, string Descricao, double Valor, string? Motivo, string? Observacao);
record IndenizacaoUpdateRequest(string Status, string? Motivo, string? Observacao);
record GuardaroupaRequest(int ClienteId, string Descricao, string? Categoria, string? Cor, string? Marca, int Quantidade, string? Localizacao, string? Observacao);
record TerceirizacaoRequest(int? OsId, string Fornecedor, string Descricao, double Valor, string? DataRetornoPrevista, string? Observacao);
record PontosRequest(int Pontos, string Tipo, string? Referencia, string? Observacao);
record RegraFidelidadeRequest(int PontosPorVenda);
record DoacaoRequest(int ClienteId, int? OsId, string Descricao, double Valor, string? Observacao);
record PixCriarRequest(int? RolId, double Valor, string? Contexto, string? Descricao);
record MinhaLicencaPixRequest(string? Plano);

// Relógio confiável para a validação de licença: não basta o relógio local do
// appliance, já que atrasá-lo manualmente burlaria o vencimento. Usa o
// cabeçalho HTTP "Date" de uma resposta HTTPS (presente em qualquer servidor,
// sem precisar de API de NTP dedicada) para calcular um desvio (skew) em
// relação ao relógio local, atualizado a cada 30min em segundo plano. Se
// ficar offline, mantém o último desvio conhecido — nunca bloqueia o app por
// falta de internet (o resto do sistema já é offline-first); só evita que
// atrasar o relógio manualmente engane a licença enquanto há conexão.
static class TrustedClock
{
    private static TimeSpan? _skew;
    private static readonly HttpClient _http = new() { Timeout = TimeSpan.FromSeconds(5) };

    public static DateTimeOffset UtcNow => DateTimeOffset.UtcNow + (_skew ?? TimeSpan.Zero);

    public static void StartBackgroundSync()
    {
        _ = Task.Run(async () =>
        {
            while (true)
            {
                await TrySyncOnce();
                await Task.Delay(TimeSpan.FromMinutes(30));
            }
        });
    }

    private static async Task TrySyncOnce()
    {
        try
        {
            using var req = new HttpRequestMessage(HttpMethod.Head, "https://www.google.com");
            using var resp = await _http.SendAsync(req);
            if (resp.Headers.Date is { } serverDate)
                _skew = serverDate - DateTimeOffset.UtcNow;
        }
        catch { /* offline — mantém o último desvio conhecido */ }
    }
}

sealed class AuthState
{
    public List<UserAccount> Users { get; set; } = [];
    public List<RoleDefinition> Roles { get; set; } = [];
    public string SigningKey { get; set; } = "";

    // Licenciamento é da APLICAÇÃO (uma instalação = uma licença), não por
    // usuário: se qualquer pessoa paga, libera para todo mundo daquela loja.
    // Campos por-usuário (UserAccount.LicensePlano/...) ficam só como legado
    // para migração de instalações antigas (ver AuthStore.MigrarLicencaLegado).
    public string? LicensePlano { get; set; }
    public DateTimeOffset? LicenseVenceEm { get; set; }
    public DateTimeOffset? LicenseInicioEm { get; set; }
}

sealed class UserAccount
{
    public Guid Id { get; set; }
    public string Username { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public string PasswordSalt { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public List<string> Roles { get; set; } = [];
    public bool IsActive { get; set; }
    public bool MustChangePassword { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public string CreatedBy { get; set; } = "";
    public DateTimeOffset? LastLoginAt { get; set; }

    // Licenciamento — administradores são sempre isentos (ver AuthStore.AvaliarLicenca).
    // LicensePlano nulo = sem licença configurada, tratado como vencida (bloqueia login).
    public string? LicensePlano { get; set; }
    public DateTimeOffset? LicenseVenceEm { get; set; }
    public DateTimeOffset? LicenseInicioEm { get; set; }
}

// ═════════════════════════════════════════════════════════════════════════════
// Licenciamento — tabela de preços fixa (Mensal R$240; Trimestral 3x -5%;
// Anual 12x -20%; Vitalício é negociado direto com o fornecedor, sem valor fixo).
// ═════════════════════════════════════════════════════════════════════════════
static class LicencaPlanos
{
    public const double Mensal = 240.00;
    public const double Trimestral = 240.0 * 3 * 0.95; // 684.00
    public const double Anual = 240.0 * 12 * 0.80;     // 2304.00

    public static double? ValorFixo(string plano) => plano.ToLowerInvariant() switch
    {
        "mensal" => Mensal,
        "trimestral" => Trimestral,
        "anual" => Anual,
        "vitalicio" => null,
        _ => throw new InvalidOperationException($"Plano de licença inválido: {plano}")
    };

    public static int? MesesDoPlano(string plano) => plano.ToLowerInvariant() switch
    {
        "mensal" => 1,
        "trimestral" => 3,
        "anual" => 12,
        "vitalicio" => null,
        _ => throw new InvalidOperationException($"Plano de licença inválido: {plano}")
    };

    public static IEnumerable<object> Catalogo() =>
    [
        new { plano = "mensal",     label = "Mensal",                              valor = (double?)Mensal,     meses = (int?)1 },
        new { plano = "trimestral", label = "Trimestral (3 meses, 5% de desconto)", valor = (double?)Trimestral, meses = (int?)3 },
        new { plano = "anual",      label = "Anual (12 meses, 20% de desconto)",    valor = (double?)Anual,      meses = (int?)12 },
        new { plano = "vitalicio",  label = "Vitalício — aquisição do programa (sob consulta, negociar com o fornecedor)", valor = (double?)null, meses = (int?)null },
    ];
}
