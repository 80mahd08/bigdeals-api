using System.Threading.Tasks;
using api.Common;
using api.Dtos.Contacts;
using api.Exceptions;
using api.Interfaces.Annonces;
using api.Interfaces.Contacts;
using api.Models;
using api.Models.Enums;

namespace api.Services.Contacts;

public class ContactService : IContactService
{
    private readonly IContactRepository _contactRepository;
    private readonly IAnnonceRepository _annonceRepository;

    public ContactService(IContactRepository contactRepository, IAnnonceRepository annonceRepository)
    {
        _contactRepository = contactRepository;
        _annonceRepository = annonceRepository;
    }

    public async Task<long> TrackContactAsync(ContactCreateRequestDto request, long? currentUserId)
    {
        // Must target an existing public annonce
        var annonce = await _annonceRepository.GetByIdAsync(request.IdAnnonce);
        if (annonce == null || annonce.Statut != StatutAnnonce.PUBLIEE || !annonce.EstActive)
            throw new NotFoundException($"Active public annonce with ID {request.IdAnnonce} not found.");

        if (request.TypeContact != TypeContact.TELEPHONE && request.TypeContact != TypeContact.WHATSAPP)
            throw new BadRequestException("Invalid contact type. Must be TELEPHONE (1) or WHATSAPP (2).");

        var contact = new ContactAnnonceur
        {
            IdUtilisateur = currentUserId,
            IdAnnonce = request.IdAnnonce,
            IdAnnonceur = annonce.IdUtilisateur, // Derived from Annonce
            TypeContact = request.TypeContact
        };

        return await _contactRepository.CreateAsync(contact);
    }

    public async Task<PagedResponse<ContactResponseDto>> GetMyOutgoingContactsAsync(long userId, int pageNumber, int pageSize)
    {
        return await _contactRepository.GetPagedByUserIdAsync(userId, pageNumber, pageSize);
    }

    public async Task<PagedResponse<ContactResponseDto>> GetMyIncomingContactsAsync(long advertiserId, int pageNumber, int pageSize)
    {
        return await _contactRepository.GetPagedByAdvertiserIdAsync(advertiserId, pageNumber, pageSize);
    }

    public async Task<PagedResponse<ContactResponseDto>> GetAllContactsAdminAsync(int pageNumber, int pageSize)
    {
        return await _contactRepository.GetAdminPagedAsync(pageNumber, pageSize);
    }
}
