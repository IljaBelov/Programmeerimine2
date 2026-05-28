using autorentimineProjekt.ToDoApi.Application.Common;
using autorentimineProjekt.ToDoApi.Data.Repositories;
using autorentimineProjekt.ToDoApi.Models;
using autorentimineProjekt.ToDoApi.Data;
using MediatR; // <-- ОБЯЗАТЕЛЬНО ДОБАВИЛИ
using System;
using System.Threading;
using System.Threading.Tasks;

namespace autorentimineProjekt.ToDoApi.Application.Bookings.Commands
{
    // Команда теперь сообщает MediatR, что она возвращает Result<int>
    public class CreateBookingCommand : IRequest<Result<int>>
    {
        public int CarId { get; set; }
    }

    // Обработчик теперь официально реализует интерфейс IRequestHandler
    public class CreateBookingHandler : IRequestHandler<CreateBookingCommand, Result<int>>
    {
        private readonly ICarRepository _carRepository;
        private readonly CarRentalContext _context;

        public CreateBookingHandler(ICarRepository carRepository, CarRentalContext context)
        {
            _carRepository = carRepository;
            _context = context;
        }

        // Добавили CancellationToken в параметры метода (требование MediatR)
        public async Task<Result<int>> Handle(CreateBookingCommand command, CancellationToken cancellationToken)
        {
            if (command == null)
                return Result<int>.Failure("Бэкенд получил пустую команду (ошибка десериализации)");

            var car = await _carRepository.GetById(command.CarId);
            if (car == null) return Result<int>.Failure("Машина не найдена в базе данных");
            if (car.Status != "free") return Result<int>.Failure("Машина уже занята");

            car.Status = "rented";

            var booking = new Booking
            {
                CarId = car.Id,
                StartTime = DateTime.Now,
                PaymentStatus = "Pending"
            };

            try
            {
                await _context.Bookings.AddAsync(booking);
                await _context.SaveChangesAsync();

                return Result<int>.Success(booking.Id); // Возвращает ID созданной брони
            }
            catch (Exception ex)
            {
                return Result<int>.Failure($"Ошибка сохранения в БД: {ex.InnerException?.Message ?? ex.Message}");
            }
        }
    }
}