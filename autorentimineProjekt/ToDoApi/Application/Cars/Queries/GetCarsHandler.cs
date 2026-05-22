using autorentimineProjekt.ToDoApi.Application.Common;
using autorentimineProjekt.ToDoApi.Data;
using autorentimineProjekt.ToDoApi.Models;
using autorentimineProjekt.ToDoApi.Application.Common.Paging;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace autorentimineProjekt.ToDoApi.Application.Cars.Queries
{
    // Меняем возвращаемый тип с List<Car> на PagedResult<Car>
    public class GetCarsQuery : IRequest<Result<PagedResult<Car>>>
    {
        public string? Search { get; set; }
        public int Page { get; set; } = 1;       // Номер страницы (по умолчанию 1)
        public int PageSize { get; set; } = 10;  // Количество элементов на странице
    }

    public class GetCarsHandler : IRequestHandler<GetCarsQuery, Result<PagedResult<Car>>>
    {
        private readonly CarRentalContext _context;

        public GetCarsHandler(CarRentalContext context)
        {
            _context = context;
        }

        public async Task<Result<PagedResult<Car>>> Handle(GetCarsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Cars.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var searchLower = request.Search.ToLower().Trim();
                query = query.Where(c => c.Mark.ToLower().Contains(searchLower) ||
                                         c.Model.ToLower().Contains(searchLower));
            }

            // 1. Считаем общее количество записей в базе данных (RowCount)
            var totalRows = await query.CountAsync(cancellationToken);

            // 2. Вычисляем количество страниц (PageCount)
            var pageCount = (int)Math.Ceiling((double)totalRows / request.PageSize);

            // 3. Пропускаем предыдущие страницы и берем порцию данных для текущей страницы
            var items = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            // 4. Формируем итоговый объект PagedResult
            var pagedResult = new PagedResult<Car>
            {
                CurrentPage = request.Page,
                PageSize = request.PageSize,
                RowCount = totalRows,
                PageCount = pageCount,
                Results = items
            };

            return Result<PagedResult<Car>>.Success(pagedResult);
        }
    }
}