using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Technical_support.Models
{
    public class StatusRequest
    {
        // ID статуса заявки
        [Key]
        [Required]
        public virtual int StatusRequestId { get; set; }

        [Required]
        [DisplayName("Статус заявки")]
        public virtual required string Status { get; set; }
    }
}
