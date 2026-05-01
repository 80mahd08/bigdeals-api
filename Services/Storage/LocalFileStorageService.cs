using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using api.Exceptions;

namespace api.Services.Storage;

public interface ILocalFileStorageService
{
    Task<(string Url, string OriginalName, string Type, long Size)> SaveDocumentAsync(IFormFile file);
    Task<string> SaveProfilePhotoAsync(IFormFile file);
}

public class LocalFileStorageService : ILocalFileStorageService
{
    private readonly string _uploadFolder;
    private readonly string[] _allowedExtensions = { ".pdf", ".jpg", ".jpeg", ".png" };
    private const long MaxFileSize = 5 * 1024 * 1024; // 5 MB

    public LocalFileStorageService(IWebHostEnvironment env)
    {
        _uploadFolder = Path.Combine(env.ContentRootPath, "Storage", "private", "advertiser-requests");
    }

    public async Task<(string Url, string OriginalName, string Type, long Size)> SaveDocumentAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new BadRequestException("Document file is required.");

        if (file.Length > MaxFileSize)
            throw new BadRequestException("Document file size must not exceed 5 MB.");

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!_allowedExtensions.Contains(extension))
            throw new BadRequestException($"File extension {extension} is not allowed.");

        if (!Directory.Exists(_uploadFolder))
            Directory.CreateDirectory(_uploadFolder);

        var fileName = $"{Guid.NewGuid():N}{extension}";
        var filePath = Path.Combine(_uploadFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var url = $"Storage/private/advertiser-requests/{fileName}";
        
        return (url, file.FileName, file.ContentType, file.Length);
    }

    public async Task<string> SaveProfilePhotoAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new BadRequestException("Photo file is required.");

        if (file.Length > 2 * 1024 * 1024) // 2MB for photos
            throw new BadRequestException("Photo file size must not exceed 2 MB.");

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        string[] allowed = { ".jpg", ".jpeg", ".png" };
        if (!allowed.Contains(extension))
            throw new BadRequestException($"File extension {extension} is not allowed for photos.");

        var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "profiles");
        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

        var fileName = $"{Guid.NewGuid():N}{extension}";
        var filePath = Path.Combine(folder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return $"/uploads/profiles/{fileName}";
    }
}
