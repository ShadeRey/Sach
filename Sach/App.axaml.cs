using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Sach.Views;

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
        // Проверяем не запущен ли уже сервер
        if (Process.GetProcessesByName("SachServer").Length > 0)
        {
            Console.WriteLine("Сервер уже запущен.");
            return;
        }

        var serverPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SachServer", "SachServer.exe");
        Console.WriteLine($"Путь к серверу: {serverPath}");
        Console.WriteLine($"Файл существует: {File.Exists(serverPath)}");

        if (!File.Exists(serverPath)) return;

        Process.Start(new ProcessStartInfo
        {
            FileName = serverPath,
            UseShellExecute = false,
            CreateNoWindow = true,
        });

        var checkClient = new HttpClient();
        for (int i = 0; i < 30; i++)
        {
            try
            {
                await Task.Delay(1000);
                var result = await checkClient.GetAsync("http://localhost:5000");
                if (result.IsSuccessStatusCode)
                {
                    Console.WriteLine("Сервер готов!");
                    break;
                }
                Console.WriteLine($"Ожидание браузера... {i + 1}/30");
            }
            catch
            {
                Console.WriteLine($"Ожидание сервера... {i + 1}/30");
            }
        }
    }

    public static string? ApiToken;

    public static void ConnectApi(string apiToken)
    {
        ApiToken = apiToken;
    }
}