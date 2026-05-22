# HƯỚNG DẪN CHI TIẾT: ỨNG DỤNG WEB CRUD SÁCH VỚI ASP.NET CORE MVC & ENTITY FRAMEWORK CORE

## MỤC LỤC

1. [Tổng quan dự án](#1--tổng-quan-dự-án)
2. [Cấu trúc Database](#2--cấu-trúc-database)
3. [Entity Models](#3--entity-models)
4. [DbContext và Program.cs](#4--dbcontext-và-programcs)
5. [Migrations và Seed Data](#5--migrations-và-seed-data)
6. [BookController CRUD](#6--bookcontroller-crud)
7. [CategoryController CRUD](#7--categorycontroller-crud)
8. [Views cho Book](#8--views-cho-book)
9. [Views cho Category](#9--views-cho-category)
10. [Layout, Navbar và Sidebar](#10--layout-navbar-và-sidebar)
11. [Xử lý hình ảnh](#11--xử-lý-hình-ảnh)
12. [Chạy ứng dụng](#12--chạy-ứng-dụng)
13. [Lỗi thường gặp](#13--lỗi-thường-gặp)

---

# BƯỚC 1: TỔNG QUAN DỰ ÁN

## 1.1. Giới thiệu

Đây là ứng dụng Web CRUD (Create, Read, Update, Delete) quản lý sách, được xây dựng với:

- **ASP.NET Core MVC** - Framework web
- **Entity Framework Core** - ORM để tương tác với database
- **SQL Server** - Database
- **Code First** - Thiết kế database từ code C#

## 1.2. Cấu trúc project

```
BookStore/
├── BookStore.csproj
├── Program.cs                              # Cấu hình ứng dụng
├── appsettings.json                       # Connection string
│
├── Models/                                # Entity Models
│   ├── Category.cs
│   ├── Book.cs
│   └── ErrorViewModel.cs
│
├── Data/                                  # DbContext
│   └── ApplicationDbContext.cs
│
├── Controllers/                           # Controllers
│   ├── HomeController.cs
│   ├── BookController.cs
│   └── CategoryController.cs
│
├── Views/                                 # Views
│   ├── Shared/
│   │   └── _Layout.cshtml               # Layout chính với Navbar
│   ├── Home/
│   │   └── Index.cshtml
│   ├── Book/
│   │   ├── Index.cshtml
│   │   ├── Details.cshtml
│   │   ├── Create.cshtml
│   │   ├── Edit.cshtml
│   │   └── Delete.cshtml
│   └── Category/
│       ├── Index.cshtml
│       ├── Details.cshtml
│       ├── Create.cshtml
│       ├── Edit.cshtml
│       └── Delete.cshtml
│
├── Migrations/                           # EF Core Migrations
│   ├── *_KhoiTaoBang_Sach_Va_TheLoai.cs
│   └── *_ThemDuLieuMau.cs
│
└── wwwroot/
    └── Content/
        └── ImageBooks/                   # Thư mục lưu ảnh sách
```

---

# BƯỚC 2: CẤU TRÚC DATABASE

## 2.1. Sơ đồ Database

```
┌─────────────────────────────────────────────────────────────────┐
│                      DATABASE: BookDB                            │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  ┌─────────────────┐          ┌─────────────────┐             │
│  │   Categories    │          │     Books        │             │
│  ├─────────────────┤          ├─────────────────┤             │
│  │ PK CategoryId   │◀───────│ FK CategoryId   │             │
│  │ CategoryName    │          │ PK Id           │             │
│  │                 │          │ Title           │             │
│  │                 │          │ Author          │             │
│  │                 │          │ Price           │             │
│  │                 │          │ Description     │             │
│  │                 │          │ Image           │             │
│  └─────────────────┘          └─────────────────┘             │
│                                                                  │
│  QUAN HỆ: Categories (1) ────n───► (n) Books                 │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

## 2.2. Bảng Categories

| Column | Data Type | Constraints | Mô tả |
|--------|-----------|------------|---------|
| CategoryId | int | PK, Identity | Khóa chính |
| CategoryName | nvarchar(100) | NOT NULL | Tên thể loại |

## 2.3. Bảng Books

| Column | Data Type | Constraints | Mô tả |
|--------|-----------|------------|---------|
| Id | int | PK, Identity | Khóa chính |
| Title | nvarchar(200) | NOT NULL | Tiêu đề sách |
| Author | nvarchar(100) | NOT NULL | Tác giả |
| Price | decimal(18,2) | NOT NULL | Giá bán |
| Description | nvarchar(max) | NULL | Mô tả |
| Image | nvarchar(max) | NULL | Đường dẫn hình ảnh |
| CategoryId | int | FK → Categories | Thể loại sách |

---

# BƯỚC 3: ENTITY MODELS

## 3.1. Category.cs

```csharp
using System.ComponentModel.DataAnnotations;

namespace BookStore.Models;

public class Category
{
    // Khóa chính - EF Core tự nhận diện property có tên "Id" hoặc "<ClassName>Id"
    public int CategoryId { get; set; }

    // Tên thể loại với Validation
    [Required(ErrorMessage = "Tên thể loại không được để trống")]
    [StringLength(100, ErrorMessage = "Tên thể loại không được vượt quá 100 ký tự")]
    [Display(Name = "Tên thể loại")]
    public string CategoryName { get; set; } = string.Empty;

    // Navigation Property - Quan hệ 1-n với Book
    public List<Book> Books { get; set; } = new List<Book>();
}
```

## 3.2. Book.cs

```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStore.Models;

public class Book
{
    // Khóa chính - EF Core nhận diện "Id" làm PK
    public int Id { get; set; }

    // Tiêu đề sách
    [Required(ErrorMessage = "Tiêu đề sách không được để trống")]
    [StringLength(200)]
    [Display(Name = "Tiêu đề sách")]
    public string Title { get; set; } = string.Empty;

    // Tác giả
    [Required]
    [StringLength(100)]
    public string Author { get; set; } = string.Empty;

    // Giá - decimal cho độ chính xác tiền tệ
    [Required]
    [Range(0, double.MaxValue)]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    // Mô tả (nullable)
    public string? Description { get; set; }

    // Hình ảnh - đường dẫn đến file trong wwwroot/Content/ImageBooks
    public string? Image { get; set; }

    // Foreign Key
    [Required]
    public int CategoryId { get; set; }

    // Navigation Property - Quan hệ n-1 với Category
    [ForeignKey("CategoryId")]
    public Category? Category { get; set; }
}
```

---

# BƯỚC 4: DBCONTEXT VÀ PROGRAM.CS

## 4.1. ApplicationDbContext.cs

```csharp
using Microsoft.EntityFrameworkCore;
using BookStore.Models;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // DbSets - đại diện cho các bảng
    public DbSet<Category> Categories { get; set; }
    public DbSet<Book> Books { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Cấu hình quan hệ 1-n: Category → Books
        modelBuilder.Entity<Category>()
            .HasMany(c => c.Books)
            .WithOne(b => b.Category)
            .HasForeignKey(b => b.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        // Cấu hình constraints
        modelBuilder.Entity<Category>()
            .Property(c => c.CategoryName)
            .IsRequired()
            .HasMaxLength(100);

        modelBuilder.Entity<Book>()
            .Property(b => b.Title)
            .IsRequired()
            .HasMaxLength(200);

        modelBuilder.Entity<Book>()
            .Property(b => b.Author)
            .IsRequired()
            .HasMaxLength(100);

        modelBuilder.Entity<Book>()
            .Property(b => b.Price)
            .HasColumnType("decimal(18,2)");
    }
}
```

## 4.2. Program.cs

```csharp
using Microsoft.EntityFrameworkCore;
using BookStore.Data;

var builder = WebApplication.CreateBuilder(args);

// Đăng ký DbContext vào DI Container
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    );
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
```

## 4.3. appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=LAPTOP-7JFPCL41\\SQLEXPRESS;Database=BookDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  }
}
```

---

# BƯỚC 5: MIGRATIONS VÀ SEED DATA

## 5.1. Các lệnh Migration

```bash
# Tạo migration khởi tạo
dotnet ef migrations add KhoiTaoBang_Sach_Va_TheLoai

# Tạo migration thêm dữ liệu mẫu
dotnet ef migrations add ThemDuLieuMau

# Apply tất cả migrations
dotnet ef database update
```

## 5.2. Dữ liệu mẫu đã thêm

**Categories:**
- Lập trình (CategoryId=1)
- Cuộc sống (CategoryId=2)
- Kinh tế (CategoryId=3)
- Văn học (CategoryId=4)

**Books:**
| Id | Title | Author | Price | CategoryId |
|----|----|--------|-------|-----------|
| 1 | Lập Trình C# Từ Cơ Bản Đến Nâng Cao | John Smith | 299,000 | 1 |
| 2 | Python Cho Người Mới Bắt Đầu | Jane Doe | 199,000 | 1 |
| 3 | Sống Đẹp Mỗi Ngày | Nguyễn Văn A | 150,000 | 2 |
| 4 | Nghĩ Lớn, Làm Lớn | Robert Kiyosaki | 350,000 | 3 |
| 5 | Truyện Kiều - Bản Đẹp | Nguyễn Du | 120,000 | 4 |

---

# BƯỚC 6: BOOKCONTROLLER CRUD

## 6.1. Code đầy đủ

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookStore.Data;
using BookStore.Models;
using System.IO;

public class BookController : Controller
{
    private readonly ApplicationDbContext _context;

    public BookController(ApplicationDbContext context)
    {
        _context = context;
    }

    // ================================================================
    // INDEX - DANH SÁCH SÁCH VỚI SIDEBAR THỐNG KÊ
    // ================================================================
    public IActionResult Index()
    {
        // Lấy danh sách sách
        var books = _context.Books
            .Include(b => b.Category)
            .OrderByDescending(b => b.Id)
            .ToList();

        // ================================================================
        // THỐNG KÊ SỐ LƯỢNG SÁCH THEO THỂ LOẠI
        // ĐÂY LÀ PHẦN "ĂN ĐIỂM"
        // ================================================================
        var categoriesWithCount = _context.Categories
            .Include(c => c.Books)
            .Select(c => new
            {
                c.CategoryId,
                c.CategoryName,
                BookCount = c.Books.Count
            })
            .OrderBy(c => c.CategoryName)
            .ToList();

        ViewBag.CategoriesWithCount = categoriesWithCount;
        ViewBag.TotalBooks = books.Count;

        return View(books);
    }

    // ================================================================
    // DETAILS - CHI TIẾT SÁCH
    // ================================================================
    public IActionResult Details(int? id)
    {
        if (id == null) return NotFound();

        var book = _context.Books
            .Include(b => b.Category)
            .FirstOrDefault(b => b.Id == id);

        if (book == null) return NotFound();

        return View(book);
    }

    // ================================================================
    // CREATE - TẠO SÁCH MỚI
    // ================================================================
    public IActionResult Create()
    {
        ViewBag.Categories = _context.Categories.OrderBy(c => c.CategoryName).ToList();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create([Bind("Id,Title,Author,Price,Description,CategoryId")] Book book,
        IFormFile? ImageFile)
    {
        // Xử lý upload hình ảnh
        if (ImageFile != null && ImageFile.Length > 0)
        {
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Content", "ImageBooks", fileName);

            using (var stream = new FileStream(uploadPath, FileMode.Create))
            {
                ImageFile.CopyTo(stream);
            }

            book.Image = "/Content/ImageBooks/" + fileName;
        }

        if (ModelState.IsValid)
        {
            _context.Add(book);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Categories = _context.Categories.OrderBy(c => c.CategoryName).ToList();
        return View(book);
    }

    // ================================================================
    // EDIT - SỬA SÁCH
    // ================================================================
    public IActionResult Edit(int? id)
    {
        if (id == null) return NotFound();
        var book = _context.Books.Find(id);
        if (book == null) return NotFound();
        ViewBag.Categories = _context.Categories.OrderBy(c => c.CategoryName).ToList();
        return View(book);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, [Bind("Id,Title,Author,Price,Description,CategoryId,Image")] Book book,
        IFormFile? ImageFile)
    {
        if (id != book.Id) return NotFound();

        // Xử lý hình ảnh mới
        if (ImageFile != null && ImageFile.Length > 0)
        {
            // Xóa ảnh cũ
            if (!string.IsNullOrEmpty(book.Image))
            {
                var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", book.Image.TrimStart('/'));
                if (System.IO.File.Exists(oldPath))
                    System.IO.File.Delete(oldPath);
            }

            // Lưu ảnh mới
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Content", "ImageBooks", fileName);
            using (var stream = new FileStream(uploadPath, FileMode.Create))
            {
                ImageFile.CopyTo(stream);
            }
            book.Image = "/Content/ImageBooks/" + fileName;
        }

        if (ModelState.IsValid)
        {
            _context.Update(book);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Categories = _context.Categories.OrderBy(c => c.CategoryName).ToList();
        return View(book);
    }

    // ================================================================
    // DELETE - XÓA SÁCH
    // ================================================================
    public IActionResult Delete(int? id)
    {
        if (id == null) return NotFound();
        var book = _context.Books.Include(b => b.Category).FirstOrDefault(b => b.Id == id);
        if (book == null) return NotFound();
        return View(book);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var book = _context.Books.Find(id);
        if (book != null)
        {
            // Xóa hình ảnh
            if (!string.IsNullOrEmpty(book.Image))
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", book.Image.TrimStart('/'));
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
            }

            _context.Books.Remove(book);
            _context.SaveChanges();
        }
        return RedirectToAction(nameof(Index));
    }

    // ================================================================
    // SEARCH - TÌM KIẾM SÁCH
    // ================================================================
    public IActionResult Search(string? keyword)
    {
        var books = _context.Books.Include(b => b.Category).AsQueryable();

        if (!string.IsNullOrEmpty(keyword))
        {
            books = books.Where(b => b.Title.Contains(keyword) || b.Author.Contains(keyword));
        }

        var result = books.OrderByDescending(b => b.Id).ToList();

        // Thống kê sidebar
        var categoriesWithCount = _context.Categories
            .Include(c => c.Books)
            .Select(c => new { c.CategoryId, c.CategoryName, BookCount = c.Books.Count })
            .ToList();

        ViewBag.CategoriesWithCount = categoriesWithCount;
        ViewBag.Keyword = keyword;

        return View("Index", result);
    }

    // ================================================================
    // FILTER BY CATEGORY - LỌC THEO THỂ LOẠI
    // ================================================================
    public IActionResult FilterByCategory(int categoryId)
    {
        var books = _context.Books
            .Include(b => b.Category)
            .Where(b => b.CategoryId == categoryId)
            .OrderByDescending(b => b.Id)
            .ToList();

        var categoriesWithCount = _context.Categories
            .Include(c => c.Books)
            .Select(c => new { c.CategoryId, c.CategoryName, BookCount = c.Books.Count })
            .ToList();

        ViewBag.CategoriesWithCount = categoriesWithCount;
        ViewBag.SelectedCategoryId = categoryId;

        return View("Index", books);
    }

    private bool BookExists(int id) => _context.Books.Any(b => b.Id == id);
}
```

---

# BƯỚC 7: CATEGORYCONTROLLER CRUD

## 7.1. Code đầy đủ

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookStore.Data;
using BookStore.Models;

public class CategoryController : Controller
{
    private readonly ApplicationDbContext _context;

    public CategoryController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var categories = _context.Categories
            .Include(c => c.Books)
            .OrderBy(c => c.CategoryName)
            .ToList();
        return View(categories);
    }

    public IActionResult Details(int? id)
    {
        if (id == null) return NotFound();
        var category = _context.Categories.Include(c => c.Books).FirstOrDefault(c => c.CategoryId == id);
        if (category == null) return NotFound();
        return View(category);
    }

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create([Bind("CategoryName")] Category category)
    {
        if (ModelState.IsValid)
        {
            _context.Add(category);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        return View(category);
    }

    public IActionResult Edit(int? id)
    {
        if (id == null) return NotFound();
        var category = _context.Categories.Find(id);
        if (category == null) return NotFound();
        return View(category);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, [Bind("CategoryId,CategoryName")] Category category)
    {
        if (id != category.CategoryId) return NotFound();
        if (ModelState.IsValid)
        {
            _context.Update(category);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        return View(category);
    }

    public IActionResult Delete(int? id)
    {
        if (id == null) return NotFound();
        var category = _context.Categories.Include(c => c.Books).FirstOrDefault(c => c.CategoryId == id);
        if (category == null) return NotFound();
        return View(category);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var category = _context.Categories.Find(id);
        if (category != null)
        {
            _context.Categories.Remove(category);
            _context.SaveChanges();
        }
        return RedirectToAction(nameof(Index));
    }

    private bool CategoryExists(int id) => _context.Categories.Any(e => e.CategoryId == id);
}
```

---

# BƯỚC 8: VIEWS CHO BOOK

## 8.1. Index.cshtml - Danh sách với Sidebar thống kê

Điểm nổi bật của trang Index:

```cshtml
@* SIDEBAR THỐNG KÊ - PHẦN ĂN ĐIỂM *@
@foreach (var category in ViewBag.CategoriesWithCount)
{
    <li class="list-group-item d-flex justify-content-between align-items-center">
        <a asp-controller="Book" asp-action="FilterByCategory"
           asp-route-categoryId="@category.CategoryId">
            @category.CategoryName
        </a>
        <span class="badge bg-secondary rounded-pill">@category.BookCount</span>
    </li>
}
```

## 8.2. Details.cshtml - Chi tiết sách (Giao diện thương mại điện tử)

Thiết kế giống Fahasa với:
- Hình ảnh lớn bên trái (col-md-5)
- Thông tin bên phải (col-md-7)
- Giá nổi bật
- Nút "Thêm vào giỏ hàng" và "Mua ngay"
- Thông tin bổ sung

## 8.3. Create.cshtml và Edit.cshtml

Các điểm quan trọng:
- Form với `enctype="multipart/form-data"` để upload file
- Tag Helper `asp-for` cho data binding
- Validation Scripts cho client-side validation
- Preview hình ảnh trước khi upload

## 8.4. Delete.cshtml

- Cảnh báo xác nhận xóa
- Hiển thị thông tin sẽ bị xóa
- Form POST để xác nhận

---

# BƯỚC 9: VIEWS CHO CATEGORY

Views cho Category tương tự Book nhưng đơn giản hơn vì chỉ có 2 trường:
- Index: Bảng danh sách với số sách
- Details: Chi tiết + danh sách sách trong thể loại
- Create, Edit, Delete: Forms đơn giản

---

# BƯỚC 10: LAYOUT, NAVBAR VÀ SIDEBAR

## 10.1. _Layout.cshtml

Layout chính với:
- Navbar Bootstrap
- Menu: Trang chủ, Sách, Thể loại
- Thanh tìm kiếm nhanh
- Footer với thông tin liên hệ

```html
<!-- Navbar với Bootstrap -->
<nav class="navbar navbar-expand-lg navbar-dark bg-primary">
    <div class="container">
        <a class="navbar-brand" asp-controller="Home" asp-action="Index">
            <i class="bi bi-book"></i> BookStore
        </a>
        <!-- Menu items -->
        <ul class="navbar-nav">
            <li class="nav-item">
                <a class="nav-link" asp-controller="Book" asp-action="Index">Sách</a>
            </li>
        </ul>
    </div>
</nav>
```

## 10.2. Sidebar trong Book/Index

Sidebar hiển thị:
- Tất cả sách (tổng số)
- Từng thể loại với số lượng sách
- Click để lọc theo thể loại

---

# BƯỚC 11: XỬ LÝ HÌNH ẢNH

## 11.1. Thư mục lưu ảnh

```
wwwroot/
└── Content/
    └── ImageBooks/           ← Lưu ảnh sách ở đây
```

## 11.2. Upload hình ảnh trong Controller

```csharp
[HttpPost]
public IActionResult Create(Book book, IFormFile? ImageFile)
{
    if (ImageFile != null && ImageFile.Length > 0)
    {
        // Tạo tên file duy nhất
        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
        
        // Đường dẫn lưu
        var uploadPath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "wwwroot", "Content", "ImageBooks",
            fileName
        );

        // Lưu file
        using (var stream = new FileStream(uploadPath, FileMode.Create))
        {
            ImageFile.CopyTo(stream);
        }

        // Lưu đường dẫn vào database
        book.Image = "/Content/ImageBooks/" + fileName;
    }
}
```

## 11.3. Hiển thị hình ảnh trong View

```html
@if (!string.IsNullOrEmpty(book.Image))
{
    <img src="@book.Image" class="card-img-top" alt="@book.Title" />
}
else
{
    <div class="bg-light text-center py-5">
        <i class="bi bi-image text-muted" style="font-size: 4rem;"></i>
    </div>
}
```

---

# BƯỚC 12: CHẠY ỨNG DỤNG

## 12.1. Chạy bằng dotnet CLI

```bash
cd BookStore
dotnet run
```

## 12.2. Truy cập ứng dụng

Sau khi chạy, truy cập:
- **Trang chủ:** http://localhost:5000/
- **Danh sách sách:** http://localhost:5000/Book
- **Thêm sách:** http://localhost:5000/Book/Create
- **Danh sách thể loại:** http://localhost:5000/Category

## 12.3. Thêm hình ảnh sách

1. Truy cập http://localhost:5000/Book/Create
2. Điền thông tin sách
3. Chọn file hình ảnh (JPG, PNG, GIF)
4. Nhấn "Lưu sách"
5. Hình ảnh sẽ được lưu vào `wwwroot/Content/ImageBooks/`

---

# BƯỚC 13: LỖI THƯỜNG GẶP

## 13.1. Unable to create DbContext

**Lỗi:** `Unable to create an object of type 'ApplicationDbContext'`

**Nguyên nhân:** Chưa đăng ký DbContext trong Program.cs

**Sửa:**
```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
```

## 13.2. Cannot resolve scoped service

**Lỗi:** `Cannot resolve scoped service 'BookStore.Data.ApplicationDbContext'`

**Nguyên nhân:** DbContext chưa được đăng ký đúng cách

## 13.3. Invalid object name

**Lỗi:** `Invalid object name 'Books'`

**Nguyên nhân:** Chưa tạo database hoặc chưa apply migration

**Sửa:**
```bash
dotnet ef database update
```

## 13.4. Upload ảnh không hoạt động

**Nguyên nhân:** Thiếu `enctype="multipart/form-data"` trong form

**Sửa:**
```html
<form asp-action="Create" enctype="multipart/form-data">
```

## 13.5. Hình ảnh không hiển thị

**Nguyên nhân:** Sai đường dẫn hoặc thiếu thư mục

**Sửa:** Tạo thư mục `wwwroot/Content/ImageBooks/`

---

# TÓM TẮT CÁC TÍNH NĂNG

## ✅ Đã hoàn thành

| Tính năng | Mô tả |
|-----------|-------|
| CRUD Book | Create, Read, Update, Delete sách |
| CRUD Category | Create, Read, Update, Delete thể loại |
| Sidebar thống kê | Đếm số sách theo thể loại |
| Tìm kiếm | Tìm theo tiêu đề hoặc tác giả |
| Lọc theo thể loại | Click sidebar để lọc |
| Upload hình ảnh | Lưu vào wwwroot/Content/ImageBooks |
| Validation | Client-side và Server-side |
| Layout với Navbar | Menu điều hướng |
| Giao diện thương mại điện tử | Trang Details giống Fahasa |
| Seed Data | Dữ liệu mẫu được thêm qua Migration |
