using System.ComponentModel.DataAnnotations;

namespace autorentimineProjekt.ToDoApi.Models
{
    public class Car
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Mark is required")]
        [StringLength(50, ErrorMessage = "Mark cannot exceed 50 characters")]
        public string Mark { get; set; } = string.Empty;

        [Required(ErrorMessage = "Model is required")]
        [StringLength(50, ErrorMessage = "Model cannot exceed 50 characters")]
        public string Model { get; set; } = string.Empty;

        // По заданию: этот номер можно будет изменять через Save
        [Required(ErrorMessage = "Registration number is required")]
        [StringLength(20, ErrorMessage = "Registration number cannot exceed 20 characters")]
        public string RegistrationNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "free"; // free, rented, service

        // По заданию: цену (DailyRate) можно будет изменять через Save
        [Required(ErrorMessage = "Daily rate is required")]
        [Range(0.01, 5000.00, ErrorMessage = "Daily rate must be between 0.01 and 5000.00")]
        public decimal DailyRate { get; set; }
    }
}