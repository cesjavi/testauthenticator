namespace ItemManager.Models;

public class User
{
    public required string Username { get; init; }
    public required string DisplayName { get; init; }
    public required string Password { get; init; }
    public required string SecretKey { get; init; }
}

public record LoginRequest(string Username, string Password, string OtpCode);

public record LoginResult(bool Success, User? User, string? SessionToken)
{
    public static LoginResult Failed() => new(false, null, null);
    public static LoginResult Successful(User user, string token) => new(true, user, token);
}
