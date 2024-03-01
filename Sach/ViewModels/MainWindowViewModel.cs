using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Collections;
using Avalonia.Media;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using ReactiveUI;
using Sach.Models;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace Sach.ViewModels;

public class MainWindowViewModel : ViewModelBase {
    private AvaloniaList<Hero> _heroesPreSearch;

    public AvaloniaList<Hero> HeroesPreSearch {
        get => _heroesPreSearch;
        set => this.RaiseAndSetIfChanged(ref _heroesPreSearch, value);
    }
    public AvaloniaList<Hero> Heroes
    {
        get => _heroes;
        set => this.RaiseAndSetIfChanged(ref _heroes, value);
    }

    public MainWindowViewModel()
    {
        OnHeroButtonClickCommand = ReactiveCommand.Create<Hero>(SetSelectedHeroId);
        OpenUrlCommand = ReactiveCommand.Create<string>(OpenUrl);
    }


    private IStratzAPI? _stratzApi => App.Services?.GetRequiredService<IStratzAPI>();

    Dictionary<short, List<Vs>> _dict = new Dictionary<short, List<Vs>>();

    private async Task GetHeroStats()
    {
        var operationResult = await _stratzApi.HeroStatistics.ExecuteAsync(SelectedHero.HeroId);
        if (operationResult.Data is null)
        {
            return;
        }

        // Определение статистики с учетом выбранной команды
        var data = new
        {
            HeroId = SelectedHero.HeroId,
            Stats = SelectedHero.IsAlly
                ? operationResult.Data.HeroStats?.HeroVsHeroMatchup?.Advantage?.FirstOrDefault()?.With.ToList()
                : SelectedHero.IsEnemy
                    ? operationResult.Data.HeroStats?.HeroVsHeroMatchup?.Advantage?.FirstOrDefault()?.Vs
                        .Select(x => x.ToWith()).ToList()
                    : null
        };

        // Обработка статистики и сортировка
        if (data.Stats != null && data.Stats.Any())
        {
            var sortedStats = data.Stats
                .OrderByDescending(x => x.Synergy) // Сортировка по синергии
                .ToList();

            // Выполните дополнительные действия с отсортированными данными, если это необходимо

            await WriteObjectToFileJson(sortedStats, $"sorted_stats_{SelectedHero.HeroId}.json");


            Top10Heroes = await UpdateTop10Heroes(); // GetTop10HeroesFromJson("top_10_heroes.json");

            // Очистите данные после обработки
            _dict.Clear();
        }
    }

    public List<With> Top10Heroes
    {
        get => _top10Heroes;
        set => this.RaiseAndSetIfChanged(ref _top10Heroes, value);
    }

    private async Task<List<With>> UpdateTop10Heroes()
    {
        var top10Heroes = await GetTop10Heroes();
        await WriteObjectToFileJson(top10Heroes, "top_10_heroes.json");
        return top10Heroes;
    }

    private async Task WriteObjectToFileJson(object? o, String filePath)
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

    private async Task<List<With>> GetTop10Heroes()
    {
        var selectedAllyHeroes = Heroes.Where(x => x.IsAlly).ToList();
        var selectedEnemyHeroes = Heroes.Where(x => x.IsEnemy).ToList();

        var top10Heroes = new List<With>();

        // Получение статистики для героев союзной команды
        var allyHeroesStats = await GetHeroesStats(selectedAllyHeroes);

        // Получение статистики для героев вражеской команды
        var enemyHeroesStats = await GetHeroesStats(selectedEnemyHeroes);

        // Объединение статистики для героев обеих команд
        var allHeroesStats = allyHeroesStats.Concat(enemyHeroesStats).ToList();

        // Сортировка статистики по синергии или противодействию
        var sortedStats = allHeroesStats
            .OrderByDescending(x => x.Synergy) // Сортировка по синергии
            .ToList();

        // Выбор 10 наилучших героев
        var selectedAllyHeroIds = selectedAllyHeroes.Select(x => x.HeroId);
        var selectedEnemyHeroIds = selectedEnemyHeroes.Select(x => x.HeroId);
        var selectedHeroIds = selectedAllyHeroIds.Concat(selectedEnemyHeroIds);
        top10Heroes = sortedStats.Where(x => !selectedHeroIds.Contains(x.HeroId2)).Take(5).ToList();

        return top10Heroes;
    }

    private async Task<List<With>> GetHeroesStats(List<Hero> heroes)
    {
        var heroesStats = new List<With>();

        foreach (var hero in heroes)
        {
            var operationResult = await _stratzApi.HeroStatistics.ExecuteAsync(hero.HeroId);
            if (operationResult.Data is not null)
            {
                var stats = hero.IsAlly
                    ? operationResult.Data.HeroStats?.HeroVsHeroMatchup?.Advantage?.FirstOrDefault()?.With.ToList()
                    : hero.IsEnemy
                        ? operationResult.Data.HeroStats?.HeroVsHeroMatchup?.Advantage?.FirstOrDefault()?.Vs
                            .Select(x => x.ToWith()).ToList()
                        : null;

                if (stats != null && stats.Any())
                {
                    heroesStats.AddRange(stats.Select(x => new With(x)));
                }
            }
        }

        return heroesStats;
    }

    private Hero? _selectedHero;

    public Hero? SelectedHero
    {
        get => _selectedHero;
        set => this.RaiseAndSetIfChanged(ref _selectedHero, value);
    }

    public ReactiveCommand<Hero, Unit> OnHeroButtonClickCommand { get; set; }
    public ReactiveCommand<string, Unit> OpenUrlCommand { get; set; }

    private async void SetSelectedHeroId(Hero hero)
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

    private AvaloniaList<Hero> _heroes = ListInit();
    private List<With> _top10Heroes;

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

    private void OpenUrl(object urlObj)
    {
        var url = urlObj as string;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            using var proc = new Process { StartInfo = { UseShellExecute = true, FileName = url } };
            proc.Start();

            return;
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            Process.Start("xdg-open", url);
            return;
        }

        if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) throw new ArgumentException("invalid url: " + url);
        Process.Start("open", url);
        return;
    }
}