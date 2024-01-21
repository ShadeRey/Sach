using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Media;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using ReactiveUI;
using Sach.Models;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace Sach.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public MainWindowViewModel()
    {
        OnHeroButtonClickCommand = ReactiveCommand.Create<short>(SetSelectedHeroId);
        PlayerHero = Application.Current.Resources["GradientBorder"] as IBrush;
    }

    private IStratzAPI _stratzApi = App.Services.GetRequiredService<IStratzAPI>();

    Dictionary<short, List<Vs>> _dict = new Dictionary<short, List<Vs>>();

    public async Task GetHeroStats()
    {
        var operationResult = await _stratzApi.HeroStatistics.ExecuteAsync(SelectedHero);
        if (operationResult.Data is null)
        {
            return;
        }

        var data = new
        {
            HeroId = SelectedHero,
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

    private short _selectedHero;

    public short SelectedHero
    {
        get => _selectedHero;
        set => this.RaiseAndSetIfChanged(ref _selectedHero, value);
    }

    public ReactiveCommand<short, Unit> OnHeroButtonClickCommand { get; set; }

    public async void SetSelectedHeroId(short id)
    {
        SelectedHero = id;
        await GetHeroStats();
    }

    private IBrush _playerHero;

    public IBrush PlayerHero
    {
        get => _playerHero;
        set => this.RaiseAndSetIfChanged(ref _playerHero, value);
    }

    private string _fish =
        "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";

    public string Fish
    {
        get => _fish;
    }
}