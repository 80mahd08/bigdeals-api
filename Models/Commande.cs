using System;
using System.Text.Json.Serialization;
using api.Models.Enums;

namespace api.Models;

public class Commande
{
    [JsonPropertyName("idCommande")]
    public long IdCommande { get; set; }

    [JsonPropertyName("idAnnonce")]
    public long IdAnnonce { get; set; }

    [JsonPropertyName("annonceTitre")]
    public string? AnnonceTitre { get; set; }

    [JsonPropertyName("idAcheteur")]
    public long IdAcheteur { get; set; }

    [JsonPropertyName("idAnnonceur")]
    public long IdAnnonceur { get; set; }

    [JsonPropertyName("montant")]
    public decimal Montant { get; set; }

    [JsonPropertyName("statutCommande")]
    public StatutCommande StatutCommande { get; set; }

    [JsonPropertyName("dateCreation")]
    public DateTime DateCreation { get; set; }

    // Delivery lifecycle
    [JsonPropertyName("statutLivraison")]
    public StatutLivraison StatutLivraison { get; set; } = StatutLivraison.EN_ATTENTE_PREPARATION;

    [JsonPropertyName("adresseLivraison")]
    public string? AdresseLivraison { get; set; }

    [JsonPropertyName("villeLivraison")]
    public string? VilleLivraison { get; set; }

    [JsonPropertyName("telephoneLivraison")]
    public string? TelephoneLivraison { get; set; }



    [JsonPropertyName("dateExpedition")]
    public DateTime? DateExpedition { get; set; }

    [JsonPropertyName("dateLivraison")]
    public DateTime? DateLivraison { get; set; }

    [JsonPropertyName("dateDerniereMiseAJourLivraison")]
    public DateTime? DateDerniereMiseAJourLivraison { get; set; }
}
