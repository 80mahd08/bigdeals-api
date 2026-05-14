using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Common;
using api.Interfaces.Annonces;
using api.Interfaces.Orders;
using api.Models;
using api.Models.Enums;

namespace api.Services.Orders;

public class OrdersService : IOrdersService
{
    private readonly IOrdersRepository _ordersRepository;
    private readonly IAnnonceRepository _annonceRepository;

    // Valid delivery state transitions
    private static readonly Dictionary<StatutLivraison, StatutLivraison[]> _validTransitions = new()
    {
        { StatutLivraison.EN_ATTENTE_PREPARATION, new[] { StatutLivraison.EN_PREPARATION, StatutLivraison.ANNULEE } },
        { StatutLivraison.EN_PREPARATION, new[] { StatutLivraison.EXPEDIEE, StatutLivraison.ANNULEE } },
        { StatutLivraison.EXPEDIEE, new[] { StatutLivraison.LIVREE, StatutLivraison.ECHEC_LIVRAISON } },
        { StatutLivraison.ECHEC_LIVRAISON, new[] { StatutLivraison.RETOURNEE, StatutLivraison.EXPEDIEE } },
    };

    public OrdersService(IOrdersRepository ordersRepository, IAnnonceRepository annonceRepository)
    {
        _ordersRepository = ordersRepository;
        _annonceRepository = annonceRepository;
    }

    public async Task<ApiResponse<IEnumerable<Commande>>> GetUserOrdersAsync(long userId)
    {
        var orders = await _ordersRepository.GetOrdersByUserIdAsync(userId);
        return ApiResponse<IEnumerable<Commande>>.Ok(orders);
    }

    public async Task<ApiResponse<IEnumerable<Commande>>> GetAnnouncerOrdersAsync(long announcerId)
    {
        var orders = await _ordersRepository.GetOrdersByAnnouncerIdAsync(announcerId);
        return ApiResponse<IEnumerable<Commande>>.Ok(orders);
    }

    public async Task<ApiResponse<IEnumerable<dynamic>>> GetAllOrdersAsync()
    {
        var orders = await _ordersRepository.GetAllOrdersAsync();
        return ApiResponse<IEnumerable<dynamic>>.Ok(orders);
    }

    public async Task<ApiResponse<Commande>> CheckoutAsync(long userId, CreateOrderRequest request)
    {
        if (request.Lignes == null || !request.Lignes.Any())
            return ApiResponse<Commande>.Fail("Le panier est vide.");

        // Validate telephone
        if (string.IsNullOrWhiteSpace(request.TelephoneLivraison) || 
            request.TelephoneLivraison.Length != 8 || 
            !System.Text.RegularExpressions.Regex.IsMatch(request.TelephoneLivraison, "^[2579][0-9]{7}$"))
        {
            return ApiResponse<Commande>.Fail("Le numéro de téléphone doit comporter exactement 8 chiffres et commencer par 2, 5, 7 ou 9.");
        }

        Commande? firstOrder = null;

        // Since the database Commandes table only supports one IdAnnonce per order,
        // we create one order record per line item for now.
        // In a real scenario with cart-based orders, we'd have an OrderLines table.
        
        foreach (var line in request.Lignes)
        {
            var annonce = await _annonceRepository.GetByIdAsync(line.IdAnnonce);
            if (annonce == null) continue;

            var order = new Commande
            {
                IdAnnonce = line.IdAnnonce,
                IdAcheteur = userId,
                IdAnnonceur = annonce.IdUtilisateur,
                Montant = annonce.Prix * line.Quantite,
                StatutCommande = StatutCommande.PAYEE, // Assuming immediate validation for mock
                StatutLivraison = StatutLivraison.EN_ATTENTE_PREPARATION,
                AdresseLivraison = request.AdresseLivraison,
                VilleLivraison = request.VilleLivraison,
                TelephoneLivraison = request.TelephoneLivraison,
                AnnonceTitre = annonce.Titre,

                DateCreation = DateTime.UtcNow
            };

            var id = await _ordersRepository.CreateOrderAsync(order);
            order.IdCommande = id;
            
            if (firstOrder == null) firstOrder = order;
        }

        if (firstOrder == null)
            return ApiResponse<Commande>.Fail("Impossible de créer la commande.");

        return ApiResponse<Commande>.Ok(firstOrder, "Commande validée avec succès.");
    }

    public async Task<ApiResponse<bool>> UpdateDeliveryStatusAsync(long orderId, long actorUserId, UpdateDeliveryStatusRequest request, bool isAdmin)
    {
        var order = await _ordersRepository.GetOrderByIdAsync(orderId);
        if (order == null)
            return ApiResponse<bool>.Fail("Commande introuvable.");

        // Only the announcer who owns the order can update (unless admin or buyer canceling)
        bool isBuyer = order.IdAcheteur == actorUserId;
        bool isAnnouncer = order.IdAnnonceur == actorUserId;

        if (!isAdmin && !isAnnouncer && !isBuyer)
            return ApiResponse<bool>.Fail("Vous n'êtes pas autorisé à modifier cette commande.");

        var newStatus = (StatutLivraison)request.StatutLivraison;

        // Buyer can ONLY cancel (status 7)
        if (isBuyer && !isAdmin && !isAnnouncer && newStatus != StatutLivraison.ANNULEE)
            return ApiResponse<bool>.Fail("En tant que client, vous pouvez uniquement annuler la livraison.");

        // Cannot update delivery if order is not paid
        if (order.StatutCommande != StatutCommande.PAYEE)
            return ApiResponse<bool>.Fail("La commande doit être payée avant de mettre à jour la livraison.");

        var currentStatus = order.StatutLivraison;


        // Validate transition
        if (!_validTransitions.ContainsKey(currentStatus) || !_validTransitions[currentStatus].Contains(newStatus))
        {
            return ApiResponse<bool>.Fail(
                $"Transition invalide : {GetFrenchLabel(currentStatus)} → {GetFrenchLabel(newStatus)}.");
        }

        // Set dates based on status
        DateTime? dateExpedition = null;
        DateTime? dateLivraison = null;

        if (newStatus == StatutLivraison.EXPEDIEE)
            dateExpedition = DateTime.UtcNow;
        else if (newStatus == StatutLivraison.LIVREE)
            dateLivraison = DateTime.UtcNow;

        var success = await _ordersRepository.UpdateDeliveryStatusAsync(
            orderId, (int)newStatus, null, dateExpedition, dateLivraison);

        if (!success)
            return ApiResponse<bool>.Fail("Échec de la mise à jour.");

        return ApiResponse<bool>.Ok(true, $"Livraison mise à jour : {GetFrenchLabel(newStatus)}.");
    }

    private static string GetFrenchLabel(StatutLivraison status) => status switch
    {
        StatutLivraison.EN_ATTENTE_PREPARATION => "En attente de préparation",
        StatutLivraison.EN_PREPARATION => "En préparation",
        StatutLivraison.EXPEDIEE => "Expédiée",
        StatutLivraison.LIVREE => "Livrée",
        StatutLivraison.ECHEC_LIVRAISON => "Échec de livraison",
        StatutLivraison.RETOURNEE => "Retournée",
        StatutLivraison.ANNULEE => "Annulée",
        _ => status.ToString()
    };
}
