namespace StudyTrack.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;
    private readonly AuthService _authService;

    [ObservableProperty]
    private string userName = "Student";

    [ObservableProperty]
    private int activeTaskCount;

    [ObservableProperty]
    private int completedTaskCount;

    [ObservableProperty]
    private int overdueTaskCount;

    [ObservableProperty]
    private ObservableCollection<TodoTask> upcomingTasks = new();

    [ObservableProperty]
    private string searchQuery = string.Empty;

    [ObservableProperty]
    private bool isBusy;

    public DashboardViewModel(DatabaseService databaseService, AuthService authService)
    {
        _databaseService = databaseService;
        _authService = authService;
    }

    public async Task InitializeAsync()
    {
        UserName = _authService.CurrentUser?.FullName ?? _authService.CurrentUser?.Username ?? "Student";
        await LoadDataAsync();
    }

    [RelayCommand]
    private async Task LoadDataAsync()
    {
        if (IsBusy || _authService.CurrentUser == null) return;

        IsBusy = true;

        try
        {
            var userId = _authService.CurrentUser.UserId;

            ActiveTaskCount = await _databaseService.GetActiveTaskCountAsync(userId);
            CompletedTaskCount = await _databaseService.GetCompletedTaskCountAsync(userId);
            OverdueTaskCount = await _databaseService.GetOverdueTaskCountAsync(userId);

            var tasks = await _databaseService.GetTasksAsync(userId);
            var upcoming = tasks.Where(t => t.Status == "Pending")
                                .OrderBy(t => t.DueDate)
                                .Take(5)
                                .ToList();

            UpcomingTasks.Clear();
            foreach (var task in upcoming)
            {
                UpcomingTasks.Add(task);
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task GoToTasksAsync()
    {
        await Shell.Current.GoToAsync(nameof(TasksPage));
    }

    [RelayCommand]
    private async Task GoToAddTaskAsync()
    {
        await Shell.Current.GoToAsync(nameof(AddTaskPage));
    }

    [RelayCommand]
    private async Task GoToTaskDetailAsync(TodoTask task)
    {
        if (task == null) return;

        var navigationParameter = new Dictionary<string, object>
        {
            { "Task", task }
        };

        await Shell.Current.GoToAsync(nameof(TaskDetailPage), navigationParameter);
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchQuery)) return;

        await Shell.Current.GoToAsync($"{nameof(TasksPage)}?query={SearchQuery}");
    }
}
