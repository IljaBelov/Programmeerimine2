using autorentimineProjekt.ToDoApi.Application.Common;
using autorentimineProjekt.ToDoApi.Data.Repositories; // Твой готовый репозиторий
using autorentimineProjekt.ToDoApi.Models;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace autorentimineProjekt.ToDoApi.Application.Cars.Commands
{
    public class SaveCarCommand : IRequest<OperationResult>
    {
        public int Id { get; set; }
        public string Mark { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string RegistrationNumber { get; set; } = string.Empty;
        public string Status { get; set; } = "free";
        public decimal DailyRate { get; set; }
    }

    public class SaveCarCommandHandler : IRequestHandler<SaveCarCommand, OperationResult>
    {
        // Внедряем твой интерфейс репозитория
        private readonly ICarRepository _carRepository;

        public SaveCarCommandHandler(ICarRepository carRepository)
        {
            _carRepository = carRepository ?? throw new ArgumentNullException(nameof(carRepository));
        }

        public async Task<OperationResult> Handle(SaveCarCommand request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var result = new OperationResult();

            if (string.IsNullOrWhiteSpace(request.Mark) || string.IsNullOrWhiteSpace(request.Model))
            {
                result.AddError("Mark and Model are required.");
                return result;
            }

            if (request.Id < 0)
            {
                result.AddError("Request ID cannot be negative");
                return result;
            }

            var car = new Car();
            if (request.Id == 0)
            {
                // Вызываем твой метод Add
                await _carRepository.Add(car);
            }
            else
            {
                // Вызываем твой метод GetById
                car = await _carRepository.GetById(request.Id);
                if (car == null)
                {
                    result.AddError("Cannot find car with ID " + request.Id);
                    return result;
                }
            }

            car.Mark = request.Mark;
            car.Model = request.Model;
            car.RegistrationNumber = request.RegistrationNumber;
            car.Status = request.Status;
            car.DailyRate = request.DailyRate;

            // Вызываем твой метод Save для коммита изменений в БД
            await _carRepository.Save();

            request.Id = car.Id;

            return result;
        }
    }
}