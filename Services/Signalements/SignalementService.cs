using System;
using System.Threading.Tasks;
using api.Common;
using api.Dtos.Signalements;
using api.Exceptions;
using api.Interfaces.Signalements;
using api.Interfaces.Annonces;
using api.Models;
using api.Models.Enums;

namespace api.Services.Signalements;

public class SignalementService : ISignalementService
{
    private readonly ISignalementRepository _signalementRepository;
    private readonly IAnnonceRepository _annonceRepository;

    public SignalementService(ISignalementRepository signalementRepository, IAnnonceRepository annonceRepository)
    {
        _signalementRepository = signalementRepository;
        _annonceRepository = annonceRepository;
    }

    public async Task<SignalementDto?> CreateAsync(CreateSignalementDto dto, long currentUserId)
    {
        // 1. Validate annonce exists and is public
        var adStatus = await _signalementRepository.GetAnnonceStatusAsync(dto.IdAnnonce);
        if (!adStatus.HasValue)
        {
            throw new NotFoundException("L'annonce n'existe pas.");
        }
        
        if (adStatus.Value != (int)StatutAnnonce.PUBLIEE)
        {
            throw new BadRequestException("Cette annonce n'est plus disponible ou a été suspendue.");
        }

        // 2. Validate user is not the owner
        var ownerId = await _signalementRepository.GetAnnonceOwnerIdAsync(dto.IdAnnonce);
        if (ownerId == currentUserId)
        {
            throw new BadRequestException("Vous ne pouvez pas signaler votre propre annonce.");
        }

        // 3. Validate user hasn't reported this before
        if (await _signalementRepository.HasUserAlreadyReportedAsync(dto.IdAnnonce, currentUserId))
        {
            throw new BadRequestException("Vous avez déjà signalé cette annonce.");
        }

        // 4. Conditional validation for Motif
        if (dto.TypeSignalement == (int)TypeSignalement.AUTRE)
        {
            if (string.IsNullOrWhiteSpace(dto.Motif) || dto.Motif.Length < 10)
            {
                throw new BadRequestException("Veuillez fournir des détails (au moins 10 caractères) pour le type 'Autre'.");
            }
        }
        else if (string.IsNullOrWhiteSpace(dto.Motif))
        {
            dto.Motif = "Signalé sans détails supplémentaires.";
        }

        // 5. Create signalement
        var signalement = new Signalement
        {
            IdAnnonce = dto.IdAnnonce,
            IdUtilisateur = currentUserId,
            TypeSignalement = (TypeSignalement)dto.TypeSignalement,
            Motif = dto.Motif,
            Statut = StatutSignalement.EN_ATTENTE,
            DateCreation = DateTime.UtcNow
        };

        var id = await _signalementRepository.CreateAsync(signalement);
        signalement.IdSignalement = id;

        return new SignalementDto
        {
            IdSignalement = signalement.IdSignalement,
            IdAnnonce = signalement.IdAnnonce,
            IdUtilisateur = signalement.IdUtilisateur,
            TypeSignalement = (int)signalement.TypeSignalement,
            Motif = signalement.Motif,
            Statut = (int)signalement.Statut,
            DateCreation = signalement.DateCreation
        };
    }

    public async Task<PagedResponse<SignalementDto>> GetPagedAdminAsync(int pageNumber, int pageSize, int? statut, string? search)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 50) pageSize = 50;

        return await _signalementRepository.GetPagedAdminAsync(pageNumber, pageSize, statut, search);
    }

    public async Task UpdateStatusAsync(long idSignalement, UpdateSignalementStatusDto dto, long adminId)
    {
        if (!await _signalementRepository.ExistsAsync(idSignalement))
        {
            throw new NotFoundException("Signalement introuvable.");
        }

        if (dto.Statut != (int)StatutSignalement.TRAITE && dto.Statut != (int)StatutSignalement.REJETE)
        {
            throw new BadRequestException("Le statut doit être Traité (2) ou Rejeté (3).");
        }

        // 1. Update report status
        await _signalementRepository.UpdateStatusAsync(idSignalement, dto.Statut, adminId);

        // 2. If TRAITE, automatically suspend the ad
        if (dto.Statut == (int)StatutSignalement.TRAITE)
        {
            var idAnnonce = await _signalementRepository.GetAnnonceIdAsync(idSignalement);
            if (idAnnonce.HasValue)
            {
                await _annonceRepository.UpdateStatutAsync(idAnnonce.Value, StatutAnnonce.SUSPENDUE);
            }
        }
    }
}
