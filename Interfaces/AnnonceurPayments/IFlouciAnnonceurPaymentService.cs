using System.Threading.Tasks;
using api.Dtos.AnnonceurPayments;

namespace api.Interfaces.AnnonceurPayments;

public interface IFlouciAnnonceurPaymentService
{
    Task<FlouciGenerateAnnonceurPaymentResponseDto> GeneratePaymentAsync(
        decimal amountTnd,
        string developerTrackingId,
        string clientId
    );

    Task<FlouciVerifyAnnonceurPaymentResponseDto> VerifyPaymentAsync(
        string providerPaymentId
    );
}
