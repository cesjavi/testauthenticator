using ItemManager.Core.Models;

namespace ItemManager.Core.Services;

public class AuthService
{
    private readonly UserStore _userStore;
    private readonly SessionService _sessionService;
    private readonly TotpService _totpService;

    public AuthService(UserStore userStore, SessionService sessionService, TotpService totpService)
    {
        _userStore = userStore;
        _sessionService = sessionService;
        _totpService = totpService;
    }

    public LoginResult TryLogin(LoginRequest request)
    {
        var user = _userStore.FindByUsername(request.Username);
        if (user is null)
        {
            return LoginResult.Failed();
        }

        if (!string.Equals(user.Password, request.Password))
        {
            return LoginResult.Failed();
        }

        if (!_totpService.ValidateCode(user, request.OtpCode))
        {
            return LoginResult.Failed();
        }

        var token = _sessionService.CreateSession(user);
        return LoginResult.Successful(user, token);
    }
}
