namespace autorentimineProjekt.ToDoApi.Models
{
    public class Car
    {
        public int Id { get; set; }
        public string Mark { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string RegistrationNumber { get; set; } = string.Empty;
        public string Status { get; set; } = "free"; // free, rented, service
        public decimal DailyRate { get; set; }
    }
}
