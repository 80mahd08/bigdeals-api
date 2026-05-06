using System.Collections.Generic;

namespace api.Dtos.Admin;

public class AdminDashboardStatsDto
{
    public int TotalUsers { get; set; }
    public int TotalAds { get; set; }
    public decimal TotalRevenue { get; set; } // Mocked for now as we don't have payments yet
    public int PendingAnnouncerRequests { get; set; }
    public int FlaggedAds { get; set; }
    
    public List<StatTrendDto> Stats { get; set; } = new();
    public List<RecentActivityDto> RecentActivities { get; set; } = new();
    public List<TopSellerDto> TopSellers { get; set; } = new();
}

public class StatTrendDto
{
    public string Label { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public int Trend { get; set; }
    public bool TrendUp { get; set; }
}

public class RecentActivityDto
{
    public string Title { get; set; } = string.Empty;
    public string Desc { get; set; } = string.Empty;
    public string Time { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
}

public class TopSellerDto
{
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int Ads { get; set; }
    public string Revenue { get; set; } = string.Empty;
    public double Rating { get; set; }
}
