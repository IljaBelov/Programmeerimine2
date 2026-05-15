using autorentimineProjekt.ToDoApi.Application.Common;
using autorentimineProjekt.ToDoApi.Application.DTOs;
using autorentimineProjekt.ToDoApi.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace autorentimineProjekt.ToDoApi.Application.Bookings.Commands
{
    public class GetBookingsQuery { }

    public class GetBookingsHandler
    {
        private readonly CarRentalContext _context;

        public GetBookingsHandler(CarRentalContext context) => _context = context;

        public async Task<Result<List<BookingDto>>> Handle(GetBookingsQuery? query)
        {
            if (query == null)
            {
                return Result<List<BookingDto>>.Success(null!);
            }
            

            var activeBookings = await _context.Bookings
                .Include(b => b.Car)
                .Where(b => b.EndTime == null) // Только активные
                .ToListAsync();

            var dtos = activeBookings.Select(b => new BookingDto
            {
                Id = b.Id,
                CarId = b.CarId,
                CarDetails = b.Car != null ? $"{b.Car.Mark} {b.Car.Model}" : "Unknown Car",
                StartTime = b.StartTime,
                EndTime = b.EndTime,
                PaymentStatus = b.PaymentStatus
            }).ToList();

            return Result<List<BookingDto>>.Success(dtos);
        }
    }
}