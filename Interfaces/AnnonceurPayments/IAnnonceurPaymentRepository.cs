using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using api.Common;
using api.Dtos.AnnonceurPayments;
using api.Models;
using api.Models.Enums;

namespace api.Interfaces.AnnonceurPayments;

public interface IAnnonceurPaymentRepository
{
    Task<long> CreateAsync(AnnonceurPayment payment);

    Task<AnnonceurPayment?> GetByIdAsync(long annonceurPaymentId);

    Task<AnnonceurPayment?> GetByDemandeAnnonceurIdAsync(long demandeAnnonceurId);

    Task<AnnonceurPayment?> GetByDeveloperTrackingIdAsync(string developerTrackingId);

    Task<AnnonceurPayment?> GetByProviderPaymentIdAsync(string providerPaymentId);

    Task<IReadOnlyList<AnnonceurPaymentDto>> GetByUserIdAsync(long userId);

    Task<PagedResponse<AnnonceurPaymentDto>> GetAdminPagedAsync(int pageNumber, int pageSize, string? search = null, string? provider = null, int? statutPaiement = null, string? sortByDateCreation = null, string? sortByDateConfirmation = null);

    Task UpdateProviderInfoAsync(
        long annonceurPaymentId,
        string? providerPaymentId,
        string? paymentUrl,
        string? rawResponseJson
    );

    Task UpdateStatusAsync(
        long annonceurPaymentId,
        AnnonceurPaymentStatus status,
        string? rawResponseJson = null,
        DateTime? confirmedAt = null
    );

    Task MarkAsPaidAndActivateAnnonceurAsync(long annonceurPaymentId, long adminId);
}
