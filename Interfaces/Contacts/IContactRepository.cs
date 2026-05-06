using System.Threading.Tasks;
using api.Common;
using api.Dtos.Contacts;
using api.Models;

namespace api.Interfaces.Contacts;

public interface IContactRepository
{
    Task<long> CreateAsync(ContactAnnonceur contact);
    Task<PagedResponse<ContactResponseDto>> GetPagedByUserIdAsync(long userId, int pageNumber, int pageSize);
    Task<PagedResponse<ContactResponseDto>> GetPagedByAdvertiserIdAsync(long advertiserId, int pageNumber, int pageSize);
    Task<PagedResponse<ContactResponseDto>> GetAdminPagedAsync(int pageNumber, int pageSize);
}
