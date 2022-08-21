using Microsoft.Extensions.Logging;
using System.Diagnostics;

string processToKill = args[0];
double maxLifetimeMinutes = double.Parse(args[1]);
double monitoringFrequencyMinutes = double.Parse(args[2]);

Console.WriteLine($"Every {monitoringFrequencyMinutes} minute(s) this monitoring app will kill the process \"{processToKill}\" if it has been running for at least {maxLifetimeMinutes} minute(s).");

using var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddConsole();
});

ILogger logger = loggerFactory.CreateLogger<Program>();

Task.Run(() => KillProcessIfApplicable());

Console.WriteLine("To stop this utility, press \"q\"");
string input = "";
while (input != "q")
    {
        input = Console.ReadLine();
    }
Environment.Exit(0);

async Task KillProcessIfApplicable()
{
    Process[] processCollection;
    while (true)
    {
        processCollection = Process.GetProcesses();

        foreach (Process p in processCollection)
        {
            if (p.ProcessName == processToKill && 
                DateTime.Now - p.StartTime > TimeSpan.FromMinutes(maxLifetimeMinutes))
            {
                p.Kill();
                logger.LogInformation($"{processToKill} has been running for at least {maxLifetimeMinutes} minute(s), so it was killed");
            }
        }
        int milliseconds = (int)(monitoringFrequencyMinutes * 60000);
        Task.Delay(milliseconds).Wait();
    }
}
