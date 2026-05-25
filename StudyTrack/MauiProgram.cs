using Microsoft.Extensions.Logging;

namespace StudyTrack;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        // Register Services
        builder.Services.AddSingleton<DatabaseService>();
        builder.Services.AddSingleton<AuthService>();

        // Register ViewModels
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<DashboardViewModel>();
        builder.Services.AddTransient<TasksViewModel>();
        builder.Services.AddTransient<AddTaskViewModel>();
        builder.Services.AddTransient<TaskDetailViewModel>();
        builder.Services.AddTransient<SubjectsViewModel>();
        builder.Services.AddTransient<NotesViewModel>();
        builder.Services.AddTransient<ProfileViewModel>();

        // Register Views
        builder.Services.AddTransient<SplashPage>();
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<DashboardPage>();
        builder.Services.AddTransient<TasksPage>();
        builder.Services.AddTransient<AddTaskPage>();
        builder.Services.AddTransient<TaskDetailPage>();
        builder.Services.AddTransient<SubjectsPage>();
        builder.Services.AddTransient<NotesPage>();
        builder.Services.AddTransient<ProfilePage>();
        builder.Services.AddTransient<AboutPage>();

        return builder.Build();
    }
}
