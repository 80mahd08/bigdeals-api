using System.Threading.Tasks;
using api.Common;
using api.Dtos.Signalements;
using api.Models;

namespace api.Interfaces.Signalements;

public interface ISignalementRepository
{
    Task<bool> AnnonceExistsAndIsPublicAsync(long idAnnonce);
    Task<long?> GetAnnonceOwnerIdAsync(long idAnnonce);
    Task<bool> HasUserAlreadyReportedAsync(long idAnnonce, long idUtilisateur);
    Task<int?> GetAnnonceStatusAsync(long idAnnonce);
    Task<long> CreateAsync(Signalement signalement);
    Task<PagedResponse<SignalementDto>> GetPagedAdminAsync(int pageNumber, int pageSize, int? statut, string? search, string? sortByDate = null, int? type = null);
    Task<bool> ExistsAsync(long idSignalement);
    Task UpdateStatusAsync(long idSignalement, int statut, long adminId);
    Task<long?> GetAnnonceIdAsync(long idSignalement);
    Task<bool> HasAdBeenModeratedAsync(long idAnnonce);
}
