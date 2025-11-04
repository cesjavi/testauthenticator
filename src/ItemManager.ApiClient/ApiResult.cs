namespace ItemManager.ApiClient;

public class ApiResult<T>
{
    private ApiResult(bool success, T? payload, string? errorMessage, bool requiresReauthentication)
    {
        Success = success;
        Payload = payload;
        ErrorMessage = errorMessage;
        RequiresReauthentication = requiresReauthentication;
    }

    public bool Success { get; }
    public T? Payload { get; }
    public string? ErrorMessage { get; }
    public bool RequiresReauthentication { get; }

    public static ApiResult<T> Ok(T payload) => new(true, payload, null, false);

    public static ApiResult<T> Fail(string? errorMessage, bool requiresReauthentication = false)
        => new(false, default, errorMessage, requiresReauthentication);
}
