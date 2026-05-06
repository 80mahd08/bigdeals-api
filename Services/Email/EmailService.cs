using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using api.Interfaces.Email;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace api.Services.Email;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<EmailService> _logger;
    private readonly IHostEnvironment _env;

    public EmailService(IConfiguration config, ILogger<EmailService> logger, IHostEnvironment env)
    {
        _config = config;
        _logger = logger;
        _env = env;
    }

    public async Task SendResetPasswordEmailAsync(string email, string token)
    {
        var settings = _config.GetSection("EmailSettings");
        var resetUrl = $"{settings["FrontendResetPasswordUrl"]}?token={token}";
        
        var subject = "Réinitialisation de votre mot de passe BigDeals";
        var body = $@"
Bonjour,

Vous avez demandé la réinitialisation de votre mot de passe BigDeals.

Cliquez sur ce lien pour créer un nouveau mot de passe:
{resetUrl}

Ce lien expire dans 30 minutes.

Si vous n'avez pas demandé cette action, ignorez cet email.
";

        if (_env.IsDevelopment())
        {
            _logger.LogInformation("====================================================");
            _logger.LogInformation("PASSWORD RESET LINK: {Link}", resetUrl);
            _logger.LogInformation("====================================================");
            
            // In development, we might not have valid SMTP settings, so we just log and return.
            // If you want to test actual email sending in dev, comment out the return below.
            return;
        }

        try
        {
            using var client = new SmtpClient(settings["SmtpHost"], int.Parse(settings["SmtpPort"]!))
            {
                Credentials = new NetworkCredential(settings["SmtpUsername"], settings["SmtpPassword"]),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(settings["FromEmail"]!, settings["FromName"]),
                Subject = subject,
                Body = body,
                IsBodyHtml = false
            };
            mailMessage.To.Add(email);

            await client.SendMailAsync(mailMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send reset password email to {Email}", email);
            // We don't throw here to avoid exposing email issues to the user in this specific flow,
            // but in a real app you might want to handle this differently.
        }
    }
}
