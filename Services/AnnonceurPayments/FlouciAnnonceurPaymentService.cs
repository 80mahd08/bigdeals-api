using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using api.Dtos.AnnonceurPayments;
using api.Exceptions;
using api.Interfaces.AnnonceurPayments;
using api.Models.Config;

namespace api.Services.AnnonceurPayments;

public class FlouciAnnonceurPaymentService : IFlouciAnnonceurPaymentService
{
    private readonly HttpClient _httpClient;
    private readonly FlouciSettings _settings;
    private readonly ILogger<FlouciAnnonceurPaymentService> _logger;

    public FlouciAnnonceurPaymentService(
        HttpClient httpClient,
        IOptions<FlouciSettings> settings,
        ILogger<FlouciAnnonceurPaymentService> logger)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<FlouciGenerateAnnonceurPaymentResponseDto> GeneratePaymentAsync(decimal amountTnd, string developerTrackingId, string clientId)
    {
        ValidateSettings();

        if (amountTnd <= 0)
            throw new BadRequestException("Amount must be greater than zero.");

        // Convert TND to millimes
        long millimes = (long)Math.Round(amountTnd * 1000);
        string amountStr = millimes.ToString();

        var requestBody = new FlouciGenerateAnnonceurPaymentRequestDto
        {
            Amount = amountStr,
            DeveloperTrackingId = developerTrackingId,
            SuccessLink = _settings.SuccessUrl,
            FailLink = _settings.FailUrl,
            Webhook = string.IsNullOrWhiteSpace(_settings.WebhookUrl) ? null : _settings.WebhookUrl,
            AcceptCard = _settings.AcceptCard,
            ClientId = clientId,
            SessionTimeoutSecs = _settings.SessionTimeoutSeconds
        };

        var request = new HttpRequestMessage(HttpMethod.Post, $"{_settings.BaseUrl.TrimEnd('/')}/generate_payment")
        {
            Content = JsonContent.Create(requestBody)
        };

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", $"{_settings.PublicKey}:{_settings.PrivateKey}");

        try
        {
            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Flouci generate_payment failed. Status: {Status}, Body: {Body}", response.StatusCode, content);
                throw new InternalServerException("Failed to generate Flouci payment.");
            }

            var result = JsonSerializer.Deserialize<FlouciGenerateAnnonceurPaymentResponseDto>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return result ?? throw new InternalServerException("Invalid Flouci generate payment response.");
        }
        catch (Exception ex) when (ex is not InternalServerException && ex is not BadRequestException)
        {
            _logger.LogError(ex, "Error calling Flouci generate_payment.");
            throw new InternalServerException("An error occurred while communicating with Flouci.");
        }
    }

    public async Task<FlouciVerifyAnnonceurPaymentResponseDto> VerifyPaymentAsync(string providerPaymentId)
    {
        ValidateSettings();

        if (string.IsNullOrWhiteSpace(providerPaymentId))
            throw new BadRequestException("Provider payment ID is required.");

        var request = new HttpRequestMessage(HttpMethod.Get, $"{_settings.BaseUrl.TrimEnd('/')}/verify_payment/{providerPaymentId}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", $"{_settings.PublicKey}:{_settings.PrivateKey}");

        try
        {
            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Flouci verify_payment failed. Status: {Status}, Body: {Body}", response.StatusCode, content);
                throw new InternalServerException("Failed to verify Flouci payment.");
            }

            var result = JsonSerializer.Deserialize<FlouciVerifyAnnonceurPaymentResponseDto>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return result ?? throw new InternalServerException("Invalid Flouci verify payment response.");
        }
        catch (Exception ex) when (ex is not InternalServerException && ex is not BadRequestException)
        {
            _logger.LogError(ex, "Error calling Flouci verify_payment.");
            throw new InternalServerException("An error occurred while communicating with Flouci.");
        }
    }

    private void ValidateSettings()
    {
        bool isInvalid = string.IsNullOrWhiteSpace(_settings.BaseUrl) ||
                         string.IsNullOrWhiteSpace(_settings.PublicKey) ||
                         string.IsNullOrWhiteSpace(_settings.PrivateKey) ||
                         string.IsNullOrWhiteSpace(_settings.SuccessUrl) ||
                         string.IsNullOrWhiteSpace(_settings.FailUrl) ||
                         _settings.PublicKey == "PUT_FLOUCI_PUBLIC_KEY_HERE" ||
                         _settings.PrivateKey == "PUT_FLOUCI_PRIVATE_KEY_HERE";

        if (isInvalid)
        {
            _logger.LogCritical("Flouci configuration is missing or invalid.");
            throw new InternalServerException("Flouci configuration is missing or invalid.");
        }
    }
}
