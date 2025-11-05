namespace ItemManager.ApiClient;

public record RegistrationResult(string Username, string DisplayName, string SecretKey, string OtpAuthUri);
