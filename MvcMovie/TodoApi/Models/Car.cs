using System.ComponentModel.DataAnnotations.Schema;
using TodoApi.Data;

namespace TodoApi.Models
{
    public class Car : Entity
    {
       
        public string CarNumber { get; set; }
        public string Status { get; set; } = "free"; // Начальный статус
        [Column(TypeName = "decimal(18,2)")]
        public decimal HourlyRate { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal Price { get; set; }
        public decimal KilometerRate { get; set; }
    }
}
