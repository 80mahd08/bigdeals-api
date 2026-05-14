using System.Collections.Generic;

namespace api.Dtos.Admin;

public class AdminGrowthChartDto
{
    public string Metric { get; set; } = string.Empty;
    public string Period { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public IReadOnlyList<AdminGrowthPointDto> Points { get; set; } = new List<AdminGrowthPointDto>();
}
