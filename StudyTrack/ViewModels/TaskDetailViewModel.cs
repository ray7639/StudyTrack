namespace StudyTrack.ViewModels;

public partial class TaskDetailViewModel : ObservableObject, IQueryAttributable
{
    private readonly DatabaseService _databaseService;
    private readonly AuthService _authService;

    [ObservableProperty]
    private TodoTask? task;

    [ObservableProperty]
    private bool isBusy;

    public TaskDetailViewModel(DatabaseService databaseService, AuthService authService)
    {
        _databaseService = databaseService;
        _authService = authService;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query == null) return;

        // Safely get the "Task" entry and only assign when it's a non-null TodoTask.
        if (query.TryGetValue("Task", out var value) && value is TodoTask taskValue)
        {
            Task = taskValue;
        }
    }

    [RelayCommand]
    private async Task ToggleCompleteAsync()
    {
        if (Task == null) return;

        Task.Status = Task.Status == "Completed" ? "Pending" : "Completed";
        await _databaseService.SaveTaskAsync(Task);

        OnPropertyChanged(nameof(Task));
    }

    [RelayCommand]
    private async Task EditTaskAsync()
    {
        if (Task == null) return;

        var navigationParameter = new Dictionary<string, object>
        {
            { "Task", Task }
        };

        await Shell.Current.GoToAsync(nameof(AddTaskPage), navigationParameter);
    }

    [RelayCommand]
    private async Task DeleteTaskAsync()
    {
        if (Task == null) return;

        var confirm = await Shell.Current.DisplayAlert("Confirm Delete", "Are you sure you want to delete this task?", "Yes", "No");

        if (!confirm) return;

        IsBusy = true;

        try
        {
            await _databaseService.DeleteTaskAsync(Task);
            await Shell.Current.DisplayAlert("Success", "Task deleted successfully", "OK");
            await Shell.Current.GoToAsync("..");
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
}
