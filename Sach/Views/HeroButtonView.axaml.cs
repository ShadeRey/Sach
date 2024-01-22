﻿using System;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace Sach.Views;

public class HeroButtonView : TemplatedControl
{
    public static readonly StyledProperty<ICommand> HeroButtonCommandProperty =
        AvaloniaProperty.Register<HeroButtonView, ICommand>(nameof(HeroButtonCommand));

    public ICommand HeroButtonCommand
    {
        get => GetValue(HeroButtonCommandProperty);
        set => SetValue(HeroButtonCommandProperty, value);
    }

    public static readonly StyledProperty<short> HeroIdProperty =
        AvaloniaProperty.Register<HeroButtonView, short>(nameof(HeroId));

    public short HeroId
    {
        get => GetValue(HeroIdProperty);
        set => SetValue(HeroIdProperty, value);
    }

    public static readonly StyledProperty<string> HeroIconProperty =
        AvaloniaProperty.Register<HeroButtonView, string>(nameof(HeroIcon));

    public string HeroIcon
    {
        get => GetValue(HeroIconProperty);
        set => SetValue(HeroIconProperty, value);
    }

    public IImage? Image => new Bitmap(AssetLoader.Open(new Uri(HeroIcon)));

    public static readonly StyledProperty<string> HeroNameProperty =
        AvaloniaProperty.Register<HeroButtonView, string>(nameof(HeroName));

    public string HeroName
    {
        get => GetValue(HeroNameProperty);
        set => SetValue(HeroNameProperty, value);
    }

    public static readonly DirectProperty<HeroButtonView, Hero> HeroProperty =
        AvaloniaProperty.RegisterDirect<HeroButtonView, Hero>(nameof(Hero),
            view => new Hero()
            {
                HeroId = view.HeroId,
                HeroIcon = view.HeroIcon,
                HeroName = view.HeroName
            }
        );

    public Hero Hero => GetValue(HeroProperty);
}

public class Hero
{
    public string HeroIcon { get; set; }
    public short HeroId { get; set; }
    public string HeroName { get; set; }

    public Team CurrentTeam { get; set; }
    
    public enum Team
    {
        Ally,
        Enemy
    }

    public bool IsAlly => CurrentTeam == Team.Ally;
    public bool IsEnemy => CurrentTeam == Team.Enemy;
}