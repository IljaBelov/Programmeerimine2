namespace autorentimineProjekt.ToDoApi.Application.DTOs
{
    public class BookingDto
    {
        public int Id { get; set; }
        public int CarId { get; set; }
        public string CarDetails { get; set; } = string.Empty; // Например, "BMW M5"
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public decimal TotalPrice { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
    }
}