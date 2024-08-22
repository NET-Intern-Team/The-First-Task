using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheFirstTask.Model
{
    public class Product
    {
        // Thuộc tính Id là khóa chính của bảng Order
        [Key] // Xác định đây là khóa chính
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Xác định ID được tạo tự động
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public int CategoryId { get; set; }

    }
}
