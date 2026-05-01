using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using api.Common;
using api.Dtos.Annonces;
using api.Exceptions;
using api.Interfaces.Annonces;
using api.Interfaces.Categories;
using api.Interfaces.Users;
using api.Models;
using api.Models.Enums;
using api.Services.Storage;

namespace api.Services.Annonces;

public class AnnonceService : IAnnonceService
{
    private readonly IAnnonceRepository _annonceRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILocalFileStorageService _storageService;

    public AnnonceService(
        IAnnonceRepository annonceRepository, 
        ICategoryRepository categoryRepository,
        ILocalFileStorageService storageService)
    {
        _annonceRepository = annonceRepository;
        _categoryRepository = categoryRepository;
        _storageService = storageService;
    }

    public async Task<long> CreateAnnonceAsync(CreateAnnonceFormDto dto, long currentUserId)
    {
        // 1. Validate Category
        var category = await _categoryRepository.GetByIdAsync(dto.IdCategorie);
        if (category == null || !category.EstActive)
            throw new BadRequestException("Invalid or inactive category.");

        // 2. Deserialize and Validate Dynamic Attributes
        List<AnnonceAttributeValueDto> submittedValues = new();
        
        if (dto.ValeursJson != null && dto.ValeursJson.Count > 0)
        {
            foreach (var part in dto.ValeursJson)
            {
                var json = part.Trim();
                
                // Robust cleaning for multipart/form-data quirks
                if (json.StartsWith("\"") && json.EndsWith("\"") && json.Length > 2)
                {
                    json = json.Substring(1, json.Length - 2);
                    if (json.Contains("\\\""))
                    {
                        json = json.Replace("\\\"", "\"").Replace("\\\\", "\\");
                    }
                }
                
                json = new string(json.Where(c => !char.IsControl(c) || c == '\n' || c == '\r' || c == '\t').ToArray()).Trim();

                try
                {
                    // Check if it's an array or a single object
                    if (json.StartsWith("["))
                    {
                        var list = JsonSerializer.Deserialize<List<AnnonceAttributeValueDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        if (list != null) submittedValues.AddRange(list);
                    }
                    else if (json.StartsWith("{"))
                    {
                        var item = JsonSerializer.Deserialize<AnnonceAttributeValueDto>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        if (item != null) submittedValues.Add(item);
                    }
                }
                catch (JsonException ex)
                {
                    throw new BadRequestException($"Invalid JSON format for valeursJson: {ex.Message}");
                }
            }
        }

        var values = await ValidateAndMapAttributes(dto.IdCategorie, submittedValues);

        // 3. Validate Images (Min 1, Max 8)
        if (dto.Images == null || dto.Images.Count == 0)
            throw new BadRequestException("At least one image is required.");
        if (dto.Images.Count > 8)
            throw new BadRequestException("Maximum 8 images allowed.");

        // 4. Save Images to Filesystem
        var savedImageUrls = new List<string>();
        try
        {
            foreach (var file in dto.Images)
            {
                var url = await _storageService.SaveAnnonceImageAsync(file);
                savedImageUrls.Add(url);
            }

            // 5. Map and Save to Database (Transactional)
            var annonce = new Annonce
            {
                IdUtilisateur = currentUserId,
                IdCategorie = dto.IdCategorie,
                Titre = dto.Titre,
                Description = dto.Description,
                Prix = dto.Prix,
                Localisation = dto.Localisation,
                Statut = StatutAnnonce.PUBLIEE,
                DateCreation = DateTime.UtcNow,
                DatePublication = DateTime.UtcNow,
                EstActive = true
            };

            var imageModels = savedImageUrls.Select((url, index) => new ImageAnnonce
            {
                Url = url,
                OrdreAffichage = index + 1,
                EstPrincipale = index == 0
            }).ToList();

            return await _annonceRepository.CreateAsync(annonce, values, imageModels);
        }
        catch
        {
            // Transactional Cleanup: Delete files if database insertion fails
            foreach (var url in savedImageUrls)
            {
                await _storageService.DeleteFileAsync(url);
            }
            throw;
        }
    }

    public async Task<bool> UpdateAnnonceAsync(long id, UpdateAnnonceDto dto, long currentUserId)
    {
        var existing = await _annonceRepository.GetByIdAsync(id);
        if (existing == null) throw new NotFoundException("Annonce not found.");
        if (existing.IdUtilisateur != currentUserId) throw new ForbiddenException("You don't own this annonce.");

        var values = await ValidateAndMapAttributes(existing.IdCategorie, dto.Valeurs);

        existing.Titre = dto.Titre;
        existing.Description = dto.Description;
        existing.Prix = dto.Prix;
        existing.Localisation = dto.Localisation;

        return await _annonceRepository.UpdateAsync(existing, values);
    }

    public async Task<bool> DeleteAnnonceAsync(long id, long currentUserId)
    {
        var existing = await _annonceRepository.GetByIdAsync(id);
        if (existing == null) throw new NotFoundException("Annonce not found.");
        if (existing.IdUtilisateur != currentUserId) throw new ForbiddenException("You don't own this annonce.");

        return await _annonceRepository.DeleteAsync(id);
    }

    public async Task<AnnonceDetailsDto> GetAnnonceByIdAsync(long id, long? currentUserId = null)
    {
        var annonce = await _annonceRepository.GetByIdAsync(id);
        if (annonce == null) throw new NotFoundException("Annonce not found.");

        var category = await _categoryRepository.GetByIdAsync(annonce.IdCategorie);
        var attributes = await _categoryRepository.GetAttributesByCategoryIdAsync(annonce.IdCategorie);
        var values = await _annonceRepository.GetValeursByAnnonceIdAsync(id);
        var images = await _annonceRepository.GetImagesByAnnonceIdAsync(id);

        var details = new AnnonceDetailsDto
        {
            IdAnnonce = annonce.IdAnnonce,
            IdUtilisateur = annonce.IdUtilisateur,
            AnnonceurNom = $"User {annonce.IdUtilisateur}", 
            IdCategorie = annonce.IdCategorie,
            CategorieNom = category?.Nom ?? "Unknown",
            Titre = annonce.Titre,
            Description = annonce.Description,
            Prix = annonce.Prix,
            Localisation = annonce.Localisation,
            Statut = annonce.Statut.ToString(),
            DateCreation = annonce.DateCreation,
            DatePublication = annonce.DatePublication,
            EstActive = annonce.EstActive,
            Images = images.Select(img => new ImageAnnonceDto
            {
                IdImageAnnonce = img.IdImageAnnonce,
                Url = img.Url,
                OrdreAffichage = img.OrdreAffichage,
                EstPrincipale = img.EstPrincipale
            }).ToList()
        };

        foreach (var attr in attributes)
        {
            var val = values.FirstOrDefault(v => v.IdAttributCategorie == attr.IdAttributCategorie);
            if (val != null)
            {
                string readableVal = "";
                switch (attr.TypeDonnee)
                {
                    case TypeDonneeAttribut.TEXTE: readableVal = val.ValeurTexte ?? ""; break;
                    case TypeDonneeAttribut.NOMBRE: readableVal = val.ValeurNombre?.ToString() ?? ""; break;
                    case TypeDonneeAttribut.DATE: readableVal = val.ValeurDate?.ToShortDateString() ?? ""; break;
                    case TypeDonneeAttribut.BOOLEAN: readableVal = (val.ValeurBooleen ?? false) ? "Oui" : "Non"; break;
                    case TypeDonneeAttribut.LISTE:
                        if (val.IdOptionAttributCategorie.HasValue)
                        {
                            var options = await _categoryRepository.GetOptionsByAttributeIdAsync(attr.IdAttributCategorie);
                            readableVal = options.FirstOrDefault(o => o.IdOptionAttributCategorie == val.IdOptionAttributCategorie)?.Valeur ?? "";
                        }
                        break;
                }
                details.Attributs.Add(new AnnonceAttributeValueDetailsDto
                {
                    IdAttributCategorie = attr.IdAttributCategorie,
                    Nom = attr.Nom,
                    Valeur = readableVal
                });
            }
        }

        return details;
    }

    public async Task<PagedResponse<AnnonceDto>> GetPublicAnnoncesAsync(int pageNumber, int pageSize)
    {
        var (items, total) = await _annonceRepository.GetPagedAsync(pageNumber, pageSize, StatutAnnonce.PUBLIEE, true);
        return await MapToPagedDto(items, total, pageNumber, pageSize);
    }

    public async Task<PagedResponse<AnnonceDto>> GetUserAnnoncesAsync(long userId, int pageNumber, int pageSize)
    {
        var (items, total) = await _annonceRepository.GetPagedAsync(pageNumber, pageSize, null, null, userId);
        return await MapToPagedDto(items, total, pageNumber, pageSize);
    }

    public async Task<PagedResponse<AnnonceDto>> GetAdminAnnoncesAsync(int pageNumber, int pageSize)
    {
        var (items, total) = await _annonceRepository.GetPagedAsync(pageNumber, pageSize);
        return await MapToPagedDto(items, total, pageNumber, pageSize);
    }

    public async Task<bool> SuspendAnnonceAsync(long id) => await _annonceRepository.UpdateStatutAsync(id, StatutAnnonce.SUSPENDUE);
    public async Task<bool> RestoreAnnonceAsync(long id) => await _annonceRepository.UpdateStatutAsync(id, StatutAnnonce.PUBLIEE);

    public async Task<PagedResponse<AnnonceDto>> SearchAnnoncesAsync(AnnonceSearchRequestDto request)
    {
        // 1. Pagination Normalization
        if (request.PageNumber < 1) request.PageNumber = 1;
        if (request.PageSize < 1) request.PageSize = 12;
        if (request.PageSize > 50) request.PageSize = 50;

        // 2. Price Validation
        if (request.PrixMin.HasValue && request.PrixMin < 0) throw new BadRequestException("PrixMin cannot be negative.");
        if (request.PrixMax.HasValue && request.PrixMax < 0) throw new BadRequestException("PrixMax cannot be negative.");
        if (request.PrixMin.HasValue && request.PrixMax.HasValue && request.PrixMin > request.PrixMax)
            throw new BadRequestException("PrixMin cannot be greater than PrixMax.");

        // 3. Dynamic Filters Validation
        if (request.FiltresDynamiques != null && request.FiltresDynamiques.Any())
        {
            if (!request.IdCategorie.HasValue)
                throw new BadRequestException("Dynamic filters require a category.");

            var category = await _categoryRepository.GetByIdAsync(request.IdCategorie.Value);
            if (category == null || !category.EstActive)
                throw new BadRequestException("Invalid or inactive category.");

            var schemaAttributes = await _categoryRepository.GetAttributesByCategoryIdAsync(request.IdCategorie.Value);
            var duplicateCheck = request.FiltresDynamiques.GroupBy(f => f.IdAttributCategorie);
            if (duplicateCheck.Any(g => g.Count() > 1))
                throw new BadRequestException("Duplicate dynamic filters for the same attribute are not allowed.");

            foreach (var filter in request.FiltresDynamiques)
            {
                var attr = schemaAttributes.FirstOrDefault(a => a.IdAttributCategorie == filter.IdAttributCategorie);
                if (attr == null)
                    throw new BadRequestException($"Attribute ID {filter.IdAttributCategorie} does not belong to this category.");
                if (!attr.EstActive)
                    throw new BadRequestException($"Attribute '{attr.Nom}' is inactive.");
                if (!attr.Filtrable)
                    throw new BadRequestException($"Attribute '{attr.Nom}' is not marked as filtrable.");

                // Validate Type Exclusivity and Field Usage
                int providedFields = 0;
                if (filter.IdOptionAttributCategorie.HasValue) providedFields++;
                if (!string.IsNullOrWhiteSpace(filter.ValeurTexte)) providedFields++;
                if (filter.ValeurNombreMin.HasValue || filter.ValeurNombreMax.HasValue) providedFields++;
                if (filter.ValeurDateMin.HasValue || filter.ValeurDateMax.HasValue) providedFields++;
                if (filter.ValeurBooleen.HasValue) providedFields++;

                if (providedFields == 0) throw new BadRequestException($"No filter value provided for attribute '{attr.Nom}'.");
                if (providedFields > 1) throw new BadRequestException($"Multiple filter types provided for attribute '{attr.Nom}'. Exactly one is allowed.");

                switch (attr.TypeDonnee)
                {
                    case TypeDonneeAttribut.LISTE:
                        if (!filter.IdOptionAttributCategorie.HasValue)
                            throw new BadRequestException($"Attribute '{attr.Nom}' (LISTE) requires IdOptionAttributCategorie.");
                        var options = await _categoryRepository.GetOptionsByAttributeIdAsync(attr.IdAttributCategorie);
                        if (!options.Any(o => o.IdOptionAttributCategorie == filter.IdOptionAttributCategorie && o.EstActive))
                            throw new BadRequestException($"Invalid or inactive option for attribute '{attr.Nom}'.");
                        break;
                    case TypeDonneeAttribut.TEXTE:
                        if (string.IsNullOrWhiteSpace(filter.ValeurTexte))
                            throw new BadRequestException($"Attribute '{attr.Nom}' (TEXTE) requires ValeurTexte.");
                        break;
                    case TypeDonneeAttribut.NOMBRE:
                        if (!filter.ValeurNombreMin.HasValue && !filter.ValeurNombreMax.HasValue)
                            throw new BadRequestException($"Attribute '{attr.Nom}' (NOMBRE) requires ValeurNombreMin or ValeurNombreMax.");
                        if (filter.ValeurNombreMin.HasValue && filter.ValeurNombreMax.HasValue && filter.ValeurNombreMin > filter.ValeurNombreMax)
                            throw new BadRequestException($"Min value cannot be greater than Max value for '{attr.Nom}'.");
                        break;
                    case TypeDonneeAttribut.DATE:
                        if (!filter.ValeurDateMin.HasValue && !filter.ValeurDateMax.HasValue)
                            throw new BadRequestException($"Attribute '{attr.Nom}' (DATE) requires ValeurDateMin or ValeurDateMax.");
                        if (filter.ValeurDateMin.HasValue && filter.ValeurDateMax.HasValue && filter.ValeurDateMin > filter.ValeurDateMax)
                            throw new BadRequestException($"Min date cannot be later than Max date for '{attr.Nom}'.");
                        break;
                    case TypeDonneeAttribut.BOOLEAN:
                        if (!filter.ValeurBooleen.HasValue)
                            throw new BadRequestException($"Attribute '{attr.Nom}' (BOOLEAN) requires ValeurBooleen.");
                        break;
                }
            }
        }

        var (items, total) = await _annonceRepository.SearchAsync(request);
        return await MapToPagedDto(items, total, request.PageNumber, request.PageSize);
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
                            throw new BadRequestException($"Attribute '{attr.Nom}' expects a boolean value (ValeurBooleen).");
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
                result.Add(val);
            }
        }

        return result;
    }

    private async Task<PagedResponse<AnnonceDto>> MapToPagedDto(IReadOnlyList<Annonce> items, int total, int page, int size)
    {
        var dtos = new List<AnnonceDto>();
        foreach (var a in items)
        {
            // If already populated by SearchAsync, use those values
            string catNom = a.CategorieNom ?? (await _categoryRepository.GetByIdAsync(a.IdCategorie))?.Nom ?? "Unknown";
            string? mainImage = a.MainImageUrl;
            
            if (string.IsNullOrEmpty(mainImage))
            {
                var images = await _annonceRepository.GetImagesByAnnonceIdAsync(a.IdAnnonce);
                mainImage = images.FirstOrDefault(i => i.EstPrincipale)?.Url ?? images.FirstOrDefault()?.Url;
            }

            dtos.Add(new AnnonceDto
            {
                IdAnnonce = a.IdAnnonce,
                IdUtilisateur = a.IdUtilisateur,
                IdCategorie = a.IdCategorie,
                CategorieNom = catNom,
                Titre = a.Titre,
                Prix = a.Prix,
                Localisation = a.Localisation,
                Statut = a.Statut.ToString(),
                DateCreation = a.DateCreation,
                MainImageUrl = mainImage
            });
        }
        return new PagedResponse<AnnonceDto>(dtos, total, page, size);
    }
}
