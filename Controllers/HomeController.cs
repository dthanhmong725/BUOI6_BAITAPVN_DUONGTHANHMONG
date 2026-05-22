using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookStore.Data;
using BookStore.Models;

namespace BookStore.Controllers;

/// <summary>
/// HomeController - Trang chủ của ứng dụng BookStore
/// </summary>
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    /// <summary>
    /// Trang chủ - Hiển thị sách nổi bật và thể loại
    /// </summary>
    public IActionResult Index()
    {
        // Lấy danh sách sách nổi bật (sách mới nhất)
        var featuredBooks = _context.Books
            .Include(b => b.Category)
            .OrderByDescending(b => b.Id)
            .Take(6)
            .ToList();

        // Lấy danh sách thể loại với số lượng sách
        var categories = _context.Categories
            .Include(c => c.Books)
            .OrderBy(c => c.CategoryName)
            .ToList();

        // Truyền dữ liệu qua ViewBag
        ViewBag.FeaturedBooks = featuredBooks;
        ViewBag.Categories = categories;

        return View();
    }

    /// <summary>
    /// Trang giới thiệu
    /// </summary>
    public IActionResult Privacy()
    {
        return View();
    }

    /// <summary>
    /// Trang lỗi
    /// </summary>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
