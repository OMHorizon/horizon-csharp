namespace HorizonChat.Services;

public class UserService
{
    private string? _username;

    public string? Username
    {
        get => _username;
        set
        {
            _username = value;
            OnUsernameChanged?.Invoke(value);
        }
    }

    public event Action<string?>? OnUsernameChanged;

    public bool HasUsername => !string.IsNullOrWhiteSpace(_username);

    public string GenerateGuestUsername()
    {
        var random = new Random();
        return $"Guest{random.Next(1000, 9999)}";
    }
}
