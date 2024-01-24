using Avalonia.Controls;
using Avalonia.Input;
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
    
    private void InputElement_OnTapped(object? sender, TappedEventArgs e) {
        if (sender is not Canvas canvas) {
            return;
        }
        
        canvas.
    }
}