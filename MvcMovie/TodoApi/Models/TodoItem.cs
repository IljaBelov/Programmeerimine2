using System.ComponentModel.DataAnnotations;

namespace TodoApi.Models
{
    public class TodoItem
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "Nimi on kohustuslik")]
        [StringLength(100, ErrorMessage = "Nimi ei tohi olla pikem kui 100 sümbolit")]
        public string? Name { get; set; }

        public bool IsComplete { get; set; }
    }
}