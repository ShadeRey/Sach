using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
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
using Newtonsoft.Json;
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
                var token = JsonConvert.DeserializeObject<string>(text);
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
        string validApiToken = ApiTextBox.Text;
        bool isValidToken = await ApiValidation("https://api.stratz.com/graphql", "Bearer " + validApiToken);
        if (isValidToken)
        {
            Console.WriteLine("Токен действителен.");
            App.ConnectApi(validApiToken);
            if (RememberMeCheckBox.IsChecked == true)
            {
                var serializeObject = JsonConvert.SerializeObject(validApiToken);
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

    private async Task<bool> ApiValidation(string apiUrl, string bearerToken)
    {
        try
        {
            using (HttpClient httpClient = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
                request.Headers.Add("Authorization", bearerToken);
                var response = await httpClient.SendAsync(request);
                Console.WriteLine(response.StatusCode.ToString());
                Console.WriteLine(await response.Content.ReadAsStringAsync());
                string responseBool = await response.Content.ReadAsStringAsync();
                if (responseBool ==
                    "{\"errors\":[{\"message\":\"GraphQL query is missing.\",\"extensions\":{\"code\":\"QUERY_MISSING\",\"codes\":[\"QUERY_MISSING\"]}}]}")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex}");
            return false;
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