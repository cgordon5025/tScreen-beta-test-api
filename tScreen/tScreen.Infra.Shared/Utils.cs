using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading;

namespace Shared;

public static class Utils
{
    public static T? ReadJsonFile<T>(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException(filePath);

        using var fileStream = File.OpenRead(filePath);
        return JsonSerializer.Deserialize<T>(fileStream, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
    }

    public static void WaitForDebuggerIfNeeded(string processName, string envVarName)
    {
        bool.TryParse(Environment.GetEnvironmentVariable(envVarName), out var debug);
        if (!debug || !Pulumi.Deployment.Instance.IsDryRun) return;

        Console.WriteLine($"{processName} in debug mode: waiting debugger to be attached (pid: {Environment.ProcessId}");
        while (!Debugger.IsAttached)
        {
            Thread.Sleep(100);
        }
    }

    public static string ToAzureStorageCompliantName(this string name) => name.Replace(" ", "");

    public static string ToSlotUrlWithEnvironment(this string url, string environment, string? location = null)
    {
        var parts = url.Split('.');
        parts[0] = location == null ? $"{parts[0]}-{environment}" : $"{parts[0]}-{environment}.{location}";
        return string.Join(".", parts);
    }
}