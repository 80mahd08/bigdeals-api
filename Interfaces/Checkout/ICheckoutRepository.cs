using System.Threading.Tasks;
using api.Models;

namespace api.Interfaces.Checkout;

public interface ICheckoutRepository
{
    Task<long> CreateCommandeAsync(Commande commande);
    Task<Commande?> GetCommandeByIdAsync(long idCommande);
    Task<Commande?> GetPendingCommandeForUserAndAnnonceAsync(long idAcheteur, long idAnnonce);
}
