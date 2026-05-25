namespace StudyTrack.ViewModels;

using System.Linq;
using Microsoft.Maui.Controls;

public partial class LoginViewModel(AuthService authService) : ObservableObject
{
    private readonly AuthService _authService = authService;

    [ObservableProperty]
    private string username = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    [ObservableProperty]
    private bool isBusy;

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (IsBusy) return;

        // Capture Shell.Current and fallback Page once so we don't repeatedly dereference a possibly null Shell.Current.
        var shell = Shell.Current;
        Page? page = shell ?? Application.Current?.MainPage;

        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            if (page != null)
                await page.DisplayAlert("Error", "Please enter username and password", "OK");
            return;
        }

        IsBusy = true;

        try
        {
            var success = await _authService.LoginAsync(Username, Password);

            if (success)
            {
                // Select the TabBar with Route="main" and activate its "dashboard" ShellContent.
                // This avoids GoToAsync URI parsing issues and the "global routes cannot be the only page" problem.
                var mainShellItem = shell?.Items.FirstOrDefault(i => i.Route == "main");
                if (mainShellItem != null && shell != null)
                {
                    shell.CurrentItem = mainShellItem;

                    var dashboardItem = mainShellItem.Items.FirstOrDefault(i => i.Route == "dashboard");
                    if (dashboardItem != null)
                    {
                        mainShellItem.CurrentItem = dashboardItem;
                        return;
                    }
                }

                // Fallback: try a relative navigation by route name.
                if (shell != null)
                    await shell.GoToAsync(nameof(DashboardPage));
            }
            else
            {
                if (page != null)
                    await page.DisplayAlert("Error", "Invalid username or password", "OK");
            }
        }
        catch (Exception ex)
        {
            if (page != null)
                await page.DisplayAlert("Error", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task GoToRegisterAsync()
    {
        var shell = Shell.Current;
        if (shell != null)
            await shell.GoToAsync(nameof(RegisterPage));
    }
}
