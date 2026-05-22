using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookStore.Data;
using BookStore.Models;
using System.IO;

namespace BookStore.Controllers;

/// <summary>
/// BookController - Xử lý các request liên quan đến Sách
/// Thực hiện CRUD đầy đủ: Create, Read, Update, Delete
/// </summary>
public class BookController : Controller
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Constructor - Nhận DbContext qua Dependency Injection
    /// </summary>
    public BookController(ApplicationDbContext context)
    {
        _context = context;
    }

    // ================================================================
    // INDEX - HIỂN THỊ DANH SÁCH SÁCH VÀ SIDEBAR THỐNG KÊ
    // ================================================================

    /// <summary>
    /// Action Index - Hiển thị danh sách sách với sidebar thống kê
    /// Route: /Book/Index hoặc /Book
    /// </summary>
    /// <returns>Danh sách sách và thống kê theo thể loại</returns>
    public IActionResult Index()
    {
        // Lấy danh sách sách kèm thông tin thể loại
        var books = _context.Books
            .Include(b => b.Category)
            .OrderByDescending(b => b.Id)
            .ToList();

        // ================================================================
        // THỐNG KÊ SỐ LƯỢNG SÁCH THEO THỂ LOẠI
        // Đây là phần "ăn điểm" - đếm số sách theo từng Category
        // ================================================================
        var categoriesWithCount = _context.Categories
            .Include(c => c.Books)  // Load luôn danh sách Books của mỗi Category
            .Select(c => new
            {
                c.CategoryId,
                c.CategoryName,
                BookCount = c.Books.Count  // Đếm số sách trong mỗi Category
            })
            .OrderBy(c => c.CategoryName)
            .ToList();

        // Truyền dữ liệu sang View qua ViewBag
        ViewBag.CategoriesWithCount = categoriesWithCount;
        ViewBag.TotalBooks = books.Count;

        return View(books);
    }

    // ================================================================
    // DETAILS - HIỂN THỊ CHI TIẾT SÁCH
    // ================================================================

    /// <summary>
    /// Action Details - Hiển thị chi tiết một cuốn sách
    /// Route: /Book/Details/{id}
    /// Thiết kế giao diện giống trang thương mại điện tử (Fahasa)
    /// </summary>
    /// <param name="id">Id của sách cần xem</param>
    /// <returns>View chi tiết sách</returns>
    public IActionResult Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        // Tìm sách theo Id, load luôn thông tin Category
        var book = _context.Books
            .Include(b => b.Category)
            .FirstOrDefault(b => b.Id == id);

        if (book == null)
        {
            return NotFound();
        }

        return View(book);
    }

    // ================================================================
    // CREATE - TẠO SÁCH MỚI
    // ================================================================

    /// <summary>
    /// Action Create (GET) - Hiển thị form tạo sách mới
    /// Route: /Book/Create
    /// </summary>
    /// <returns>View form tạo sách</returns>
    public IActionResult Create()
    {
        // Lấy danh sách Categories để hiển thị trong dropdown
        // Dùng SelectList để phù hợp với asp-items trong View
        ViewBag.Categories = new SelectList(
            _context.Categories.OrderBy(c => c.CategoryName).ToList(),
            "CategoryId",
            "CategoryName"
        );
        return View();
    }

    /// <summary>
    /// Action Create (POST) - Xử lý tạo sách mới
    /// Route: /Book/Create (POST)
    /// </summary>
    /// <param name="book">Đối tượng Book từ form</param>
    /// <returns>Redirect về Index nếu thành công</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create([Bind("Id,Title,Author,Price,Description,CategoryId")] Book book,
        IFormFile? ImageFile)
    {
        // ================================================================
        // XỬ LÝ HÌNH ẢNH
        // Nếu có file ảnh được upload, lưu vào thư mục wwwroot/Content/ImageBooks
        // ================================================================
        if (ImageFile != null && ImageFile.Length > 0)
        {
            // Tạo tên file duy nhất để tránh trùng lặp
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Content", "ImageBooks", fileName);

            // Lưu file vào thư mục
            using (var stream = new FileStream(uploadPath, FileMode.Create))
            {
                ImageFile.CopyTo(stream);
            }

            // Lưu đường dẫn vào thuộc tính Image của Book
            book.Image = "/Content/ImageBooks/" + fileName;
        }

        // Kiểm tra ModelState - Validation phía Server
        if (ModelState.IsValid)
        {
            _context.Add(book);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // Nếu validation lỗi, load lại danh sách Categories
        ViewBag.Categories = new SelectList(
            _context.Categories.OrderBy(c => c.CategoryName).ToList(),
            "CategoryId",
            "CategoryName"
        );
        return View(book);
    }

    // ================================================================
    // EDIT - SỬA SÁCH
    // ================================================================

    /// <summary>
    /// Action Edit (GET) - Hiển thị form sửa sách
    /// Route: /Book/Edit/{id}
    /// </summary>
    /// <param name="id">Id của sách cần sửa</param>
    /// <returns>View form sửa sách</returns>
    public IActionResult Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var book = _context.Books.Find(id);

        if (book == null)
        {
            return NotFound();
        }

        // Load danh sách Categories cho dropdown
        ViewBag.Categories = new SelectList(
            _context.Categories.OrderBy(c => c.CategoryName).ToList(),
            "CategoryId",
            "CategoryName"
        );
        return View(book);
    }

    /// <summary>
    /// Action Edit (POST) - Xử lý cập nhật sách
    /// Route: /Book/Edit/{id} (POST)
    /// </summary>
    /// <param name="id">Id của sách cần cập nhật</param>
    /// <param name="book">Đối tượng Book từ form</param>
    /// <returns>Redirect về Index nếu thành công</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, [Bind("Id,Title,Author,Price,Description,CategoryId,Image")] Book book,
        IFormFile? ImageFile)
    {
        if (id != book.Id)
        {
            return NotFound();
        }

        // Xử lý hình ảnh mới nếu có
        if (ImageFile != null && ImageFile.Length > 0)
        {
            // Xóa hình ảnh cũ nếu tồn tại
            if (!string.IsNullOrEmpty(book.Image))
            {
                var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", book.Image.TrimStart('/'));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }

            // Lưu hình ảnh mới
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
            try
            {
                _context.Update(book);
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(book.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction(nameof(Index));
        }

        ViewBag.Categories = new SelectList(
            _context.Categories.OrderBy(c => c.CategoryName).ToList(),
            "CategoryId",
            "CategoryName"
        );
        return View(book);
    }

    // ================================================================
    // DELETE - XÓA SÁCH
    // ================================================================

    /// <summary>
    /// Action Delete (GET) - Hiển thị xác nhận xóa
    /// Route: /Book/Delete/{id}
    /// </summary>
    /// <param name="id">Id của sách cần xóa</param>
    /// <returns>View xác nhận xóa</returns>
    public IActionResult Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var book = _context.Books
            .Include(b => b.Category)
            .FirstOrDefault(b => b.Id == id);

        if (book == null)
        {
            return NotFound();
        }

        return View(book);
    }

    /// <summary>
    /// Action DeleteConfirmed (POST) - Xác nhận xóa sách
    /// Route: /Book/DeleteConfirmed/{id} (POST)
    /// </summary>
    /// <param name="id">Id của sách cần xóa</param>
    /// <returns>Redirect về Index</returns>
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var book = _context.Books.Find(id);

        if (book != null)
        {
            // Xóa hình ảnh nếu tồn tại
            if (!string.IsNullOrEmpty(book.Image))
            {
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", book.Image.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            _context.Books.Remove(book);
            _context.SaveChanges();
        }

        return RedirectToAction(nameof(Index));
    }

    // ================================================================
    // SEARCH - TÌM KIẾM SÁCH
    // ================================================================

    /// <summary>
    /// Action Search - Tìm kiếm sách theo tiêu đề hoặc tác giả
    /// Route: /Book/Search?keyword=...
    /// </summary>
    /// <param name="keyword">Từ khóa tìm kiếm</param>
    /// <returns>Danh sách sách phù hợp</returns>
    public IActionResult Search(string? keyword)
    {
        var books = _context.Books.Include(b => b.Category).AsQueryable();

        if (!string.IsNullOrEmpty(keyword))
        {
            books = books.Where(b =>
                b.Title.Contains(keyword) ||
                b.Author.Contains(keyword));
        }

        var result = books.OrderByDescending(b => b.Id).ToList();

        // Thống kê sidebar
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
        ViewBag.Keyword = keyword;

        return View("Index", result);
    }

    /// <summary>
    /// Action FilterByCategory - Lọc sách theo thể loại
    /// Route: /Book/FilterByCategory/{categoryId}
    /// </summary>
    /// <param name="categoryId">Id của thể loại</param>
    /// <returns>Danh sách sách theo thể loại</returns>
    public IActionResult FilterByCategory(int categoryId)
    {
        var books = _context.Books
            .Include(b => b.Category)
            .Where(b => b.CategoryId == categoryId)
            .OrderByDescending(b => b.Id)
            .ToList();

        // Thống kê sidebar
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
        ViewBag.SelectedCategoryId = categoryId;

        return View("Index", books);
    }

    /// <summary>
    /// Kiểm tra sách có tồn tại hay không
    /// </summary>
    private bool BookExists(int id)
    {
        return _context.Books.Any(b => b.Id == id);
    }
}
