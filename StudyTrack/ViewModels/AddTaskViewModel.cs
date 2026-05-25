namespace StudyTrack.ViewModels;

public partial class AddTaskViewModel : ObservableObject, IQueryAttributable
{
    private readonly DatabaseService _databaseService;
    private readonly AuthService _authService;

    [ObservableProperty]
    private TodoTask task = new();

    [ObservableProperty]
    private ObservableCollection<Subject> subjects = new();

    // Make this nullable since FirstOrDefault can return null
    [ObservableProperty]
    private Subject? selectedSubject;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private bool isEditMode;

    public List<string> PriorityOptions { get; } = new() { "High", "Medium", "Low" };

    public AddTaskViewModel(DatabaseService databaseService, AuthService authService)
    {
        _databaseService = databaseService;
        _authService = authService;
        Task = new TodoTask
        {
            DueDate = DateTime.Today.AddDays(1),
            DueTime = new TimeSpan(23, 59, 0),
            Priority = "Medium"
        };
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("Task"))
        {
            var existingTask = query["Task"] as TodoTask;
            if (existingTask != null)
            {
                Task = existingTask;
                IsEditMode = true;
            }
        }
    }

    public async Task InitializeAsync()
    {
        await LoadSubjectsAsync();
    }

    [RelayCommand]
    private async Task LoadSubjectsAsync()
    {
        if (_authService.CurrentUser == null) return;

        try
        {
            var subjectList = await _databaseService.GetSubjectsAsync(_authService.CurrentUser.UserId);

            Subjects.Clear();
            Subjects.Add(new Subject { SubjectId = 0, SubjectName = "No Subject" });

            foreach (var subject in subjectList)
            {
                Subjects.Add(subject);
            }

            if (Task.SubjectId.HasValue)
            {
                SelectedSubject = Subjects.FirstOrDefault(s => s.SubjectId == Task.SubjectId);
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
    }
     
    [RelayCommand]
    private async Task SaveTaskAsync()
    {
        if (IsBusy) return;

        if (string.IsNullOrWhiteSpace(Task.Title))
        {
            await Shell.Current.DisplayAlert("Error", "Task title is required", "OK");
            return;
        }

        // Guard against a null CurrentUser to avoid dereferencing a possibly null reference (CS8602)
        if (_authService.CurrentUser == null)
        {
            await Shell.Current.DisplayAlert("Error", "User not logged in", "OK");
            return;
        }

        IsBusy = true;

        try
        {
            Task.UserId = _authService.CurrentUser.UserId;

            if (SelectedSubject != null && SelectedSubject.SubjectId != 0)
            {
                Task.SubjectId = SelectedSubject.SubjectId;
            }
            else
            {
                Task.SubjectId = null;
            }

            await _databaseService.SaveTaskAsync(Task);

            await Shell.Current.DisplayAlert("Success", IsEditMode ? "Task updated successfully" : "Task created successfully", "OK");
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

    [RelayCommand]
    private async Task CancelAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}
