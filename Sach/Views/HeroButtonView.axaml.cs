using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

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
    
    public static readonly StyledProperty<IImage> HeroIconProperty =
        AvaloniaProperty.Register<HeroButtonView, IImage>(nameof(HeroIcon));
    public IImage HeroIcon
    {
        get => GetValue(HeroIconProperty);
        set => SetValue(HeroIconProperty, value);
    }
}