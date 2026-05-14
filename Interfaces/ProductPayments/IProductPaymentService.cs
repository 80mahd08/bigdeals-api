using System.Threading.Tasks;
using api.Dtos.ProductPayments;

namespace api.Interfaces.ProductPayments;

public interface IProductPaymentService
{
    Task<MockProductPaymentResponseDto> ProcessMockPaymentAsync(MockProductPaymentRequestDto request, long userId);
}
