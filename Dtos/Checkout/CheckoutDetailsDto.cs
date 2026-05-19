using System;
using api.Models.Enums;

namespace api.Dtos.Checkout;

public class CheckoutDetailsDto
{
    public long IdCommande { get; set; }
    public long IdAnnonce { get; set; }
    public string TitreAnnonce { get; set; } = string.Empty;
    public decimal MontantAnnonce { get; set; }
    public decimal FraisLivraison { get; set; }
    public decimal MontantTotal { get; set; }
    public decimal Montant { get; set; }
    public StatutCommande StatutCommande { get; set; }
    public string AnnonceurNom { get; set; } = string.Empty;
    public DateTime DateCreation { get; set; }
}
