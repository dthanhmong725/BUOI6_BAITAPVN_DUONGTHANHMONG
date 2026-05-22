using System.ComponentModel.DataAnnotations;

namespace BookStore.Models;

/// <summary>
/// Category - Thể loại sách
/// Mỗi Category có thể chứa nhiều Book (quan hệ 1-n)
/// </summary>
public class Category
{
    /// <summary>
    /// Khóa chính của bảng Category
    /// EF Core tự động nhận diện property có tên kết thúc bằng "Id"
    /// làm Primary Key với Auto-increment
    /// </summary>
    public int CategoryId { get; set; }

    /// <summary>
    /// Tên thể loại sách (ví dụ: "Lập trình", "Cuộc sống", "Kinh tế")
    /// </summary>
    [Required(ErrorMessage = "Tên thể loại không được để trống")]
    [StringLength(100, ErrorMessage = "Tên thể loại không được vượt quá 100 ký tự")]
    [Display(Name = "Tên thể loại")]
    public string CategoryName { get; set; } = string.Empty;

    /// <summary>
    /// Navigation Property - Thể hiện quan hệ One-to-Many với Book
    /// Một Category có thể có nhiều Book
    /// </summary>
    public List<Book> Books { get; set; } = new List<Book>();
}
