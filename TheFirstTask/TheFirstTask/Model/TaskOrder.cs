using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TheFirstTask.Model
{
    public class TaskOrder
    {
        // Thuộc tính Id là khóa chính của bảng Order
        [Key] // Xác định đây là khóa chính
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Xác định ID được tạo tự động

        public int Id { get; set; }
        [Required]
        public string TaskContent { get; set; }

    }
}
