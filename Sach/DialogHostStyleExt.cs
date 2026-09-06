using Avalonia;
using DialogHostAvalonia;

namespace Sach;

public class DialogHostStyleExt : DialogHostStyle
{
    public static void SetBorderThickness(DialogHost element, Thickness value) =>
        element.SetValue(BorderThicknessProperty, (object) value);
}