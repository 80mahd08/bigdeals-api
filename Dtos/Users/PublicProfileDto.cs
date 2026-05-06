using api.Dtos.Annonces;
using System.Collections.Generic;

namespace api.Dtos.Users;

public class PublicProfileDto
{
    public UserProfileDto User { get; set; } = null!;
    public List<AnnonceDto> Ads { get; set; } = new();
}
