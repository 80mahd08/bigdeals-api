using System;

namespace api.Dtos.AnnonceurPayments;

public class AnnonceurPaymentDto
{
    public long AnnonceurPaymentId { get; set; }
    public long UserId { get; set; }
    public long DemandeAnnonceurId { get; set; }
    public string Provider { get; set; } = string.Empty;
    public string? ProviderPaymentId { get; set; }
    public string DeveloperTrackingId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string PaymentStatus { get; set; } = string.Empty;
    public string? PaymentUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ConfirmedAt { get; set; }
    
    public string? NomUtilisateur { get; set; }
    public string? PrenomUtilisateur { get; set; }
    public string? EmailUtilisateur { get; set; }
}
