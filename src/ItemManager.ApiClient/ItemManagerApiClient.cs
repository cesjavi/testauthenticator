using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ItemManager.Core.Models;

namespace ItemManager.ApiClient;

public class ItemManagerApiClient
{
    private readonly HttpClient _httpClient;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public ItemManagerApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ApiResult<AuthSession>> LoginAsync(string username, string password, string otpCode)
    {
        try
        {
            var request = new LoginRequest(username, password, otpCode);
            var response = await _httpClient.PostAsJsonAsync("/auth/login", request);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return ApiResult<AuthSession>.Fail("Credenciales o código TOTP inválidos.");
            }

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                return ApiResult<AuthSession>.Fail($"Error {response.StatusCode:D}: {error}");
            }

            var payload = await response.Content.ReadFromJsonAsync<AuthResponse>(JsonOptions);
            if (payload is null || payload.Token is null)
            {
                return ApiResult<AuthSession>.Fail("La respuesta del servidor no tiene el formato esperado.");
            }

            var session = new AuthSession(payload.Token, payload.User.Username, payload.User.DisplayName);
            return ApiResult<AuthSession>.Ok(session);
        }
        catch (Exception ex)
        {
            return ApiResult<AuthSession>.Fail($"No se pudo contactar al servidor: {ex.Message}");
        }
    }

    public async Task<ApiResult<RegistrationResult>> RegisterUserAsync(string username, string displayName, string password)
    {
        try
        {
            var request = new RegisterUserRequest(username, displayName, password);
            var response = await _httpClient.PostAsJsonAsync("/auth/register", request);

            if (response.StatusCode == HttpStatusCode.Conflict)
            {
                var raw = await response.Content.ReadAsStringAsync();
                var message = ExtractErrorMessage(raw, "El usuario ya existe. Elegí otro nombre de usuario.");
                return ApiResult<RegistrationResult>.Fail(message);
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var raw = await response.Content.ReadAsStringAsync();
                var message = ExtractErrorMessage(raw, "La solicitud de registro es inválida.");
                return ApiResult<RegistrationResult>.Fail(message);
            }

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                return ApiResult<RegistrationResult>.Fail($"Error {response.StatusCode:D}: {error}");
            }

            var payload = await response.Content.ReadFromJsonAsync<RegisterUserResponse>(JsonOptions);
            if (payload?.User is null || string.IsNullOrWhiteSpace(payload.SecretKey) || string.IsNullOrWhiteSpace(payload.OtpAuthUri))
            {
                return ApiResult<RegistrationResult>.Fail("La respuesta del servidor no tiene el formato esperado.");
            }

            var result = new RegistrationResult(
                payload.User.Username,
                payload.User.DisplayName,
                payload.SecretKey,
                payload.OtpAuthUri);

            return ApiResult<RegistrationResult>.Ok(result);
        }
        catch (Exception ex)
        {
            return ApiResult<RegistrationResult>.Fail($"No se pudo contactar al servidor: {ex.Message}");
        }
    }

    public async Task<ApiResult<PushDeviceRegistration>> RegisterPushDeviceAsync(PushDeviceRegistrationRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/auth/push/register", request);

            if (!response.IsSuccessStatusCode)
            {
                var raw = await response.Content.ReadAsStringAsync();
                var message = ExtractErrorMessage(raw, "No se pudo registrar el dispositivo para notificaciones push.");
                return ApiResult<PushDeviceRegistration>.Fail(message, response.StatusCode == HttpStatusCode.Unauthorized);
            }

            var payload = await response.Content.ReadFromJsonAsync<PushDeviceRegistrationResponse>(JsonOptions);
            if (payload?.Device is null)
            {
                return ApiResult<PushDeviceRegistration>.Fail("La respuesta del servidor no tiene el formato esperado.");
            }

            var registration = new PushDeviceRegistration(
                payload.Device.DeviceId,
                payload.Device.DeviceName,
                payload.Device.RegisteredAt);

            return ApiResult<PushDeviceRegistration>.Ok(registration);
        }
        catch (Exception ex)
        {
            return ApiResult<PushDeviceRegistration>.Fail($"No se pudo contactar al servidor: {ex.Message}");
        }
    }

    public async Task<ApiResult<PushLoginChallenge>> InitiatePushLoginAsync(PushLoginRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/auth/push/login", request);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return ApiResult<PushLoginChallenge>.Fail("Credenciales inválidas para el login push.");
            }

            if (response.StatusCode == HttpStatusCode.Conflict)
            {
                var raw = await response.Content.ReadAsStringAsync();
                var message = ExtractErrorMessage(raw, "El usuario no tiene dispositivos registrados para push.");
                return ApiResult<PushLoginChallenge>.Fail(message);
            }

            if (response.StatusCode == HttpStatusCode.Accepted)
            {
                var payload = await response.Content.ReadFromJsonAsync<PushLoginResponse>(JsonOptions);
                if (payload?.Challenge is null || payload.User is null)
                {
                    return ApiResult<PushLoginChallenge>.Fail("La respuesta del servidor no tiene el formato esperado.");
                }

                var devices = payload.Challenge.Devices
                    .Select(d => new PushChallengeTarget(d.DeviceId, d.DeviceName))
                    .ToList()
                    .AsReadOnly();

                var result = new PushLoginChallenge(
                    payload.Challenge.Id,
                    payload.Challenge.ExpiresAt,
                    devices,
                    payload.User.Username,
                    payload.User.DisplayName);

                return ApiResult<PushLoginChallenge>.Ok(result);
            }

            var rawError = await response.Content.ReadAsStringAsync();
            var genericMessage = ExtractErrorMessage(rawError, "No se pudo iniciar el desafío push.");
            return ApiResult<PushLoginChallenge>.Fail(genericMessage);
        }
        catch (Exception ex)
        {
            return ApiResult<PushLoginChallenge>.Fail($"No se pudo contactar al servidor: {ex.Message}");
        }
    }

    public async Task<ApiResult<AuthSession>> ConfirmPushLoginAsync(PushLoginConfirmationRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/auth/push/confirm", request);

            if (response.StatusCode == HttpStatusCode.Gone)
            {
                var raw = await response.Content.ReadAsStringAsync();
                var message = ExtractErrorMessage(raw, "El desafío expiró. Inicia sesión nuevamente.");
                return ApiResult<AuthSession>.Fail(message);
            }

            if (!response.IsSuccessStatusCode)
            {
                var raw = await response.Content.ReadAsStringAsync();
                var message = ExtractErrorMessage(raw, "No se pudo completar el login push.");
                return ApiResult<AuthSession>.Fail(message);
            }

            var payload = await response.Content.ReadFromJsonAsync<AuthResponse>(JsonOptions);
            if (payload is null || string.IsNullOrWhiteSpace(payload.Token))
            {
                return ApiResult<AuthSession>.Fail("La respuesta del servidor no tiene el formato esperado.");
            }

            var session = new AuthSession(payload.Token, payload.User.Username, payload.User.DisplayName);
            return ApiResult<AuthSession>.Ok(session);
        }
        catch (Exception ex)
        {
            return ApiResult<AuthSession>.Fail($"No se pudo contactar al servidor: {ex.Message}");
        }
    }

    public async Task<ApiResult<IReadOnlyList<Item>>> GetItemsAsync(string sessionToken)
    {
        var result = await SendAsync<List<Item>>(HttpMethod.Get, "/items", sessionToken);
        if (!result.Success)
        {
            return ApiResult<IReadOnlyList<Item>>.Fail(result.ErrorMessage, result.RequiresReauthentication);
        }

        var list = result.Payload ?? new List<Item>();
        return ApiResult<IReadOnlyList<Item>>.Ok(list);
    }

    public async Task<ApiResult<Item>> CreateItemAsync(string sessionToken, ItemInput input)
        => await SendAsync<Item>(HttpMethod.Post, "/items", sessionToken, input);

    public async Task<ApiResult<Item>> UpdateItemAsync(string sessionToken, int id, ItemInput input)
        => await SendAsync<Item>(HttpMethod.Put, $"/items/{id}", sessionToken, input);

    public async Task<ApiResult<bool>> DeleteItemAsync(string sessionToken, int id)
    {
        var result = await SendAsync<object?>(HttpMethod.Delete, $"/items/{id}", sessionToken);
        if (!result.Success)
        {
            return ApiResult<bool>.Fail(result.ErrorMessage, result.RequiresReauthentication);
        }

        return ApiResult<bool>.Ok(true);
    }

    private async Task<ApiResult<T>> SendAsync<T>(HttpMethod method, string path, string sessionToken, object? payload = null)
    {
        try
        {
            using var request = new HttpRequestMessage(method, path);
            request.Headers.Add("X-Session-Token", sessionToken);

            if (payload is not null)
            {
                request.Content = JsonContent.Create(payload);
            }

            using var response = await _httpClient.SendAsync(request);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return ApiResult<T>.Fail("Sesión expirada o inválida. Inicia sesión nuevamente.", true);
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                var message = await response.Content.ReadAsStringAsync();
                var error = string.IsNullOrWhiteSpace(message) ? "Recurso no encontrado." : message;
                return ApiResult<T>.Fail(error);
            }

            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return ApiResult<T>.Ok(default!);
            }

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                return ApiResult<T>.Fail($"Error {response.StatusCode:D}: {error}");
            }

            var content = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(content))
            {
                return ApiResult<T>.Ok(default!);
            }

            try
            {
                var model = JsonSerializer.Deserialize<T>(content, JsonOptions);
                if (model is null)
                {
                    return ApiResult<T>.Fail("No se pudo deserializar la respuesta del servidor.");
                }

                return ApiResult<T>.Ok(model);
            }
            catch (JsonException ex)
            {
                return ApiResult<T>.Fail($"No se pudo deserializar la respuesta del servidor: {ex.Message}");
            }
        }
        catch (Exception ex)
        {
            return ApiResult<T>.Fail($"No se pudo contactar al servidor: {ex.Message}");
        }
    }

    private sealed record AuthResponse(string Token, UserSummary User);

    private sealed record RegisterUserResponse(UserSummary User, string SecretKey, string OtpAuthUri);

    private sealed record PushDeviceRegistrationResponse(PushDeviceSummary Device);

    private sealed record PushDeviceSummary(string DeviceId, string DeviceName, DateTimeOffset RegisteredAt);

    private sealed record PushLoginResponse(PushChallengePayload Challenge, UserSummary User);

    private sealed record PushChallengePayload(string Id, DateTimeOffset ExpiresAt, List<PushChallengeDevicePayload> Devices);

    private sealed record PushChallengeDevicePayload(string DeviceId, string DeviceName);

    private sealed record UserSummary(string Username, string DisplayName);

    private static string ExtractErrorMessage(string raw, string fallback)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return fallback;
        }

        try
        {
            using var document = JsonDocument.Parse(raw);
            if (document.RootElement.TryGetProperty("error", out var errorElement) && errorElement.ValueKind == JsonValueKind.String)
            {
                var message = errorElement.GetString();
                if (!string.IsNullOrWhiteSpace(message))
                {
                    return message!;
                }
            }
        }
        catch (JsonException)
        {
            // ignoramos errores de formato y devolvemos el contenido original
        }

        return raw;
    }
}
