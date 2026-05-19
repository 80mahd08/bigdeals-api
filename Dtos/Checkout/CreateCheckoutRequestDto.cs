using System.ComponentModel.DataAnnotations;

namespace api.Dtos.Checkout;

public class CreateCheckoutRequestDto
{
    [Required]
    public string Adresse { get; set; } = string.Empty;

    [Required]
    public string Ville { get; set; } = string.Empty;

    [Required]
    [RegularExpression(@"^[2579][0-9]{7}$", ErrorMessage = "Numéro de téléphone invalide.")]
    public string Telephone { get; set; } = string.Empty;
}
