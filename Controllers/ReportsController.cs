using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReportService.Data;
using ReportService.Models;
using ReportService.Services;

namespace ReportService.Controllers;

[ApiController]
[Route("reports")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly ReportDbContext _context;
    private readonly ExternalServicesClient _client;

    public ReportsController(ReportDbContext context, ExternalServicesClient client)
    {
        _context = context;
        _client = client;
    }

    [HttpGet("products")]
    public async Task<ActionResult<IEnumerable<ProductReportResponse>>> GetProductReports()
    {
        var reports = await _context.ProductReports
            .AsNoTracking()
            .OrderBy(r => r.Id)
            .ToListAsync();

        var products = await _client.GetProductsAsync(Request.Headers.Authorization.ToString());
        var productMap = products?.ToDictionary(p => p.Id) ?? new Dictionary<int, ExternalProductDto>();

        var result = reports.Select(r =>
        {
            productMap.TryGetValue(r.ProductId, out var product);
            return MapProductReport(r, product);
        });

        return Ok(result);
    }

    [HttpGet("products/{id:int}")]
    public async Task<ActionResult<ProductReportResponse>> GetProductReport(int id)
    {
        var report = await _context.ProductReports
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id);

        if (report == null)
            return NotFound(new { message = $"Không tìm thấy báo cáo sản phẩm có ID {id}" });

        var product = await _client.GetProductAsync(report.ProductId, Request.Headers.Authorization.ToString());
        return Ok(MapProductReport(report, product));
    }

    [HttpGet("orders")]
    public async Task<ActionResult<IEnumerable<OrderReportResponse>>> GetOrderReports()
    {
        var reports = await _context.OrderReports
            .Include(r => r.ProductReports)
            .AsNoTracking()
            .OrderBy(r => r.Id)
            .ToListAsync();

        var orders = await _client.GetOrdersAsync(Request.Headers.Authorization.ToString());
        var orderMap = orders?.ToDictionary(o => o.Id) ?? new Dictionary<int, ExternalOrderDto>();

        var result = reports.Select(r =>
        {
            orderMap.TryGetValue(r.OrderId, out var order);
            return MapOrderReport(r, order);
        });

        return Ok(result);
    }

    [HttpGet("orders/{id:int}")]
    public async Task<ActionResult<OrderReportResponse>> GetOrderReport(int id)
    {
        var report = await _context.OrderReports
            .Include(r => r.ProductReports)
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id);

        if (report == null)
            return NotFound(new { message = $"Không tìm thấy báo cáo đơn hàng có ID {id}" });

        var order = await _client.GetOrderAsync(report.OrderId, Request.Headers.Authorization.ToString());
        return Ok(MapOrderReport(report, order));
    }

    [HttpPost("products")]
    public async Task<ActionResult<ProductReportResponse>> CreateProductReport(
        [FromBody] CreateProductReportRequest request)
    {
        var token = Request.Headers.Authorization.ToString();
        var product = await _client.GetProductAsync(request.ProductId, token);
        if (product == null)
            return NotFound(new { message = $"Không tìm thấy sản phẩm có ID {request.ProductId}" });

        var totalSold = 0;
        var revenue = 0m;

        if (request.OrderReportId.HasValue)
        {
            var orderReport = await _context.OrderReports.FindAsync(request.OrderReportId.Value);
            if (orderReport == null)
                return NotFound(new { message = $"Không tìm thấy báo cáo đơn hàng có ID {request.OrderReportId.Value}" });

            var order = await _client.GetOrderAsync(orderReport.OrderId, token);
            if (order == null)
                return NotFound(new { message = $"Không tìm thấy đơn hàng có ID {orderReport.OrderId}" });

            var items = order.OrderItems.Where(i => i.ProductId == request.ProductId).ToList();
            totalSold = items.Sum(i => i.Quantity);
            revenue = items.Sum(i => i.TotalPrice);
        }
        else
        {
            var orders = await _client.GetOrdersAsync(token);
            if (orders == null)
                return BadRequest(new { message = "Không thể lấy dữ liệu từ OrderService" });

            var items = orders
                .Where(o => o.Status.ToLower() != "cancelled")
                .SelectMany(o => o.OrderItems)
                .Where(i => i.ProductId == request.ProductId)
                .ToList();

            totalSold = items.Sum(i => i.Quantity);
            revenue = items.Sum(i => i.TotalPrice);
        }

        var rate = request.CostRate ?? 0.7m;
        var cost = request.Cost
            ?? (request.UnitCost.HasValue
                ? request.UnitCost.Value * totalSold
                : Math.Round(revenue * rate, 2));

        var report = new ProductReport
        {
            OrderReportId = request.OrderReportId,
            ProductId = product.Id,
            TotalSold = totalSold,
            Revenue = revenue,
            Cost = cost,
            Profit = revenue - cost
        };

        _context.ProductReports.Add(report);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetProductReport),
            new { id = report.Id },
            MapProductReport(report, product));
    }

    [HttpPost("orders")]
    public async Task<ActionResult<OrderReportResponse>> CreateOrderReport(
        [FromBody] CreateOrderReportRequest request)
    {
        var order = await _client.GetOrderAsync(request.OrderId, Request.Headers.Authorization.ToString());
        if (order == null)
            return NotFound(new { message = $"Không tìm thấy đơn hàng có ID {request.OrderId}" });

        var totalRevenue = order.OrderItems.Count > 0
            ? order.OrderItems.Sum(i => i.TotalPrice)
            : order.TotalAmount;

        var rate = request.CostRate ?? 0.7m;
        var totalCost = request.TotalCost ?? Math.Round(totalRevenue * rate, 2);

        var report = new OrderReport
        {
            OrderId = order.Id,
            TotalRevenue = totalRevenue,
            TotalCost = totalCost,
            TotalProfit = totalRevenue - totalCost
        };

        _context.OrderReports.Add(report);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetOrderReport),
            new { id = report.Id },
            MapOrderReport(report, order));
    }

    [HttpDelete("products/{id:int}")]
    public async Task<IActionResult> DeleteProductReport(int id)
    {
        var report = await _context.ProductReports.FindAsync(id);
        if (report == null)
            return NotFound(new { message = $"Không tìm thấy báo cáo sản phẩm có ID {id}" });

        _context.ProductReports.Remove(report);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("orders/{id:int}")]
    public async Task<IActionResult> DeleteOrderReport(int id)
    {
        var report = await _context.OrderReports.FindAsync(id);
        if (report == null)
            return NotFound(new { message = $"Không tìm thấy báo cáo đơn hàng có ID {id}" });

        _context.OrderReports.Remove(report);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private static ProductReportResponse MapProductReport(ProductReport report, ExternalProductDto? product)
    {
        return new ProductReportResponse
        {
            Id = report.Id,
            OrderReportId = report.OrderReportId,
            ProductId = report.ProductId,
            ProductName = product?.Name,
            StockQuantity = product?.Quantity,
            TotalSold = report.TotalSold,
            Revenue = report.Revenue,
            Cost = report.Cost,
            Profit = report.Profit
        };
    }

    private static OrderReportResponse MapOrderReport(OrderReport report, ExternalOrderDto? order)
    {
        return new OrderReportResponse
        {
            Id = report.Id,
            OrderId = report.OrderId,
            CustomerName = order?.CustomerName,
            OrderStatus = order?.Status,
            TotalRevenue = report.TotalRevenue,
            TotalCost = report.TotalCost,
            TotalProfit = report.TotalProfit,
            ProductReports = report.ProductReports
        };
    }
}
