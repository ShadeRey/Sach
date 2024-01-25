using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using ReactiveUI;

namespace Sach.Models;

public class Hero : ReactiveObject
{
    public Hero()
    {
        _heroBrush = this
            .WhenAnyValue(x => x.HeroIconPath)
            .Select(heroIconPath =>
            {
                if (string.IsNullOrEmpty(heroIconPath))
                {
                    return EmptyHero;
                }

                if (_heroBrushRegistry.TryGetValue(HeroId, out var brush)) return brush;
                var stream = AssetLoader.Open(new Uri(heroIconPath));
                _heroBrushRegistry[HeroId] = new ImageBrush(new Bitmap(stream))
                {
                    Stretch = Stretch.UniformToFill
                };

                return _heroBrushRegistry[HeroId];
            })
            .ToProperty(this, x => x.HeroBrush);
    }

    public string HeroIconPath
    {
        get => _heroIconPath;
        set => this.RaiseAndSetIfChanged(ref _heroIconPath, value);
    }

    public IBrush HeroBrush => _heroBrush.Value;

    public short HeroId
    {
        get => _heroId;
        set => this.RaiseAndSetIfChanged(ref _heroId, value);
    }

    public string HeroName
    {
        get => _heroName;
        set => this.RaiseAndSetIfChanged(ref _heroName, value);
    }

    public Team CurrentTeam
    {
        get => _currentTeam;
        set => this.RaiseAndSetIfChanged(ref _currentTeam, value);
    }

    public enum Team
    {
        Ally,
        Enemy
    }

    public bool IsAlly => CurrentTeam == Team.Ally;
    public bool IsEnemy => CurrentTeam == Team.Enemy;

    private static Dictionary<short, IBrush> _heroBrushRegistry = new Dictionary<short, IBrush>();
    public static IBrush EmptyHero = App.Current.Resources["GradientBorder"] as IBrush;
    private readonly ObservableAsPropertyHelper<IBrush> _heroBrush;
    private string _heroIconPath = string.Empty;
    private short _heroId;
    private string _heroName = string.Empty;
    private Team _currentTeam;
}