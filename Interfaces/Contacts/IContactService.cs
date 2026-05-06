using System.Threading.Tasks;
using api.Common;
using api.Dtos.Contacts;

namespace api.Interfaces.Contacts;

public interface IContactService
{
    Task<long> TrackContactAsync(ContactCreateRequestDto request, long? currentUserId);
    Task<PagedResponse<ContactResponseDto>> GetMyOutgoingContactsAsync(long userId, int pageNumber, int pageSize);
    Task<PagedResponse<ContactResponseDto>> GetMyIncomingContactsAsync(long advertiserId, int pageNumber, int pageSize);
    Task<PagedResponse<ContactResponseDto>> GetAllContactsAdminAsync(int pageNumber, int pageSize);
}
