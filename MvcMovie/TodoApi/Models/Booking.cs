using System.ComponentModel.DataAnnotations;
using TodoApi.Data;

namespace TodoApi.Models
{
    public class Booking : Entity
    {
        

        [Required]
        public int CarId { get; set; }
        public Car Car { get; set; } // Связь с машиной

        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; } // null, пока машина не вернулась

        public double KilometersTraveled { get; set; }

        // Статус оплаты из нижнего блока схемы
        public string PaymentStatus { get; set; } = "Pending";

        // Геттер для автоматического расчета стоимости по твоей формуле
        public decimal TotalPrice
        {
            get
            {
                if (EndTime == null || Car == null) return 0;

                // 1. Считаем длительность в минутах
                var durationInMinutes = (EndTime.Value - StartTime).TotalMinutes;

                // 2. Считаем стоимость времени (минуты * (часовая ставка / 60))
                var timeCost = (decimal)durationInMinutes * (Car.HourlyRate / 60);

                // 3. Считаем стоимость пробега (км * ставка за км)
                var distanceCost = (decimal)KilometersTraveled * Car.KilometerRate;

                return timeCost + distanceCost;
            }
        }
    }
}