using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStore.Models;

/// <summary>
/// Book - Thông tin sách
/// Mỗi Book thuộc về một Category (quan hệ n-1)
/// </summary>
public class Book
{
    /// <summary>
    /// Khóa chính của bảng Book
    /// EF Core tự động nhận diện property có tên "Id" hoặc "<ClassName>Id"
    /// làm Primary Key với Auto-increment
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Tiêu đề sách
    /// </summary>
    [Required(ErrorMessage = "Tiêu đề sách không được để trống")]
    [StringLength(200, ErrorMessage = "Tiêu đề không được vượt quá 200 ký tự")]
    [Display(Name = "Tiêu đề sách")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Tên tác giả sách
    /// </summary>
    [Required(ErrorMessage = "Tên tác giả không được để trống")]
    [StringLength(100, ErrorMessage = "Tên tác giả không được vượt quá 100 ký tự")]
    [Display(Name = "Tác giả")]
    public string Author { get; set; } = string.Empty;

    /// <summary>
    /// Giá sách
    /// Sử dụng kiểu decimal để đảm bảo độ chính xác cho tiền tệ
    /// </summary>
    [Required(ErrorMessage = "Giá sách không được để trống")]
    [Range(0, double.MaxValue, ErrorMessage = "Giá sách phải lớn hơn hoặc bằng 0")]
    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Giá bán")]
    public decimal Price { get; set; }

    /// <summary>
    /// Mô tả chi tiết về sách
    /// </summary>
    [Display(Name = "Mô tả")]
    public string? Description { get; set; }

    /// <summary>
    /// Đường dẫn hình ảnh bìa sách
    /// Lưu trong thư mục /Content/ImageBooks/
    /// </summary>
    [Display(Name = "Hình ảnh")]
    public string? Image { get; set; }

    /// <summary>
    /// Foreign Key - Khóa ngoại tham chiếu đến bảng Category
    /// Xác định sách thuộc thể loại nào
    /// </summary>
    [Required(ErrorMessage = "Vui lòng chọn thể loại sách")]
    [Display(Name = "Thể loại")]
    public int CategoryId { get; set; }

    /// <summary>
    /// Navigation Property - Thể hiện quan hệ Many-to-One với Category
    /// Mỗi Book thuộc một Category
    /// </summary>
    [ForeignKey("CategoryId")]
    public Category? Category { get; set; }
}
