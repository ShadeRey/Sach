﻿using System;
using System.Collections.Generic;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.VisualTree;
using Sach.Models;
using Sach.ViewModels;

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
                HeroIconPath = view.HeroIcon,
                HeroName = view.HeroName
            }
        );

    public Hero Hero => GetValue(HeroProperty);

    public static readonly StyledProperty<double> RectangleOpacityProperty =
        AvaloniaProperty.Register<HeroButtonView, double>(nameof(RectangleOpacity));

    public double RectangleOpacity
    {
        get => GetValue(RectangleOpacityProperty);
        set => SetValue(RectangleOpacityProperty, value);
    }

    protected override async void OnPointerPressed(PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(null).Properties.IsRightButtonPressed)
        {
            if (RectangleOpacity == 0)
            {
                RectangleOpacity = 1;
                MainWindowViewModel.bannedHeroes.Add(HeroId);
            }
            else
            {
                RectangleOpacity = 0;
                MainWindowViewModel.bannedHeroes.Remove(HeroId);
            }
        }

        await ((App.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime).MainWindow.DataContext as
            MainWindowViewModel).GetHeroStats();
    }

    public event EventHandler<RoutedEventArgs>? Click
    {
        add
        {
            var control = this.FindDescendantOfType<Button>(false);
            control?.AddHandler(Button.ClickEvent, value);
        }
        remove
        {
            var control = this.FindControl<Button>("HeroButton");
            control?.RemoveHandler(Button.ClickEvent, value);
        }
    }
}