using System.IO;
using Microsoft.Maui.Storage;
using SQLite;

namespace StudyTrack.Services;

public class DatabaseService
{
    private SQLiteAsyncConnection? _database;
    private readonly string _dbPath;

    public DatabaseService()
    {       
        _dbPath = Path.Combine(FileSystem.AppDataDirectory, "studytrack.db3");
    }

    private async Task InitAsync()
    {
        if (_database != null)
            return;

        Directory.CreateDirectory(Path.GetDirectoryName(_dbPath) ?? string.Empty);

        _database = new SQLiteAsyncConnection(_dbPath, SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.SharedCache);

        await _database.CreateTableAsync<User>();
        await _database.CreateTableAsync<TodoTask>();
        await _database.CreateTableAsync<Subject>();
        await _database.CreateTableAsync<Note>();
    }

    // USER OPERATIONS
    public async Task<User> GetUserAsync(string username, string password)
    { 
        await InitAsync();
        return await _database!.Table<User>()
            .Where(u => u.Username == username && u.Password == password)
            .FirstOrDefaultAsync();
    }

    public async Task<User> GetUserByIdAsync(int userId)
    {
        await InitAsync();
        return await _database!.Table<User>()
            .Where(u => u.UserId == userId)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> UserExistsAsync(string username)
    {
        await InitAsync();
        var user = await _database!.Table<User>()
            .Where(u => u.Username == username)
            .FirstOrDefaultAsync();
        return user != null;
    }

    public async Task<int> SaveUserAsync(User user)
    {
        await InitAsync();
        if (user.UserId != 0)
            return await _database!.UpdateAsync(user);
        else
            return await _database!.InsertAsync(user);
    }

    // TASK OPERATIONS
    public async Task<List<TodoTask>> GetTasksAsync(int userId)
    {
        await InitAsync();
        var tasks = await _database!.Table<TodoTask>()
            .Where(t => t.UserId == userId)
            .OrderBy(t => t.DueDate)
            .ToListAsync();

        // Populate subject names
        foreach (var task in tasks)
        {
            if (task.SubjectId.HasValue)
            {
                var subject = await GetSubjectAsync(task.SubjectId.Value);
                task.SubjectName = subject?.SubjectName ?? "General";
            }
        }

        return tasks;
    }

    public async Task<List<TodoTask>> GetTasksByStatusAsync(int userId, string status)
    {
        await InitAsync();
        return await _database!.Table<TodoTask>()
            .Where(t => t.UserId == userId && t.Status == status)
            .OrderBy(t => t.DueDate)
            .ToListAsync();
    }

    public async Task<List<TodoTask>> GetTasksDueTodayAsync(int userId)
    {
        await InitAsync();
        var today = DateTime.Today;
        return await _database!.Table<TodoTask>()
            .Where(t => t.UserId == userId && t.Status == "Pending")
            .ToListAsync()
            .ContinueWith(t => t.Result.Where(task => task.DueDate.Date == today).ToList());
    }

    public async Task<TodoTask> GetTaskAsync(int taskId)
    {
        await InitAsync();
        return await _database!.Table<TodoTask>()
            .Where(t => t.TaskId == taskId)
            .FirstOrDefaultAsync();
    }

    public async Task<int> SaveTaskAsync(TodoTask task)
    {
        await InitAsync();
        if (task.TaskId != 0)
            return await _database!.UpdateAsync(task);
        else
            return await _database!.InsertAsync(task);
    }

    public async Task<int> DeleteTaskAsync(TodoTask task)
    {
        await InitAsync();
        return await _database!.DeleteAsync(task);
    }

    public async Task<int> GetActiveTaskCountAsync(int userId)
    {
        await InitAsync();
        return await _database!.Table<TodoTask>()
            .Where(t => t.UserId == userId && t.Status == "Pending")
            .CountAsync();
    }

    public async Task<int> GetCompletedTaskCountAsync(int userId)
    {
        await InitAsync();
        return await _database!.Table<TodoTask>()
            .Where(t => t.UserId == userId && t.Status == "Completed")
            .CountAsync();
    }

    public async Task<int> GetOverdueTaskCountAsync(int userId)
    {
        await InitAsync();
        var tasks = await _database!.Table<TodoTask>()
            .Where(t => t.UserId == userId && t.Status == "Pending")
            .ToListAsync();

        return tasks.Count(t => t.IsOverdue);
    }

    // SUBJECT OPERATIONS
    public async Task<List<Subject>> GetSubjectsAsync(int userId)
    {
        await InitAsync();
        var subjects = await _database!.Table<Subject>()
            .Where(s => s.UserId == userId)
            .OrderBy(s => s.SubjectName)
            .ToListAsync();

        // Get counts for each subject
        foreach (var subject in subjects)
        {
            subject.TaskCount = await _database!.Table<TodoTask>()
                .Where(t => t.SubjectId == subject.SubjectId)
                .CountAsync();

            subject.NoteCount = await _database!.Table<Note>()
                .Where(n => n.SubjectId == subject.SubjectId)
                .CountAsync();
        }

        return subjects;
    }

    public async Task<Subject> GetSubjectAsync(int subjectId)
    {
        await InitAsync();
        return await _database!.Table<Subject>()
            .Where(s => s.SubjectId == subjectId)
            .FirstOrDefaultAsync();
    }

    public async Task<int> SaveSubjectAsync(Subject subject)
    {
        await InitAsync();
        if (subject.SubjectId != 0)
            return await _database!.UpdateAsync(subject);
        else
            return await _database!.InsertAsync(subject);
    }

    public async Task<int> DeleteSubjectAsync(Subject subject)
    {
        await InitAsync();
        return await _database!.DeleteAsync(subject);
    }

    // NOTE OPERATIONS
    public async Task<List<Note>> GetNotesAsync(int userId)
    {
        await InitAsync();
        var notes = await _database!.Table<Note>()
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.LastModified)
            .ToListAsync();

        // Populate subject names
        foreach (var note in notes)
        {
            if (note.SubjectId.HasValue)
            {
                var subject = await GetSubjectAsync(note.SubjectId.Value);
                note.SubjectName = subject?.SubjectName ?? "General";
            }
        }

        return notes;
    }

    public async Task<Note> GetNoteAsync(int noteId)
    {
        await InitAsync();
        return await _database!.Table<Note>()
            .Where(n => n.NoteId == noteId)
            .FirstOrDefaultAsync();
    }

    public async Task<int> SaveNoteAsync(Note note)
    {
        await InitAsync();
        note.LastModified = DateTime.Now;

        if (note.NoteId != 0)
            return await _database!.UpdateAsync(note);
        else
            return await _database!.InsertAsync(note);
    }

    public async Task<int> DeleteNoteAsync(Note note)
    {
        await InitAsync();
        return await _database!.DeleteAsync(note);
    }

    // SEARCH OPERATIONS
    public async Task<List<TodoTask>> SearchTasksAsync(int userId, string query)
    {
        await InitAsync();
        return await _database!.Table<TodoTask>()
            .Where(t => t.UserId == userId &&
                   (t.Title.Contains(query) || t.Description.Contains(query)))
            .ToListAsync();
    }

    public async Task<List<Note>> SearchNotesAsync(int userId, string query)
    {
        await InitAsync();
        return await _database!.Table<Note>()
            .Where(n => n.UserId == userId &&
                   (n.Title.Contains(query) || n.Content.Contains(query)))
            .ToListAsync();
    }
}
