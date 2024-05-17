using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Technical_support.Models
{
    public class Prioritet
    {
        // ID приоритета заявки
        [Key]
        [Required]
        public virtual int PrioritetId { get; set; }

        [Required]
        [DisplayName("Приоритет заявки")]
        public virtual required string PrioritetRequest { get; set; }
    }
}
