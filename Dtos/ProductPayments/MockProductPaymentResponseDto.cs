using api.Models.Enums;

namespace api.Dtos.ProductPayments;

public class MockProductPaymentResponseDto
{
    public long IdCommande { get; set; }
    public long IdPaiementCommande { get; set; }
    public decimal Montant { get; set; }
    public StatutPaiementCommande StatutPaiement { get; set; }
    public string NumeroCarteMasque { get; set; } = string.Empty;
}
