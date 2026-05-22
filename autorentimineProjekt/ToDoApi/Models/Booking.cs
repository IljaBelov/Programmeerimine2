using System;
using System.ComponentModel.DataAnnotations;

namespace autorentimineProjekt.ToDoApi.Models
{
    public class Booking
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "CarId is required")]
        public int CarId { get; set; } // По заданию: машину в букинге можно будет менять

        public Car? Car { get; set; }

        [Required(ErrorMessage = "Start time is required")]
        public DateTime StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Kilometers traveled cannot be negative")]
        public double KilometersTraveled { get; set; }

        [Required]
        [StringLength(30)]
        public string PaymentStatus { get; set; } = "Pending";

        public decimal TotalPrice => CalculatePrice();

        private decimal CalculatePrice()
        {
            if (!EndTime.HasValue || Car == null) return 0;

            var durationInMinutes = (decimal)(EndTime.Value - StartTime).TotalMinutes;
            var hourlyRate = Car.DailyRate;
            var minuteRate = hourlyRate / 60m;

            var kmRate = 0.20m;

            return (durationInMinutes * minuteRate) + ((decimal)KilometersTraveled * kmRate);
        }
    }
}