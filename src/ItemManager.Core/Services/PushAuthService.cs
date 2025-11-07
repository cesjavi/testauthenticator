using ItemManager.Core.Models;
using Microsoft.Extensions.Logging;

namespace ItemManager.Core.Services;

public class PushAuthService
{
    private readonly UserStore _userStore;
    private readonly SessionService _sessionService;
    private readonly TotpService _totpService;
    private readonly PushDeviceStore _deviceStore;
    private readonly PushChallengeService _challengeService;
    private readonly FirebasePushNotificationService _pushNotificationService;
    private readonly ILogger<PushAuthService> _logger;

    public PushAuthService(
        UserStore userStore,
        SessionService sessionService,
        TotpService totpService,
        PushDeviceStore deviceStore,
        PushChallengeService challengeService,
        FirebasePushNotificationService pushNotificationService,
        ILogger<PushAuthService> logger)
    {
        _userStore = userStore;
        _sessionService = sessionService;
        _totpService = totpService;
        _deviceStore = deviceStore;
        _challengeService = challengeService;
        _pushNotificationService = pushNotificationService;
        _logger = logger;
    }

    public PushRegistrationResult RegisterDevice(PushDeviceRegistrationRequest request)
    {
        var user = _userStore.FindByUsername(request.Username);
        if (user is null || !string.Equals(user.Password, request.Password, StringComparison.Ordinal))
        {
            return PushRegistrationResult.Failed("Credenciales inválidas.");
        }

        if (!_totpService.ValidateCode(user, request.OtpCode))
        {
            return PushRegistrationResult.Failed("Código TOTP inválido.");
        }

        var device = _deviceStore.RegisterDevice(user.Username, request.DeviceName, request.RegistrationToken);
        _logger.LogInformation("Push device {DeviceId} registrado para el usuario {Username}.", device.DeviceId, user.Username);
        return PushRegistrationResult.Successful(device);
    }

    public async Task<PushLoginResult> InitiateLoginAsync(PushLoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = _userStore.FindByUsername(request.Username);
        if (user is null || !string.Equals(user.Password, request.Password, StringComparison.Ordinal))
        {
            return PushLoginResult.Failed(PushLoginFailureReason.InvalidCredentials, "Credenciales inválidas.");
        }

        var devices = _deviceStore.GetDevices(user.Username);
        if (devices.Count == 0)
        {
            return PushLoginResult.Failed(PushLoginFailureReason.NoDevicesRegistered, "El usuario no tiene dispositivos registrados.");
        }

        var challengeContext = _challengeService.CreateChallenge(user, devices);
        var sentNotifications = 0;

        foreach (var device in challengeContext.Devices)
        {
            var sent = await _pushNotificationService.SendLoginApprovalRequestAsync(user, device, challengeContext.Challenge, cancellationToken);
            if (sent)
            {
                sentNotifications++;
            }
        }

        if (sentNotifications == 0)
        {
            _challengeService.CancelChallenge(challengeContext.Challenge.Id);
            return PushLoginResult.Failed(PushLoginFailureReason.NotificationError, "No se pudo enviar la notificación push.");
        }

        _logger.LogInformation(
            "Desafío de login push {ChallengeId} iniciado para el usuario {Username} en {Devices} dispositivos.",
            challengeContext.Challenge.Id,
            user.Username,
            sentNotifications);

        return PushLoginResult.Pending(user, challengeContext.Challenge);
    }

    public PushLoginConfirmationResult CompleteLogin(PushLoginConfirmationRequest request)
    {
        var status = _challengeService.CompleteChallenge(request.ChallengeId, request.DeviceId, out var challenge);

        if (status == PushChallengeCompletionStatus.NotFound)
        {
            return PushLoginConfirmationResult.Failed("Desafío inválido o ya utilizado.");
        }

        if (status == PushChallengeCompletionStatus.InvalidDevice)
        {
            return PushLoginConfirmationResult.Failed("El dispositivo no coincide con el desafío enviado.");
        }

        if (status == PushChallengeCompletionStatus.Expired)
        {
            return PushLoginConfirmationResult.Expired("El desafío expiró. Inicia sesión nuevamente.");
        }

        if (challenge is null)
        {
            return PushLoginConfirmationResult.Failed("El desafío no se pudo recuperar.");
        }

        var user = _userStore.FindByUsername(challenge.Username);
        if (user is null)
        {
            return PushLoginConfirmationResult.Failed("El usuario asociado al desafío ya no existe.");
        }

        var sessionToken = _sessionService.CreateSession(user);
        var loginResult = LoginResult.Successful(user, sessionToken);
        return PushLoginConfirmationResult.Successful(loginResult);
    }
}
