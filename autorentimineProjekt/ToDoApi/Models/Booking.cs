namespace autorentimineProjekt.ToDoApi.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public int CarId { get; set; }
        public Car? Car { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public double KilometersTraveled { get; set; }
        public string PaymentStatus { get; set; } = "Pending";

        // Расчет цены: (время в часах * тариф) + (км * 0.20)
        public decimal TotalPrice => CalculatePrice();

        private decimal CalculatePrice()
        {
            if (!EndTime.HasValue || Car == null) return 0;

            // Считаем минуты, как на схеме
            var durationInMinutes = (decimal)(EndTime.Value - StartTime).TotalMinutes;
            var hourlyRate = Car.DailyRate; // Допустим, это ставка в час
            var minuteRate = hourlyRate / 60m; // Переводим часовую ставку в минутную

            var kmRate = 0.20m; // Ставка за км

            return (durationInMinutes * minuteRate) + ((decimal)KilometersTraveled * kmRate);
        }
    }
}