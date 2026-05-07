using TodoApi.Models;
namespace TodoApi.Data.Repositories
{
    public interface IBookingRepository
    {
        Task<List<Booking>> GetAll();
        Task<Booking?> GetById(int id);
        Task<Booking?> GetBookingWithCar(int id); // Спец. метод для связи
        Task Add(Booking booking);
        Task Save();
    }
}