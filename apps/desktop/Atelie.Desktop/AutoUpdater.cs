using System.Formats.Tar;
using System.IO.Compression;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text.Json.Serialization;

namespace Atelie.Desktop;

/// Verifica e instala atualizações do backend a partir dos GitHub Releases
/// públicos do repositório (rgerio01/R-L-Atelie), sem precisar de token —
/// o repositório é público. Roda em segundo plano, nunca derruba o app: se
/// não tiver internet ou o GitHub estiver fora do ar, só ignora e segue
/// rodando na versão atual.
public sealed class AutoUpdater
{
    private const string Repo = "rgerio01/R-L-Atelie";
    private const string AssetPrefix = "atelie-nextgen-windows-";

    private readonly string _appRoot;
    private readonly string _backendDir;
    private readonly string _versionFile;
    private readonly Func<Task> _pararBackend;
    private readonly Func<Task> _iniciarBackend;
    private readonly Action<string> _log;

    public AutoUpdater(string appRoot, string backendDir, Func<Task> pararBackend, Func<Task> iniciarBackend, Action<string> log)
    {
        _appRoot = appRoot;
        _backendDir = backendDir;
        _versionFile = Path.Combine(appRoot, "version.txt");
        _pararBackend = pararBackend;
        _iniciarBackend = iniciarBackend;
        _log = log;
    }

    public void StartBackground()
    {
        _ = Task.Run(async () =>
        {
            await Task.Delay(TimeSpan.FromMinutes(1));
            while (true)
            {
                try { await CheckAndUpdateAsync(); }
                catch (Exception ex) { _log($"Falha ao checar atualizacao: {ex.Message}"); }
                await Task.Delay(TimeSpan.FromMinutes(30));
            }
        });
    }

    private async Task CheckAndUpdateAsync()
    {
        using var http = new HttpClient();
        http.DefaultRequestHeaders.UserAgent.ParseAdd("AtelieDesktopUpdater/1.0");

        var releases = await http.GetFromJsonAsync<List<GhRelease>>(
            $"https://api.github.com/repos/{Repo}/releases?per_page=30");
        if (releases is null || releases.Count == 0) { _log("Nenhum release encontrado."); return; }

        GhRelease? melhor = null;
        Version? melhorVersao = null;
        foreach (var r in releases)
        {
            var asset = r.Assets?.FirstOrDefault(a => a.Name.StartsWith(AssetPrefix, StringComparison.OrdinalIgnoreCase)
                && a.Name.EndsWith(".tar.gz", StringComparison.OrdinalIgnoreCase));
            if (asset is null) continue;
            if (!TentarParsearVersao(r.TagName, out var versao)) continue;
            if (melhorVersao is null || versao > melhorVersao) { melhorVersao = versao; melhor = r; }
        }
        if (melhor is null || melhorVersao is null) { _log("Nenhum release com asset Windows encontrado."); return; }

        var atual = LerVersaoAtual();
        if (atual is not null && melhorVersao <= atual) { _log($"Ja na ultima versao ({atual})."); return; }

        _log($"Atualizacao disponivel: {melhor.TagName} (atual: {atual?.ToString() ?? "nenhuma"}) -- baixando...");
        var asset2 = melhor.Assets!.First(a => a.Name.StartsWith(AssetPrefix, StringComparison.OrdinalIgnoreCase)
            && a.Name.EndsWith(".tar.gz", StringComparison.OrdinalIgnoreCase));
        var checksumsAsset = melhor.Assets!.FirstOrDefault(a => a.Name == "checksums.txt");

        var tmpDir = Path.Combine(Path.GetTempPath(), "atelie-update-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tmpDir);
        try
        {
            var tarPath = Path.Combine(tmpDir, asset2.Name);
            await using (var stream = await http.GetStreamAsync(asset2.BrowserDownloadUrl))
            await using (var file = File.Create(tarPath))
                await stream.CopyToAsync(file);

            if (checksumsAsset is not null)
            {
                var checksumsTxt = await http.GetStringAsync(checksumsAsset.BrowserDownloadUrl);
                var esperado = checksumsTxt.Split('\n')
                    .Select(l => l.Trim()).FirstOrDefault(l => l.EndsWith(asset2.Name))
                    ?.Split(' ', 2)[0];
                if (!string.IsNullOrWhiteSpace(esperado))
                {
                    var real = Convert.ToHexString(SHA256.HashData(await File.ReadAllBytesAsync(tarPath))).ToLowerInvariant();
                    if (!string.Equals(real, esperado.Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        _log($"Checksum nao confere (esperado {esperado}, obtido {real}) -- abortando atualizacao.");
                        return;
                    }
                }
            }

            var extractDir = Path.Combine(tmpDir, "extracted");
            Directory.CreateDirectory(extractDir);
            ExtrairTarGz(tarPath, extractDir);

            _log("Parando o backend pra aplicar a atualizacao...");
            await _pararBackend();
            await Task.Delay(1500);

            CopiarSobrescrevendo(extractDir, _backendDir);
            await File.WriteAllTextAsync(_versionFile, melhorVersao.ToString());
            _log($"Atualizacao {melhor.TagName} instalada com sucesso.");

            await _iniciarBackend();
        }
        finally
        {
            try { Directory.Delete(tmpDir, recursive: true); } catch { /* melhor esforco */ }
        }
    }

    private Version? LerVersaoAtual()
    {
        if (!File.Exists(_versionFile)) return null;
        return TentarParsearVersao(File.ReadAllText(_versionFile).Trim(), out var v) ? v : null;
    }

    private static bool TentarParsearVersao(string tag, out Version versao)
    {
        // Tags no formato "v1.1.15" ou "v1.1.15-windows" -- ignora sufixo apos '-'.
        var limpo = tag.TrimStart('v', 'V').Split('-')[0];
        return Version.TryParse(limpo, out versao!);
    }

    private static void CopiarSobrescrevendo(string origem, string destino)
    {
        Directory.CreateDirectory(destino);
        foreach (var arquivo in Directory.GetFiles(origem, "*", SearchOption.AllDirectories))
        {
            var relativo = Path.GetRelativePath(origem, arquivo);
            var alvo = Path.Combine(destino, relativo);
            Directory.CreateDirectory(Path.GetDirectoryName(alvo)!);
            File.Copy(arquivo, alvo, overwrite: true);
        }
    }

    private static void ExtrairTarGz(string tarGzPath, string destino)
    {
        using var fileStream = File.OpenRead(tarGzPath);
        using var gzip = new GZipStream(fileStream, CompressionMode.Decompress);
        Directory.CreateDirectory(destino);
        TarFile.ExtractToDirectory(gzip, destino, overwriteFiles: true);
    }

    private sealed record GhRelease(
        [property: JsonPropertyName("tag_name")] string TagName,
        [property: JsonPropertyName("assets")] List<GhAsset>? Assets);

    private sealed record GhAsset(
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("browser_download_url")] string BrowserDownloadUrl);
}
