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

        // 1. Total Users
        using (var cmd = new SqlCommand("SELECT COUNT(*) FROM Utilisateurs WHERE EstActif = 1", connection))
        {
            var result = await cmd.ExecuteScalarAsync();
            stats.TotalUsers = result != null ? Convert.ToInt32(result) : 0;
        }

        // 2. Total Ads
        using (var cmd = new SqlCommand("SELECT COUNT(*) FROM Annonces WHERE EstActive = 1", connection))
        {
            var result = await cmd.ExecuteScalarAsync();
            stats.TotalAds = result != null ? Convert.ToInt32(result) : 0;
        }

        // 3. Pending Announcer Requests
        using (var cmd = new SqlCommand("SELECT COUNT(*) FROM DemandesAnnonceur WHERE Statut = 0", connection))
        {
            var result = await cmd.ExecuteScalarAsync();
            stats.PendingAnnouncerRequests = result != null ? Convert.ToInt32(result) : 0;
        }

        // 4. Flagged Ads (Mocked for now as we don't have a specific table yet)
        stats.FlaggedAds = 0;

        // Populate Stats list for the UI
        stats.Stats = new List<StatTrendDto>
        {
            new StatTrendDto { Label = "Utilisateurs", Value = stats.TotalUsers.ToString("N0"), Icon = "ri-user-line", Color = "primary", Trend = 12, TrendUp = true },
            new StatTrendDto { Label = "Annonces", Value = stats.TotalAds.ToString("N0"), Icon = "ri-stack-line", Color = "success", Trend = 25, TrendUp = true },
            new StatTrendDto { Label = "Revenus", Value = "0 DT", Icon = "ri-money-dollar-circle-line", Color = "info", Trend = 0, TrendUp = true },
            new StatTrendDto { Label = "Signalements", Value = stats.FlaggedAds.ToString(), Icon = "ri-flag-line", Color = "danger", Trend = 0, TrendUp = false }
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
            new TopSellerDto { Name = "ElectroPlus", Category = "Électronique", Ads = 48, Revenue = "12,400 DT", Rating = 4.9 },
            new TopSellerDto { Name = "ModeCity", Category = "Mode", Ads = 35, Revenue = "8,200 DT", Rating = 4.7 }
        };

        return stats;
    }
}
