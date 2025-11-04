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

    private sealed record UserSummary(string Username, string DisplayName);
}
