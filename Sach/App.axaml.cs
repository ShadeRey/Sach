using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Sach.ViewModels;
using Sach.Views;

namespace Sach;

public class App : Application
{
    public static IServiceProvider? Services;
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    public static void ConnectApi(string apiToken)
    {
        var servicesCollection = new ServiceCollection();
        servicesCollection.AddStratzAPI()
            .ConfigureHttpClient(
                client =>
                {
                    client.BaseAddress = new Uri("https://api.stratz.com/graphql");
                    client.DefaultRequestHeaders.Add(
                        "Authorization", "Bearer " + apiToken
                    );
                }
            );

        IServiceProvider services = servicesCollection.BuildServiceProvider();
        Services = services;
    }
}