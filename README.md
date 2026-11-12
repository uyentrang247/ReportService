# ReportService – BTH5 SOA

# 1. Giới thiệu
ReportService là REST API thống kê báo cáo được xây dựng bằng ASP.NET Core Web API trong bài thực hành Service-Oriented Architecture (SOA).

# 2. Công nghệ sử dụng
- C# và ASP.NET Core Web API (.NET 8)
- Entity Framework Core
- SQLite
- JWT Bearer Authentication
- Swagger / OpenAPI

# 3. Chức năng
- Thống kê số lượng hàng tồn, số lượng đã bán, doanh thu, chi phí nhập và lợi nhuận.
- Xem danh sách và chi tiết báo cáo theo sản phẩm.
- Xem danh sách và chi tiết báo cáo theo đơn hàng.
- Tạo báo cáo sản phẩm mới từ ProductService và OrderService.
- Tạo báo cáo đơn hàng mới từ OrderService.
- Xóa báo cáo sản phẩm và báo cáo đơn hàng.
- Xác thực JWT khi truy cập các API báo cáo.

# 4. Các API

| Method | Endpoint | Chức năng |
|---|---|---|
| GET | '/reports/products' | Lấy danh sách báo cáo sản phẩm |
| GET | '/reports/products/{id}' | Lấy báo cáo sản phẩm theo ID |
| GET | '/reports/orders' | Lấy danh sách báo cáo đơn hàng |
| GET | '/reports/orders/{id}' | Lấy báo cáo đơn hàng theo ID |
| POST | '/reports/products' | Tạo báo cáo sản phẩm mới |
| POST | '/reports/orders' | Tạo báo cáo đơn hàng mới |
| DELETE | '/reports/products/{id}' | Xóa báo cáo sản phẩm |
| DELETE | '/reports/orders/{id}' | Xóa báo cáo đơn hàng |

# 5. Cách chạy
1. Khởi chạy service xác thực (BTH2) ở 'http://localhost:5144', ProductService ở 'http://localhost:5145' và OrderService ở 'http://localhost:5146'.
2. Mở project 'ReportService.csproj' bằng Visual Studio.
3. Khôi phục các NuGet packages.
4. Chạy project bằng profile HTTP đã cấu hình.
5. Truy cập 'http://localhost:5147/swagger' để thử nghiệm API.

# 6. Database
Ứng dụng sử dụng SQLite. Database được tạo tự động khi khởi chạy ứng dụng nếu chưa tồn tại.
