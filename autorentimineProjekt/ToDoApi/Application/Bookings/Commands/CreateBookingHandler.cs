using autorentimineProjekt.ToDoApi.Application.Common;
using autorentimineProjekt.ToDoApi.Data.Repositories;
using autorentimineProjekt.ToDoApi.Models;
using autorentimineProjekt.ToDoApi.Data;

namespace autorentimineProjekt.ToDoApi.Application.Bookings.Commands
{
    public class CreateBookingCommand
    {
        public int CarId { get; set; }
    }

    public class CreateBookingHandler
    {
        private readonly ICarRepository _carRepository;
        private readonly CarRentalContext _context;

        public CreateBookingHandler(ICarRepository carRepository, CarRentalContext context)
        {
            _carRepository = carRepository;
            _context = context;
        }

        public async Task<Result<int>> Handle(CreateBookingCommand command)
        {
            var car = await _carRepository.GetById(command.CarId);
            if (car == null) return Result<int>.Failure("Машина не найдена");
            if (car.Status != "free") return Result<int>.Failure("Машина уже занята");

            car.Status = "rented";

            // 1. Создаем объект (объявляем переменную)
            var booking = new Booking
            {
                CarId = car.Id,
                StartTime = DateTime.Now,
                PaymentStatus = "Pending"
            };

            // 2. Сохраняем в базу
            await _context.Bookings.AddAsync(booking);
            await _context.SaveChangesAsync();

            // 3. Теперь можно безопасно возвращать ID
            return Result<int>.Success(booking.Id);
        }
    }
}