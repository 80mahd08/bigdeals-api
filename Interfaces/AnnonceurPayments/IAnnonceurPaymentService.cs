using System.Collections.Generic;
using System.Threading.Tasks;
using api.Common;
using api.Dtos.AnnonceurPayments;

namespace api.Interfaces.AnnonceurPayments;

public interface IAnnonceurPaymentService
{
    Task<InitiateAnnonceurPaymentResponseDto> InitiateMockPaymentAsync(long demandeAnnonceurId, long currentUserId);

    Task<AnnonceurPaymentDto> GetByIdAsync(long annonceurPaymentId, long currentUserId, bool isAdmin);

    Task<IReadOnlyList<AnnonceurPaymentDto>> GetMyPaymentsAsync(long currentUserId);

    Task<PagedResponse<AnnonceurPaymentDto>> GetAdminPagedAsync(int pageNumber, int pageSize, string? search = null, string? provider = null, int? statutPaiement = null, string? sortByDateCreation = null, string? sortByDateConfirmation = null);

    Task<AnnonceurPaymentDto> MarkMockPaymentAsPaidAsync(long annonceurPaymentId, long adminId);
}
