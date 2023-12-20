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
}