using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Technical_support.Models
{
    public class ThemeRequest
    {
        // ID темы заявки
        [Key]
        [Required]
        [DisplayName("ID темы заявки")]
        public virtual int ThemeId { get; set; }

        // Название роли
        [Required]
        [DisplayName("Название темы")]
        public virtual required string NameTheme { get; set; }

        // Описание роли
        [Required]
        [DisplayName("Описание темы")]
        public virtual string? DescriptionTheme { get; set; }
    }
}
