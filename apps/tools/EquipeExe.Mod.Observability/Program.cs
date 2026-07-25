using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Runtime.InteropServices;

internal static class Program
{
    private static int Main(string[] args)
    {
        if (args.Length == 0 || args.Contains("--help", StringComparer.OrdinalIgnoreCase))
        {
            PrintHelp();
            return 0;
        }

        var command = args[0].ToLowerInvariant();
        var options = ParseOptions(args.Skip(1).ToArray());

        return command switch
        {
            "snapshot" => Snapshot(options),
            "monitor" => Monitor(options),
            _ => Unknown(command)
        };
    }

    private static int Snapshot(Dictionary<string, string> options)
    {
        var outDir = GetOption(options, "out", @"D:\AtelieProd\MOD\logs\observability");
        Directory.CreateDirectory(outDir);

        var stamp = DateTime.Now.ToString("yyyyMMdd-HHmmss", CultureInfo.InvariantCulture);
        var processCsv = Path.Combine(outDir, $"process-snapshot-{stamp}.csv");
        var netCsv = Path.Combine(outDir, $"network-snapshot-{stamp}.csv");
        var json = Path.Combine(outDir, $"snapshot-{stamp}.json");

        var processRows = Process.GetProcesses()
            .OrderBy(p => p.ProcessName)
            .Select(ProcessRow.TryCreate)
            .Where(p => p is not null)
            .Cast<ProcessRow>()
            .ToList();

        var networkRows = GetTcpRows();

        WriteCsv(processCsv, processRows);
        WriteCsv(netCsv, networkRows);
        WriteJson(json, new
        {
            GeneratedAt = DateTimeOffset.Now,
            Machine = Environment.MachineName,
            User = Environment.UserName,
            ProcessCsv = processCsv,
            NetworkCsv = netCsv,
            ProcessCount = processRows.Count,
            NetworkCount = networkRows.Count
        });

        Console.WriteLine($"Snapshot gravado: {json}");
        Console.WriteLine($"Processos: {processCsv}");
        Console.WriteLine($"Rede: {netCsv}");
        return 0;
    }

    private static int Monitor(Dictionary<string, string> options)
    {
        var exe = RequireOption(options, "exe");
        if (!File.Exists(exe))
        {
            Console.Error.WriteLine($"Executavel nao encontrado: {exe}");
            return 2;
        }

        var outDir = GetOption(options, "out", @"D:\AtelieProd\MOD\logs\observability");
        var seconds = int.Parse(GetOption(options, "seconds", "60"), CultureInfo.InvariantCulture);
        var intervalMs = int.Parse(GetOption(options, "interval-ms", "1000"), CultureInfo.InvariantCulture);
        var args = GetOption(options, "args", "");
        var cwd = GetOption(options, "cwd", Path.GetDirectoryName(exe) ?? Environment.CurrentDirectory);

        Directory.CreateDirectory(outDir);
        var stamp = DateTime.Now.ToString("yyyyMMdd-HHmmss", CultureInfo.InvariantCulture);
        var baseName = Path.GetFileNameWithoutExtension(exe);
        var samplesCsv = Path.Combine(outDir, $"{baseName}-samples-{stamp}.csv");
        var modulesCsv = Path.Combine(outDir, $"{baseName}-modules-{stamp}.csv");
        var childrenCsv = Path.Combine(outDir, $"{baseName}-children-{stamp}.csv");
        var netCsv = Path.Combine(outDir, $"{baseName}-network-{stamp}.csv");
        var summaryJson = Path.Combine(outDir, $"{baseName}-summary-{stamp}.json");

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = exe,
                Arguments = args,
                WorkingDirectory = cwd,
                UseShellExecute = false
            }
        };

        process.Start();
        var parentPid = process.Id;
        var startedAt = DateTimeOffset.Now;

        var samples = new List<SampleRow>();
        var childRows = new List<ChildProcessRow>();
        var networkRows = new List<TcpRow>();
        var moduleRows = new List<ModuleRow>();

        var sw = Stopwatch.StartNew();
        while (sw.Elapsed.TotalSeconds < seconds)
        {
            TryRefresh(process);

            if (process.HasExited)
            {
                break;
            }

            samples.Add(SampleRow.FromProcess(process, sw.Elapsed));

            var children = FindChildren(parentPid);
            foreach (var child in children)
            {
                childRows.Add(child);
            }

            networkRows.AddRange(GetTcpRows().Where(r => r.ProcessId == parentPid || children.Any(c => c.ProcessId == r.ProcessId)));

            Thread.Sleep(intervalMs);
        }

        moduleRows.AddRange(GetModules(parentPid));
        foreach (var child in childRows.Select(c => c.ProcessId).Distinct())
        {
            moduleRows.AddRange(GetModules(child));
        }

        var exited = process.HasExited;
        if (!exited && HasFlag(options, "close"))
        {
            TryClose(process);
            TryRefresh(process);
            exited = process.HasExited;
        }

        var exitCode = exited ? process.ExitCode : (int?)null;

        WriteCsv(samplesCsv, samples);
        WriteCsv(childrenCsv, childRows.DistinctBy(c => $"{c.ProcessId}:{c.SampleTime:O}").ToList());
        WriteCsv(netCsv, networkRows.DistinctBy(n => $"{n.ProcessId}:{n.LocalAddress}:{n.LocalPort}:{n.RemoteAddress}:{n.RemotePort}:{n.State}").ToList());
        WriteCsv(modulesCsv, moduleRows.DistinctBy(m => $"{m.ProcessId}:{m.ModuleName}:{m.FileName}").ToList());

        WriteJson(summaryJson, new
        {
            GeneratedAt = DateTimeOffset.Now,
            Executable = exe,
            Arguments = args,
            WorkingDirectory = cwd,
            ProcessId = parentPid,
            StartedAt = startedAt,
            DurationSeconds = sw.Elapsed.TotalSeconds,
            Exited = exited,
            ExitCode = exitCode,
            SampleCount = samples.Count,
            ChildObservationCount = childRows.Count,
            NetworkObservationCount = networkRows.Count,
            ModuleCount = moduleRows.Count,
            PeakWorkingSetBytes = samples.Count == 0 ? 0 : samples.Max(s => s.WorkingSetBytes),
            PeakPrivateMemoryBytes = samples.Count == 0 ? 0 : samples.Max(s => s.PrivateMemoryBytes),
            PeakThreadCount = samples.Count == 0 ? 0 : samples.Max(s => s.ThreadCount),
            PeakHandleCount = samples.Count == 0 ? 0 : samples.Max(s => s.HandleCount),
            Files = new { samplesCsv, childrenCsv, netCsv, modulesCsv }
        });

        Console.WriteLine($"Monitoramento gravado: {summaryJson}");
        return 0;
    }

    private static List<ModuleRow> GetModules(int pid)
    {
        var rows = new List<ModuleRow>();
        try
        {
            using var p = Process.GetProcessById(pid);
            foreach (ProcessModule module in p.Modules)
            {
                rows.Add(new ModuleRow(pid, p.ProcessName, module.ModuleName, module.FileName, module.BaseAddress.ToInt64(), module.ModuleMemorySize));
            }
        }
        catch
        {
            // Alguns processos protegidos ou finalizados nao permitem enumeracao de modulos.
        }

        return rows;
    }

    private static List<ChildProcessRow> FindChildren(int parentPid)
    {
        var rows = new List<ChildProcessRow>();
        var query = $"ParentProcessId={parentPid}";
        using var searcher = new System.Management.ManagementObjectSearcher("SELECT ProcessId,ParentProcessId,Name,ExecutablePath,CommandLine FROM Win32_Process WHERE " + query);
        foreach (System.Management.ManagementObject result in searcher.Get())
        {
            rows.Add(new ChildProcessRow(
                DateTimeOffset.Now,
                Convert.ToInt32(result["ProcessId"], CultureInfo.InvariantCulture),
                Convert.ToInt32(result["ParentProcessId"], CultureInfo.InvariantCulture),
                Convert.ToString(result["Name"], CultureInfo.InvariantCulture) ?? "",
                Convert.ToString(result["ExecutablePath"], CultureInfo.InvariantCulture) ?? "",
                Convert.ToString(result["CommandLine"], CultureInfo.InvariantCulture) ?? ""));
        }

        return rows;
    }

    private static List<TcpRow> GetTcpRows()
    {
        var rows = new List<TcpRow>();
        var bufferSize = 0;
        _ = GetExtendedTcpTable(IntPtr.Zero, ref bufferSize, true, 2, TcpTableClass.TCP_TABLE_OWNER_PID_ALL, 0);
        var buffer = Marshal.AllocHGlobal(bufferSize);
        try
        {
            var result = GetExtendedTcpTable(buffer, ref bufferSize, true, 2, TcpTableClass.TCP_TABLE_OWNER_PID_ALL, 0);
            if (result != 0)
            {
                return rows;
            }

            var rowCount = Marshal.ReadInt32(buffer);
            var rowPtr = IntPtr.Add(buffer, 4);
            var rowSize = Marshal.SizeOf<MibTcpRowOwnerPid>();
            var processNames = Process.GetProcesses().ToDictionary(p => p.Id, p => p.ProcessName);

            for (var i = 0; i < rowCount; i++)
            {
                var row = Marshal.PtrToStructure<MibTcpRowOwnerPid>(rowPtr);
                var localPort = DecodePort(row.LocalPort);
                var remotePort = DecodePort(row.RemotePort);
                processNames.TryGetValue((int)row.OwningPid, out var processName);

                rows.Add(new TcpRow(
                    DateTimeOffset.Now,
                    (int)row.OwningPid,
                    processName ?? "",
                    new IPAddress(row.LocalAddr).ToString(),
                    localPort,
                    new IPAddress(row.RemoteAddr).ToString(),
                    remotePort,
                    ((TcpState)row.State).ToString()));

                rowPtr = IntPtr.Add(rowPtr, rowSize);
            }
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }

        return rows;
    }

    private static void TryRefresh(Process process)
    {
        try { process.Refresh(); } catch { }
    }

    private static void TryClose(Process process)
    {
        try
        {
            if (process.CloseMainWindow())
            {
                if (!process.WaitForExit(5000))
                {
                    process.Kill(entireProcessTree: false);
                }
            }
        }
        catch
        {
            // Monitoramento nao deve falhar por erro de encerramento.
        }
    }

    private static Dictionary<string, string> ParseOptions(string[] args)
    {
        var options = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        for (var i = 0; i < args.Length; i++)
        {
            var arg = args[i];
            if (!arg.StartsWith("--", StringComparison.Ordinal)) { continue; }
            var key = arg[2..];
            if (i + 1 < args.Length && !args[i + 1].StartsWith("--", StringComparison.Ordinal))
            {
                options[key] = args[++i];
            }
            else
            {
                options[key] = "true";
            }
        }

        return options;
    }

    private static string GetOption(Dictionary<string, string> options, string key, string fallback)
        => options.TryGetValue(key, out var value) ? value : fallback;

    private static string RequireOption(Dictionary<string, string> options, string key)
    {
        if (options.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        throw new ArgumentException($"Parametro obrigatorio ausente: --{key}");
    }

    private static bool HasFlag(Dictionary<string, string> options, string key)
        => options.TryGetValue(key, out var value) && value.Equals("true", StringComparison.OrdinalIgnoreCase);

    private static int DecodePort(uint encodedPort)
        => (int)(((encodedPort & 0xFF) << 8) + ((encodedPort >> 8) & 0xFF));

    private static void WriteJson(string path, object value)
    {
        var json = JsonSerializer.Serialize(value, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(path, json, Encoding.UTF8);
    }

    private static void WriteCsv<T>(string path, IReadOnlyCollection<T> rows)
    {
        var properties = typeof(T).GetProperties();
        using var writer = new StreamWriter(path, false, new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
        writer.WriteLine(string.Join(",", properties.Select(p => Csv(p.Name))));
        foreach (var row in rows)
        {
            writer.WriteLine(string.Join(",", properties.Select(p => Csv(Convert.ToString(p.GetValue(row), CultureInfo.InvariantCulture) ?? ""))));
        }
    }

    private static string Csv(string value)
        => "\"" + value.Replace("\"", "\"\"", StringComparison.Ordinal) + "\"";

    private static int Unknown(string command)
    {
        Console.Error.WriteLine($"Comando desconhecido: {command}");
        PrintHelp();
        return 1;
    }

    private static void PrintHelp()
    {
        Console.WriteLine("EquipeExe.Mod.Observability");
        Console.WriteLine("snapshot --out <dir>");
        Console.WriteLine("monitor --exe <path> --seconds 60 --interval-ms 1000 --out <dir> [--cwd <dir>] [--args <args>] [--close]");
    }

    [DllImport("iphlpapi.dll", SetLastError = true)]
    private static extern uint GetExtendedTcpTable(
        IntPtr pTcpTable,
        ref int dwOutBufLen,
        bool sort,
        int ipVersion,
        TcpTableClass tblClass,
        uint reserved);
}

internal enum TcpTableClass
{
    TCP_TABLE_BASIC_LISTENER,
    TCP_TABLE_BASIC_CONNECTIONS,
    TCP_TABLE_BASIC_ALL,
    TCP_TABLE_OWNER_PID_LISTENER,
    TCP_TABLE_OWNER_PID_CONNECTIONS,
    TCP_TABLE_OWNER_PID_ALL,
    TCP_TABLE_OWNER_MODULE_LISTENER,
    TCP_TABLE_OWNER_MODULE_CONNECTIONS,
    TCP_TABLE_OWNER_MODULE_ALL
}

internal enum TcpState
{
    Closed = 1,
    Listen = 2,
    SynSent = 3,
    SynReceived = 4,
    Established = 5,
    FinWait1 = 6,
    FinWait2 = 7,
    CloseWait = 8,
    Closing = 9,
    LastAck = 10,
    TimeWait = 11,
    DeleteTcb = 12
}

[StructLayout(LayoutKind.Sequential)]
internal struct MibTcpRowOwnerPid
{
    public uint State;
    public uint LocalAddr;
    public uint LocalPort;
    public uint RemoteAddr;
    public uint RemotePort;
    public uint OwningPid;
}

internal sealed record ProcessRow(
    DateTimeOffset SampleTime,
    int ProcessId,
    string ProcessName,
    string FileName,
    long WorkingSetBytes,
    long PrivateMemoryBytes,
    int ThreadCount,
    int HandleCount)
{
    public static ProcessRow? TryCreate(Process process)
    {
        try
        {
            return new ProcessRow(
                DateTimeOffset.Now,
                process.Id,
                process.ProcessName,
                TryGetFileName(process),
                process.WorkingSet64,
                process.PrivateMemorySize64,
                process.Threads.Count,
                process.HandleCount);
        }
        catch
        {
            return null;
        }
    }

    private static string TryGetFileName(Process process)
    {
        try { return process.MainModule?.FileName ?? ""; }
        catch { return ""; }
    }
}

internal sealed record SampleRow(
    DateTimeOffset SampleTime,
    double ElapsedSeconds,
    int ProcessId,
    string ProcessName,
    long WorkingSetBytes,
    long PrivateMemoryBytes,
    long VirtualMemoryBytes,
    int ThreadCount,
    int HandleCount,
    double TotalProcessorSeconds)
{
    public static SampleRow FromProcess(Process process, TimeSpan elapsed)
        => new(
            DateTimeOffset.Now,
            elapsed.TotalSeconds,
            process.Id,
            process.ProcessName,
            process.WorkingSet64,
            process.PrivateMemorySize64,
            process.VirtualMemorySize64,
            process.Threads.Count,
            process.HandleCount,
            process.TotalProcessorTime.TotalSeconds);
}

internal sealed record ChildProcessRow(
    DateTimeOffset SampleTime,
    int ProcessId,
    int ParentProcessId,
    string Name,
    string ExecutablePath,
    string CommandLine);

internal sealed record ModuleRow(
    int ProcessId,
    string ProcessName,
    string ModuleName,
    string FileName,
    long BaseAddress,
    int ModuleMemorySize);

internal sealed record TcpRow(
    DateTimeOffset SampleTime,
    int ProcessId,
    string ProcessName,
    string LocalAddress,
    int LocalPort,
    string RemoteAddress,
    int RemotePort,
    string State);
