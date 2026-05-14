using System.ComponentModel.DataAnnotations;

namespace api.Dtos.ProductPayments;

public class MockProductPaymentRequestDto
{
    [Required(ErrorMessage = "Le numéro de commande est obligatoire.")]
    public long IdCommande { get; set; }

    [Required(ErrorMessage = "Le numéro de carte est obligatoire.")]
    [RegularExpression(@"^\d{16}$", ErrorMessage = "Le numéro de carte doit contenir exactement 16 chiffres.")]
    public string CardNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "La date d'expiration est obligatoire.")]
    [RegularExpression(@"^(0[1-9]|1[0-2])\/\d{2}$", ErrorMessage = "La date d’expiration doit être au format MM/AA.")]
    public string Expiry { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le CVV est obligatoire.")]
    [RegularExpression(@"^\d{3}$", ErrorMessage = "Le CVV doit contenir exactement 3 chiffres.")]
    public string Cvv { get; set; } = string.Empty;
}
