using TodoApi.Models;
using Microsoft.EntityFrameworkCore;
namespace TodoApi.Data.Repositories
{
    public class BookingRepository : BaseRepository<Booking>, IBookingRepository
    {
        public BookingRepository(TodoContext context) : base(context) { }

        public async Task<Booking?> GetBookingWithCar(int id)
        {
            return await _context.Bookings.Include(b => b.Car).FirstOrDefaultAsync(b => b.Id == id);
        }
    }
}