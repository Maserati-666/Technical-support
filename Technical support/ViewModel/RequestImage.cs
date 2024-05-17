using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Technical_support.Models;

namespace Technical_support.ViewModel
{
    public class RequestImage
    {
        // ID заявки
        [Key]
        [Required]
        public virtual int RequestId { get; set; }

        // ID Пользователя
        [Required]
        [DisplayName("ID Пользователя")]
        public virtual required string UserId { get; set; }
        public IdentityUser User { get; set; }

        // ID темы заявки
        [Required]
        [DisplayName("ID темы заявки")]
        public virtual int ThemeId { get; set; }
        public required ThemeRequest Theme { get; set; }

        // ID приоритета заявки
        [Required]
        [DisplayName("Приоритет заявки")]
        public virtual int PrioritetId { get; set; }
        public required Prioritet Prioritet { get; set; }

        [Required]
        [DisplayName("Текст заявки")]
        public virtual required string TextRequest { get; set; }


        [DisplayName("Файл")]
        public virtual IFormFile? image { get; set; }
        
        [DisplayName("Файл")]
        public virtual byte[]? File { get; set; }

        // Дата открытия заявки
        [Required]
        [DisplayName("Дата открытия заявки")]
        //[DisplayFormat(DataFormatString = "{0:dd/MM/yy}")]
        public virtual required DateTime DateOpen { get; set; }

        // ID закрепленного Сотрудника
        [DisplayName("ID Сотрудника")]
        public virtual string? ManagerId { get; set; }

        public IdentityUser Manager { get; set; }

        // Дата закрытия заявки
        [DisplayName("Дата закрытия заявки")]
        //[DisplayFormat(DataFormatString = "{0:dd/MM/yy}")]
        public virtual DateTime? DateClose { get; set; }

        // ID Состояния заявки
        [Required]
        [DisplayName("ID Состояния заявки")]
        public virtual int StatusRequestId { get; set; }
        public StatusRequest StatusRequest { get; set; }
    }
}
