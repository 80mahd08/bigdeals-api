using System.Collections.Generic;
using System.Threading.Tasks;
using api.Models;
using api.Models.Enums;
using api.Dtos.Annonces;

namespace api.Interfaces.Annonces;

public interface IAnnonceRepository
{
    Task<long> CreateAsync(Annonce annonce, List<ValeurAttributAnnonce> valeurs, List<ImageAnnonce> images);
    Task<bool> UpdateAsync(Annonce annonce, List<ValeurAttributAnnonce> valeurs);
    Task<bool> DeleteAsync(long id);
    Task<Annonce?> GetByIdAsync(long id);
    Task<(IReadOnlyList<Annonce> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, StatutAnnonce? statut = null, bool? estActif = null, long? idUtilisateur = null, string? keyword = null);
    Task<(IReadOnlyList<Annonce> Items, int TotalCount)> SearchAsync(AnnonceSearchRequestDto request);
    Task<IReadOnlyList<ValeurAttributAnnonce>> GetValeursByAnnonceIdAsync(long idAnnonce);
    Task<IReadOnlyList<ImageAnnonce>> GetImagesByAnnonceIdAsync(long idAnnonce);
    Task<bool> UpdateStatutAsync(long id, StatutAnnonce statut);
    Task<long> AddImageAsync(ImageAnnonce image);
    Task<bool> DeleteImageAsync(long idImage);
}
