using System;
using api.Models.Enums;

namespace api.Models;

public class AnnonceurPayment
{
    public long AnnonceurPaymentId { get; set; }
    public long UserId { get; set; }
    public long DemandeAnnonceurId { get; set; }
    public string Provider { get; set; } = string.Empty;
    public string? ProviderPaymentId { get; set; }
    public string DeveloperTrackingId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public AnnonceurPaymentStatus PaymentStatus { get; set; }
    public string? PaymentUrl { get; set; }
    public string? RawResponseJson { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ConfirmedAt { get; set; }
}
