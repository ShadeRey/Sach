using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Threading;
using ReactiveUI;
using Sach.Models;
using Sach.ViewModels;

namespace Sach.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();

        mainColor = HeroSearchTextBox.Foreground;
    }

    public MainWindowViewModel ViewModel => (DataContext as MainWindowViewModel)!;

    private void InputElement_OnTapped(object? sender, TappedEventArgs e)
    {
        if (sender is not Canvas canvas)
        {
            return;
        }

        if (canvas.DataContext is not Hero hero)
        {
            return;
        }

        var selectingItemsControl = canvas.FindParentOfType<SelectingItemsControl>();
        if (selectingItemsControl is null)
        {
            return;
        }

        selectingItemsControl.SelectedItem = hero;
    }

    protected override async void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        if (File.Exists("apiToken"))
        {
            var text = await File.ReadAllTextAsync("apiToken", Encoding.UTF8);
            if (!string.IsNullOrEmpty(text))
            {
                var token = System.Text.Json.JsonSerializer.Deserialize<string>(text);
                ApiTextBox.Text = token;
            }
        }

        ViewModel.WhenAnyValue(x => x.TopHeroes)
            .DistinctUntilChanged()
            .WhereNotNull()
            .Subscribe(MarkHeroes);

        ConfigureButtonPicked();

        PlayersSelectingItemsControl.SelectedIndex = 0;
    }

    private void MarkHeroes(List<With> heroes)
    {
        foreach (var logical in this.GetLogicalDescendants())
        {
            if (logical is not HeroButtonView herobtn) continue;
            if (herobtn.Classes.Contains("suggestion"))
            {
                herobtn.Classes.Remove("suggestion");
            }

            if (heroes.All(x => x.HeroId2 != herobtn.HeroId))
            {
                continue;
            }

            herobtn.Classes.Add("suggestion");

            Console.WriteLine(herobtn.HeroId);
        }
    }

    private void ConfigureButtonPicked()
    {
        foreach (var logical in this.GetLogicalDescendants())
        {
            if (logical is not HeroButtonView herobtn) continue;
            herobtn.Click += (sender, args) =>
            {
                if (!herobtn.Classes.Contains("picked"))
                {
                    herobtn.Classes.Add("picked");
                }
            };
        }
    }

    private async void ConfirmButton_OnClick(object? sender, RoutedEventArgs e)
    {
        // ApiTextBox.Text может быть null если пользователь ничего не ввёл
        var validApiToken = ApiTextBox.Text;
    
        if (string.IsNullOrWhiteSpace(validApiToken))
        {
            Console.WriteLine("Токен пустой.");
            return;
        }

        ConfirmButton.IsEnabled = false; // блокируем повторный клик
        ConfirmButton.Content = "Проверка...";

        bool isValidToken = await ApiValidation(validApiToken);
    
        ConfirmButton.IsEnabled = true;
        ConfirmButton.Content = "ПРИНЯТЬ";

        if (isValidToken)
        {
            Console.WriteLine("Токен действителен.");
            App.ConnectApi(validApiToken);
            if (RememberMeCheckBox.IsChecked == true)
            {
                var serializeObject = System.Text.Json.JsonSerializer.Serialize(validApiToken);
                await File.WriteAllTextAsync("apiToken", serializeObject, Encoding.UTF8);
            }
            ApiValidationDialog.IsOpen = false;
            _isLogin = true;
        }
        else
        {
            Console.WriteLine("Токен недействителен.");
        }
    }

    private Task<bool> ApiValidation(string apiToken)
    {
        if (string.IsNullOrWhiteSpace(apiToken))
            return Task.FromResult(false);

        var parts = apiToken.Trim().Split('.');
        if (parts.Length != 3)
            return Task.FromResult(false);

        try
        {
            foreach (var part in parts)
            {
                var padded = part.PadRight(part.Length + (4 - part.Length % 4) % 4, '=')
                    .Replace('-', '+')
                    .Replace('_', '/');
                Convert.FromBase64String(padded);
            }

            var payload = parts[1];
            var padded2 = payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=')
                .Replace('-', '+')
                .Replace('_', '/');
            var json = Encoding.UTF8.GetString(Convert.FromBase64String(padded2));
            Console.WriteLine($"Payload токена: {json}");

            var doc = System.Text.Json.JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty("exp", out var exp))
            {
                var expTime = DateTimeOffset.FromUnixTimeSeconds(exp.GetInt64());
                if (expTime < DateTimeOffset.UtcNow)
                {
                    Console.WriteLine($"Токен истёк: {expTime}");
                    return Task.FromResult(false);
                }
                Console.WriteLine($"Токен действителен до: {expTime}");
            }

            return Task.FromResult(true);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    private void ExitButton_OnClick(object? sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private bool _isLogin = false;

    private float textTimer = 0;

    private async void InputElement_OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (!_isLogin)
            return;

        if ((e.Key >= Key.A && e.Key <= Key.Z) || e.Key == Key.Back || e.Key == Key.Space)
        {
            var search = e.Key;
            if (search == Key.Back)
            {
                if (HeroSearchTextBox.Text.Length > 0)
                    HeroSearchTextBox.Text = HeroSearchTextBox.Text.Substring(0, HeroSearchTextBox.Text.Length - 1);
            }
            else if (search == Key.Space)
            {
                HeroSearchTextBox.Text += " ";
            }
            else
            {
                HeroSearchTextBox.Text += search.ToString();
            }

            // Обновление визуальных эффектов
            Transitions transitions = new Transitions
            {
                new BrushTransition()
                {
                    Property = TextBox.ForegroundProperty,
                    Duration = TimeSpan.FromSeconds(0),
                }
            };
            HeroSearchTextBox.Transitions = transitions;

            if (TimerStarted)
            {
                ct.Cancel();
                ct = new CancellationTokenSource();
            }

            HeroSearchTextBox.Foreground = mainColor;

            Transitions transitions2 = new Transitions
            {
                new BrushTransition()
                {
                    Property = TextBox.ForegroundProperty,
                    Duration = TimeSpan.FromSeconds(1),
                }
            };
            HeroSearchTextBox.Transitions = transitions2;

            await StartTextTimer(ct.Token);
        }
    }

    private IBrush? mainColor;

    private CancellationTokenSource ct = new CancellationTokenSource();

    private bool TimerStarted = false;

    private async Task StartTextTimer(CancellationToken token)
    {
        try
        {
            await Task.Run(async () =>
            {
                TimerStarted = true;
                await Task.Delay(TimeSpan.FromSeconds(2), token);
                Dispatcher.UIThread.Post(() =>
                {
                    HeroSearchTextBox.Foreground = new SolidColorBrush(Colors.Transparent);
                });
            }, token).ConfigureAwait(false);
        }
        catch
        {
        }
    }

    private void HeroSearchTextBox_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (ViewModel.HeroesPreSearch == null)
        {
            ViewModel.HeroesPreSearch = ViewModel.AllHeroes;
        }

        var searchText = HeroSearchTextBox.Text?.Trim();

        if (string.IsNullOrEmpty(searchText))
        {
            foreach (var logical in this.GetLogicalDescendants())
            {
                if (logical is HeroButtonView btn)
                {
                    if (btn.Classes.Contains("searched"))
                    {
                        btn.Classes.Remove("searched");
                    }

                    if (btn.Classes.Contains("not_suitable"))
                    {
                        btn.Classes.Remove("not_suitable");
                    }
                }
            }

            return;
        }

        var searched = ViewModel.HeroesPreSearch
            .Where(it => it.HeroName.Contains(searchText))
            .ToList();

        foreach (var logical in this.GetLogicalDescendants())
        {
            if (logical is HeroButtonView herobtn)
            {
                if (searched.Any(s => s.HeroName.Contains(herobtn.HeroName)))
                {
                    if (!herobtn.Classes.Contains("searched"))
                    {
                        herobtn.Classes.Add("searched");
                    }

                    if (herobtn.Classes.Contains("not_suitable"))
                    {
                        herobtn.Classes.Remove("not_suitable");
                    }
                }
                else
                {
                    if (!herobtn.Classes.Contains("not_suitable"))
                    {
                        herobtn.Classes.Add("not_suitable");
                    }

                    if (herobtn.Classes.Contains("searched"))
                    {
                        herobtn.Classes.Remove("searched");
                    }
                }
            }
        }
    }
}

public static class ControlUtils
{
    public static T? FindParentOfType<T>(this StyledElement element)
    {
        var control = element;
        while (control != null)
        {
            if (control is T parentControl)
            {
                return parentControl;
            }

            control = control.Parent;
        }

        return default;
    }
}