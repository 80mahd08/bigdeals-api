using System;
using api.Models.Enums;

namespace api.Models;

public class PaiementCommande
{
    public long IdPaiementCommande { get; set; }
    public long IdCommande { get; set; }
    public decimal Montant { get; set; }
    public string MethodePaiement { get; set; } = string.Empty;
    public StatutPaiementCommande StatutPaiement { get; set; }
    public string? NumeroCarteMasque { get; set; }
    public DateTime DatePaiement { get; set; }
}
