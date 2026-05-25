namespace StudyTrack.ViewModels;

public partial class SubjectsViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;
    private readonly AuthService _authService;

    [ObservableProperty]
    private ObservableCollection<Subject> subjects = new();

    [ObservableProperty]
    private bool isBusy;

    public SubjectsViewModel(DatabaseService databaseService, AuthService authService)
    {
        _databaseService = databaseService;
        _authService = authService;
    }

    public async Task InitializeAsync()
    {
        await LoadSubjectsAsync();
    }

    [RelayCommand]
    private async Task LoadSubjectsAsync()
    {
        if (IsBusy || _authService.CurrentUser == null) return;

        IsBusy = true;

        try
        {
            var subjectList = await _databaseService.GetSubjectsAsync(_authService.CurrentUser.UserId);

            Subjects.Clear();
            foreach (var subject in subjectList)
            {
                Subjects.Add(subject);
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
    private async Task AddSubjectAsync()
    {
        // Guard against a null CurrentUser to avoid CS8602 (possible null dereference).
        if (_authService.CurrentUser == null)
        {
            await Shell.Current.DisplayAlert("Error", "No user is logged in.", "OK");
            return;
        }

        string subjectName = await Shell.Current.DisplayPromptAsync("New Subject", "Enter subject name:");

        if (string.IsNullOrWhiteSpace(subjectName)) return;

        string instructor = await Shell.Current.DisplayPromptAsync("Instructor", "Enter instructor name (optional):");

        var subject = new Subject
        {
            UserId = _authService.CurrentUser.UserId,
            SubjectName = subjectName,
            Instructor = instructor ?? string.Empty,
            Color = GetRandomColor(),
            CreatedDate = DateTime.Now
        };

        await _databaseService.SaveSubjectAsync(subject);
        await LoadSubjectsAsync();
    }

    [RelayCommand]
    private async Task EditSubjectAsync(Subject subject)
    {
        if (subject == null) return;

        string newName = await Shell.Current.DisplayPromptAsync("Edit Subject", "Enter new name:", initialValue: subject.SubjectName);

        if (string.IsNullOrWhiteSpace(newName)) return;

        subject.SubjectName = newName;
        await _databaseService.SaveSubjectAsync(subject);
        await LoadSubjectsAsync();
    }

    [RelayCommand]
    private async Task DeleteSubjectAsync(Subject subject)
    {
        if (subject == null) return;

        var confirm = await Shell.Current.DisplayAlert("Confirm Delete", $"Delete {subject.SubjectName}?", "Yes", "No");

        if (!confirm) return;

        await _databaseService.DeleteSubjectAsync(subject);
        await LoadSubjectsAsync();
    }

    private string GetRandomColor()
    {
        var colors = new[] { "#3B82F6", "#EF4444", "#10B981", "#F59E0B", "#8B5CF6", "#EC4899", "#06B6D4" };
        return colors[new Random().Next(colors.Length)];
    }
}
