using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ReportService.Models;

[Table("product_reports")]
public class ProductReport
{
    [Key]
    public int Id { get; set; }

    public int? OrderReportId { get; set; }

    public int ProductId { get; set; }

    public int TotalSold { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal Revenue { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal Cost { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal Profit { get; set; }

    [JsonIgnore]
    public OrderReport? OrderReport { get; set; }
}
