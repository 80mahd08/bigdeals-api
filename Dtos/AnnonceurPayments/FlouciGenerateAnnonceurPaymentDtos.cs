using System.Text.Json.Serialization;

namespace api.Dtos.AnnonceurPayments;

public class FlouciGenerateAnnonceurPaymentRequestDto
{
    [JsonPropertyName("amount")]
    public string Amount { get; set; } = string.Empty;

    [JsonPropertyName("developer_tracking_id")]
    public string DeveloperTrackingId { get; set; } = string.Empty;

    [JsonPropertyName("success_link")]
    public string SuccessLink { get; set; } = string.Empty;

    [JsonPropertyName("fail_link")]
    public string FailLink { get; set; } = string.Empty;

    [JsonPropertyName("webhook")]
    public string? Webhook { get; set; }

    [JsonPropertyName("accept_card")]
    public bool AcceptCard { get; set; }

    [JsonPropertyName("client_id")]
    public string? ClientId { get; set; }

    [JsonPropertyName("session_timeout_secs")]
    public int? SessionTimeoutSecs { get; set; }
}

public class FlouciGenerateAnnonceurPaymentResponseDto
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("code")]
    public int? Code { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("result")]
    public FlouciGenerateAnnonceurPaymentResultDto? Result { get; set; }
}

public class FlouciGenerateAnnonceurPaymentResultDto
{
    [JsonPropertyName("payment_id")]
    public string? PaymentId { get; set; }

    [JsonPropertyName("link")]
    public string? Link { get; set; }

    [JsonPropertyName("developer_tracking_id")]
    public string? DeveloperTrackingId { get; set; }

    [JsonPropertyName("success")]
    public bool? Success { get; set; }
}
