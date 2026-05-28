using System.ComponentModel.DataAnnotations;

namespace AutoRentimine.BlazorWasm.DTOs
{
    public class CarDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Марка обязательна")]
        public string Mark { get; set; } = string.Empty;

        [Required(ErrorMessage = "Модель обязательна")]
        public string Model { get; set; } = string.Empty;

        [Required(ErrorMessage = "Статус обязателен")]
        public string Status { get; set; } = string.Empty;

        [Range(1, 10000, ErrorMessage = "Цена должна быть от 1 до 10000")]
        public decimal DailyRate { get; set; }

        [Required(ErrorMessage = "Номер обязателен")]
        public string RegistrationNumber { get; set; } = string.Empty;
    }
}