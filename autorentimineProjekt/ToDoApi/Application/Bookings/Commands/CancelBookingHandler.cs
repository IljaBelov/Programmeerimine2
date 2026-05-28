using autorentimineProjekt.ToDoApi.Data;
using autorentimineProjekt.ToDoApi.Application.Common;
using Microsoft.EntityFrameworkCore;
using MediatR; // <-- 1. ОБЯЗАТЕЛЬНО ДОБАВИЛИ ДЛЯ МЕДИАТОРА
using System;
using System.Threading;
using System.Threading.Tasks;

namespace autorentimineProjekt.ToDoApi.Application.Bookings.Commands
{
    // 2. Унаследовали команду от IRequest и указали, что она возвращает Result<decimal>
    public class CancelBookingCommand : IRequest<Result<decimal>>
    {
        public int CarId { get; set; }
        public double Kilometers { get; set; }
    }

    // 3. Реализовали интерфейс IRequestHandler
    public class CancelBookingHandler : IRequestHandler<CancelBookingCommand, Result<decimal>>
    {
        private readonly CarRentalContext _context;

        public CancelBookingHandler(CarRentalContext context) => _context = context;

        // 4. Добавили CancellationToken в параметры метода Handle (требование интерфейса MediatR)
        public async Task<Result<decimal>> Handle(CancelBookingCommand command, CancellationToken cancellationToken)
        {
            if (command == null)
            {
                return Result<decimal>.Success(default);
            }

            var booking = await _context.Bookings
                .Include(b => b.Car)
                .FirstOrDefaultAsync(b => b.CarId == command.CarId && b.EndTime == null);

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

            return Result<decimal>.Success(booking.TotalPrice);
        }
    }
}