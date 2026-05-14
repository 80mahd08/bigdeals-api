using System;
using System.Threading.Tasks;
using api.Dtos.ProductPayments;
using api.Exceptions;
using api.Interfaces.Checkout;
using api.Interfaces.ProductPayments;
using api.Models;
using api.Models.Enums;

namespace api.Services.ProductPayments;

public class ProductPaymentService : IProductPaymentService
{
    private readonly IProductPaymentRepository _paymentRepo;
    private readonly ICheckoutRepository _checkoutRepo;

    public ProductPaymentService(IProductPaymentRepository paymentRepo, ICheckoutRepository checkoutRepo)
    {
        _paymentRepo = paymentRepo;
        _checkoutRepo = checkoutRepo;
    }

    public async Task<MockProductPaymentResponseDto> ProcessMockPaymentAsync(MockProductPaymentRequestDto request, long userId)
    {
        var commande = await _checkoutRepo.GetCommandeByIdAsync(request.IdCommande);
        if (commande == null)
            throw new NotFoundException("Commande introuvable.");

        if (commande.IdAcheteur != userId)
            throw new ForbiddenException("Vous n'êtes pas autorisé à payer cette commande.");

        if (commande.StatutCommande != StatutCommande.EN_ATTENTE_PAIEMENT)
            throw new BadRequestException("Cette commande ne peut pas être payée dans son statut actuel.");

        // Simulate basic validation
        if (request.CardNumber.StartsWith("0000"))
            throw new BadRequestException("Paiement refusé : Carte non valide ou bloquée (Simulation).");

        string maskedCard = $"**** **** **** {request.CardNumber.Substring(request.CardNumber.Length - 4)}";

        var paiement = new PaiementCommande
        {
            IdCommande = commande.IdCommande,
            Montant = commande.Montant,
            MethodePaiement = "CARTE_BANCAIRE",
            StatutPaiement = StatutPaiementCommande.ACCEPTE,
            NumeroCarteMasque = maskedCard,
            DatePaiement = DateTime.UtcNow
        };

        var newPaiementId = await _paymentRepo.CreatePaiementCommandeAsync(paiement);

        await _paymentRepo.UpdateCommandeStatutAsync(commande.IdCommande, StatutCommande.PAYEE);

        return new MockProductPaymentResponseDto
        {
            IdCommande = commande.IdCommande,
            IdPaiementCommande = newPaiementId,
            Montant = paiement.Montant,
            StatutPaiement = paiement.StatutPaiement,
            NumeroCarteMasque = maskedCard
        };
    }
}
