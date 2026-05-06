using System.ComponentModel.DataAnnotations;
using api.Models.Enums;

namespace api.Dtos.Contacts;

public class ContactCreateRequestDto
{
    [Required]
    public long IdAnnonce { get; set; }

    [Required]
    public TypeContact TypeContact { get; set; }
}
