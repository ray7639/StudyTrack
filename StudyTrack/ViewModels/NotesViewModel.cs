namespace StudyTrack.ViewModels;

public partial class NotesViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;
    private readonly AuthService _authService;

    [ObservableProperty]
    private ObservableCollection<Note> notes = new();

    [ObservableProperty]
    private string searchQuery = string.Empty;

    [ObservableProperty]
    private bool isBusy;

    public NotesViewModel(DatabaseService databaseService, AuthService authService)
    {
        _databaseService = databaseService;
        _authService = authService;
    }

    public async Task InitializeAsync()
    {
        await LoadNotesAsync();
    }

    [RelayCommand]
    private async Task LoadNotesAsync()
    {
        if (IsBusy) return;

        var currentUser = _authService.CurrentUser;
        if (currentUser == null) return;

        IsBusy = true;

        try
        {
            var noteList = await _databaseService.GetNotesAsync(currentUser.UserId);

            if (!string.IsNullOrWhiteSpace(SearchQuery))
            {
                noteList = noteList.Where(n =>
                    n.Title.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                    n.Content.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            Notes.Clear();
            foreach (var note in noteList)
            {
                Notes.Add(note);
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
    private async Task AddNoteAsync()
    {
        var currentUser = _authService.CurrentUser;
        if (currentUser == null)
        {
            await Shell.Current.DisplayAlert("Error", "You must be logged in to add notes.", "OK");
            return;
        }

        string title = await Shell.Current.DisplayPromptAsync("New Note", "Enter note title:");

        if (string.IsNullOrWhiteSpace(title)) return;

        string content = await Shell.Current.DisplayPromptAsync("Note Content", "Enter your notes:");

        if (string.IsNullOrWhiteSpace(content)) return;

        var note = new Note
        {
            UserId = currentUser.UserId,
            Title = title,
            Content = content,
            CreatedDate = DateTime.Now,
            LastModified = DateTime.Now
        };

        await _databaseService.SaveNoteAsync(note);
        await LoadNotesAsync();
    }

    [RelayCommand]
    private async Task ViewNoteAsync(Note note)
    {
        if (note == null) return;

        await Shell.Current.DisplayAlert(note.Title, note.Content, "OK");
    }

    [RelayCommand]
    private async Task EditNoteAsync(Note note)
    {
        if (note == null) return;

        string newContent = await Shell.Current.DisplayPromptAsync("Edit Note", "Update content:", initialValue: note.Content);

        if (string.IsNullOrWhiteSpace(newContent)) return;

        note.Content = newContent;
        note.LastModified = DateTime.Now;

        await _databaseService.SaveNoteAsync(note);
        await LoadNotesAsync();
    }

    [RelayCommand]
    private async Task DeleteNoteAsync(Note note)
    {
        if (note == null) return;

        var confirm = await Shell.Current.DisplayAlert("Confirm Delete", $"Delete '{note.Title}'?", "Yes", "No");

        if (!confirm) return;

        await _databaseService.DeleteNoteAsync(note);
        await LoadNotesAsync();
    }

    partial void OnSearchQueryChanged(string value)
    {
        _ = LoadNotesAsync();
    }
}
