using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using ReactiveUI;
using Sach.Models;
using Sach.Views;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace Sach.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public AvaloniaList<Hero> Heroes
    {
        get => _heroes;
        set => this.RaiseAndSetIfChanged(ref _heroes, value);
    }

    public MainWindowViewModel()
    {
        OnHeroButtonClickCommand = ReactiveCommand.Create<Hero>(SetSelectedHeroId);
        OnConfirmApiKey = ReactiveCommand.Create<Tuple<string, string>>(tuple =>
        {
            ApiValidation(tuple.Item1, tuple.Item2);
        });
    }

    private IStratzAPI _stratzApi = App.Services.GetRequiredService<IStratzAPI>();

    Dictionary<short, List<Vs>> _dict = new Dictionary<short, List<Vs>>();

    public async Task GetHeroStats()
    {
        var operationResult = await _stratzApi.HeroStatistics.ExecuteAsync(SelectedHero.HeroId);
        if (operationResult.Data is null)
        {
            return;
        }

        var data = new
        {
            HeroId = SelectedHero.HeroId,
            Advantage = operationResult.Data.HeroStats?.HeroVsHeroMatchup?.Advantage?[0]
        };

        await WriteObjectToFileJson(data, $"hero{SelectedHero}.json");

        _dict.TryAdd(
            data.HeroId,
            data.Advantage?.Vs?.Select(
                x => new Vs(x)
            ).ToList()
        );

        if (_dict.Count == 5)
        {
            // Типо заполнился
            await WriteObjectToFileJson(_dict, "heroDict.json");
            await ReadHeroStats(_dict);
            _dict.Clear();
        }
    }

    public async Task WriteObjectToFileJson(object? o, String filePath)
    {
        var serializer = new JsonSerializer
        {
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.Indented,
        };

        await using var file = File.Create(filePath);
        await using var sw = new StreamWriter(file, Encoding.UTF8);
        await using JsonWriter writer = new JsonTextWriter(sw);

        serializer.Serialize(writer, o);
    }

    public async Task ReadHeroStats(Dictionary<short, List<Vs>> data)
    {
        IEnumerable<Vs> teamVsCombined = data.SelectMany(x => x.Value)
            .Where(x => !_dict.Keys.Contains(x.HeroId2));
        var teamChancesVsAnyHero = teamVsCombined.GroupBy(x => x.HeroId2)
            .Select(x => new
            {
                HeroId2 = x.Key,
                Avg = x.Average(x => x.Synergy)
            }).OrderByDescending(x => x.Avg).Take(5);
    }

    private Hero? _selectedHero;

    public Hero? SelectedHero
    {
        get => _selectedHero;
        set => this.RaiseAndSetIfChanged(ref _selectedHero, value);
    }

    public ReactiveCommand<Hero, Unit> OnHeroButtonClickCommand { get; set; }

    public async void SetSelectedHeroId(Hero hero)
    {
        if (SelectedHero is null)
        {
            return;
        }
        SelectedHero.HeroId = hero.HeroId;
        SelectedHero.HeroName = hero.HeroName;
        SelectedHero.HeroIconPath = hero.HeroIconPath;
        await GetHeroStats();
    }

    private IBrush _playerHero;
    
    private string _fish =
        "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";

    private AvaloniaList<Hero> _heroes = ListInit();

    private static AvaloniaList<Hero> ListInit()
    {
        var list = new AvaloniaList<Hero>();
        for (int i = 0; i < 5; i++)
        {
            list.Add(new Hero()
            {
                CurrentTeam = Hero.Team.Ally
            });
        }

        for (int i = 0; i < 5; i++)
        {
            list.Add(new Hero()
            {
                CurrentTeam = Hero.Team.Enemy
            });
        }
        return list;
    }

    public string Fish
    {
        get => _fish;
    }

    private readonly HttpClient _httpClient;

    public ReactiveCommand<Tuple<string, string>, Unit> OnConfirmApiKey { get; set; }
    public async Task<bool> ApiValidation(string apiUrl, string bearerToken)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex}");
            return false;
        }
    }
    
    
}