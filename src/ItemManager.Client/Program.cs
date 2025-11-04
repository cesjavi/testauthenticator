using ItemManager.ClientApp;

var baseUrlArg = args.FirstOrDefault(arg => arg.StartsWith("--url=", StringComparison.OrdinalIgnoreCase));
var baseUrl = baseUrlArg?.Split('=', 2)[1]?.Trim();
baseUrl = string.IsNullOrWhiteSpace(baseUrl)
    ? Environment.GetEnvironmentVariable("ITEM_MANAGER_BASE_URL")
    : baseUrl;

baseUrl = string.IsNullOrWhiteSpace(baseUrl) ? "http://localhost:5000" : baseUrl;

if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out var baseUri))
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"La URL '{baseUrl}' no es válida. Usa --url=http://localhost:5000");
    Console.ResetColor();
    return;
}

using var httpClient = new HttpClient
{
    BaseAddress = baseUri,
    Timeout = TimeSpan.FromSeconds(30)
};

var apiClient = new ItemManagerApiClient(httpClient);
var app = new CliApplication(apiClient, baseUri);
await app.RunAsync();
