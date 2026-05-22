using System;
using System.Threading.Tasks;
using api.Common;
using api.Dtos.SignalementsUtilisateurs;
using api.Exceptions;
using api.Interfaces.Signalements;
using api.Interfaces.Users;
using api.Models;
using api.Models.Enums;

namespace api.Services.Signalements;

public class SignalementUtilisateurService : ISignalementUtilisateurService
{
    private readonly ISignalementUtilisateurRepository _signalementRepository;
    private readonly IUserRepository _userRepository;

    public SignalementUtilisateurService(ISignalementUtilisateurRepository signalementRepository, IUserRepository userRepository)
    {
        _signalementRepository = signalementRepository;
        _userRepository = userRepository;
    }

    public async Task<SignalementUtilisateurDto?> CreateAsync(CreateSignalementUtilisateurDto dto, long currentUserId)
    {
        // 1. Validate user exists
        var targetUser = await _userRepository.GetByIdAsync(dto.IdUtilisateurSignale);
        if (targetUser == null)
        {
            throw new NotFoundException("L'utilisateur signalé n'existe pas.");
        }
        
        // 2. Validate user is not reporting themselves
        if (dto.IdUtilisateurSignale == currentUserId)
        {
            throw new BadRequestException("Vous ne pouvez pas vous signaler vous-même.");
        }

        // 3. Validate user hasn't reported this user before
        if (await _signalementRepository.HasUserAlreadyReportedAsync(dto.IdUtilisateurSignale, currentUserId))
        {
            throw new BadRequestException("Vous avez déjà signalé cet utilisateur.");
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
        var signalement = new SignalementUtilisateur
        {
            IdUtilisateurSignale = dto.IdUtilisateurSignale,
            IdUtilisateurReporter = currentUserId,
            TypeSignalement = (TypeSignalement)dto.TypeSignalement,
            Motif = dto.Motif,
            Statut = StatutSignalement.EN_ATTENTE,
            DateCreation = DateTime.UtcNow
        };

        var id = await _signalementRepository.CreateAsync(signalement);
        
        return new SignalementUtilisateurDto
        {
            IdSignalement = id,
            IdUtilisateurSignale = signalement.IdUtilisateurSignale,
            IdUtilisateurReporter = signalement.IdUtilisateurReporter,
            TypeSignalement = (int)signalement.TypeSignalement,
            Motif = signalement.Motif,
            Statut = (int)signalement.Statut,
            DateCreation = signalement.DateCreation
        };
    }

    public async Task<PagedResponse<SignalementUtilisateurDto>> GetPagedAdminAsync(int pageNumber, int pageSize, int? statut, string? search, string? sortByDate = null, int? raison = null)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 50) pageSize = 50;

        return await _signalementRepository.GetPagedAdminAsync(pageNumber, pageSize, statut, search, sortByDate, raison);
    }

    public async Task UpdateStatusAsync(long idSignalement, UpdateSignalementUtilisateurStatusDto dto, long adminId)
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

        // 2. If TRAITE, automatically block the user
        if (dto.Statut == (int)StatutSignalement.TRAITE)
        {
            var idUtilisateurSignale = await _signalementRepository.GetReportedUserIdAsync(idSignalement);
            if (idUtilisateurSignale.HasValue)
            {
                var user = await _userRepository.GetByIdAsync(idUtilisateurSignale.Value);
                if (user != null)
                {
                    user.StatutCompte = StatutCompte.BLOQUE;
                    await _userRepository.UpdateUserAsync(user);
                }
            }
        }
    }
}
