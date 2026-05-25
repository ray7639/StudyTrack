namespace StudyTrack.Views;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using System;

public partial class LoginPage : ContentPage
{
    // Parameterless constructor required for Shell routing / Activator.CreateInstance
    public LoginPage() : this(ResolveViewModelFromServices())
    {
    }

    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private static LoginViewModel ResolveViewModelFromServices()
    {
        var services = IPlatformApplication.Current?.Services
            ?? throw new InvalidOperationException("IPlatformApplication.Current.Services is not available. Ensure the MAUI app is initialized before instantiating LoginPage.");
        return services.GetRequiredService<LoginViewModel>();
    }
}
