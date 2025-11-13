using System.Security.Cryptography;

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
        // Use cryptographically secure random number generator
        var guestNumber = RandomNumberGenerator.GetInt32(1000, 10000);
        return $"Guest{guestNumber}";
    }
}
