using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Sach.Models;
using Sach.ViewModels;

namespace Sach.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
    }

    public MainWindowViewModel ViewModel => (DataContext as MainWindowViewModel)!;
    
    private void InputElement_OnTapped(object? sender, TappedEventArgs e)
    {
        if (sender is not Canvas canvas) {
            return;
        }

        if (canvas.DataContext is not Hero hero) {
            return;
        }

        var selectingItemsControl = canvas.FindParentOfType<SelectingItemsControl>();
        if (selectingItemsControl is null) {
            return;
        }
        selectingItemsControl.SelectedItem = hero;
    }
}

public static class ControlUtils {
    public static T? FindParentOfType<T>(this StyledElement element) {
        var control = element;
        while (control != null)
        {
            if (control is T parentControl)
            {
                return parentControl;
            }
            control = control.Parent;
        }
        return default;
    }
}