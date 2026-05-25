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
        if (query.ContainsKey("query"))
        {
            SearchQuery = query["query"].ToString();
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
            var taskList = await _databaseService.GetTasksAsync(_authService.CurrentUser.UserId);

            Tasks.Clear();
            foreach (var task in taskList)
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
            filtered = filtered.Where(t =>
                t.Title.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                t.Description.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                t.SubjectName.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase)
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
