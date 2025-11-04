using ItemManager.Core.Models;

namespace ItemManager.Core.Services;

public class SessionService
{
    private readonly Dictionary<string, (User User, DateTime Expiration)> _sessions = new();
    private readonly TimeSpan _sessionDuration = TimeSpan.FromHours(8);

    public string CreateSession(User user)
    {
        var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        _sessions[token] = (user, DateTime.UtcNow.Add(_sessionDuration));
        return token;
    }

    public User? ValidateSession(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return null;
        }

        if (!_sessions.TryGetValue(token, out var session))
        {
            return null;
        }

        if (session.Expiration < DateTime.UtcNow)
        {
            _sessions.Remove(token);
            return null;
        }

        return session.User;
    }
}
