using System.ComponentModel.DataAnnotations;
using TodoApi.Data;

namespace TodoApi.Models
{
    public class User : Entity
    {
       

        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty; // В реальных проектах тут хэш

        // Этот флаг нужен для блока "Administrator" на твоей схеме
        public bool IsAdmin { get; set; } = false;
    }
}