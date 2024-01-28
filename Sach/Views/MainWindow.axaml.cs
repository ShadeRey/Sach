using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
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

    private async void ConfirmButton_OnClick(object? sender, RoutedEventArgs e)
    {
        string validApiToken = ApiTextBox.Text;
        bool isValidToken = await ApiValidation("https://api.stratz.com/graphql", "Bearer " + validApiToken);

        if (isValidToken)
        {
            Console.WriteLine("Токен действителен.");
            App.ConnectApi(validApiToken);
            ApiValidationDialog.IsOpen = false;
        }
        else
        {
            Console.WriteLine("Токен недействителен.");
        }
    }

    public async Task<bool> ApiValidation(string apiUrl, string bearerToken)
    {
        try
        {
            using (HttpClient httpClient = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
                request.Headers.Add("Authorization", bearerToken);
                var response = await httpClient.SendAsync(request);
                Console.WriteLine(response.StatusCode.ToString());
                Console.WriteLine(await response.Content.ReadAsStringAsync());
                string responseBool = await response.Content.ReadAsStringAsync();
                if (responseBool ==
                    "{\"errors\":[{\"message\":\"GraphQL query is missing.\",\"extensions\":{\"code\":\"QUERY_MISSING\",\"codes\":[\"QUERY_MISSING\"]}}]}")
                {
                    return true;
                }
                
                else
                {
                    return false;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex}");
            return false;
        }
    }

    private void ExitButton_OnClick(object? sender, RoutedEventArgs e)
    {
        this.Close();
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