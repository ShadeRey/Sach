using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reactive;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Collections;
using Avalonia.Media;
using ReactiveUI;
using Sach.Models;
using Sach.Views;
using Sach.Services;
using Serilog;

namespace Sach.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private static readonly IConfigurationService _configService = new ConfigurationService();

    public MainWindowViewModel()
    {
        OnHeroButtonClickCommand = ReactiveCommand.Create<Hero>(SetSelectedHeroId);
        OpenUrlCommand = ReactiveCommand.Create<string>(OpenUrl);
    }

    public static TimeSpan opacityTime { get; set; } = TimeSpan.FromSeconds(_configService.OpacityAnimationSeconds);

    private AvaloniaList<HeroButtonView> _heroesPreSearch;

    public AvaloniaList<HeroButtonView> HeroesPreSearch
    {
        get => _heroesPreSearch;
        set => this.RaiseAndSetIfChanged(ref _heroesPreSearch, value);
    }

    private AvaloniaList<HeroButtonView> _allHeroes = SetAllHeroes();

    public AvaloniaList<HeroButtonView> AllHeroes
    {
        get => _allHeroes;
        set => this.RaiseAndSetIfChanged(ref _allHeroes, value);
    }

    private static AvaloniaList<HeroButtonView> SetAllHeroes()
    {
        AvaloniaList<HeroButtonView> allHeroes = new AvaloniaList<HeroButtonView>();

        try
        {
            var heroesPath = Path.Combine(AppContext.BaseDirectory, "heroes.json");
            if (!File.Exists(heroesPath))
            {
                LoggingService.Logger.Warning("Heroes file not found at {Path}", heroesPath);
                return allHeroes;
            }

            var json = File.ReadAllText(heroesPath);
            var heroList = System.Text.Json.JsonSerializer.Deserialize<List<HeroData>>(json);

            if (heroList != null)
            {
                foreach (var hero in heroList)
                {
                    allHeroes.Add(new HeroButtonView
                    {
                        HeroId = hero.HeroId,
                        HeroName = hero.HeroName,
                        HeroIcon = hero.HeroIcon
                    });
                }
                LoggingService.Logger.Information("Loaded {HeroCount} heroes from heroes.json", allHeroes.Count);
            }
        }
        catch (Exception ex)
        {
            LoggingService.Logger.Error(ex, "Error loading heroes from JSON");
        }

        return allHeroes;
    }

    private class HeroData
    {
        public short HeroId { get; set; }
        public string HeroName { get; set; }
        public string HeroIcon { get; set; }
    }


    private AvaloniaList<Hero> _playerHeroes = ListInit();

    public AvaloniaList<Hero> PlayerHeroes
    {
        get => _playerHeroes;
        set => this.RaiseAndSetIfChanged(ref _playerHeroes, value);
    }

    Dictionary<short, List<Vs>> _dict = new Dictionary<short, List<Vs>>();

    private bool _isVisibleLoading;

    public bool IsVisibleLoading
    {
        get => _isVisibleLoading;
        set => this.RaiseAndSetIfChanged(ref _isVisibleLoading, value);
    }

    private bool _isEnabledHeroView = true;

    public bool IsEnabledHeroView
    {
        get => _isEnabledHeroView;
        set => this.RaiseAndSetIfChanged(ref _isEnabledHeroView, value);
    }

    public async Task GetHeroStats()
    {
        if (SelectedHero is null) return;

        IsVisibleLoading = true;
        IsEnabledHeroView = false;

        try
        {
            HttpResponseMessage? response = null;

            for (int attempt = 0; attempt < 5; attempt++)
            {
                try
                {
                    var request = new HttpRequestMessage(HttpMethod.Post,
                        $"http://localhost:5000/hero/{SelectedHero.HeroId}");
                    request.Headers.Add("X-Api-Token", App.ApiToken);
                    response = await _httpClient.SendAsync(request);
                    break;
                }
                catch (HttpRequestException)
                {
                    if (attempt == 4) throw;
                    LoggingService.Logger.Debug("Server not ready, attempt {Attempt}/5", attempt + 1);
                    await Task.Delay(20000);
                }
            }

            if (response is null) return;

            var body = await response.Content.ReadAsStringAsync();
            LoggingService.Logger.Information("API Response Status: {StatusCode}", response.StatusCode);

            if (!response.IsSuccessStatusCode) return;

            var json = System.Text.Json.JsonDocument.Parse(body);
            LoggingService.Logger.Debug("API Response body: {Body}", body[..Math.Min(500, body.Length)]);

            if (!json.RootElement.TryGetProperty("data", out var dataElement))
            {
                LoggingService.Logger.Warning("No data in API response for hero {HeroId}. Root keys: {Keys}",
                    SelectedHero.HeroId,
                    string.Join(", ", json.RootElement.EnumerateObject().Select(p => p.Name)));
                return;
            }

            if (!dataElement.TryGetProperty("heroStats", out var heroStatsElement) ||
                !heroStatsElement.TryGetProperty("heroVsHeroMatchup", out var matchupElement) ||
                !matchupElement.TryGetProperty("advantage", out var advantageArray))
            {
                LoggingService.Logger.Warning("Missing matchup structure for hero {HeroId}", SelectedHero.HeroId);
                return;
            }

            if (advantageArray.GetArrayLength() == 0)
            {
                LoggingService.Logger.Debug("No matchup data available for hero {HeroId}", SelectedHero.HeroId);
                return;
            }

            var advantage = advantageArray[0];

            List<With> stats = new();
            if (SelectedHero.IsAlly && advantage.TryGetProperty("with", out var withElement))
            {
                stats = withElement.EnumerateArray()
                    .Select(x => new With
                    {
                        HeroId2 = x.GetProperty("heroId2").GetInt16(),
                        Synergy = x.GetProperty("synergy").GetDecimal()
                    }).ToList();
            }
            else if (SelectedHero.IsEnemy && advantage.TryGetProperty("vs", out var vsElement))
            {
                stats = vsElement.EnumerateArray()
                    .Select(x => new With
                    {
                        HeroId2 = x.GetProperty("heroId2").GetInt16(),
                        Synergy = -x.GetProperty("synergy").GetDecimal()
                    }).ToList();
            }
            else
            {
                LoggingService.Logger.Warning("Hero is neither ally nor enemy: {HeroId}", SelectedHero.HeroId);
                return;
            }

            if (stats.Any())
            {
                var sortedStats = stats.OrderByDescending(x => x.Synergy).ToList();
                await WriteObjectToFileJson(sortedStats, $"sorted_stats_{SelectedHero.HeroId}.json");

                TopHeroes = GetTopHeroesForSelected(sortedStats);
                LoggingService.Logger.Information("Displayed top {Count} suggestions for {HeroName}",
                    TopHeroes.Count, SelectedHero.HeroName);
                _dict.Clear();
            }
            else
            {
                LoggingService.Logger.Warning("No stats found for hero {HeroId}", SelectedHero.HeroId);
            }
        }
        catch (Exception ex)
        {
            LoggingService.Logger.Error(ex, "Error fetching hero stats for hero {HeroId}", SelectedHero?.HeroId);
        }
        finally
        {
            IsVisibleLoading = false;
            IsEnabledHeroView = true;
        }
    }

    private async Task<List<With>> GetHeroesStats(List<Hero> heroes)
    {
        var heroesStats = new List<With>();

        foreach (var hero in heroes)
        {
            if (hero.HeroId <= 0)
            {
                LoggingService.Logger.Debug("Skipping invalid hero ID {HeroId}", hero.HeroId);
                continue;
            }

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post,
                    $"http://localhost:5000/hero/{hero.HeroId}");
                request.Headers.Add("X-Api-Token", App.ApiToken);

                var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    LoggingService.Logger.Debug("API error for hero {HeroId}: {StatusCode}", hero.HeroId, response.StatusCode);
                    continue;
                }

                var body = await response.Content.ReadAsStringAsync();
                var json = System.Text.Json.JsonDocument.Parse(body);

                if (!json.RootElement.TryGetProperty("data", out var dataElement))
                {
                    LoggingService.Logger.Debug("No data in API response for hero {HeroId}. Response: {Response}",
                        hero.HeroId, body[..Math.Min(200, body.Length)]);
                    continue;
                }

                if (!dataElement.TryGetProperty("heroStats", out var heroStatsElement) ||
                    !heroStatsElement.TryGetProperty("heroVsHeroMatchup", out var matchupElement) ||
                    !matchupElement.TryGetProperty("advantage", out var advantageArray))
                {
                    LoggingService.Logger.Debug("Missing matchup data for hero {HeroId}", hero.HeroId);
                    continue;
                }

                if (advantageArray.GetArrayLength() == 0)
                {
                    LoggingService.Logger.Debug("Empty advantage array for hero {HeroId}", hero.HeroId);
                    continue;
                }

                var advantage = advantageArray[0];

                List<With> stats = new();
                if (hero.IsAlly && advantage.TryGetProperty("with", out var withElement))
                {
                    stats = withElement.EnumerateArray()
                        .Select(x => new With
                        {
                            HeroId2 = x.GetProperty("heroId2").GetInt16(),
                            Synergy = x.GetProperty("synergy").GetDecimal()
                        }).ToList();
                }
                else if (hero.IsEnemy && advantage.TryGetProperty("vs", out var vsElement))
                {
                    stats = vsElement.EnumerateArray()
                        .Select(x => new With
                        {
                            HeroId2 = x.GetProperty("heroId2").GetInt16(),
                            Synergy = -x.GetProperty("synergy").GetDecimal()
                        }).ToList();
                }

                if (stats.Any())
                {
                    heroesStats.AddRange(stats);
                    LoggingService.Logger.Debug("Fetched {StatCount} stats for hero {HeroId}", stats.Count, hero.HeroId);
                }
            }
            catch (Exception ex)
            {
                LoggingService.Logger.Error(ex, "Error fetching stats for hero {HeroId}", hero.HeroId);
            }
        }

        LoggingService.Logger.Information("Total stats collected: {TotalCount}", heroesStats.Count);
        return heroesStats;
    }

    public List<With> TopHeroes
    {
        get => _topHeroes;
        set => this.RaiseAndSetIfChanged(ref _topHeroes, value);
    }

    private List<With> GetTopHeroesForSelected(List<With> sortedStats)
    {
        // Only filter out: the selected hero itself and banned heroes
        var heroesToFilter = bannedHeroes.ToHashSet();
        heroesToFilter.Add(SelectedHero.HeroId);

        var topHeroes = sortedStats
            .Where(x => !heroesToFilter.Contains(x.HeroId2))
            .Take(5)
            .ToList();

        LoggingService.Logger.Information("GetTopHeroesForSelected: total_stats={TotalStats}, filtered_out={FilteredCount}, top={TopCount}",
            sortedStats.Count, heroesToFilter.Count, topHeroes.Count);

        return topHeroes;
    }

    private async Task<List<With>> UpdateTopHeroes()
    {
        var topHeroes = await GetTopHeroes();
        await WriteObjectToFileJson(topHeroes, "top_10_heroes.json");
        return topHeroes;
    }

    private async Task WriteObjectToFileJson(object? o, string filePath)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(o, new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        });
        await File.WriteAllTextAsync(filePath, json, Encoding.UTF8);
    }

    public static List<short> bannedHeroes = new List<short>();

    public async Task<List<With>> GetTopHeroes()
    {
        var selectedAllyHeroes = PlayerHeroes.Where(x => x.IsAlly && x.HeroId > 0).ToList();
        var selectedEnemyHeroes = PlayerHeroes.Where(x => x.IsEnemy && x.HeroId > 0).ToList();

        LoggingService.Logger.Information("Getting top heroes. Ally count: {AllyCount}, Enemy count: {EnemyCount}",
            selectedAllyHeroes.Count, selectedEnemyHeroes.Count);

        var topHeroes = new List<With>();

        if (selectedAllyHeroes.Count == 0 && selectedEnemyHeroes.Count == 0)
        {
            LoggingService.Logger.Warning("No heroes selected to get counters for");
            return topHeroes;
        }

        var allyHeroesStats = await GetHeroesStats(selectedAllyHeroes);
        var enemyHeroesStats = await GetHeroesStats(selectedEnemyHeroes);

        var allHeroesStats = allyHeroesStats.Concat(enemyHeroesStats).ToList();

        if (allHeroesStats.Count == 0)
        {
            LoggingService.Logger.Warning("No stats collected for any heroes");
            return topHeroes;
        }

        var sortedStats = allHeroesStats
            .OrderByDescending(x => x.Synergy)
            .ToList();

        var selectedHeroIds = selectedAllyHeroes.Concat(selectedEnemyHeroes).Select(x => x.HeroId);
        var heroesToFilter = selectedHeroIds.Concat(bannedHeroes).ToHashSet();

        topHeroes = sortedStats
            .Where(x => !heroesToFilter.Contains(x.HeroId2))
            .Take(5)
            .ToList();

        LoggingService.Logger.Information("Top {Count} counter-pick heroes: {HeroIds}",
            topHeroes.Count,
            string.Join(", ", topHeroes.Select(h => h.HeroId2)));

        return topHeroes;
    }

    private static readonly HttpClient _httpClient = new HttpClient(new HttpClientHandler
    {
        Proxy = _configService.ProxyEnabled
            ? new WebProxy(_configService.ProxyAddress, _configService.ProxyPort)
            {
                BypassList = new[] { "localhost", "127.0.0.1" },
                BypassProxyOnLocal = true,
            }
            : null,
        UseProxy = _configService.ProxyEnabled,
    });

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

        // Preserve the team from the current selected slot
        var currentTeam = SelectedHero.CurrentTeam;

        SelectedHero.HeroId = hero.HeroId;
        SelectedHero.HeroName = hero.HeroName;
        SelectedHero.HeroIconPath = hero.HeroIconPath;
        SelectedHero.CurrentTeam = currentTeam;

        IsVisibleLoading = true;
        IsEnabledHeroView = false;

        try
        {
            TopHeroes = await GetTopHeroes();
            LoggingService.Logger.Information("Updated top heroes based on full team composition for {HeroName} ({Team})",
                SelectedHero.HeroName, SelectedHero.CurrentTeam);
        }
        catch (Exception ex)
        {
            LoggingService.Logger.Error(ex, "Error getting top heroes for team composition");
        }
        finally
        {
            IsVisibleLoading = false;
            IsEnabledHeroView = true;
        }
    }

    private IBrush _playerHero;

    private List<With> _topHeroes;

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