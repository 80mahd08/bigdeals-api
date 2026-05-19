using System.Threading.Tasks;
using api.Models;
using api.Models.Enums;

namespace api.Interfaces.ProductPayments;

public interface IProductPaymentRepository
{
    Task<long> CreatePaiementCommandeAsync(PaiementCommande paiement);
    Task UpdateCommandeStatutAsync(long idCommande, StatutCommande statut);
    Task<PaiementCommande?> GetPaymentByOrderIdAsync(long orderId);
    Task<bool> UpdatePaymentStatusAsync(long paymentId, StatutPaiementCommande status);
}
