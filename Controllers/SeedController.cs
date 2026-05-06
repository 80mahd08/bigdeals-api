using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using api.Data.Connections;
using api.Models.Enums;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace api.Controllers;

[Route("api/seed")]
[ApiController]
public class SeedController : ControllerBase
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public SeedController(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    [HttpGet("complete-restore-v8")]
    public async Task<IActionResult> CompleteRestore()
    {
        using var connection = (SqlConnection)_connectionFactory.CreateConnection();
        await connection.OpenAsync();
        
        // Migrations
        const string sqlCheckCol = "IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'AttributsCategorie' AND COLUMN_NAME = 'Placeholder') BEGIN ALTER TABLE AttributsCategorie ADD Placeholder NVARCHAR(255) NULL; END";
        await new SqlCommand(sqlCheckCol, connection).ExecuteNonQueryAsync();
        
        const string sqlCheckColRange = "IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'AttributsCategorie' AND COLUMN_NAME = 'EstPlage') BEGIN ALTER TABLE AttributsCategorie ADD EstPlage BIT NOT NULL DEFAULT 0; END";
        await new SqlCommand(sqlCheckColRange, connection).ExecuteNonQueryAsync();

        const string sqlCheckColVille = "IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Utilisateurs' AND COLUMN_NAME = 'Ville') BEGIN ALTER TABLE Utilisateurs ADD Ville NVARCHAR(100) NULL; END";
        await new SqlCommand(sqlCheckColVille, connection).ExecuteNonQueryAsync();

        using var transaction = connection.BeginTransaction();
        try
        {
            const string sqlUser = "SELECT IdUtilisateur FROM Utilisateurs WHERE Email = 'mahdi1@gmail.com'";
            using var cmdUser = new SqlCommand(sqlUser, connection, transaction);
            var userIdObj = await cmdUser.ExecuteScalarAsync();
            if (userIdObj == null) return NotFound("User mahdi1@gmail.com not found");
            long userId = (long)userIdObj;

            // Cleanup
            await new SqlCommand("DELETE FROM ValeursAttributAnnonce", connection, transaction).ExecuteNonQueryAsync();
            await new SqlCommand("DELETE FROM ImagesAnnonce", connection, transaction).ExecuteNonQueryAsync();
            await new SqlCommand("DELETE FROM Favoris", connection, transaction).ExecuteNonQueryAsync();
            await new SqlCommand("DELETE FROM ContactsAnnonceur", connection, transaction).ExecuteNonQueryAsync();
            await new SqlCommand("DELETE FROM Annonces", connection, transaction).ExecuteNonQueryAsync();
            await new SqlCommand("DELETE FROM OptionsAttributCategorie", connection, transaction).ExecuteNonQueryAsync();
            await new SqlCommand("DELETE FROM AttributsCategorie", connection, transaction).ExecuteNonQueryAsync();

            async Task<int> InsAttr(int catId, string nom, int type, string? placeholder = null, bool range = false, bool req = false, bool filt = true, int order = 1)
            {
                const string sql = "INSERT INTO AttributsCategorie (IdCategorie, Nom, TypeDonnee, Obligatoire, Filtrable, OrdreAffichage, Placeholder, EstPlage, EstActive) VALUES (@CatId, @Nom, @Type, @Req, @Filt, @Ordre, @Placeholder, @Range, 1); SELECT CAST(SCOPE_IDENTITY() as INT);";
                using var cmd = new SqlCommand(sql, connection, transaction);
                cmd.Parameters.AddWithValue("@CatId", catId); cmd.Parameters.AddWithValue("@Nom", nom);
                cmd.Parameters.AddWithValue("@Type", type); cmd.Parameters.AddWithValue("@Req", req);
                cmd.Parameters.AddWithValue("@Filt", filt); cmd.Parameters.AddWithValue("@Ordre", order);
                cmd.Parameters.AddWithValue("@Placeholder", (object?)placeholder ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Range", range);
                var result = await cmd.ExecuteScalarAsync();
                return result != null ? Convert.ToInt32(result) : 0;
            }

            async Task<int> InsOpt(int attrId, string val, int order = 1)
            {
                const string sql = "INSERT INTO OptionsAttributCategorie (IdAttributCategorie, Valeur, OrdreAffichage, EstActive) VALUES (@AttrId, @Val, @Ordre, 1); SELECT CAST(SCOPE_IDENTITY() as INT);";
                using var cmd = new SqlCommand(sql, connection, transaction);
                cmd.Parameters.AddWithValue("@AttrId", attrId); cmd.Parameters.AddWithValue("@Val", val); cmd.Parameters.AddWithValue("@Ordre", order);
                var result = await cmd.ExecuteScalarAsync();
                return result != null ? Convert.ToInt32(result) : 0;
            }

            var categoryIds = new Dictionary<string, int>();
            using (var reader = await new SqlCommand("SELECT IdCategorie, Nom FROM Categories", connection, transaction).ExecuteReaderAsync())
            {
                while (await reader.ReadAsync()) categoryIds[reader.GetString(1)] = reader.GetInt32(0);
            }

            // --- 1. VÉHICULES ---
            if (categoryIds.TryGetValue("Véhicules", out int catVeh)) {
                int a1 = await InsAttr(catVeh, "Type véhicule", 5, null, false, false, true, 1);
                foreach (var o in new[] { "Voiture", "Moto", "Camion", "Bus", "Utilitaire", "Tracteur", "Remorque" }) await InsOpt(a1, o);
                await InsAttr(catVeh, "Marque", 1, null, false, false, true, 2);
                await InsAttr(catVeh, "Modèle", 1, null, false, false, true, 3);
                await InsAttr(catVeh, "Année", 2, null, true, false, true, 4);
                await InsAttr(catVeh, "Kilométrage", 2, null, true, false, true, 5);
                int a6 = await InsAttr(catVeh, "Carburant", 5, null, false, false, true, 6);
                foreach (var o in new[] { "Essence", "Diesel", "Hybride", "Électrique", "GPL" }) await InsOpt(a6, o);
                int a7 = await InsAttr(catVeh, "Boîte vitesse", 5, null, false, false, true, 7);
                foreach (var o in new[] { "Manuelle", "Automatique" }) await InsOpt(a7, o);
                await InsAttr(catVeh, "Puissance fiscale", 2, null, true, false, true, 8);
                await InsAttr(catVeh, "Couleur", 1, null, false, false, true, 9);
                int a10 = await InsAttr(catVeh, "État", 5, null, false, false, true, 10);
                foreach (var o in new[] { "Neuf", "Très bon état", "Bon état", "État moyen", "À réparer" }) await InsOpt(a10, o);
                await InsAttr(catVeh, "Première main", 4, null, false, false, true, 11);
                await InsAttr(catVeh, "Climatisation", 4, null, false, false, true, 12);
                await InsAttr(catVeh, "Nombre portes", 2, null, false, false, true, 13);
            }

            // --- 2. IMMOBILIER ---
            if (categoryIds.TryGetValue("Immobilier", out int catImmo)) {
                await InsAttr(catImmo, "Type bien", 1, null, false, false, true, 1);
                int a2 = await InsAttr(catImmo, "Transaction", 5, null, false, false, true, 2);
                foreach (var o in new[] { "Vente", "Location" }) await InsOpt(a2, o);
                await InsAttr(catImmo, "Surface", 2, null, true, false, true, 3);
                await InsAttr(catImmo, "Nombre pièces", 2, null, true, false, true, 4);
                await InsAttr(catImmo, "Nombre chambres", 2, null, true, false, true, 5);
                await InsAttr(catImmo, "Nombre salles de bain", 2, null, true, false, true, 6);
                await InsAttr(catImmo, "Étage", 2, null, false, false, true, 7);
                await InsAttr(catImmo, "Meublé", 4, null, false, false, true, 8);
                await InsAttr(catImmo, "Ascenseur", 4, null, false, false, true, 9);
                await InsAttr(catImmo, "Garage", 4, null, false, false, true, 10);
                await InsAttr(catImmo, "Jardin", 4, null, false, false, true, 11);
                await InsAttr(catImmo, "Terrasse / Balcon", 4, null, false, false, true, 12);
                int a13 = await InsAttr(catImmo, "Chauffage", 5, null, false, false, true, 13);
                foreach (var o in new[] { "Aucun", "Gaz", "Électrique", "Central", "Climatisation réversible", "Solaire" }) await InsOpt(a13, o);
                int a14 = await InsAttr(catImmo, "État du bien", 5, null, false, false, true, 14);
                foreach (var o in new[] { "Neuf", "Très bon état", "Bon état", "À rénover", "En construction" }) await InsOpt(a14, o);
            }

            // --- 3. TÉLÉPHONES ---
            if (categoryIds.TryGetValue("Téléphones", out int catTel)) {
                await InsAttr(catTel, "Marque", 1, null, false, false, true, 1);
                await InsAttr(catTel, "Modèle", 1, null, false, false, true, 2);
                int a3 = await InsAttr(catTel, "État", 5, null, false, false, true, 3);
                foreach (var o in new[] { "Neuf", "Très bon état", "Bon état", "Écran cassé", "Batterie faible", "À réparer" }) await InsOpt(a3, o);
                await InsAttr(catTel, "Stockage", 2, "GB", false, false, true, 4);
                await InsAttr(catTel, "RAM", 2, "GB", false, false, true, 5);
                await InsAttr(catTel, "Couleur", 1, null, false, false, true, 6);
                await InsAttr(catTel, "Double SIM", 4, null, false, false, true, 7);
                await InsAttr(catTel, "5G", 4, null, false, false, true, 8);
                await InsAttr(catTel, "Batterie changée", 4, null, false, false, true, 9);
                await InsAttr(catTel, "Avec chargeur", 4, null, false, false, true, 10);
                await InsAttr(catTel, "Avec boîte", 4, null, false, false, true, 11);
                await InsAttr(catTel, "Garantie", 4, null, false, false, true, 12);
                await InsAttr(catTel, "Durée garantie", 2, null, false, false, true, 13);
            }

            // --- 4. INFORMATIQUE ---
            if (categoryIds.TryGetValue("Informatique", out int catInfo)) {
                await InsAttr(catInfo, "Type produit", 1, null, false, false, true, 1);
                await InsAttr(catInfo, "Marque", 1, null, false, false, true, 2);
                await InsAttr(catInfo, "Modèle", 1, null, false, false, true, 3);
                await InsAttr(catInfo, "Processeur", 1, null, false, false, true, 4);
                await InsAttr(catInfo, "RAM", 2, null, false, false, true, 5);
                await InsAttr(catInfo, "Stockage", 2, null, false, false, true, 6);
                await InsAttr(catInfo, "Carte graphique", 1, null, false, false, true, 7);
                await InsAttr(catInfo, "Taille écran", 2, null, true, false, true, 8);
                await InsAttr(catInfo, "Résolution", 1, null, false, false, true, 9);
                await InsAttr(catInfo, "Système", 1, null, false, false, true, 10);
                int a11 = await InsAttr(catInfo, "État", 5, null, false, false, true, 11);
                foreach (var o in new[] { "Neuf", "Très bon état", "Bon état", "État moyen", "À réparer" }) await InsOpt(a11, o);
                await InsAttr(catInfo, "Garantie", 4, null, false, false, true, 12);
            }

            // --- 5. MODE ---
            if (categoryIds.TryGetValue("Mode", out int catMode)) {
                await InsAttr(catMode, "Type article", 1, null, false, false, true, 1);
                int a2 = await InsAttr(catMode, "Genre", 5, null, false, false, true, 2);
                foreach (var o in new[] { "Homme", "Femme", "Enfant", "Bébé", "Unisexe" }) await InsOpt(a2, o);
                await InsAttr(catMode, "Marque", 1, null, false, false, true, 3);
                await InsAttr(catMode, "Taille", 1, null, false, false, true, 4);
                await InsAttr(catMode, "Couleur", 1, null, false, false, true, 5);
                await InsAttr(catMode, "Matière", 1, null, false, false, true, 6);
                int a7 = await InsAttr(catMode, "État", 5, null, false, false, true, 7);
                foreach (var o in new[] { "Neuf", "Très bon état", "Bon état", "État moyen" }) await InsOpt(a7, o);
                await InsAttr(catMode, "Original", 4, null, false, false, true, 8);
                int a9 = await InsAttr(catMode, "Saison", 5, null, false, false, true, 9);
                foreach (var o in new[] { "Printemps", "Été", "Automne", "Hiver", "Toutes saisons" }) await InsOpt(a9, o);
            }

            // --- 6. BEAUTÉ ---
            if (categoryIds.TryGetValue("Beauté", out int catBeau)) {
                await InsAttr(catBeau, "Type produit", 1, null, false, false, true, 1);
                await InsAttr(catBeau, "Marque", 1, null, false, false, true, 2);
                int a3 = await InsAttr(catBeau, "Genre", 5, null, false, false, true, 3);
                foreach (var o in new[] { "Homme", "Femme", "Unisexe" }) await InsOpt(a3, o);
                int a4 = await InsAttr(catBeau, "État", 5, null, false, false, true, 4);
                foreach (var o in new[] { "Neuf", "Utilisé une fois", "Ouvert", "Presque vide" }) await InsOpt(a4, o);
                await InsAttr(catBeau, "Volume", 2, "ml", true, false, true, 5);
                int a6 = await InsAttr(catBeau, "Type peau", 5, null, false, false, true, 6);
                foreach (var o in new[] { "Peau normale", "Peau sèche", "Peau grasse", "Peau mixte", "Peau sensible", "Peau acnéique", "Tous types de peau" }) await InsOpt(a6, o);
                int a7 = await InsAttr(catBeau, "Type cheveux", 5, null, false, false, true, 7);
                foreach (var o in new[] { "Cheveux normaux", "Cheveux secs", "Cheveux gras", "Cheveux bouclés", "Cheveux frisés", "Cheveux colorés", "Cheveux abîmés", "Tous types de cheveux" }) await InsOpt(a7, o);
                await InsAttr(catBeau, "Bio / Naturel", 4, null, false, false, true, 8);
                await InsAttr(catBeau, "Date expiration", 3, null, true, false, true, 9);
                await InsAttr(catBeau, "Original", 4, null, false, false, true, 10);
            }

            // --- 7. MAISON ---
            if (categoryIds.TryGetValue("Maison", out int catMaison)) {
                int a1 = await InsAttr(catMaison, "Type article", 5, null, false, false, true, 1);
                foreach (var o in new[] { "Meuble", "Canapé", "Lit", "Table", "Chaise", "Armoire", "Décoration", "Tapis", "Rideau", "Cuisine", "Vaisselle", "Électroménager", "Literie", "Rangement", "Luminaire" }) await InsOpt(a1, o);
                await InsAttr(catMaison, "Marque", 1, null, false, false, true, 2);
                int a3 = await InsAttr(catMaison, "Matière", 5, null, false, false, true, 3);
                foreach (var o in new[] { "Bois", "Métal", "Plastique", "Verre", "Tissu", "Cuir", "Simili cuir", "Céramique", "Marbre", "Rotin", "Bambou" }) await InsOpt(a3, o);
                await InsAttr(catMaison, "Couleur", 1, null, false, false, true, 4);
                int a5 = await InsAttr(catMaison, "État", 5, null, false, false, true, 5);
                foreach (var o in new[] { "Neuf", "Très bon état", "Bon état", "État moyen", "À réparer" }) await InsOpt(a5, o);
                await InsAttr(catMaison, "Longueur", 2, null, true, false, true, 6);
                await InsAttr(catMaison, "Largeur", 2, null, true, false, true, 7);
                await InsAttr(catMaison, "Hauteur", 2, null, true, false, true, 8);
                await InsAttr(catMaison, "Surface", 2, null, true, false, true, 9);
                await InsAttr(catMaison, "Poids", 2, null, true, false, true, 10);
                await InsAttr(catMaison, "Démontable", 4, null, false, false, true, 11);
                await InsAttr(catMaison, "Livraison possible", 4, null, false, false, true, 12);
            }

            // --- 8. JARDIN ---
            if (categoryIds.TryGetValue("Jardin", out int catJardin)) {
                int a1 = await InsAttr(catJardin, "Type article", 5, null, false, false, true, 1);
                foreach (var o in new[] { "Plante", "Arbre", "Fleur", "Pot", "Terreau", "Outil jardin", "Mobilier jardin", "Décoration extérieure", "Arrosage", "Gazon", "Graines", "Engrais" }) await InsOpt(a1, o);
                int a2 = await InsAttr(catJardin, "État", 5, null, false, false, true, 2);
                foreach (var o in new[] { "Neuf", "Très bon état", "Bon état", "État moyen" }) await InsOpt(a2, o);
                int a3 = await InsAttr(catJardin, "Matière", 5, null, false, false, true, 3);
                foreach (var o in new[] { "Bois", "Métal", "Plastique", "Terre cuite", "Céramique", "Rotin", "Bambou", "Tissu", "Pierre" }) await InsOpt(a3, o);
                await InsAttr(catJardin, "Taille", 2, null, true, false, true, 4);
                int a5 = await InsAttr(catJardin, "Type plante", 5, null, false, false, true, 5);
                foreach (var o in new[] { "Plante intérieure", "Plante extérieure", "Fleur", "Arbre fruitier", "Arbre décoratif", "Cactus", "Succulente", "Plante aromatique", "Plante grimpante" }) await InsOpt(a5, o);
                int a6 = await InsAttr(catJardin, "Exposition soleil", 5, null, false, false, true, 6);
                foreach (var o in new[] { "Plein soleil", "Mi-ombre", "Ombre", "Intérieur lumineux", "Faible lumière" }) await InsOpt(a6, o);
                int a7 = await InsAttr(catJardin, "Arrosage", 5, null, false, false, true, 7);
                foreach (var o in new[] { "Faible", "Moyen", "Fréquent", "Quotidien", "Rare" }) await InsOpt(a7, o);
                await InsAttr(catJardin, "Avec pot", 4, null, false, false, true, 8);
                await InsAttr(catJardin, "Livraison possible", 4, null, false, false, true, 9);
            }

            // --- 9. SERVICES ---
            if (categoryIds.TryGetValue("Services", out int catServ)) {
                int a1 = await InsAttr(catServ, "Type service", 5, null, false, false, true, 1);
                foreach (var o in new[] { "Réparation", "Transport", "Nettoyage", "Formation", "Design", "Développement web", "Marketing", "Photographie", "Vidéo", "Plomberie", "Électricité", "Peinture", "Menuiserie", "Maçonnerie", "Jardinage", "Mécanique", "Livraison", "Déménagement", "Assistance informatique" }) await InsOpt(a1, o);
                await InsAttr(catServ, "Expérience", 2, null, true, false, true, 2);
                int a3 = await InsAttr(catServ, "Disponibilité", 5, null, false, false, true, 3);
                foreach (var o in new[] { "Temps plein", "Temps partiel", "Week-end", "Soir", "Matin", "Après-midi", "Sur demande", "Urgence" }) await InsOpt(a3, o);
                await InsAttr(catServ, "Zone service", 1, null, false, false, true, 4);
                await InsAttr(catServ, "Déplacement possible", 4, null, false, false, true, 5);
                await InsAttr(catServ, "Service à distance", 4, null, false, false, true, 6);
                int a7 = await InsAttr(catServ, "Tarif par", 5, null, false, false, true, 7);
                foreach (var o in new[] { "Heure", "Jour", "Projet", "Intervention", "Mois" }) await InsOpt(a7, o);
                await InsAttr(catServ, "Durée estimée", 2, null, true, false, true, 8);
                await InsAttr(catServ, "Certification", 4, null, false, false, true, 9);
                await InsAttr(catServ, "Urgence acceptée", 4, null, false, false, true, 10);
            }

            // --- 10. EMPLOI ---
            if (categoryIds.TryGetValue("Emploi", out int catEmploi)) {
                int a1 = await InsAttr(catEmploi, "Type annonce", 5, null, false, false, true, 1);
                foreach (var o in new[] { "Offre d’emploi", "Stage", "Freelance", "Alternance", "Mission temporaire" }) await InsOpt(a1, o);
                await InsAttr(catEmploi, "Domaine", 1, null, false, false, true, 2);
                int a3 = await InsAttr(catEmploi, "Type contrat", 5, null, false, false, true, 3);
                foreach (var o in new[] { "CDI", "CDD", "Stage", "Freelance", "Temps partiel", "Temps plein", "Saisonnier", "Contrat temporaire" }) await InsOpt(a3, o);
                int a4 = await InsAttr(catEmploi, "Niveau étude", 5, null, false, false, true, 4);
                foreach (var o in new[] { "Aucun diplôme", "Primaire", "Collège", "Bac", "Formation professionnelle", "BTP", "BTS", "Bac+2", "Licence", "Bac+3", "Master", "Bac+5", "Ingénieur", "Doctorat" }) await InsOpt(a4, o);
                await InsAttr(catEmploi, "Expérience requise", 2, null, true, false, true, 5);
                await InsAttr(catEmploi, "Salaire min", 2, null, true, false, true, 6);
                await InsAttr(catEmploi, "Salaire max", 2, null, true, false, true, 7);
                await InsAttr(catEmploi, "Télétravail", 4, null, false, false, true, 8);
                await InsAttr(catEmploi, "Compétences", 1, null, false, false, true, 9);
                await InsAttr(catEmploi, "Date début", 3, null, true, false, true, 10);
                await InsAttr(catEmploi, "Langues", 1, null, false, false, true, 11);
            }

            // --- 50 ADS SEEDING ---
            string[] villes = { "Tunis", "Sousse", "Sfax", "Bizerte", "Hammamet" };
            Random rnd = new Random();
            var catList = new List<string>(categoryIds.Keys);

            for (int i = 1; i <= 50; i++)
            {
                string catName = catList[i % catList.Count];
                int catId = categoryIds[catName];
                string titre = $"{catName} - Annonce V3 Premium #{i}";
                decimal prix = rnd.Next(10, 5000);
                string ville = villes[rnd.Next(villes.Length)];

                const string sqlInsAd = @"INSERT INTO Annonces (IdUtilisateur, IdCategorie, Titre, Description, Prix, Localisation, Statut, DateCreation, DatePublication, EstActive)
                                         VALUES (@UserId, @CatId, @Titre, @Desc, @Prix, @Loc, @Statut, SYSUTCDATETIME(), SYSUTCDATETIME(), 1);
                                         SELECT CAST(SCOPE_IDENTITY() as BIGINT);";
                using var cmdIns = new SqlCommand(sqlInsAd, connection, transaction);
                cmdIns.Parameters.AddWithValue("@UserId", userId); cmdIns.Parameters.AddWithValue("@CatId", catId);
                cmdIns.Parameters.AddWithValue("@Titre", titre); cmdIns.Parameters.AddWithValue("@Desc", $"Annonce professionnelle V3 optimisée for {catName}."); 
                cmdIns.Parameters.AddWithValue("@Prix", prix); cmdIns.Parameters.AddWithValue("@Loc", ville);
                cmdIns.Parameters.AddWithValue("@Statut", (int)StatutAnnonce.PUBLIEE);
                var adIdObj = await cmdIns.ExecuteScalarAsync();
                long adId = adIdObj != null ? Convert.ToInt64(adIdObj) : 0;

                string img = "/uploads/annonces/mercedes_seed.png";
                await new SqlCommand($"INSERT INTO ImagesAnnonce (IdAnnonce, Url, OrdreAffichage, EstPrincipale, DateCreation) VALUES ({adId}, '{img}', 0, 1, SYSUTCDATETIME())", connection, transaction).ExecuteNonQueryAsync();
            }

            await transaction.CommitAsync();
            return Ok("V3 Schema with Smart Ranges (EstPlage) successfully deployed.");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return StatusCode(500, $"Error: {ex.Message}");
        }
    }
}
