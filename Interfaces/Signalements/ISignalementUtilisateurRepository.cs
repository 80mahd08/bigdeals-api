using System.Threading.Tasks;
using api.Common;
using api.Dtos.SignalementsUtilisateurs;
using api.Models;

namespace api.Interfaces.Signalements;

public interface ISignalementUtilisateurRepository
{
    Task<long> CreateAsync(SignalementUtilisateur signalement);
    Task<bool> ExistsAsync(long idSignalement);
    Task<bool> HasUserAlreadyReportedAsync(long idUtilisateurSignale, long idUtilisateurReporter);
    Task<PagedResponse<SignalementUtilisateurDto>> GetPagedAdminAsync(int pageNumber, int pageSize, int? statut, string? search, string? sortByDate = null, int? raison = null);
    Task UpdateStatusAsync(long idSignalement, int statut, long adminId);
    Task<long?> GetReportedUserIdAsync(long idSignalement);
}
