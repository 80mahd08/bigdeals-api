namespace api.Models.Config;

public class FlouciSettings
{
    public string BaseUrl { get; set; } = "https://developers.flouci.com/api/v2";
    public string PublicKey { get; set; } = string.Empty;
    public string PrivateKey { get; set; } = string.Empty;
    public string SuccessUrl { get; set; } = string.Empty;
    public string FailUrl { get; set; } = string.Empty;
    public string WebhookUrl { get; set; } = string.Empty;
    public bool AcceptCard { get; set; } = true;
    public int SessionTimeoutSeconds { get; set; } = 1200;
}
