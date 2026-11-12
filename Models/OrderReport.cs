using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReportService.Models;

[Table("orders_reports")]
public class OrderReport
{
    [Key]
    public int Id { get; set; }

    public int OrderId { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal TotalRevenue { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal TotalCost { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal TotalProfit { get; set; }

    public List<ProductReport> ProductReports { get; set; } = new();
}
