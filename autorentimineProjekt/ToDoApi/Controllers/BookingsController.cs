using Microsoft.AspNetCore.Mvc;
using autorentimineProjekt.ToDoApi.Application.Bookings.Commands;
using MediatR;
using System.Threading.Tasks;

namespace autorentimineProjekt.ToDoApi.Controllers
{
    [Route("api/[controller]")] // Базовый маршрут: api/bookings
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BookingsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // 1. Эндпоинт для СОЗДАНИЯ аренды (POST: api/bookings)
        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                // Если ошибка — отправляем текст ошибки на фронтенд
                return BadRequest(result.Error);
            }

            // Возвращаем ID созданной брони (int)
            return Ok(result.Value);
        }

        // 2. Эндпоинт для ОТМЕНЫ/ЗАКРЫТИЯ аренды (POST: api/bookings/cancel)
        // ИМЕННО ЭТОГО МЕТОДА НЕ ХВАТАЛО!
        [HttpPost("cancel")]
        public async Task<IActionResult> CancelBooking([FromBody] CancelBookingCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                // Если на бэкенде что-то пошло не так (например, поездка не найдена)
                return BadRequest(result.Error);
            }

            // Возвращаем итоговую стоимость поездки (decimal)
            return Ok(result.Value);
        }

        // 3. Твой старый универсальный эндпоинт сохранения (POST: api/bookings/save)
        [HttpPost("save")]
        public async Task<IActionResult> SaveBooking([FromBody] SaveBookingCommand command)
        {
            var result = await _mediator.Send(command);

            // Здесь проверяем старый OperationResult, если он у тебя там остался
            if (result.HasErrors)
            {
                return BadRequest(result);
            }

            return Ok(new { Message = "Booking saved successfully" });
        }
    }
}