namespace StudyTrack.Models;

[Table("Subjects")]
public class Subject
{
    [PrimaryKey, AutoIncrement]
    public int SubjectId { get; set; }

    public int UserId { get; set; }

    [MaxLength(100)]
    public string SubjectName { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Instructor { get; set; } = string.Empty;

    [MaxLength(20)]
    public string Color { get; set; } = "#3B82F6";

    public string Schedule { get; set; } = string.Empty;

    public string Room { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; } = DateTime.Now;

    [Ignore]
    public int TaskCount { get; set; }

    [Ignore]
    public int NoteCount { get; set; }
}
