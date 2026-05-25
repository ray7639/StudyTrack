using Microsoft.Maui.Controls;
using Microsoft.Maui.ApplicationModel;

namespace StudyTrack;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new AppShell();

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            // Use relative route (no leading "//") to avoid JavaProxyThrowable
            await Shell.Current.GoToAsync("LoginPage");
        });
    }
}
