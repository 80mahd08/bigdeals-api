using System.Threading.Tasks;
using api.Common;
using api.Dtos.Admin;

namespace api.Interfaces.Admin;

public interface IAdminUserService
{
    Task<PagedResponse<AdminUserListItemDto>> GetUsersAsync(int pageNumber, int pageSize, string? search, int? statutCompte, int? role, string? ville);
    Task<bool> BlockUserAsync(long idUtilisateur);
    Task<bool> UnblockUserAsync(long idUtilisateur);
}
