using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TheFirstTask.Model
{
    public class TaskOrderDetail
    {
        // Thuộc tính Id là khóa chính của bảng Order
        [Key] // Xác định đây là khóa chính
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Xác định ID được tạo tự động
        public int Id { get; set; }
        [Required]
        public int TaskId { get; set; }
        [Required]
        public int OrderId { get; set; }
    }
}
