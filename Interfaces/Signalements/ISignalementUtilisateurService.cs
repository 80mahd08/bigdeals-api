using System.Threading.Tasks;
using api.Common;
using api.Dtos.SignalementsUtilisateurs;

namespace api.Interfaces.Signalements;

public interface ISignalementUtilisateurService
{
    Task<SignalementUtilisateurDto?> CreateAsync(CreateSignalementUtilisateurDto dto, long currentUserId);
    Task<PagedResponse<SignalementUtilisateurDto>> GetPagedAdminAsync(int pageNumber, int pageSize, int? statut, string? search, string? sortByDate = null, int? raison = null);
    Task UpdateStatusAsync(long idSignalement, UpdateSignalementUtilisateurStatusDto dto, long adminId);
}
