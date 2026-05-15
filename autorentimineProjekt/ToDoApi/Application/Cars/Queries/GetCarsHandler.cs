using autorentimineProjekt.ToDoApi.Application.Common;
using autorentimineProjekt.ToDoApi.Application.DTOs;
using autorentimineProjekt.ToDoApi.Data.Repositories;

namespace autorentimineProjekt.ToDoApi.Application.Cars.Queries
{
    public class GetCarsQuery { } // Пустой объект запроса

    public class GetCarsHandler
    {
        private readonly ICarRepository _repository;

        public GetCarsHandler(ICarRepository repository)    
        {
            _repository = repository;
        }

        public async Task<Result<List<CarDto>>> Handle(GetCarsQuery? query)
        {
            // ПУНКТ 9-10 задания: проверка на null
            if (query == null)
            {
                return Result<List<CarDto>>.Success(null!   );
            }

            var cars = await _repository.GetAll();
            var dtos = cars.Select(c => new CarDto
            {
                Id = c.Id,
                Mark = c.Mark,
                Model = c.Model,
                Status = c.Status,
                DailyRate = c.DailyRate
            }).ToList();

            return Result<List<CarDto>>.Success(dtos);
        }
    }
}