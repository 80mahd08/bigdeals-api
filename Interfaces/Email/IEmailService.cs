using System.Threading.Tasks;

namespace api.Interfaces.Email;

public interface IEmailService
{
    Task SendResetPasswordEmailAsync(string email, string token);
}
