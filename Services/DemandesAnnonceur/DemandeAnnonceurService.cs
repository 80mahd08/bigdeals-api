using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.DemandesAnnonceur;
using api.Exceptions;
using api.Interfaces.DemandesAnnonceur;
using api.Interfaces.Users;
using api.Models;
using api.Models.Enums;

using api.Services.Storage;

namespace api.Services.DemandesAnnonceur;

public class DemandeAnnonceurService : IDemandeAnnonceurService
{
    private readonly IDemandeAnnonceurRepository _repository;
    private readonly IUserRepository _userRepository;
    private readonly ILocalFileStorageService _fileStorageService;

    public DemandeAnnonceurService(
        IDemandeAnnonceurRepository repository, 
        IUserRepository userRepository,
        ILocalFileStorageService fileStorageService)
    {
        _repository = repository;
        _userRepository = userRepository;
        _fileStorageService = fileStorageService;
    }

    public async Task<DemandeAnnonceurDto> CreateRequestAsync(long idUtilisateur, CreateDemandeAnnonceurDto request)
    {
        var user = await _userRepository.GetByIdAsync(idUtilisateur);
        if (user == null) throw new NotFoundException("User not found.");

        if (user.Role == RoleUtilisateur.ADMIN)
            throw new ForbiddenException("Admins cannot submit advertiser requests.");

        if (user.Role == RoleUtilisateur.ANNONCEUR)
            throw new ConflictException("User is already an advertiser.");

        if (await _repository.HasPendingRequestAsync(idUtilisateur))
            throw new ConflictException("You already have a pending request.");

        var fileResult = await _fileStorageService.SaveDocumentAsync(request.Document);

        var demande = new DemandeAnnonceur
        {
            IdUtilisateur = idUtilisateur,
            Statut = StatutDemandeAnnonceur.EN_ATTENTE_VERIFICATION,
            DocumentUrl = fileResult.Url,
            DocumentNomOriginal = fileResult.OriginalName,
            DocumentType = fileResult.Type,
            DocumentTaille = fileResult.Size,
            DateDemande = DateTime.UtcNow
        };

        var id = await _repository.CreateAsync(demande);
        return await GetRequestByIdAsync(id);
    }

    public async Task<IReadOnlyList<DemandeAnnonceurDto>> GetMyRequestsAsync(long idUtilisateur)
    {
        var requests = await _repository.GetByUserIdAsync(idUtilisateur);
        return requests.Select(MapToDto).ToList();
    }

    public async Task<IReadOnlyList<DemandeAnnonceurDto>> GetAllRequestsAsync()
    {
        var requests = await _repository.GetAllAsync();
        return requests.Select(MapToDto).ToList();
    }

    public async Task<(IReadOnlyList<DemandeAnnonceurDto> Items, int TotalCount)> GetAllRequestsPagedAsync(int pageNumber, int pageSize, int? statut, string? search, string? sortByDateDemande = null, string? sortByDateTraitement = null)
    {
        var requests = await _repository.GetAllPagedAsync(pageNumber, pageSize, statut, search, sortByDateDemande, sortByDateTraitement);
        var totalCount = await _repository.GetCountAsync(statut, search);
        
        return (requests.Select(MapToDto).ToList(), totalCount);
    }

    public async Task<DemandeAnnonceurDto> GetRequestByIdAsync(long idDemandeAnnonceur)
    {
        var request = await _repository.GetByIdAsync(idDemandeAnnonceur);
        if (request == null)
            throw new NotFoundException("Request not found.");

        return MapToDto(request);
    }

    public async Task ApproveRequestAsync(long idDemandeAnnonceur, long idAdminTraitant)
    {
        var request = await _repository.GetByIdAsync(idDemandeAnnonceur);
        if (request == null)
            throw new NotFoundException("Request not found.");

        if (request.Statut == StatutDemandeAnnonceur.EN_ATTENTE_PAIEMENT)
            throw new BadRequestException("This advertiser request is already waiting for payment.");

        if (request.Statut == StatutDemandeAnnonceur.APPROUVEE)
            throw new BadRequestException("This advertiser request has already been approved.");

        if (request.Statut == StatutDemandeAnnonceur.REJETEE)
            throw new BadRequestException("This advertiser request has already been rejected.");

        if (request.Statut != StatutDemandeAnnonceur.EN_ATTENTE_VERIFICATION)
            throw new BadRequestException("Only requests pending verification can be accepted.");

        var success = await _repository.UpdateStatusToWaitingForPaymentAsync(idDemandeAnnonceur, idAdminTraitant);
        if (!success)
            throw new InternalServerException("Failed to update request status.");
    }

    public async Task RejectRequestAsync(long idDemandeAnnonceur, long idAdminTraitant, RejectDemandeAnnonceurDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.MotifRejet))
            throw new BadRequestException("Rejection reason is required.");

        var request = await _repository.GetByIdAsync(idDemandeAnnonceur);
        if (request == null)
            throw new NotFoundException("Request not found.");

        if (request.Statut == StatutDemandeAnnonceur.EN_ATTENTE_PAIEMENT)
            throw new BadRequestException("This advertiser request is already waiting for payment and cannot be rejected here.");

        if (request.Statut == StatutDemandeAnnonceur.APPROUVEE)
            throw new BadRequestException("This advertiser request has already been approved and cannot be rejected.");

        if (request.Statut == StatutDemandeAnnonceur.REJETEE)
            throw new BadRequestException("This advertiser request has already been rejected.");

        if (request.Statut != StatutDemandeAnnonceur.EN_ATTENTE_VERIFICATION)
            throw new BadRequestException("Only requests pending verification can be rejected.");

        var success = await _repository.UpdateStatusToRejectedAsync(idDemandeAnnonceur, idAdminTraitant, dto.MotifRejet);
        if (!success)
            throw new InternalServerException("Failed to reject request.");
    }

    public async Task<(byte[] Content, string ContentType, string FileName)> GetDocumentAsync(long idDemandeAnnonceur)
    {
        var request = await _repository.GetByIdAsync(idDemandeAnnonceur);
        if (request == null)
            throw new NotFoundException("Request not found.");

        var filePath = Path.Combine(Directory.GetCurrentDirectory(), request.DocumentUrl);
        if (!File.Exists(filePath))
            throw new NotFoundException("Document file not found on server.");

        var content = await File.ReadAllBytesAsync(filePath);
        return (content, request.DocumentType, request.DocumentNomOriginal);
    }

    private DemandeAnnonceurDto MapToDto(DemandeAnnonceur model)
    {
        return new DemandeAnnonceurDto
        {
            IdDemandeAnnonceur = model.IdDemandeAnnonceur,
            IdUtilisateur = model.IdUtilisateur,
            NomUtilisateur = model.NomUtilisateur ?? "Utilisateur",
            PrenomUtilisateur = model.PrenomUtilisateur ?? "",
            NomCompletUtilisateur = $"{model.PrenomUtilisateur} {model.NomUtilisateur}".Trim(),
            EmailUtilisateur = model.EmailUtilisateur ?? "",
            PhotoProfilUrl = model.PhotoProfilUrl,
            Statut = model.Statut.ToString(),
            DocumentUrl = $"/api/admin/demandes-annonceur/{model.IdDemandeAnnonceur}/document",
            DocumentNomOriginal = model.DocumentNomOriginal,
            DocumentType = model.DocumentType,
            DocumentTaille = model.DocumentTaille,
            MotifRejet = model.MotifRejet,
            DateDemande = model.DateDemande,
            DateTraitement = model.DateTraitement,
            IdAdminTraitant = model.IdAdminTraitant
        };
    }
}
