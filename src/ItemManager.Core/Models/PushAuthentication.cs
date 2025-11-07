namespace ItemManager.Core.Models;

public record PushDevice(string DeviceId, string DeviceName, string RegistrationToken, DateTimeOffset RegisteredAt);

public record PushDeviceRegistrationRequest(string Username, string Password, string OtpCode, string DeviceName, string RegistrationToken);

public record PushRegistrationResult(bool Success, string? Error, PushDevice? Device)
{
    public static PushRegistrationResult Failed(string error) => new(false, error, null);
    public static PushRegistrationResult Successful(PushDevice device) => new(true, null, device);
}

public record PushLoginRequest(string Username, string Password);

public record PushChallengeTarget(string DeviceId, string DeviceName);

public record PushChallenge(
    string Id,
    string Username,
    DateTimeOffset CreatedAt,
    DateTimeOffset ExpiresAt,
    IReadOnlyList<PushChallengeTarget> Targets,
    string? ApprovedDeviceId = null,
    bool IsApproved = false);

public enum PushLoginFailureReason
{
    None = 0,
    InvalidCredentials,
    NoDevicesRegistered,
    NotificationError
}

public record PushLoginResult(bool Success, PushLoginFailureReason FailureReason, string? Error, PushChallenge? Challenge, User? User)
{
    public static PushLoginResult Failed(PushLoginFailureReason reason, string error) => new(false, reason, error, null, null);
    public static PushLoginResult Pending(User user, PushChallenge challenge) => new(true, PushLoginFailureReason.None, null, challenge, user);
}

public record PushLoginConfirmationRequest(string ChallengeId, string DeviceId);

public enum PushChallengeCompletionStatus
{
    Approved,
    NotFound,
    Expired,
    InvalidDevice
}

public record PushLoginConfirmationResult(bool Success, string? Error, bool Expired, LoginResult? Login)
{
    public static PushLoginConfirmationResult Successful(LoginResult login) => new(true, null, false, login);
    public static PushLoginConfirmationResult Failed(string error) => new(false, error, false, null);
    //public static PushLoginConfirmationResult Expired(string error) => new(false, error, true, null);
}
