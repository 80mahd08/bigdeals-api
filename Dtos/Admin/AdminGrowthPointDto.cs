using System;

namespace api.Dtos.Admin;

public class AdminGrowthPointDto
{
    public string Label { get; set; } = string.Empty;
    public DateTime? Date { get; set; }
    public decimal Value { get; set; }
}
