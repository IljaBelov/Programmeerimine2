using autorentimineProjekt.ToDoApi.Data;
using autorentimineProjekt.ToDoApi.Application.Common;
using Microsoft.EntityFrameworkCore;

namespace autorentimineProjekt.ToDoApi.Application.Bookings.Commands
{
    public class CancelBookingCommand
    {
        public int CarId { get; set; }
        public double Kilometers { get; set; }
    }

    public class CancelBookingHandler
    {
        private readonly CarRentalContext _context;

        public CancelBookingHandler(CarRentalContext context) => _context = context;

        // ВАЖНО: Убедись, что здесь Task<Result<decimal>>
        public async Task<Result<decimal>> Handle(CancelBookingCommand command)
        {
            if (command == null)
            {
                // Выходим сразу, возвращая пустой результат, чтобы не было ошибки
                return Result<decimal>.Success(default);
            }
            var booking = await _context.Bookings
                .Include(b => b.Car)
                .FirstOrDefaultAsync(b => b.CarId == command.CarId && b.EndTime == null);

            // Ошибка CS0029 была тут: нельзя возвращать Failure("...") для bool, если ждем decimal
            if (booking == null)
                return Result<decimal>.Failure("Активная поездка не найдена");

            booking.EndTime = DateTime.Now;
            booking.KilometersTraveled = command.Kilometers;

            if (booking.Car != null)
            {
                booking.Car.Status = "free";
            }

            booking.PaymentStatus = "Paid";

            await _context.SaveChangesAsync();

            // Ошибка CS1503 была тут: ты пытался вернуть true (bool) вместо цены (decimal)
            // Теперь возвращаем TotalPrice, который посчитала твоя модель Booking
            return Result<decimal>.Success(booking.TotalPrice);
        }
    }
}