namespace StudyTrack.Models;

[Table("Notes")]
public class Note
{
    [PrimaryKey, AutoIncrement]
    public int NoteId { get; set; }

    public int UserId { get; set; }

    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public int? SubjectId { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.Now;

    public DateTime LastModified { get; set; } = DateTime.Now;

    [Ignore]
    public string SubjectName { get; set; } = "General";

    [Ignore]
    public string PreviewText => Content.Length > 100
        ? Content.Substring(0, 100) + "..."
        : Content;

    [Ignore]
    public string LastModifiedFormatted => LastModified.ToString("MMM dd, yyyy");
}
