using System.Threading.Tasks;
using api.Dtos.Users;
using api.Models;

namespace api.Interfaces.Users;

public interface IUserService
{
    Task<UserProfileDto> GetCurrentUserAsync(long idUtilisateur);
    Task<UserProfileDto> UpdateCurrentUserAsync(long idUtilisateur, UpdateUserProfileDto request);
    Task ChangePasswordAsync(long idUtilisateur, ChangePasswordRequestDto request);
    Task DeleteCurrentUserAsync(long idUtilisateur);
    Task<(byte[] Content, string ContentType, string FileName)> GetProfilePhotoAsync(long idUtilisateur);
    Task<PublicProfileDto> GetPublicProfileAsync(long idUtilisateur);
}

public interface IUserRepository
{
    Task<Utilisateur?> GetByIdAsync(long idUtilisateur);
    Task UpdateUserAsync(Utilisateur user);
    Task UpdatePasswordAsync(long idUtilisateur, string newPasswordHash);
    Task DeleteUserAsync(long idUtilisateur);
}
