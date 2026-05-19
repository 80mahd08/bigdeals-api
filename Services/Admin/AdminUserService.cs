using System.Threading.Tasks;
using api.Common;
using api.Dtos.Admin;
using api.Exceptions;
using api.Interfaces.Admin;
using api.Interfaces.Annonces;

namespace api.Services.Admin;

public class AdminUserService : IAdminUserService
{
    private readonly IAdminUserRepository _userRepository;
    private readonly IAnnonceRepository _annonceRepository;

    public AdminUserService(IAdminUserRepository userRepository, IAnnonceRepository annonceRepository)
    {
        _userRepository = userRepository;
        _annonceRepository = annonceRepository;
    }

    public async Task<PagedResponse<AdminUserListItemDto>> GetUsersAsync(int pageNumber, int pageSize, string? search, int? statutCompte, int? role, string? ville, string? sortByDateInscription = null, string? sortByNbAnnonces = null)
    {
        if (pageSize > 50) pageSize = 50;
        return await _userRepository.GetPagedUsersAsync(pageNumber, pageSize, search, statutCompte, role, ville, sortByDateInscription, sortByNbAnnonces);
    }

    public async Task<bool> BlockUserAsync(long idUtilisateur)
    {
        if (await _userRepository.IsAdminAsync(idUtilisateur))
            throw new ForbiddenException("Impossible de bloquer un compte administrateur.");

        var success = await _userRepository.UpdateUserStatusAsync(idUtilisateur, 2);
        if (!success)
            throw new NotFoundException("Utilisateur non trouvé ou action non autorisée.");

        // Suspend all published ads for this user
        await _annonceRepository.SuspendAllUserAnnoncesAsync(idUtilisateur);

        return true;
    }

    public async Task<bool> UnblockUserAsync(long idUtilisateur)
    {
        if (await _userRepository.IsAdminAsync(idUtilisateur))
            throw new ForbiddenException("Action non autorisée pour un compte administrateur.");

        var success = await _userRepository.UpdateUserStatusAsync(idUtilisateur, 1);
        if (!success)
            throw new NotFoundException("Utilisateur non trouvé ou action non autorisée.");

        // Restore all suspended ads for this user
        await _annonceRepository.RestoreAllUserAnnoncesAsync(idUtilisateur);

        return true;
    }
}
