using autorentimineProjekt.ToDoApi.Application.Common;
using autorentimineProjekt.ToDoApi.Data;
using autorentimineProjekt.ToDoApi.Models;
using autorentimineProjekt.ToDoApi.Application.Common.Paging;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace autorentimineProjekt.ToDoApi.Application.Bookings.Commands
{
    public class SaveBookingCommand : IRequest<OperationResult>
    {
        public int Id { get; set; }
        public int CarId { get; set; } // Сюда передаем новый ID машины для изменения
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public double KilometersTraveled { get; set; }
        public string PaymentStatus { get; set; } = "Pending";
    }

    public class SaveBookingCommandHandler : IRequestHandler<SaveBookingCommand, OperationResult>
    {
        private readonly CarRentalContext _dbContext;

        public SaveBookingCommandHandler(CarRentalContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<OperationResult> Handle(SaveBookingCommand request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var result = new OperationResult();
            if (request.Id < 0)
            {
                result.AddError("Request ID cannot be negative");
                return result;
            }

            // Проверяем безопасность: существует ли машина, на которую меняют букинг
            var carExists = await _dbContext.Cars.AnyAsync(c => c.Id == request.CarId, cancellationToken);
            if (!carExists)
            {
                result.AddError($"Car with ID {request.CarId} does not exist. Cannot link booking.");
                return result;
            }

            var booking = new Booking();
            if (request.Id == 0)
            {
                // Создание нового бронирования
                await _dbContext.Bookings.AddAsync(booking, cancellationToken);
            }
            else
            {
                // Редактирование существующего
                booking = await _dbContext.Bookings.FindAsync(new object[] { request.Id }, cancellationToken);
                if (booking == null)
                {
                    result.AddError("Cannot find booking with ID " + request.Id);
                    return result;
                }
            }

            // Обновляем поля, в том числе привязанную машину (CarId)
            booking.CarId = request.CarId;
            booking.StartTime = request.StartTime;
            booking.EndTime = request.EndTime;
            booking.KilometersTraveled = request.KilometersTraveled;
            booking.PaymentStatus = request.PaymentStatus;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return result;
        }
    }
}