namespace StudyTrack.Models;

[Table("Tasks")]
public class TodoTask
{
    [PrimaryKey, AutoIncrement]
    public int TaskId { get; set; }

    public int UserId { get; set; }

    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime DueDate { get; set; } = DateTime.Now.AddDays(1);

    public TimeSpan DueTime { get; set; } = new TimeSpan(23, 59, 0);

    [MaxLength(20)]
    public string Priority { get; set; } = "Medium"; // High, Medium, Low

    [MaxLength(20)]
    public string Status { get; set; } = "Pending"; // Pending, Completed

    public int? SubjectId { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.Now;

    public bool IsCompleted => Status == "Completed";

    public bool IsOverdue => !IsCompleted && DateTime.Now > DueDate.Add(DueTime);

    public bool IsDueToday => DueDate.Date == DateTime.Today;

    public bool IsDueTomorrow => DueDate.Date == DateTime.Today.AddDays(1);

    [Ignore]
    public string PriorityColor => Priority switch
    {
        "High" => "#EF4444",
        "Medium" => "#F59E0B",
        "Low" => "#10B981",
        _ => "#6B7280"
    };

    [Ignore]
    public string DueDateFormatted => DueDate.ToString("MMM dd, yyyy");

    [Ignore]
    public string DueTimeFormatted => DueTime.ToString(@"hh\:mm\ tt");

    [Ignore]
    public string SubjectName { get; set; } = "General";
}
