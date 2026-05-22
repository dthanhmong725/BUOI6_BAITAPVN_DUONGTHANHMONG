using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookStore.Migrations
{
    /// <inheritdoc />
    public partial class ThemDuLieuMau : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ================================================================
            // THÊM DỮ LIỆU MẪU VÀO BẢNG CATEGORIES
            // ================================================================

            // Thêm Category 1: Lập trình
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryName" },
                values: new object[] { "Lập trình" });

            // Thêm Category 2: Cuộc sống
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryName" },
                values: new object[] { "Cuộc sống" });

            // Thêm Category 3: Kinh tế
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryName" },
                values: new object[] { "Kinh tế" });

            // Thêm Category 4: Văn học
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryName" },
                values: new object[] { "Văn học" });

            // ================================================================
            // THÊM DỮ LIỆU MẪU VÀO BẢNG BOOKS
            // ================================================================
            // Lưu ý: CategoryId phải tham chiếu đến Category hợp lệ
            // CategoryId = 1 → Lập trình
            // CategoryId = 2 → Cuộc sống
            // CategoryId = 3 → Kinh tế
            // CategoryId = 4 → Văn học
            // ================================================================

            // Thêm Book 1: Sách Lập trình C#
            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "Title", "Author", "Price", "Description", "Image", "CategoryId" },
                values: new object[] { "Lập Trình C# Từ Cơ Bản Đến Nâng Cao", "John Smith", 299000m, 
                    "Cuốn sách hướng dẫn lập trình C# từ những khái niệm cơ bản nhất, đi sâu vào lập trình hướng đối tượng, LINQ, Entity Framework và xây dựng ứng dụng thực tế.", 
                    "/Content/ImageBooks/book1.jpg", 1 });

            // Thêm Book 2: Sách Lập trình Python
            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "Title", "Author", "Price", "Description", "Image", "CategoryId" },
                values: new object[] { "Python Cho Người Mới Bắt Đầu", "Jane Doe", 199000m,
                    "Học lập trình Python từ con số 0 với phong cách viết dễ hiểu, có nhiều bài tập thực hành và ví dụ minh họa sinh động.",
                    "/Content/ImageBooks/book2.jpg", 1 });

            // Thêm Book 3: Sách Cuộc sống
            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "Title", "Author", "Price", "Description", "Image", "CategoryId" },
                values: new object[] { "Sống Đẹp Mỗi Ngày", "Nguyễn Văn A", 150000m,
                    "Những bài học về cách sống tích cực, yêu thương và hạnh phúc mỗi ngày. Một cuốn sách truyền cảm hứng cho tất cả mọi người.",
                    "/Content/ImageBooks/book3.jpg", 2 });

            // Thêm Book 4: Sách Kinh tế
            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "Title", "Author", "Price", "Description", "Image", "CategoryId" },
                values: new object[] { "Nghĩ Lớn, Làm Lớn", "Robert Kiyosaki", 350000m,
                    "Cuốn sách kinh điển về tư duy tài chính và đầu tư. Học cách xây dựng sự giàu có bền vững từ người thầy về tài chính cá nhân.",
                    "/Content/ImageBooks/book4.jpg", 3 });

            // Thêm Book 5: Sách Văn học
            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "Title", "Author", "Price", "Description", "Image", "CategoryId" },
                values: new object[] { "Truyện Kiều - Bản Đẹp", "Nguyễn Du", 120000m,
                    "Tác phẩm văn học kinh điển của đại thi hào Nguyễn Du, với ngôn ngữ thơ mộng và những bài học nhân sinh sâu sắc.",
                    "/Content/ImageBooks/book5.jpg", 4 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // ================================================================
            // ROLLBACK: Xóa dữ liệu đã thêm
            // Xóa theo thứ tự ngược lại: Books trước, sau đó Categories
            // ================================================================

            // Xóa Books (xóa theo thứ tự ngược lại)
            migrationBuilder.DeleteData(table: "Books", keyColumn: "Id", keyValue: 5);
            migrationBuilder.DeleteData(table: "Books", keyColumn: "Id", keyValue: 4);
            migrationBuilder.DeleteData(table: "Books", keyColumn: "Id", keyValue: 3);
            migrationBuilder.DeleteData(table: "Books", keyColumn: "Id", keyValue: 2);
            migrationBuilder.DeleteData(table: "Books", keyColumn: "Id", keyValue: 1);

            // Xóa Categories (xóa theo thứ tự ngược lại)
            migrationBuilder.DeleteData(table: "Categories", keyColumn: "CategoryId", keyValue: 4);
            migrationBuilder.DeleteData(table: "Categories", keyColumn: "CategoryId", keyValue: 3);
            migrationBuilder.DeleteData(table: "Categories", keyColumn: "CategoryId", keyValue: 2);
            migrationBuilder.DeleteData(table: "Categories", keyColumn: "CategoryId", keyValue: 1);
        }
    }
}
