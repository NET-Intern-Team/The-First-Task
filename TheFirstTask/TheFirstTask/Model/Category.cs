using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheFirstTask.Model
{
    public class Category
    {
        // Thuộc tính Id là khóa chính của bảng Category
        [Key] // Xác định đây là khóa chính
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Xác định ID được tạo tự động
        public int Id { get; set; }

        public string? Name { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}