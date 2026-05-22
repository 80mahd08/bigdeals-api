using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using api.Data.Connections;
using api.Dtos.Admin;
using api.Interfaces.Admin;

namespace api.Services.Admin;

public class AdminDashboardService : IAdminDashboardService
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public AdminDashboardService(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<AdminDashboardStatsDto> GetDashboardStatsAsync()
    {
        using var connection = (SqlConnection)_connectionFactory.CreateConnection();
        await connection.OpenAsync();

        var stats = new AdminDashboardStatsDto();

        // 1. Total Users (Active and non-Admin)
        using (var cmd = new SqlCommand("SELECT COUNT(*) FROM Utilisateurs WHERE StatutCompte = 1 AND Role <> 3", connection))
        {
            var result = await cmd.ExecuteScalarAsync();
            stats.TotalUsers = result != null ? Convert.ToInt32(result) : 0;
        }

        // 2. Total Ads (Published and from Active Users)
        using (var cmd = new SqlCommand(@"
            SELECT COUNT(*) 
            FROM Annonces a 
            JOIN Utilisateurs u ON a.IdUtilisateur = u.IdUtilisateur 
            WHERE a.EstActive = 1 AND a.Statut = 1 AND u.StatutCompte = 1", connection))
        {
            var result = await cmd.ExecuteScalarAsync();
            stats.TotalAds = result != null ? Convert.ToInt32(result) : 0;
        }

        // 3. Pending Announcer Requests (Statut 1 = EN_ATTENTE_VERIFICATION)
        using (var cmd = new SqlCommand("SELECT COUNT(*) FROM DemandesAnnonceur WHERE Statut = 1", connection))
        {
            var result = await cmd.ExecuteScalarAsync();
            stats.PendingAnnouncerRequests = result != null ? Convert.ToInt32(result) : 0;
        }

        // 4. Total Revenue (Annonceur Payments)
        decimal totalRevenue = 0;
        using (var cmd = new SqlCommand("SELECT ISNULL(SUM(Montant), 0) FROM dbo.PaiementsAnnonceur WHERE StatutPaiement = 2", connection))
        {
            var result = await cmd.ExecuteScalarAsync();
            totalRevenue = result != DBNull.Value && result != null ? Convert.ToDecimal(result) : 0;
        }

        // 5. Flagged Ads (Pending Signalements)
        using (var cmd = new SqlCommand("SELECT COUNT(*) FROM Signalements WHERE Statut = 1", connection))
        {
            var result = await cmd.ExecuteScalarAsync();
            stats.FlaggedAds = result != null ? Convert.ToInt32(result) : 0;
        }

        // 6. Flagged Users (Pending SignalementsUtilisateurs)
        using (var cmd = new SqlCommand("SELECT COUNT(*) FROM SignalementsUtilisateurs WHERE Statut = 1", connection))
        {
            var result = await cmd.ExecuteScalarAsync();
            stats.FlaggedUsers = result != null ? Convert.ToInt32(result) : 0;
        }

        // Populate Stats list for the UI
        stats.Stats = new List<StatTrendDto>
        {
            new StatTrendDto { Label = "Utilisateurs", Value = stats.TotalUsers.ToString("N0"), Icon = "ri-user-line", Color = "primary" },
            new StatTrendDto { Label = "Annonces", Value = stats.TotalAds.ToString("N0"), Icon = "ri-stack-line", Color = "success" },
            new StatTrendDto { Label = "Revenus Plateforme", Value = totalRevenue.ToString("N0") + " DT", Icon = "ri-money-dollar-circle-line", Color = "info" },
            new StatTrendDto { Label = "Signalements", Value = (stats.FlaggedAds + stats.FlaggedUsers).ToString(), Icon = "ri-flag-line", Color = "danger" }
        };

        // Recent Activities (Mocked for now or could pull from a log table if exists)
        stats.RecentActivities = new List<RecentActivityDto>
        {
            new RecentActivityDto { Title = "Nouvelle annonce", Desc = "Une nouvelle annonce vient d'être publiée.", Time = "il y a 5 min", Icon = "ri-stack-line", Color = "primary" },
            new RecentActivityDto { Title = "Nouvel utilisateur", Desc = "Un client vient de s'inscrire.", Time = "il y a 10 min", Icon = "ri-user-add-line", Color = "info" }
        };

        // Top Sellers (Mocked for now)
        stats.TopSellers = new List<TopSellerDto>
        {
            new TopSellerDto { Name = "ElectroPlus", Category = "Électronique", Ads = 48, Revenue = "-", Rating = 4.9 },
            new TopSellerDto { Name = "ModeCity", Category = "Mode", Ads = 35, Revenue = "-", Rating = 4.7 }
        };

        return stats;
    }

    public async Task<AdminGrowthChartDto> GetGrowthChartAsync(string metric, string period)
    {
        var validMetrics = new[] { "users", "annonces", "revenue", "signalements" };
        var validPeriods = new[] { "7d", "30d", "12m" };

        if (!Array.Exists(validMetrics, m => m == metric))
            throw new api.Exceptions.BadRequestException("Invalid metric.");

        if (!Array.Exists(validPeriods, p => p == period))
            throw new api.Exceptions.BadRequestException("Invalid period.");

        var chart = new AdminGrowthChartDto
        {
            Metric = metric,
            Period = period,
            Title = GetTitleForMetric(metric)
        };

        var endDate = DateTime.UtcNow.Date;
        var startDate = endDate;
        bool isMonthly = false;

        if (period == "7d") startDate = endDate.AddDays(-6);
        else if (period == "30d") startDate = endDate.AddDays(-29);
        else if (period == "12m")
        {
            startDate = new DateTime(endDate.Year, endDate.Month, 1).AddMonths(-11);
            isMonthly = true;
        }

        using var connection = (SqlConnection)_connectionFactory.CreateConnection();
        await connection.OpenAsync();

        string sql = "";
        
        if (metric == "users")
        {
            if (isMonthly)
            {
                sql = @"SELECT YEAR(DateCreation) as Y, MONTH(DateCreation) as M, COUNT(*) as V
                        FROM Utilisateurs
                        WHERE DateCreation >= @StartDate AND Role <> 3
                        GROUP BY YEAR(DateCreation), MONTH(DateCreation)";
            }
            else
            {
                sql = @"SELECT CAST(DateCreation AS DATE) as D, COUNT(*) as V
                        FROM Utilisateurs
                        WHERE DateCreation >= @StartDate AND Role <> 3
                        GROUP BY CAST(DateCreation AS DATE)";
            }
        }
        else if (metric == "annonces")
        {
            if (isMonthly)
            {
                sql = @"SELECT YEAR(DateCreation) as Y, MONTH(DateCreation) as M, COUNT(*) as V
                        FROM Annonces
                        WHERE DateCreation >= @StartDate
                        GROUP BY YEAR(DateCreation), MONTH(DateCreation)";
            }
            else
            {
                sql = @"SELECT CAST(DateCreation AS DATE) as D, COUNT(*) as V
                        FROM Annonces
                        WHERE DateCreation >= @StartDate
                        GROUP BY CAST(DateCreation AS DATE)";
            }
        }
        else if (metric == "revenue")
        {
            if (isMonthly)
            {
                sql = @"SELECT YEAR(DateConfirmation) as Y, MONTH(DateConfirmation) as M, SUM(Montant) as V
                        FROM PaiementsAnnonceur
                        WHERE DateConfirmation >= @StartDate AND StatutPaiement = 2
                        GROUP BY YEAR(DateConfirmation), MONTH(DateConfirmation)";
            }
            else
            {
                sql = @"SELECT CAST(DateConfirmation AS DATE) as D, SUM(Montant) as V
                        FROM PaiementsAnnonceur
                        WHERE DateConfirmation >= @StartDate AND StatutPaiement = 2
                        GROUP BY CAST(DateConfirmation AS DATE)";
            }
        }
        else if (metric == "signalements")
        {
            if (isMonthly)
            {
                sql = @"SELECT Y, M, SUM(V) as V FROM (
                            SELECT YEAR(DateCreation) as Y, MONTH(DateCreation) as M, COUNT(*) as V
                            FROM Signalements
                            WHERE DateCreation >= @StartDate
                            GROUP BY YEAR(DateCreation), MONTH(DateCreation)
                            UNION ALL
                            SELECT YEAR(DateCreation) as Y, MONTH(DateCreation) as M, COUNT(*) as V
                            FROM SignalementsUtilisateurs
                            WHERE DateCreation >= @StartDate
                            GROUP BY YEAR(DateCreation), MONTH(DateCreation)
                        ) combined GROUP BY Y, M";
            }
            else
            {
                sql = @"SELECT D, SUM(V) as V FROM (
                            SELECT CAST(DateCreation AS DATE) as D, COUNT(*) as V
                            FROM Signalements
                            WHERE DateCreation >= @StartDate
                            GROUP BY CAST(DateCreation AS DATE)
                            UNION ALL
                            SELECT CAST(DateCreation AS DATE) as D, COUNT(*) as V
                            FROM SignalementsUtilisateurs
                            WHERE DateCreation >= @StartDate
                            GROUP BY CAST(DateCreation AS DATE)
                        ) combined GROUP BY D";
            }
        }

        var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@StartDate", startDate);

        var dataMap = new Dictionary<string, decimal>();
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            if (isMonthly)
            {
                var y = reader.GetInt32(0);
                var m = reader.GetInt32(1);
                var v = reader.IsDBNull(2) ? 0 : Convert.ToDecimal(reader.GetValue(2));
                dataMap[$"{y}-{m:D2}"] = v;
            }
            else
            {
                var d = reader.GetDateTime(0);
                var v = reader.IsDBNull(1) ? 0 : Convert.ToDecimal(reader.GetValue(1));
                dataMap[d.ToString("yyyy-MM-dd")] = v;
            }
        }

        var points = new List<AdminGrowthPointDto>();
        if (isMonthly)
        {
            for (int i = 0; i < 12; i++)
            {
                var d = startDate.AddMonths(i);
                var key = $"{d.Year}-{d.Month:D2}";
                points.Add(new AdminGrowthPointDto
                {
                    Label = d.ToString("MMM yyyy", new System.Globalization.CultureInfo("fr-FR")),
                    Date = d,
                    Value = dataMap.ContainsKey(key) ? dataMap[key] : 0
                });
            }
        }
        else
        {
            int days = period == "7d" ? 7 : 30;
            for (int i = 0; i < days; i++)
            {
                var d = startDate.AddDays(i);
                var key = d.ToString("yyyy-MM-dd");
                points.Add(new AdminGrowthPointDto
                {
                    Label = d.ToString("dd MMM", new System.Globalization.CultureInfo("fr-FR")),
                    Date = d,
                    Value = dataMap.ContainsKey(key) ? dataMap[key] : 0
                });
            }
        }

        chart.Points = points;
        return chart;
    }

    private string GetTitleForMetric(string metric)
    {
        return metric switch
        {
            "users" => "Évolution des utilisateurs",
            "annonces" => "Évolution des annonces",
            "revenue" => "Évolution des revenus plateforme",
            "signalements" => "Évolution des signalements",
            _ => "Évolution"
        };
    }
}
