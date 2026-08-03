using System.Diagnostics;
using System.Net.Http;
using Microsoft.Web.WebView2.WinForms;

namespace Atelie.Desktop;

/// Janela única do app: sobe o backend local (EquipeExe.Mod.Api.exe) em
/// segundo plano, espera responder, e mostra a interface web dele dentro
/// de um WebView2 preenchendo a janela inteira — sem barra de endereço,
/// sem abas, sem cara de navegador. Mesmo princípio do modo quiosque que
/// já usamos no appliance Linux (Chromium --kiosk apontado pro localhost),
/// só que como programa Windows instalado de verdade. Também roda o
/// AutoUpdater em segundo plano, puxando novas versões do backend
/// direto dos releases públicos do GitHub, sem precisar de pendrive.
public class MainForm : Form
{
    private const string BackendUrl = "http://127.0.0.1:8095";

    private readonly WebView2 _webView = new() { Dock = DockStyle.Fill };
    private readonly string _backendDir = Path.Combine(AppContext.BaseDirectory, "backend");
    private readonly string _dadosDir = Path.Combine(AppContext.BaseDirectory, "dados");
    private Process? _backendProcess;
    private Label? _statusLabel;
    private AutoUpdater? _updater;

    public MainForm()
    {
        Text = "Ateliê da Luci — PDV";
        WindowState = FormWindowState.Maximized;
        MinimumSize = new Size(1024, 700);
        StartPosition = FormStartPosition.CenterScreen;

        Controls.Add(_webView);
        Load += MainForm_Load;
        FormClosing += MainForm_FormClosing;
    }

    private async void MainForm_Load(object? sender, EventArgs e)
    {
        MostrarStatus("Iniciando o sistema...");

        if (!await BackendRespondeAsync())
        {
            IniciarBackend();
            var pronto = await EsperarBackendAsync(TimeSpan.FromSeconds(30));
            if (!pronto)
            {
                MostrarStatus("Não foi possível iniciar o sistema. Tente fechar e abrir de novo.\nSe persistir, contate o suporte.");
                return;
            }
        }

        await _webView.EnsureCoreWebView2Async();
        _webView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
        RemoverStatus();
        _webView.CoreWebView2.Navigate(BackendUrl);

        var logDir = Path.Combine(_dadosDir, "logs");
        Directory.CreateDirectory(logDir);
        var logPath = Path.Combine(logDir, "auto-update.log");
        _updater = new AutoUpdater(
            appRoot: AppContext.BaseDirectory,
            backendDir: _backendDir,
            pararBackend: PararBackendAsync,
            iniciarBackend: async () =>
            {
                IniciarBackend();
                await EsperarBackendAsync(TimeSpan.FromSeconds(30));
                if (_webView.CoreWebView2 is not null)
                    _webView.CoreWebView2.Reload();
            },
            log: msg => File.AppendAllText(logPath, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {msg}\n"));
        _updater.StartBackground();
    }

    private void IniciarBackend()
    {
        var exe = Path.Combine(_backendDir, "EquipeExe.Mod.Api.exe");
        if (!File.Exists(exe))
        {
            MostrarStatus($"Arquivo do sistema não encontrado em:\n{exe}");
            return;
        }

        Directory.CreateDirectory(_dadosDir);

        var psi = new ProcessStartInfo
        {
            FileName = exe,
            WorkingDirectory = _backendDir,
            UseShellExecute = false,
            CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Hidden,
        };
        psi.EnvironmentVariables["ASPNETCORE_URLS"] = BackendUrl;
        psi.EnvironmentVariables["EquipeExe__DataDirectory"] = _dadosDir;
        psi.EnvironmentVariables["EquipeExe__AuditDirectory"] = Path.Combine(_dadosDir, "audit");

        _backendProcess = Process.Start(psi);
    }

    private Task PararBackendAsync()
    {
        try
        {
            if (_backendProcess is { HasExited: false })
                _backendProcess.Kill(entireProcessTree: true);
        }
        catch { /* processo já pode ter encerrado sozinho */ }
        return Task.CompletedTask;
    }

    private static async Task<bool> BackendRespondeAsync()
    {
        try
        {
            using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(2) };
            await http.GetAsync(BackendUrl);
            return true;
        }
        catch { return false; }
    }

    private async Task<bool> EsperarBackendAsync(TimeSpan timeout)
    {
        var inicio = DateTime.UtcNow;
        while (DateTime.UtcNow - inicio < timeout)
        {
            if (await BackendRespondeAsync()) return true;
            await Task.Delay(500);
        }
        return false;
    }

    private void MostrarStatus(string msg)
    {
        if (_statusLabel is null)
        {
            _statusLabel = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 14),
                BackColor = Color.FromArgb(15, 23, 42),
                ForeColor = Color.White,
            };
            Controls.Add(_statusLabel);
            _statusLabel.BringToFront();
        }
        _statusLabel.Text = msg;
    }

    private void RemoverStatus()
    {
        if (_statusLabel is null) return;
        Controls.Remove(_statusLabel);
        _statusLabel.Dispose();
        _statusLabel = null;
    }

    private async void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
    {
        await PararBackendAsync();
    }
}
