using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using api.Common;
using api.Dtos.AnnonceurPayments;
using api.Exceptions;
using api.Interfaces.AnnonceurPayments;
using api.Interfaces.DemandesAnnonceur;
using api.Models;
using api.Models.Enums;

namespace api.Services.AnnonceurPayments;

public class AnnonceurPaymentService : IAnnonceurPaymentService
{
    private readonly IAnnonceurPaymentRepository _paymentRepository;
    private readonly IDemandeAnnonceurRepository _demandeRepository;
    private readonly IConfiguration _configuration;

    public AnnonceurPaymentService(
        IAnnonceurPaymentRepository paymentRepository,
        IDemandeAnnonceurRepository demandeRepository,
        IConfiguration configuration)
    {
        _paymentRepository = paymentRepository;
        _demandeRepository = demandeRepository;
        _configuration = configuration;
    }

    public async Task<InitiateAnnonceurPaymentResponseDto> InitiateMockPaymentAsync(long demandeAnnonceurId, long currentUserId)
    {
        var demande = await _demandeRepository.GetByIdAsync(demandeAnnonceurId);
        if (demande == null)
            throw new NotFoundException("Advertiser request not found.");

        if (demande.IdUtilisateur != currentUserId)
            throw new ForbiddenException("You cannot pay for another user's advertiser request.");

        if (demande.Statut != StatutDemandeAnnonceur.EN_ATTENTE_PAIEMENT)
            throw new BadRequestException("This advertiser request is not ready for payment.");

        var existingPayment = await _paymentRepository.GetByDemandeAnnonceurIdAsync(demandeAnnonceurId);
        if (existingPayment != null)
        {
            if (existingPayment.PaymentStatus == AnnonceurPaymentStatus.Paid)
                throw new ConflictException("This advertiser request has already been paid.");

            // Return existing pending/failed/expired payment as per requirements
            return new InitiateAnnonceurPaymentResponseDto
            {
                AnnonceurPaymentId = existingPayment.AnnonceurPaymentId,
                DemandeAnnonceurId = existingPayment.DemandeAnnonceurId,
                Amount = existingPayment.Amount,
                PaymentUrl = existingPayment.PaymentUrl ?? string.Empty,
                DeveloperTrackingId = existingPayment.DeveloperTrackingId
            };
        }

        decimal fee = _configuration.GetValue<decimal>("BigDealsBusiness:AnnonceurAccessFee", 200.000m);
        if (fee != 200.000m)
            throw new InternalServerException("Invalid annonceur access fee configuration.");

        string trackingId = $"BIGDEALS-ANNONCEUR-{demandeAnnonceurId}-{Guid.NewGuid():N}";
        
        var payment = new AnnonceurPayment
        {
            UserId = currentUserId,
            DemandeAnnonceurId = demandeAnnonceurId,
            Provider = "Mock",
            DeveloperTrackingId = trackingId,
            Amount = fee,
            PaymentStatus = AnnonceurPaymentStatus.Pending,
            PaymentUrl = $"/mock-payment/annonceur/{trackingId}",
            CreatedAt = DateTime.UtcNow
        };

        long paymentId = await _paymentRepository.CreateAsync(payment);

        return new InitiateAnnonceurPaymentResponseDto
        {
            AnnonceurPaymentId = paymentId,
            DemandeAnnonceurId = demandeAnnonceurId,
            Amount = fee,
            PaymentUrl = payment.PaymentUrl,
            DeveloperTrackingId = trackingId
        };
    }

    public async Task<AnnonceurPaymentDto> GetByIdAsync(long annonceurPaymentId, long currentUserId, bool isAdmin)
    {
        var payment = await _paymentRepository.GetByIdAsync(annonceurPaymentId);
        if (payment == null)
            throw new NotFoundException("Payment not found.");

        if (!isAdmin && payment.UserId != currentUserId)
            throw new ForbiddenException("You do not have permission to view this payment.");

        return MapToDto(payment);
    }

    public async Task<IReadOnlyList<AnnonceurPaymentDto>> GetMyPaymentsAsync(long currentUserId)
    {
        return await _paymentRepository.GetByUserIdAsync(currentUserId);
    }

    public async Task<PagedResponse<AnnonceurPaymentDto>> GetAdminPagedAsync(int pageNumber, int pageSize, string? search = null, string? provider = null, int? statutPaiement = null, string? sortByDateCreation = null, string? sortByDateConfirmation = null)
    {
        return await _paymentRepository.GetAdminPagedAsync(pageNumber, pageSize, search, provider, statutPaiement, sortByDateCreation, sortByDateConfirmation);
    }

    public async Task<AnnonceurPaymentDto> MarkMockPaymentAsPaidAsync(long annonceurPaymentId, long adminId)
    {
        var payment = await _paymentRepository.GetByIdAsync(annonceurPaymentId);
        if (payment == null)
            throw new NotFoundException("Annonceur payment not found.");

        if (payment.PaymentStatus == AnnonceurPaymentStatus.Paid)
            return MapToDto(payment);

        if (payment.PaymentStatus != AnnonceurPaymentStatus.Pending)
            throw new BadRequestException("Only pending annonceur payments can be marked as paid.");

        await _paymentRepository.MarkAsPaidAndActivateAnnonceurAsync(annonceurPaymentId, adminId);

        var updatedPayment = await _paymentRepository.GetByIdAsync(annonceurPaymentId);
        return MapToDto(updatedPayment!);
    }

    private AnnonceurPaymentDto MapToDto(AnnonceurPayment payment)
    {
        return new AnnonceurPaymentDto
        {
            AnnonceurPaymentId = payment.AnnonceurPaymentId,
            UserId = payment.UserId,
            DemandeAnnonceurId = payment.DemandeAnnonceurId,
            Provider = payment.Provider,
            ProviderPaymentId = payment.ProviderPaymentId,
            DeveloperTrackingId = payment.DeveloperTrackingId,
            Amount = payment.Amount,
            PaymentStatus = payment.PaymentStatus.ToString(),
            PaymentUrl = payment.PaymentUrl,
            CreatedAt = payment.CreatedAt,
            ConfirmedAt = payment.ConfirmedAt
        };
    }
}
