using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookStore.Data;
using BookStore.Models;

namespace BookStore.Controllers;

/// <summary>
/// CategoryController - Xử lý các request liên quan đến Thể loại sách
/// Thực hiện CRUD đầy đủ: Create, Read, Update, Delete
/// </summary>
public class CategoryController : Controller
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Constructor - Nhận DbContext qua Dependency Injection
    /// </summary>
    public CategoryController(ApplicationDbContext context)
    {
        _context = context;
    }

    // ================================================================
    // INDEX - HIỂN THỊ DANH SÁCH THỂ LOẠI
    // ================================================================

    /// <summary>
    /// Action Index - Hiển thị danh sách tất cả thể loại
    /// Route: /Category/Index hoặc /Category
    /// </summary>
    /// <returns>Danh sách thể loại</returns>
    public IActionResult Index()
    {
        // Lấy danh sách thể loại kèm số lượng sách
        var categories = _context.Categories
            .Include(c => c.Books)
            .OrderBy(c => c.CategoryName)
            .ToList();

        return View(categories);
    }

    // ================================================================
    // DETAILS - HIỂN THỊ CHI TIẾT THỂ LOẠI
    // ================================================================

    /// <summary>
    /// Action Details - Hiển thị chi tiết một thể loại và danh sách sách
    /// Route: /Category/Details/{id}
    /// </summary>
    /// <param name="id">CategoryId cần xem</param>
    /// <returns>View chi tiết thể loại</returns>
    public IActionResult Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        // Tìm thể loại và load luôn danh sách sách
        var category = _context.Categories
            .Include(c => c.Books)
            .FirstOrDefault(c => c.CategoryId == id);

        if (category == null)
        {
            return NotFound();
        }

        return View(category);
    }

    // ================================================================
    // CREATE - TẠO THỂ LOẠI MỚI
    // ================================================================

    /// <summary>
    /// Action Create (GET) - Hiển thị form tạo thể loại mới
    /// Route: /Category/Create
    /// </summary>
    /// <returns>View form tạo thể loại</returns>
    public IActionResult Create()
    {
        return View();
    }

    /// <summary>
    /// Action Create (POST) - Xử lý tạo thể loại mới
    /// Route: /Category/Create (POST)
    /// </summary>
    /// <param name="category">Đối tượng Category từ form</param>
    /// <returns>Redirect về Index nếu thành công</returns>
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

    // ================================================================
    // EDIT - SỬA THỂ LOẠI
    // ================================================================

    /// <summary>
    /// Action Edit (GET) - Hiển thị form sửa thể loại
    /// Route: /Category/Edit/{id}
    /// </summary>
    /// <param name="id">CategoryId cần sửa</param>
    /// <returns>View form sửa thể loại</returns>
    public IActionResult Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var category = _context.Categories.Find(id);

        if (category == null)
        {
            return NotFound();
        }

        return View(category);
    }

    /// <summary>
    /// Action Edit (POST) - Xử lý cập nhật thể loại
    /// Route: /Category/Edit/{id} (POST)
    /// </summary>
    /// <param name="id">CategoryId cần cập nhật</param>
    /// <param name="category">Đối tượng Category từ form</param>
    /// <returns>Redirect về Index nếu thành công</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, [Bind("CategoryId,CategoryName")] Category category)
    {
        if (id != category.CategoryId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(category);
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(category.CategoryId))
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

        return View(category);
    }

    // ================================================================
    // DELETE - XÓA THỂ LOẠI
    // ================================================================

    /// <summary>
    /// Action Delete (GET) - Hiển thị xác nhận xóa
    /// Route: /Category/Delete/{id}
    /// </summary>
    /// <param name="id">CategoryId cần xóa</param>
    /// <returns>View xác nhận xóa</returns>
    public IActionResult Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var category = _context.Categories
            .Include(c => c.Books)
            .FirstOrDefault(c => c.CategoryId == id);

        if (category == null)
        {
            return NotFound();
        }

        return View(category);
    }

    /// <summary>
    /// Action DeleteConfirmed (POST) - Xác nhận xóa thể loại
    /// Route: /Category/DeleteConfirmed/{id} (POST)
    /// </summary>
    /// <param name="id">CategoryId cần xóa</param>
    /// <returns>Redirect về Index</returns>
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

    /// <summary>
    /// Kiểm tra thể loại có tồn tại hay không
    /// </summary>
    private bool CategoryExists(int id)
    {
        return _context.Categories.Any(e => e.CategoryId == id);
    }
}
