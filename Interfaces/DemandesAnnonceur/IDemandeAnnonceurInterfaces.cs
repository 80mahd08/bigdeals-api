using System.Collections.Generic;
using System.Threading.Tasks;
using api.Dtos.DemandesAnnonceur;
using api.Models;

namespace api.Interfaces.DemandesAnnonceur;

public interface IDemandeAnnonceurService
{
    Task<DemandeAnnonceurDto> CreateRequestAsync(long idUtilisateur, CreateDemandeAnnonceurDto request);
    Task<IReadOnlyList<DemandeAnnonceurDto>> GetMyRequestsAsync(long idUtilisateur);
    Task<IReadOnlyList<DemandeAnnonceurDto>> GetAllRequestsAsync();
    Task<DemandeAnnonceurDto> GetRequestByIdAsync(long idDemandeAnnonceur);
    Task ApproveRequestAsync(long idDemandeAnnonceur, long idAdminTraitant);
    Task RejectRequestAsync(long idDemandeAnnonceur, long idAdminTraitant, RejectDemandeAnnonceurDto request);
    Task<(byte[] Content, string ContentType, string FileName)> GetDocumentAsync(long idDemandeAnnonceur);
}

public interface IDemandeAnnonceurRepository
{
    Task<long> CreateAsync(DemandeAnnonceur demande);
    Task<bool> HasPendingRequestAsync(long idUtilisateur);
    Task<IReadOnlyList<DemandeAnnonceur>> GetByUserIdAsync(long idUtilisateur);
    Task<IReadOnlyList<DemandeAnnonceur>> GetAllAsync();
    Task<DemandeAnnonceur?> GetByIdAsync(long idDemandeAnnonceur);
    Task<bool> UpdateStatusToApprovedAsync(long idDemandeAnnonceur, long idAdminTraitant, long idUtilisateur);
    Task<bool> UpdateStatusToRejectedAsync(long idDemandeAnnonceur, long idAdminTraitant, string motifRejet);
}
