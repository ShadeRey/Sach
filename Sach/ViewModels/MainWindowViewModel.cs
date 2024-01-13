using System;
using System.Drawing;
using System.IO;
using System.Net.Mime;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using Image = Avalonia.Controls.Image;

namespace Sach.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public MainWindowViewModel()
    {
        OnHeroButtonClickCommand = ReactiveCommand.Create<short>(SetSelectedHeroId);
    }
    
    private IStratzAPI _stratzApi = App.Services.GetRequiredService<IStratzAPI>();
    
    public async Task GetHeroStats()
    {
        var operationResult = await _stratzApi.HeroStatistics.ExecuteAsync(SelectedHero);
        if (operationResult.Data is null)
        {
            return;
        }
        
        var serialize = System.Text.Json.JsonSerializer.Serialize(operationResult.Data);
        Console.WriteLine(serialize);
    }

    private short _selectedHero;

    public short SelectedHero
    {
        get => _selectedHero;
        set => this.RaiseAndSetIfChanged(ref _selectedHero, value);
    }
    
    public ReactiveCommand<short, Unit> OnHeroButtonClickCommand { get; set; }
    
    public void SetSelectedHeroId(short id)
    {
        SelectedHero = id;
        GetHeroStats();
    }

    private AvaloniaList<TabItem> _playersList;

    public AvaloniaList<TabItem> PlayersList
    {
        get => _playersList;
        set => this.RaiseAndSetIfChanged(ref _playersList, value);
    }
    
    private string _fish =
        "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";

    public string Fish
    {
        get => _fish;
    }
}