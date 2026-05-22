using autorentimineProjekt.ToDoApi.Application.Behaviors;
using autorentimineProjekt.ToDoApi.Application.Common;
using autorentimineProjekt.ToDoApi.Data;
using autorentimineProjekt.ToDoApi.Models;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace autorentimineProjekt.ToDoApi.Application.Cars.Commands
{
    // Команда создания машины, возвращает ID созданной тачки (Result<int>)
    public class CreateCarCommand : IRequest<Result<int>>, ITransactional
    {
        public string Mark { get; set; }
        public string Model { get; set; }
        public string RegistrationNumber { get; set; }
        public string Status { get; set; }
        public decimal DailyRate { get; set; }
    }

    public class CreateCarHandler : IRequestHandler<CreateCarCommand, Result<int>>
    {
        private readonly CarRentalContext _context;

        public CreateCarHandler(CarRentalContext context)
        {
            _context = context;
        }

        public async Task<Result<int>> Handle(CreateCarCommand request, CancellationToken cancellationToken)
        {
            // Валидация данных
            if (string.IsNullOrWhiteSpace(request.Mark) || string.IsNullOrWhiteSpace(request.Model))
            {
                return Result<int>.Failure("Mark and Model are required.");
            }

            var car = new Car
            {
                Mark = request.Mark,
                Model = request.Model,
                RegistrationNumber = request.RegistrationNumber,
                Status = request.Status,
                DailyRate = request.DailyRate
            };

            _context.Cars.Add(car);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<int>.Success(car.Id);
        }
    }
}