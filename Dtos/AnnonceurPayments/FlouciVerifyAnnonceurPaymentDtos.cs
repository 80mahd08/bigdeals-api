using System.Text.Json.Serialization;

namespace api.Dtos.AnnonceurPayments;

public class FlouciVerifyAnnonceurPaymentResponseDto
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("code")]
    public int? Code { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("result")]
    public FlouciVerifyAnnonceurPaymentResultDto? Result { get; set; }
}

public class FlouciVerifyAnnonceurPaymentResultDto
{
    [JsonPropertyName("payment_id")]
    public string? PaymentId { get; set; }

    [JsonPropertyName("developer_tracking_id")]
    public string? DeveloperTrackingId { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [JsonPropertyName("amount")]
    public decimal? Amount { get; set; }

    [JsonPropertyName("success")]
    public bool? Success { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }
}
