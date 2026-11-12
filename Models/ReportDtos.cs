namespace ReportService.Models;

public class ExternalProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}

public class ExternalOrderItemDto
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
}

public class ExternalOrderDto
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<ExternalOrderItemDto> OrderItems { get; set; } = new();
}

public class CreateOrderReportRequest
{
    public int OrderId { get; set; }
    public decimal? TotalCost { get; set; }
    public decimal? CostRate { get; set; }
}

public class CreateProductReportRequest
{
    public int ProductId { get; set; }
    public int? OrderReportId { get; set; }
    public decimal? UnitCost { get; set; }
    public decimal? Cost { get; set; }
    public decimal? CostRate { get; set; }
}

public class ProductReportResponse
{
    public int Id { get; set; }
    public int? OrderReportId { get; set; }
    public int ProductId { get; set; }
    public string? ProductName { get; set; }
    public int? StockQuantity { get; set; }
    public int TotalSold { get; set; }
    public decimal Revenue { get; set; }
    public decimal Cost { get; set; }
    public decimal Profit { get; set; }
}

public class OrderReportResponse
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public string? CustomerName { get; set; }
    public string? OrderStatus { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal TotalCost { get; set; }
    public decimal TotalProfit { get; set; }
    public List<ProductReport> ProductReports { get; set; } = new();
}
