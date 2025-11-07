using System.Net.Http.Json;
using ItemManager.Core.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ItemManager.Core.Services;

public class FirebasePushNotificationService
{
    private readonly HttpClient _httpClient;
    private readonly FirebasePushOptions _options;
    private readonly ILogger<FirebasePushNotificationService> _logger;

    public FirebasePushNotificationService(HttpClient httpClient, IOptions<FirebasePushOptions> options, ILogger<FirebasePushNotificationService> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<bool> SendLoginApprovalRequestAsync(User user, PushDevice device, PushChallenge challenge, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.ServerKey))
        {
            _logger.LogWarning("Firebase ServerKey is not configured. Skipping push notification for user {Username}.", user.Username);
            return false;
        }

        var notificationTitle = string.IsNullOrWhiteSpace(_options.LoginTitle)
            ? "Confirmar acceso"
            : _options.LoginTitle;
        var notificationBodyTemplate = string.IsNullOrWhiteSpace(_options.LoginBody)
            ? "Aprobá el inicio de sesión de {0}"
            : _options.LoginBody!;
        var notificationBody = string.Format(notificationBodyTemplate, user.DisplayName);

        using var request = new HttpRequestMessage(HttpMethod.Post, "fcm/send");
        request.Headers.TryAddWithoutValidation("Authorization", $"key={_options.ServerKey.Trim()}");
        request.Headers.TryAddWithoutValidation("Content-Type", "application/json");

        var payload = new
        {
            to = device.RegistrationToken,
            notification = new
            {
                title = notificationTitle,
                body = notificationBody
            },
            data = new Dictionary<string, string>
            {
                ["type"] = "login_approval",
                ["challengeId"] = challenge.Id,
                ["username"] = user.Username,
                ["deviceId"] = device.DeviceId
            }
        };

        request.Content = JsonContent.Create(payload);

        try
        {
            var response = await _httpClient.SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogWarning(
                    "Firebase push notification failed for user {Username} (device {DeviceId}) with status {Status}: {Body}",
                    user.Username,
                    device.DeviceId,
                    response.StatusCode,
                    responseBody);
                return false;
            }

            _logger.LogInformation(
                "Login approval push notification sent to user {Username} (device {DeviceId}).",
                user.Username,
                device.DeviceId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error sending Firebase push notification for user {Username} (device {DeviceId}).", user.Username, device.DeviceId);
            return false;
        }
    }
}
