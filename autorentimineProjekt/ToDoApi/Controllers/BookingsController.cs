using Microsoft.AspNetCore.Mvc;
using autorentimineProjekt.ToDoApi.Application.Bookings.Commands;
using MediatR;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class BookingsController : ControllerBase
{
    private readonly IMediator _mediator;

    public BookingsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // Универсальный эндпоинт сохранения (Создание бронирования или смена машины внутри существующего)
    [HttpPost("save")]
    public async Task<IActionResult> SaveBooking([FromBody] SaveBookingCommand command)
    {
        var result = await _mediator.Send(command);

        if (result.HasErrors)
        {
            return BadRequest(result);
        }

        return Ok(new { Message = "Booking saved successfully" });
    }
}