using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace Sach.Views;

public class PlayerTabItemView : TemplatedControl
{
    public static readonly StyledProperty<IImage> PlayerHeroIconProperty =
        AvaloniaProperty.Register<HeroButtonView, IImage>(nameof(PlayerHeroIcon));
    public IImage PlayerHeroIcon
    {
        get => GetValue(PlayerHeroIconProperty);
        set => SetValue(PlayerHeroIconProperty, value);
    }
    
    public static readonly StyledProperty<short> PlayerHeroIdProperty = 
        AvaloniaProperty.Register<HeroButtonView, short>(nameof(PlayerHeroId));
    public short PlayerHeroId
    {
        get => GetValue(PlayerHeroIdProperty);
        set => SetValue(PlayerHeroIdProperty, value);
    }
}