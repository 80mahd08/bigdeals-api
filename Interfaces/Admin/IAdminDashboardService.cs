using System.Threading.Tasks;
using api.Dtos.Admin;

namespace api.Interfaces.Admin;

public interface IAdminDashboardService
{
    Task<AdminDashboardStatsDto> GetDashboardStatsAsync();
}
