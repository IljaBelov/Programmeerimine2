using autorentimineProjekt.ToDoApi.Application.Common;
using autorentimineProjekt.ToDoApi.Data;
using autorentimineProjekt.ToDoApi.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace autorentimineProjekt.ToDoApi.Application.Cars.Queries
{
    // 1. Добавляем параметр поиска в сам запрос (Пункт 2 из задания)
    public class GetCarsQuery : IRequest<Result<List<Car>>>
    {
        public string? Search { get; set; } // Сюда будет приходить текст для поиска
    }

    public class GetCarsHandler : IRequestHandler<GetCarsQuery, Result<List<Car>>>
    {
        private readonly CarRentalContext _context;

        public GetCarsHandler(CarRentalContext context)
        {
            _context = context;
        }

        public async Task<Result<List<Car>>> Handle(GetCarsQuery request, CancellationToken cancellationToken)
        {
            // Начинаем строить запрос к базе данных
            var query = _context.Cars.AsQueryable();

            // 2. Если пользователь передал строку поиска, фильтруем данные (Пункт 3 из задания)
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var searchLower = request.Search.ToLower().Trim();

                // Ищем совпадения по марке или модели (без учета регистра)
                query = query.Where(c => c.Mark.ToLower().Contains(searchLower) ||
                                         c.Model.ToLower().Contains(searchLower));
            }

            var cars = await query.ToListAsync(cancellationToken);

            return Result<List<Car>>.Success(cars);
        }
    }
}