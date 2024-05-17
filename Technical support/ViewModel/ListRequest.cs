using System.ComponentModel;

namespace Technical_support.ViewModel
{
    public class ListRequest
    {
        [DisplayName("ID Заявки")]
        public virtual int RequestId { get; set; }

        [DisplayName("Имя Пользователя")]
        public virtual required string UserName { get; set; }

        [DisplayName("Тема заявки")]
        public virtual required string NameTheme { get; set; }

        [DisplayName("Приоритет заявки")]
        public virtual required string Prioritet { get; set; }

        [DisplayName("Текст заявки")]
        public virtual required string TextRequest { get; set; }

        [DisplayName("Файл")]
        public virtual byte[]? File { get; set; }

        [DisplayName("Дата открытия заявки")]
        public virtual required DateTime DateOpen { get; set; }

        [DisplayName("Статус заявки")]
        public virtual required string Status { get; set; }
    }
}
