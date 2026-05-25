namespace StudyTrack.Models;

[Table("Users")]
public class User
{
    [PrimaryKey, AutoIncrement]
    public int UserId { get; set; }

    [MaxLength(50), Unique]
    public string Username { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(255)]
    public string Password { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; } = DateTime.Now;

    public string FullName { get; set; } = string.Empty;

    public string ProfileColor { get; set; } = "#3B82F6";
}
