using System;
using System.Collections.Generic;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace Sach.Views;

public class HeroButtonView : TemplatedControl {
    public static readonly StyledProperty<ICommand> HeroButtonCommandProperty =
        AvaloniaProperty.Register<HeroButtonView, ICommand>(nameof(HeroButtonCommand));

    public ICommand HeroButtonCommand {
        get => GetValue(HeroButtonCommandProperty);
        set => SetValue(HeroButtonCommandProperty, value);
    }

    public static readonly StyledProperty<short> HeroIdProperty =
        AvaloniaProperty.Register<HeroButtonView, short>(nameof(HeroId));

    public short HeroId {
        get => GetValue(HeroIdProperty);
        set => SetValue(HeroIdProperty, value);
    }

    public static readonly StyledProperty<string> HeroIconProperty =
        AvaloniaProperty.Register<HeroButtonView, string>(nameof(HeroIcon));

    public string HeroIcon {
        get => GetValue(HeroIconProperty);
        set => SetValue(HeroIconProperty, value);
    }

    public IImage? Image => new Bitmap(AssetLoader.Open(new Uri(HeroIcon)));

    public static readonly StyledProperty<string> HeroNameProperty =
        AvaloniaProperty.Register<HeroButtonView, string>(nameof(HeroName));

    public string HeroName {
        get => GetValue(HeroNameProperty);
        set => SetValue(HeroNameProperty, value);
    }

    public static readonly DirectProperty<HeroButtonView, Hero> HeroProperty =
        AvaloniaProperty.RegisterDirect<HeroButtonView, Hero>(nameof(Hero),
            view => new Hero() {
                HeroId = view.HeroId,
                HeroIconPath = view.HeroIcon,
                HeroName = view.HeroName
            }
        );

    public Hero Hero => GetValue(HeroProperty);
}

public class Hero {
    public string HeroIconPath { get; set; } = string.Empty;

    public IBrush HeroBrush {
        get {
            if (_heroBrushRegistry.TryGetValue(HeroId, out var brush)) return brush;
            var stream = AssetLoader.Open(new Uri(HeroIconPath));
            _heroBrushRegistry[HeroId] = new ImageBrush(new Bitmap(stream)) {
                Stretch = Stretch.UniformToFill
            };

            return _heroBrushRegistry[HeroId];
        }
    }

    public short HeroId { get; set; }
    public string HeroName { get; set; } = string.Empty;

    public Team CurrentTeam { get; set; }

    public enum Team {
        Ally,
        Enemy
    }

    public bool IsAlly => CurrentTeam == Team.Ally;
    public bool IsEnemy => CurrentTeam == Team.Enemy;

    private static Dictionary<short, IBrush> _heroBrushRegistry = new Dictionary<short, IBrush>();
    public static IBrush EmptyHero = App.Current.Resources["GradientBorder"] as IBrush;
}