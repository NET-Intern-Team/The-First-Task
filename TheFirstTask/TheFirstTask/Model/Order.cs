using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheFirstTask.Model
{
    public class Order
    {
        // Thuộc tính Id là khóa chính của bảng Order
        [Key] // Xác định đây là khóa chính
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Xác định ID được tạo tự động
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public long Quantity { get; set; }
    }
}