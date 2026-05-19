using System.Threading.Tasks;
using api.Dtos.Checkout;

namespace api.Interfaces.Checkout;

public interface ICheckoutService
{
    Task<CreateCheckoutResponseDto> CreateCheckoutAsync(long idAnnonce, long userId, CreateCheckoutRequestDto request);
    Task<CheckoutDetailsDto> GetCheckoutDetailsAsync(long idCommande, long userId);
}
