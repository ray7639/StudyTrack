namespace StudyTrack.Services;

public class AuthService
{
    private readonly DatabaseService _databaseService;
    private User? _currentUser;

    public AuthService(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    public User? CurrentUser => _currentUser;

    public bool IsLoggedIn => _currentUser != null;

    public async Task<bool> LoginAsync(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            return false;

        var hashedPassword = HashPassword(password);
        var user = await _databaseService.GetUserAsync(username, hashedPassword);

        if (user != null)
        {
            _currentUser = user;
            await SecureStorage.SetAsync("user_id", user.UserId.ToString());
            await SecureStorage.SetAsync("username", user.Username);
            return true;
        }

        return false;
    }

    public async Task<(bool Success, string Message)> RegisterAsync(string username, string email, string password, string confirmPassword, string fullName)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(username))
            return (false, "Username is required");

        if (string.IsNullOrWhiteSpace(email))
            return (false, "Email is required");

        if (string.IsNullOrWhiteSpace(password))
            return (false, "Password is required");

        if (password != confirmPassword)
            return (false, "Passwords do not match");

        if (password.Length < 6)
            return (false, "Password must be at least 6 characters");

        if (!IsValidEmail(email))
            return (false, "Invalid email format");

        // Check if username exists
        if (await _databaseService.UserExistsAsync(username))
            return (false, "Username already exists");

        // Create user
        var user = new User
        {
            Username = username,
            Email = email,
            Password = HashPassword(password),
            FullName = fullName,
            CreatedDate = DateTime.Now,
            ProfileColor = GetRandomColor()
        };

        await _databaseService.SaveUserAsync(user);
        return (true, "Registration successful");
    }

    public async Task LogoutAsync()
    {
        _currentUser = null;
        SecureStorage.Remove("user_id");
        SecureStorage.Remove("username");
        await Task.CompletedTask;
    }

    public async Task<bool> TryAutoLoginAsync()
    {
        try
        {
            var userIdStr = await SecureStorage.GetAsync("user_id");
            if (!string.IsNullOrEmpty(userIdStr) && int.TryParse(userIdStr, out int userId))
            {
                _currentUser = await _databaseService.GetUserByIdAsync(userId);
                return _currentUser != null;
            }
        }
        catch
        {
            // SecureStorage not available or error
        }

        return false;
    }

    private string HashPassword(string password)
    {
        // Simple hash - in production, use BCrypt or similar
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(password + "StudyTrack_Salt_2026");
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    private string GetRandomColor()
    {
        var colors = new[] { "#3B82F6", "#EF4444", "#10B981", "#F59E0B", "#8B5CF6", "#EC4899" };
        return colors[new Random().Next(colors.Length)];
    }
}
