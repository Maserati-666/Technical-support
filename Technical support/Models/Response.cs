using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Technical_support.Models
{
    public class Response
    {
        // ID ответа
        [Key]
        [Required]
        [DisplayName("Номер ответа")]
        public virtual int ResponseId { get; set; }

        // ID заявки
        [Required]
        [DisplayName("Номер заявки")]
        public virtual int RequestId { get; set; }
        public Request Request { get; set; }

        [Required]
        [DisplayName("Ответ на заявку")]
        public virtual string TextResponse { get; set; }
    }
}
