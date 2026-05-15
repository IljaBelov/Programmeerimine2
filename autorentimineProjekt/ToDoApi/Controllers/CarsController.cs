using Microsoft.AspNetCore.Mvc;
using autorentimineProjekt.ToDoApi.Application.Cars.Queries;
using autorentimineProjekt.ToDoApi.Models;
using autorentimineProjekt.ToDoApi.Data;

namespace autorentimineProjekt.ToDoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarsController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetCars([FromServices] GetCarsHandler handler)
        {
            var result = await handler.Handle(new GetCarsQuery());
            return Ok(result.Value);
        }
    }
}