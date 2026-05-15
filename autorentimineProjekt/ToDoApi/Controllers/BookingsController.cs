using Microsoft.AspNetCore.Mvc;
using autorentimineProjekt.ToDoApi.Models;
using autorentimineProjekt.ToDoApi.Data;
using autorentimineProjekt.ToDoApi.Application.Bookings.Commands;

[Route("api/[controller]")]
[ApiController]
public class BookingsController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetBookings([FromServices] GetBookingsHandler handler)
    {
        var result = await handler.Handle(new GetBookingsQuery());
        return Ok(result.Value);
    }
    [HttpPost("start")]
    public async Task<IActionResult> StartRental([FromServices] CreateBookingHandler handler, [FromBody] CreateBookingCommand command)
    {
        var result = await handler.Handle(command);
        return result.IsSuccess ? Ok(result) : BadRequest(result.Error);
    }

    [HttpPost("finish")]
    public async Task<IActionResult> FinishRental(
    [FromServices] CancelBookingHandler handler,
    [FromBody] CancelBookingCommand command)
    {
        var result = await handler.Handle(command);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }

        // Возвращаем Ok, внутри которого будет только итоговая цена
        return Ok(new
        {
            Message = "Поездка завершена",
            TotalPrice = result.Value,
            Currency = "EUR"
        });
    }
}