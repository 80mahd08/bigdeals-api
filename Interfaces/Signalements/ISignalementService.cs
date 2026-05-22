using System.Threading.Tasks;
using api.Common;
using api.Dtos.Signalements;

namespace api.Interfaces.Signalements;

public interface ISignalementService
{
    Task<SignalementDto?> CreateAsync(CreateSignalementDto dto, long currentUserId);
    Task<PagedResponse<SignalementDto>> GetPagedAdminAsync(int pageNumber, int pageSize, int? statut, string? search, string? sortByDate = null, int? type = null);
    Task UpdateStatusAsync(long idSignalement, UpdateSignalementStatusDto dto, long adminId);
}
