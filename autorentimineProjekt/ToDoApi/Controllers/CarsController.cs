using autorentimineProjekt.ToDoApi.Application.Cars.Commands;
using autorentimineProjekt.ToDoApi.Application.Cars.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace autorentimineProjekt.ToDoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarsController : ControllerBase
    {
        private readonly IMediator _mediator;

        // Внедряем MediatR через конструктор
        public CarsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // 1. Получение всех машин (ОСТАВИЛИ ТОЛЬКО ОДИН МЕТОД)
        [HttpGet]
        public async Task<IActionResult> GetCars()
        {
            var result = await _mediator.Send(new GetCarsQuery());

            if (!result.IsSuccess)
            {
                return BadRequest("Could not retrieve cars");
            }

            return Ok(result.Value);
        }

        // 2. Получение одной машины по ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCarById(int id)
        {
            var result = await _mediator.Send(new GetCarByIdQuery { Id = id });

            if (!result.IsSuccess)
            {
                return BadRequest("Invalid request or car not found");
            }

            return Ok(result.Value);
        }

        // 3. Добавление новой машины (POST)
        [HttpPost]
        public async Task<IActionResult> CreateCar([FromBody] CreateCarCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                return BadRequest("Could not create car");
            }

            return Ok(result.Value);
        }

        // 4. Удаление машины по ID (DELETE)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCar(int id)
        {
            var result = await _mediator.Send(new DeleteCarCommand { Id = id });

            if (!result.IsSuccess)
            {
                return BadRequest("Could not delete car or car not found");
            }

            return Ok(result.Value);
        }
    }
}