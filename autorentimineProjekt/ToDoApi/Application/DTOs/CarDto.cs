namespace autorentimineProjekt.ToDoApi.Application.DTOs
{
    public class CarDto
    {
        public int Id { get; set; }
        public string Mark { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal DailyRate { get; set; }
    }
}