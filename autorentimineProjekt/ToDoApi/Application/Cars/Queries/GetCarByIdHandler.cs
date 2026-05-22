using autorentimineProjekt.ToDoApi.Application.Common;
using autorentimineProjekt.ToDoApi.Data;
using autorentimineProjekt.ToDoApi.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace autorentimineProjekt.ToDoApi.Application.Cars.Queries
{
    // Добавляем класс запроса по ID для MediatR
    public class GetCarByIdQuery : IRequest<Result<Car>>
    {
        public int Id { get; set; }
    }

    public class GetCarByIdHandler : IRequestHandler<GetCarByIdQuery, Result<Car>>
    {
        private readonly CarRentalContext _context;

        public GetCarByIdHandler(CarRentalContext context)
        {
            _context = context;
        }

        public async Task<Result<Car>> Handle(GetCarByIdQuery request, CancellationToken cancellationToken)
        {
            var car = await _context.Cars.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

            if (car == null)
            {
                return Result<Car>.Failure("Car not found");
            }

            return Result<Car>.Success(car);
        }
    }
}