namespace ItemManager.Core.Models;

public class FirebasePushOptions
{
    public string ServerKey { get; set; } = string.Empty;
    public string? ProjectId { get; set; }
    public string? ApplicationId { get; set; }
    public string? SenderId { get; set; }
    public string? LoginTitle { get; set; }
    public string? LoginBody { get; set; }
}
