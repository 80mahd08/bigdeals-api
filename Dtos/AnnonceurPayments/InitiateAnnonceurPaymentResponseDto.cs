namespace api.Dtos.AnnonceurPayments;

public class InitiateAnnonceurPaymentResponseDto
{
    public long AnnonceurPaymentId { get; set; }
    public long DemandeAnnonceurId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentUrl { get; set; } = string.Empty;
    public string DeveloperTrackingId { get; set; } = string.Empty;
}
