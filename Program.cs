// ================================================================
// PROGRAM.CS - CẤU HÌNH ENTITY FRAMEWORK CORE
// ================================================================

// Import namespaces cần thiết cho EF Core
using Microsoft.EntityFrameworkCore;           // DbContext, DbContextOptions
using BookStore.Data;                        // ApplicationDbContext

var builder = WebApplication.CreateBuilder(args);

// ================================================================
// ĐĂNG KÝ DbContext VÀO SERVICE CONTAINER
// ================================================================
//
// builder.Services là IServiceCollection chứa danh sách các services
// của ứng dụng. Khi đăng ký DbContext, ASP.NET Core sẽ tự động
// inject DbContext vào các Controller khi cần.
//
// Tại sao phải đăng ký?
// 1. Để Controller có thể nhận DbContext qua Constructor (DI)
// 2. DI Container quản lý vòng đời của DbContext (mỗi request 1 DbContext)
// 3. Connection string được đọc và inject tự động
// ================================================================

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    // UseSqlServer: Sử dụng SQL Server làm database provider
    // GetConnectionString: Đọc connection string từ appsettings.json
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    );
});

// Các services khác của MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days.
    // You may want to change this for production scenarios.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
