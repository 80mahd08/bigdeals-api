using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using api.Common;
using api.Dtos.Annonces;
using api.Interfaces.Annonces;
using api.Helpers.Security;
using api.Models.Enums;
using api.Exceptions;
using api.Interfaces.Categories;
using api.Models;
using System.Collections.Generic;
using System.Linq;

namespace api.Controllers;

[Route("api/annonces")]
[ApiController]
public class AnnoncesController : ControllerBase
{
    private readonly IAnnonceService _service;
    private readonly ICurrentUserService _currentUserService;
    private readonly ICategoryRepository _categoryRepository;

    public AnnoncesController(IAnnonceService service, ICurrentUserService currentUserService, ICategoryRepository categoryRepository)
    {
        _service = service;
        _currentUserService = currentUserService;
        _categoryRepository = categoryRepository;
    }

    private async Task<List<ValeurAttributAnnonce>> ValidateAndMapAttributes(int idCategorie, List<AnnonceAttributeValueDto> submittedValues)
    {
        var schemaAttributes = await _categoryRepository.GetAttributesByCategoryIdAsync(idCategorie);
        var schemaAttributeIds = schemaAttributes.Select(a => a.IdAttributCategorie).ToHashSet();
        var result = new List<ValeurAttributAnnonce>();

        // 1. Strict Unknown Attribute Validation
        var unknownIds = submittedValues
            .Select(v => v.IdAttributCategorie)
            .Where(id => !schemaAttributeIds.Contains(id))
            .ToList();

        if (unknownIds.Any())
            throw new BadRequestException($"Unknown attributes for this category: {string.Join(", ", unknownIds)}");

        // 2. Duplicate Validation
        if (submittedValues.GroupBy(v => v.IdAttributCategorie).Any(g => g.Count() > 1))
            throw new BadRequestException("Duplicate attributes in request.");

        // 3. Type-Specific and Exclusivity Validation
        foreach (var attr in schemaAttributes)
        {
            var submitted = submittedValues.FirstOrDefault(v => v.IdAttributCategorie == attr.IdAttributCategorie);
            
            if (attr.Obligatoire && submitted == null)
                throw new BadRequestException($"Attribute '{attr.Nom}' is required.");

            if (submitted != null)
            {
                // Count how many value fields are provided
                int providedFieldsCount = 0;
                if (submitted.IdOptionAttributCategorie.HasValue) providedFieldsCount++;
                if (!string.IsNullOrWhiteSpace(submitted.ValeurTexte)) providedFieldsCount++;
                if (submitted.ValeurNombre.HasValue) providedFieldsCount++;
                if (submitted.ValeurDate.HasValue) providedFieldsCount++;
                if (submitted.ValeurBooleen.HasValue) providedFieldsCount++;

                if (providedFieldsCount == 0)
                    throw new BadRequestException($"No value provided for attribute '{attr.Nom}'.");
                if (providedFieldsCount > 1)
                    throw new BadRequestException($"Multiple value fields provided for attribute '{attr.Nom}'. Exactly one is required.");

                var val = new ValeurAttributAnnonce { IdAttributCategorie = attr.IdAttributCategorie };
                
                switch (attr.TypeDonnee)
                {
                    case TypeDonneeAttribut.TEXTE:
                        if (string.IsNullOrWhiteSpace(submitted.ValeurTexte)) 
                            throw new BadRequestException($"Attribute '{attr.Nom}' expects a text value (ValeurTexte).");
                        val.ValeurTexte = submitted.ValeurTexte;
                        break;
                    case TypeDonneeAttribut.NOMBRE:
                        if (!submitted.ValeurNombre.HasValue) 
                            throw new BadRequestException($"Attribute '{attr.Nom}' expects a numeric value (ValeurNombre).");
                        val.ValeurNombre = submitted.ValeurNombre;
                        break;
                    case TypeDonneeAttribut.DATE:
                        if (!submitted.ValeurDate.HasValue) 
                            throw new BadRequestException($"Attribute '{attr.Nom}' expects a date value (ValeurDate).");
                        val.ValeurDate = submitted.ValeurDate;
                        break;
                    case TypeDonneeAttribut.BOOLEAN:
                        if (!submitted.ValeurBooleen.HasValue) 
                            break; // Handled below for bool
                        val.ValeurBooleen = submitted.ValeurBooleen;
                        break;
                    case TypeDonneeAttribut.LISTE:
                        if (!submitted.IdOptionAttributCategorie.HasValue) 
                            throw new BadRequestException($"Attribute '{attr.Nom}' expects an option ID (IdOptionAttributCategorie).");
                        var options = await _categoryRepository.GetOptionsByAttributeIdAsync(attr.IdAttributCategorie);
                        var option = options.FirstOrDefault(o => o.IdOptionAttributCategorie == submitted.IdOptionAttributCategorie && o.EstActive);
                        if (option == null) throw new BadRequestException($"Invalid or inactive option for '{attr.Nom}'.");
                        val.IdOptionAttributCategorie = submitted.IdOptionAttributCategorie;
                        break;
                }

                // Final type exclusivity check (re-confirming submitted value matches expected field)
                if (attr.TypeDonnee == TypeDonneeAttribut.BOOLEAN && !submitted.ValeurBooleen.HasValue)
                     throw new BadRequestException($"Attribute '{attr.Nom}' expects a boolean value (ValeurBooleen).");

                result.Add(val);
            }
        }

        return result;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<PagedResponse<AnnonceDto>>>> GetPublicList([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 12)
    {
        if (pageSize > 50) pageSize = 50;
        var result = await _service.GetPublicAnnoncesAsync(pageNumber, pageSize);
        return Ok(ApiResponse<PagedResponse<AnnonceDto>>.Ok(result));
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<AnnonceDetailsDto>>> GetById(long id)
    {
        var result = await _service.GetAnnonceByIdAsync(id);
        
        // Strict Visibility: Public can ONLY see PUBLIEE and EstActive = 1
        if (result.Statut != StatutAnnonce.PUBLIEE.ToString() || !result.EstActive)
        {
            throw new NotFoundException("Annonce not found.");
        }

        return Ok(ApiResponse<AnnonceDetailsDto>.Ok(result));
    }

    [HttpGet("admin/{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<ApiResponse<AnnonceDetailsDto>>> GetByIdAdmin(long id)
    {
        var result = await _service.GetAnnonceByIdAsync(id);
        return Ok(ApiResponse<AnnonceDetailsDto>.Ok(result));
    }

    [HttpPost]
    [Authorize(Roles = "ANNONCEUR")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<ApiResponse<long>>> Create([FromForm] CreateAnnonceFormDto dto)
    {
        var currentUserId = _currentUserService.GetUserId();
        var id = await _service.CreateAnnonceAsync(dto, currentUserId);
        return Ok(ApiResponse<long>.Ok(id, "Annonce created successfully."));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "ANNONCEUR")]
    public async Task<ActionResult<ApiResponse<bool>>> Update(long id, UpdateAnnonceDto dto)
    {
        var currentUserId = _currentUserService.GetUserId();
        await _service.UpdateAnnonceAsync(id, dto, currentUserId);
        return Ok(ApiResponse<bool>.Ok(true, "Annonce updated successfully."));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "ANNONCEUR")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(long id)
    {
        var currentUserId = _currentUserService.GetUserId();
        await _service.DeleteAnnonceAsync(id, currentUserId);
        return Ok(ApiResponse<bool>.Ok(true, "Annonce deleted successfully."));
    }
}
