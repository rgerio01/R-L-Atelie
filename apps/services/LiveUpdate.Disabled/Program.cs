using System.Text.Json;

var modRoot = Environment.GetEnvironmentVariable("EQUIPEEXE_MOD_ROOT") ?? @"D:\AtelieProd\MOD";
var logDirectory = Path.Combine(modRoot, "logs", "communication");
Directory.CreateDirectory(logDirectory);

var record = new
{
    timestamp = DateTimeOffset.Now,
    component = "LiveUpdate.Disabled",
    status = "blocked",
    message = "Atualizacao automatica bloqueada por politica do EquipeExe MOD.",
    currentDirectory = Environment.CurrentDirectory,
    arguments = args,
    machine = Environment.MachineName,
    user = Environment.UserName
};

var logPath = Path.Combine(logDirectory, $"liveupdate-blocked-{DateTime.Now:yyyyMMdd}.jsonl");
File.AppendAllText(logPath, JsonSerializer.Serialize(record) + Environment.NewLine);

Console.WriteLine("EquipeExe MOD: atualizacao automatica bloqueada por politica interna.");
return 0;
