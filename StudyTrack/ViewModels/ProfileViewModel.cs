namespace StudyTrack.ViewModels;
 
public partial class ProfileViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;
    private readonly AuthService _authService;
 
    [ObservableProperty]
    private User? currentUser;
 
    [ObservableProperty]
    private int totalTasks;
 
    [ObservableProperty]
    private int completedTasks;
 
    [ObservableProperty]
    private int successRate;
 
    public ProfileViewModel(DatabaseService databaseService, AuthService authService)
    {
        _databaseService = databaseService;
        _authService = authService;
        CurrentUser = _authService.CurrentUser;
    }
 
    public async Task InitializeAsync()
    {
        await LoadStatisticsAsync();
    }
 
    [RelayCommand]
    private async Task LoadStatisticsAsync()
    {
        if (_authService.CurrentUser == null) return;
 
        try
        {
            var userId = _authService.CurrentUser.UserId;
 
            var activeTasks = await _databaseService.GetActiveTaskCountAsync(userId);
            CompletedTasks = await _databaseService.GetCompletedTaskCountAsync(userId);
            TotalTasks = activeTasks + CompletedTasks;
 
            if (TotalTasks > 0)
            {
                SuccessRate = (int)((double)CompletedTasks / TotalTasks * 100);
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
    }
 
    [RelayCommand]
    private async Task EditProfileAsync()
    {
        string initial = CurrentUser?.FullName ?? string.Empty;
        string newName = await Shell.Current.DisplayPromptAsync("Edit Profile", "Enter new name:", initialValue: initial);
 
        if (string.IsNullOrWhiteSpace(newName) || CurrentUser == null) return;
 
        CurrentUser.FullName = newName;
        await _databaseService.SaveUserAsync(CurrentUser);
 
        await Shell.Current.DisplayAlert("Success", "Profile updated successfully", "OK");
    }
 
    [RelayCommand]
    private async Task ChangePasswordAsync()
    {
        await Shell.Current.DisplayAlert("Info", "Password change feature coming soon!", "OK");
    }
 
    [RelayCommand]
    private async Task LogoutAsync()
    {
        var confirm = await Shell.Current.DisplayAlert("Confirm Logout", "Are you sure you want to logout?", "Yes", "No");
        if (!confirm) return;
 
        await _authService.LogoutAsync();
        CurrentUser = null;
 
        // Navigate back to the login ShellContent route and reset navigation stack
        // AppShell defines the login ShellContent with Route="login"
        await Shell.Current.GoToAsync("//login");
    }
}
