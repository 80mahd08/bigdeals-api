using System.Threading.Tasks;
using api.Common;
using api.Dtos.Admin;

namespace api.Interfaces.Admin;

public interface IAdminUserRepository
{
    Task<PagedResponse<AdminUserListItemDto>> GetPagedUsersAsync(int pageNumber, int pageSize, string? search, int? statutCompte, int? role, string? ville);
    Task<bool> UpdateUserStatusAsync(long idUtilisateur, int newStatus);
    Task<bool> IsAdminAsync(long idUtilisateur);
}
