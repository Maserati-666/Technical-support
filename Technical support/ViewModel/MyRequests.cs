using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using Technical_support.Models;

namespace Technical_support.ViewModel
{
    public class MyRequests
    {
        [DisplayName("Номер заявки")]
        public int RequestId { get; set; }
        [DisplayName("Тема заявки")]
        public string Theme { get; set; }
        [DisplayName("Приоритет заявки")]
        public string Prioritet { get; set; }

        [DisplayName("Текст заявки")]
        public string TextRequest { get; set; }
        [DisplayName("Дата открытия")]
        public DateTime DateOpen { get; set; }
        [DisplayName("Менеджер")]
        public string? ManagerName { get; set; }
        [DisplayName("Дата закрытия")]
        public DateTime? DateClose { get; set; }
        [DisplayName("Статус заявки")]
        public string StatusRequest { get; set; }
    }
}
