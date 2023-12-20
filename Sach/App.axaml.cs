using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Sach.ViewModels;
using Sach.Views;

namespace Sach;

public partial class App : Application
{
    public static IServiceProvider Services;
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var servicesCollection = new ServiceCollection();
            servicesCollection.AddStratzAPI()
                .ConfigureHttpClient(
                    client =>
                    {
                        client.BaseAddress = new Uri("https://api.stratz.com/graphql");
                        client.DefaultRequestHeaders.Add(
                            "Authorization", "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiJodHRwczovL3N0ZWFtY29tbXVuaXR5LmNvbS9vcGVuaWQvaWQvNzY1NjExOTg5ODUxODc3ODkiLCJ1bmlxdWVfbmFtZSI6IkwnZW5mZXIgYydlc3QgbGVzIGF1dHJlcyIsIlN1YmplY3QiOiJiYjhjMDVlNS01YjVlLTQxMmUtYmQxNy1iYTIwZjg5NTdmYmUiLCJTdGVhbUlkIjoiMTAyNDkyMjA2MSIsIm5iZiI6MTY4NDA1Mzg2MiwiZXhwIjoxNzE1NTg5ODYyLCJpYXQiOjE2ODQwNTM4NjIsImlzcyI6Imh0dHBzOi8vYXBpLnN0cmF0ei5jb20ifQ.FCa8qAln_-Lm0ldUVpWzdls2wmOTwwCg257XJPUaEU0"
                        );
                    }
                );

            IServiceProvider services = servicesCollection.BuildServiceProvider();
            Services = services;
            
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}