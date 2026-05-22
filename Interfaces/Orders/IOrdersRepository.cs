using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using api.Models;
using api.Models.Enums;

namespace api.Interfaces.Orders;

public interface IOrdersRepository
{
    Task<IEnumerable<Commande>> GetOrdersByUserIdAsync(long userId);
    Task<IEnumerable<Commande>> GetOrdersByAnnouncerIdAsync(long announcerId);
    Task<IEnumerable<dynamic>> GetAllOrdersAsync();
    Task<Commande?> GetOrderByIdAsync(long orderId);
    Task<long> CreateOrderAsync(Commande order);
    Task<bool> UpdateOrderStatusAsync(long orderId, int status);
    Task<bool> UpdateDeliveryStatusAsync(long orderId, int statutLivraison, string? notes, DateTime? dateExpedition, DateTime? dateLivraison);
    Task<bool> CancelOrderAndRefundAsync(long orderId);
    Task<bool> HasPurchasedProductAsync(long userId, long annonceId);
}
