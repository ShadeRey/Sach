using Avalonia.Controls;
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
}