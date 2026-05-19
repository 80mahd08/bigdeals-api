using System;
using System.Threading.Tasks;
using api.Dtos.Checkout;
using api.Exceptions;
using api.Interfaces.Annonces;
using api.Interfaces.Categories;
using api.Interfaces.Checkout;
using api.Models;
using api.Models.Enums;

using Microsoft.Extensions.Options;
using api.Models.Config;

namespace api.Services.Checkout;

public class CheckoutService : ICheckoutService
{
    private readonly ICheckoutRepository _checkoutRepo;
    private readonly IAnnonceRepository _annonceRepo;
    private readonly BigDealsBusinessSettings _settings;

    public CheckoutService(ICheckoutRepository checkoutRepo, IAnnonceRepository annonceRepo, IOptions<BigDealsBusinessSettings> settings)
    {
        _checkoutRepo = checkoutRepo;
        _annonceRepo = annonceRepo;
        _settings = settings.Value;
    }

    public async Task<CreateCheckoutResponseDto> CreateCheckoutAsync(long idAnnonce, long userId, CreateCheckoutRequestDto request)
    {
        var annonce = await _annonceRepo.GetByIdAsync(idAnnonce);
        if (annonce == null || !annonce.EstActive || annonce.Statut != StatutAnnonce.PUBLIEE)
            throw new NotFoundException("Annonce introuvable ou non disponible.");

        if (annonce.IdUtilisateur == userId)
            throw new BadRequestException("Vous ne pouvez pas acheter votre propre annonce.");

        if (!annonce.SupportePaiement)
            throw new BadRequestException("Le paiement n’est pas disponible pour cette catégorie. Veuillez contacter l’annonceur.");

        if (annonce.Prix <= 0)
            throw new BadRequestException("Le prix de l'annonce doit être supérieur à 0 pour effectuer un paiement.");

        // Check for existing pending order
        var existingPending = await _checkoutRepo.GetPendingCommandeForUserAndAnnonceAsync(userId, idAnnonce);
        if (existingPending != null)
        {
            return new CreateCheckoutResponseDto
            {
                IdCommande = existingPending.IdCommande,
                IdAnnonce = existingPending.IdAnnonce,
                TitreAnnonce = annonce.Titre,
                MontantAnnonce = existingPending.MontantAnnonce,
                FraisLivraison = existingPending.FraisLivraison,
                MontantTotal = existingPending.Montant,
                Montant = existingPending.Montant,
                StatutCommande = existingPending.StatutCommande,
                AnnonceurNom = annonce.AnnonceurNom ?? "Vendeur"
            };
        }

        var montantAnnonce = annonce.Prix;
        var fraisLivraison = _settings.FraisLivraisonFixe;
        var montantTotal = montantAnnonce + fraisLivraison;

        var newCommande = new Commande
        {
            IdAnnonce = idAnnonce,
            IdAcheteur = userId,
            IdAnnonceur = annonce.IdUtilisateur,
            MontantAnnonce = montantAnnonce,
            FraisLivraison = fraisLivraison,
            Montant = montantTotal,
            StatutCommande = StatutCommande.EN_ATTENTE_PAIEMENT,
            StatutLivraison = StatutLivraison.EN_ATTENTE_PREPARATION,
            AdresseLivraison = request.Adresse,
            VilleLivraison = request.Ville,
            TelephoneLivraison = request.Telephone,
            AnnonceTitre = annonce.Titre,
            DateCreation = DateTime.UtcNow
        };

        var newId = await _checkoutRepo.CreateCommandeAsync(newCommande);

        return new CreateCheckoutResponseDto
        {
            IdCommande = newId,
            IdAnnonce = idAnnonce,
            TitreAnnonce = annonce.Titre,
            MontantAnnonce = newCommande.MontantAnnonce,
            FraisLivraison = newCommande.FraisLivraison,
            MontantTotal = newCommande.Montant,
            Montant = newCommande.Montant,
            StatutCommande = newCommande.StatutCommande,
            AnnonceurNom = annonce.AnnonceurNom ?? "Vendeur"
        };
    }

    public async Task<CheckoutDetailsDto> GetCheckoutDetailsAsync(long idCommande, long userId)
    {
        var commande = await _checkoutRepo.GetCommandeByIdAsync(idCommande);
        if (commande == null)
            throw new NotFoundException("Commande introuvable.");

        if (commande.IdAcheteur != userId)
            throw new ForbiddenException("Vous n'êtes pas autorisé à voir cette commande.");

        var annonce = await _annonceRepo.GetByIdAsync(commande.IdAnnonce);

        return new CheckoutDetailsDto
        {
            IdCommande = commande.IdCommande,
            IdAnnonce = commande.IdAnnonce,
            TitreAnnonce = annonce?.Titre ?? "Annonce introuvable",
            MontantAnnonce = commande.MontantAnnonce,
            FraisLivraison = commande.FraisLivraison,
            MontantTotal = commande.Montant,
            Montant = commande.Montant,
            StatutCommande = commande.StatutCommande,
            AnnonceurNom = annonce?.AnnonceurNom ?? "Vendeur",
            DateCreation = commande.DateCreation
        };
    }
}
