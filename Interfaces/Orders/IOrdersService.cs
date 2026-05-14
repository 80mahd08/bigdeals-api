using System.Collections.Generic;
using System.Threading.Tasks;
using api.Common;
using api.Models;

namespace api.Interfaces.Orders;

public class CreateOrderRequest
{
    public string MethodePaiement { get; set; } = string.Empty;
    public List<OrderLineRequest> Lignes { get; set; } = new();

    // Delivery address
    public string? AdresseLivraison { get; set; }
    public string? VilleLivraison { get; set; }
    public string? TelephoneLivraison { get; set; }

}

public class OrderLineRequest
{
    public long IdAnnonce { get; set; }
    public int Quantite { get; set; }
}

public class UpdateDeliveryStatusRequest
{
    public int StatutLivraison { get; set; }

}

public interface IOrdersService
{
    Task<ApiResponse<IEnumerable<Commande>>> GetUserOrdersAsync(long userId);
    Task<ApiResponse<IEnumerable<Commande>>> GetAnnouncerOrdersAsync(long announcerId);
    Task<ApiResponse<IEnumerable<dynamic>>> GetAllOrdersAsync();
    Task<ApiResponse<Commande>> CheckoutAsync(long userId, CreateOrderRequest request);
    Task<ApiResponse<bool>> UpdateDeliveryStatusAsync(long orderId, long actorUserId, UpdateDeliveryStatusRequest request, bool isAdmin);
}
