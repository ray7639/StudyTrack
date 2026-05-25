namespace StudyTrack.ViewModels;

public partial class TasksViewModel : ObservableObject, IQueryAttributable
{
    private readonly DatabaseService _databaseService;
    private readonly AuthService _authService;

    [ObservableProperty]
    private ObservableCollection<TodoTask> tasks = new();

    [ObservableProperty]
    private ObservableCollection<TodoTask> filteredTasks = new();

    [ObservableProperty]
    private string searchQuery = string.Empty;

    [ObservableProperty]
    private string selectedFilter = "All";

    [ObservableProperty]
    private bool isBusy;

    public List<string> FilterOptions { get; } = new() { "All", "Pending", "Completed", "High Priority", "Due Today" };

    public TasksViewModel(DatabaseService databaseService, AuthService authService)
    {
        _databaseService = databaseService;
        _authService = authService;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query == null) return;

        if (query.TryGetValue("query", out var q))
        {
            // Guard against null and ensure SearchQuery remains non-nullable
            SearchQuery = q?.ToString() ?? string.Empty;
        }
    }

    public async Task InitializeAsync()
    {
        await LoadTasksAsync();
    }

    [RelayCommand]
    private async Task LoadTasksAsync()
    {
        if (IsBusy || _authService.CurrentUser == null) return;

        IsBusy = true;

        try
        {
            // Make sure we handle a possible null return from the database service
            var taskList = await _databaseService.GetTasksAsync(_authService.CurrentUser.UserId) ?? new List<TodoTask>();

            Tasks.Clear();
            foreach (var task in taskList.Where(t => t != null))
            {
                Tasks.Add(task);
            }

            ApplyFilter();
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
    private void ApplyFilter()
    {
        var filtered = SelectedFilter switch
        {
            "Pending" => Tasks.Where(t => t.Status == "Pending").ToList(),
            "Completed" => Tasks.Where(t => t.Status == "Completed").ToList(),
            "High Priority" => Tasks.Where(t => t.Priority == "High").ToList(),
            "Due Today" => Tasks.Where(t => t.IsDueToday).ToList(),
            _ => Tasks.ToList()
        };

        if (!string.IsNullOrWhiteSpace(SearchQuery))
        {
            // Null-safe string checks to avoid null reference dereferences
            filtered = filtered.Where(t =>
                (t.Title ?? string.Empty).Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                (t.Description ?? string.Empty).Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                (t.SubjectName ?? string.Empty).Contains(SearchQuery, StringComparison.OrdinalIgnoreCase)
            ).ToList();
        }

        FilteredTasks.Clear();
        foreach (var task in filtered)
        {
            FilteredTasks.Add(task);
        }
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
    private async Task ToggleCompleteAsync(TodoTask task)
    {
        if (task == null) return;

        task.Status = task.Status == "Completed" ? "Pending" : "Completed";
        await _databaseService.SaveTaskAsync(task);
        await LoadTasksAsync();
    }

    partial void OnSearchQueryChanged(string value)
    {
        ApplyFilter();
    }

    partial void OnSelectedFilterChanged(string value)
    {
        ApplyFilter();
    }
}
