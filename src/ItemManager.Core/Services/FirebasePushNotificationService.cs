using Google.Apis.Auth.OAuth2;
using System.Net.Http.Json;
using System.Text.Json;
using ItemManager.Core.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ItemManager.Core.Models;


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
        try
        {
            var credential = GoogleCredential
                .FromFile(_options.ServiceAccountPath)
                .CreateScoped("https://www.googleapis.com/auth/firebase.messaging");

            var accessToken = await credential.UnderlyingCredential.GetAccessTokenForRequestAsync();

            var projectId = _options.ProjectId;
            var endpoint = $"https://fcm.googleapis.com/v1/projects/{projectId}/messages:send";

            var payload = new
            {
                message = new
                {
                    token = device.RegistrationToken,
                    notification = new
                    {
                        title = string.IsNullOrWhiteSpace(_options.LoginTitle) ? "Confirmar acceso" : _options.LoginTitle,
                        body = string.Format(
                            string.IsNullOrWhiteSpace(_options.LoginBody) ? "Aprobá el inicio de sesión de {0}" : _options.LoginBody!,
                            user.DisplayName)
                    },
                    data = new Dictionary<string, string>
                    {
                        ["type"] = "login_approval",
                        ["challengeId"] = challenge.Id,
                        ["username"] = user.Username,
                        ["deviceId"] = device.DeviceId
                    }
                }
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            request.Content = JsonContent.Create(payload);

            var response = await _httpClient.SendAsync(request, cancellationToken);
            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Firebase v1 failed: {Status} {Body}", response.StatusCode, responseBody);
                return false;
            }

            _logger.LogInformation("Notificación enviada correctamente: {Body}", responseBody);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error enviando notificación FCM");
            return false;
        }
    }
}
