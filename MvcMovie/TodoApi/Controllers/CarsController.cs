using Microsoft.AspNetCore.Mvc;
using TodoApi.Models;
using TodoApi.Data.Repositories;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarsController : ControllerBase
    {
        private readonly ICarRepository _repository;

        // Внедряем интерфейс репозитория вместо контекста
        public CarsController(ICarRepository repository)
        {
            _repository = repository;
        }

        // 1. Получить список всех машин через репозиторий
        [HttpGet]
        public async Task<ActionResult<PagedResult<Car>>> GetCars([FromQuery] int page = 1, [FromQuery] int pageSize = 5)
        {
            var result = await _repository.GetAllPaged(page, pageSize);
            return Ok(result);
        }

        // 2. Добавить новую машину
        [HttpPost]
        public async Task<ActionResult<Car>> PostCar(Car car)
        {
            await _repository.Add(car);
            await _repository.Save();
            return CreatedAtAction(nameof(GetCars), new { id = car.Id }, car);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCar(int id)
        {
            await _repository.Delete(id);
            await _repository.Save();
            return NoContent();
        }

        // 3. Изменить статус
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> ChangeStatus(int id, [FromBody] string newStatus)
        {
            var car = await _repository.GetById(id);
            if (car == null) return NotFound();

            car.Status = newStatus;
            await _repository.Save();

            return NoContent();
        }
    }
}