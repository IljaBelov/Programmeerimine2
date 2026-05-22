using autorentimineProjekt.ToDoApi.Application.Behaviors;
using autorentimineProjekt.ToDoApi.Application.Common;
using autorentimineProjekt.ToDoApi.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace autorentimineProjekt.ToDoApi.Application.Cars.Commands
{
    public class DeleteCarCommand : IRequest<Result<bool>>, ITransactional
    {
        public int Id { get; set; }
    }

    public class DeleteCarHandler : IRequestHandler<DeleteCarCommand, Result<bool>>
    {
        private readonly CarRentalContext _context;

        public DeleteCarHandler(CarRentalContext context)
        {
            _context = context;
        }

        public async Task<Result<bool>> Handle(DeleteCarCommand request, CancellationToken cancellationToken)
        {
            if (request.Id <= 0)
            {
                return Result<bool>.Failure("Invalid ID");
            }

            var car = await _context.Cars.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
            if (car == null)
            {
                return Result<bool>.Failure("Car not found");
            }

            _context.Cars.Remove(car);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}