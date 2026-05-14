using api.Models.Enums;

namespace api.Dtos.Checkout;

public class CreateCheckoutResponseDto
{
    public long IdCommande { get; set; }
    public long IdAnnonce { get; set; }
    public string TitreAnnonce { get; set; } = string.Empty;
    public decimal Montant { get; set; }
    public StatutCommande StatutCommande { get; set; }
    public string AnnonceurNom { get; set; } = string.Empty;
}
