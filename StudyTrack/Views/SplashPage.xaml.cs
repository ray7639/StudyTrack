namespace StudyTrack.Views;

public partial class SplashPage : ContentPage
{
    private readonly AuthService _authService;

    public SplashPage(AuthService authService)
    {
        InitializeComponent();
        _authService = authService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await Task.Delay(2000); // Show splash for 2 seconds

        var isLoggedIn = await _authService.TryAutoLoginAsync();

        if (isLoggedIn)
        {
            await Shell.Current.GoToAsync($"//{nameof(DashboardPage)}");
        }
        else
        {
            await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
        }
    }
}
