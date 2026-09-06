using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Sach.Views;
using Sach.Services;
using Serilog;

namespace Sach;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
            
            desktop.ShutdownRequested += (_, _) =>
            {
                foreach (var proc in Process.GetProcessesByName("SachServer"))
                {
                    proc.Kill();
                }
            };
        }

        base.OnFrameworkInitializationCompleted();

        // Запускаем сервер в фоне
        _ = StartServerAsync();
    }

    private async Task StartServerAsync()
    {
        if (Process.GetProcessesByName("SachServer").Length > 0)
        {
            LoggingService.Logger.Information("Server already running");
            return;
        }

        var possiblePaths = new[]
        {
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SachServer", "SachServer.exe"),
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "SachServer", "bin", "Debug", "net7.0", "SachServer.exe"),
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "SachServer", "bin", "Debug", "net7.0", "SachServer.exe"),
            @"D:\Claude\Sach\SachServer\bin\Debug\net7.0\SachServer.exe"
        };

        string? serverPath = null;
        foreach (var path in possiblePaths)
        {
            LoggingService.Logger.Debug("Checking: {Path}", path);
            if (File.Exists(path))
            {
                serverPath = path;
                LoggingService.Logger.Information("Found server at: {ServerPath}", serverPath);
                break;
            }
        }

        if (serverPath == null)
        {
            LoggingService.Logger.Warning("SachServer.exe not found in any expected location");
            return;
        }

        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = serverPath,
                UseShellExecute = false,
                CreateNoWindow = true,
            });

            var checkClient = new HttpClient();
            for (int i = 0; i < 60; i++)
            {
                try
                {
                    await Task.Delay(1000);
                    var result = await checkClient.GetAsync("http://localhost:5000");
                    if (result.IsSuccessStatusCode)
                    {
                        LoggingService.Logger.Information("Server ready");
                        break;
                    }
                    LoggingService.Logger.Debug("Waiting for browser... {Attempt}/60", i + 1);
                }
                catch
                {
                    LoggingService.Logger.Debug("Waiting for server... {Attempt}/60", i + 1);
                }
            }
        }
        catch (Exception ex)
        {
            LoggingService.Logger.Error(ex, "Failed to start server");
        }
    }

    public static string? ApiToken;

    public static void ConnectApi(string apiToken)
    {
        ApiToken = apiToken;
    }
}