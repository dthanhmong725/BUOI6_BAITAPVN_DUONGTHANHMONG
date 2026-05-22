using Microsoft.EntityFrameworkCore;
using BookStore.Models;

namespace BookStore.Data;

/// <summary>
/// ApplicationDbContext - Lớp quản lý kết nối và thao tác với database
/// Kế thừa từ DbContext - class cốt lõi của Entity Framework Core
/// </summary>
public class ApplicationDbContext : DbContext
{
    /// <summary>
    /// Constructor nhận vào DbContextOptions để cấu hình kết nối database
    /// DbContextOptions chứa thông tin kết nối và các tùy chọn khác
    /// </summary>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// DbSet<Category> đại diện cho bảng Categories trong database
    /// Dùng để CRUD với bảng thể loại sách
    /// </summary>
    public DbSet<Category> Categories { get; set; }

    /// <summary>
    /// DbSet<Book> đại diện cho bảng Books trong database
    /// Dùng để CRUD với bảng sách
    /// </summary>
    public DbSet<Book> Books { get; set; }

    /// <summary>
    /// Phương thức OnModelCreating được gọi khi DbContext được khởi tạo
    /// Dùng để cấu hình chi tiết các entity, relationships, constraints
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ============================================
        // CẤU HÌNH QUAN HỆ ONE-TO-MANY
        // Category (1) -----> (*) Book
        // ============================================
        modelBuilder.Entity<Category>()
            .HasMany(c => c.Books)           // Category có nhiều Books
            .WithOne(b => b.Category)       // Book thuộc một Category
            .HasForeignKey(b => b.CategoryId) // Khóa ngoại là CategoryId
            .OnDelete(DeleteBehavior.Cascade); // Xóa Category → Xóa Books

        // ============================================
        // CẤU HÌNH CÁC RÀNG BUỘC
        // ============================================

        // CategoryName: Required, max 100 characters
        modelBuilder.Entity<Category>()
            .Property(c => c.CategoryName)
            .IsRequired()
            .HasMaxLength(100);

        // Book.Title: Required, max 200 characters
        modelBuilder.Entity<Book>()
            .Property(b => b.Title)
            .IsRequired()
            .HasMaxLength(200);

        // Book.Author: Required, max 100 characters
        modelBuilder.Entity<Book>()
            .Property(b => b.Author)
            .IsRequired()
            .HasMaxLength(100);

        // Book.Price: Decimal với 2 chữ số thập phân
        modelBuilder.Entity<Book>()
            .Property(b => b.Price)
            .HasColumnType("decimal(18,2)");
    }
}
