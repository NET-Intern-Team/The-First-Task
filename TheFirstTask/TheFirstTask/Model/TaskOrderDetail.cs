using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TheFirstTask.Model
{
    public class TaskOrderDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public int TaskId { get; set; }
        [Required]
        public int OrderId { get; set; }
    }
}
