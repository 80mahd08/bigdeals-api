using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Annonces;
using api.Exceptions;
using api.Interfaces.Annonces;
using api.Models;
using api.Common;

namespace api.Services.Annonces;

public class AvisService : IAvisService
{
    private readonly IAvisRepository _avisRepository;
    private readonly IAnnonceRepository _annonceRepository;

    public AvisService(IAvisRepository avisRepository, IAnnonceRepository annonceRepository)
    {
        _avisRepository = avisRepository;
        _annonceRepository = annonceRepository;
    }

    public async Task<IEnumerable<AvisDto>> GetByAnnonceIdAsync(long idAnnonce)
    {
        var avis = await _avisRepository.GetByAnnonceIdAsync(idAnnonce);
        return avis.Select(MapToDto);
    }

    public async Task<IEnumerable<AvisDto>> GetByAnnonceurIdAsync(long idAnnonceur)
    {
        var avis = await _avisRepository.GetByAnnonceurIdAsync(idAnnonceur);
        return avis.Select(MapToDto);
    }

    public async Task<PagedResponse<AvisDto>> GetPagedByAnnonceIdAsync(long idAnnonce, int page, int pageSize)
    {
        var (items, totalCount) = await _avisRepository.GetPagedByAnnonceIdAsync(idAnnonce, page, pageSize);
        var dtos = items.Select(MapToDto).ToList();
        return new PagedResponse<AvisDto>(dtos, totalCount, page, pageSize);
    }

    public async Task<AvisDto> CreateAsync(long idAnnonce, long idUtilisateur, CreateAvisDto dto)
    {
        if (dto.Note < 1 || dto.Note > 5)
            throw new BadRequestException("La note doit être comprise entre 1 et 5.");

        if (string.IsNullOrWhiteSpace(dto.Commentaire))
            throw new BadRequestException("Le commentaire est requis.");

        var annonce = await _annonceRepository.GetByIdAsync(idAnnonce);
        if (annonce == null)
            throw new NotFoundException("Annonce non trouvée.");

        if (annonce.IdUtilisateur == idUtilisateur)
            throw new ForbiddenException("Vous ne pouvez pas publier un avis sur votre propre annonce.");

        var existingAvis = await _avisRepository.GetByUserAndAnnonceAsync(idUtilisateur, idAnnonce);
        if (existingAvis != null)
            throw new ConflictException("Vous avez déjà publié un avis pour cette annonce.");

        var avis = new Avis
        {
            IdAnnonce = idAnnonce,
            IdUtilisateur = idUtilisateur,
            Note = dto.Note,
            Commentaire = dto.Commentaire,
            DateCreation = DateTime.UtcNow,
            EstActif = true
        };

        var id = await _avisRepository.CreateAsync(avis);
        var createdAvis = await _avisRepository.GetByIdAsync(id);
        
        if (createdAvis == null)
            throw new InternalServerException("Erreur lors de la création de l'avis.");

        return MapToDto(createdAvis);
    }

    public async Task<bool> UpdateAsync(long idAnnonce, long idUtilisateur, CreateAvisDto dto)
    {
        if (dto.Note < 1 || dto.Note > 5)
            throw new BadRequestException("La note doit être comprise entre 1 et 5.");

        if (string.IsNullOrWhiteSpace(dto.Commentaire))
            throw new BadRequestException("Le commentaire est requis.");

        var avis = await _avisRepository.GetByUserAndAnnonceAsync(idUtilisateur, idAnnonce);
        if (avis == null)
            throw new NotFoundException("Avis non trouvé.");

        avis.Commentaire = dto.Commentaire;
        avis.Note = dto.Note;
        return await _avisRepository.UpdateAsync(avis);
    }

    public async Task<bool> DeleteAsync(long idAnnonce, long idUtilisateur)
    {
        var avis = await _avisRepository.GetByUserAndAnnonceAsync(idUtilisateur, idAnnonce);
        if (avis == null)
            throw new NotFoundException("Avis non trouvé.");

        return await _avisRepository.DeleteAsync(avis.IdAvis);
    }

    private static AvisDto MapToDto(Avis avis)
    {
        return new AvisDto
        {
            IdAvis = avis.IdAvis,
            IdAnnonce = avis.IdAnnonce,
            IdUtilisateur = avis.IdUtilisateur,
            NomUtilisateur = avis.NomUtilisateur ?? string.Empty,
            PrenomUtilisateur = avis.PrenomUtilisateur ?? string.Empty,
            Note = avis.Note,
            Commentaire = avis.Commentaire,
            PhotoProfilUrl = avis.PhotoProfilUrl,
            TitreAnnonce = avis.TitreAnnonce ?? string.Empty,
            DateCreation = avis.DateCreation,
            DateModification = avis.DateModification
        };
    }
}
