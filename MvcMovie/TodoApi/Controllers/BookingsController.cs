using Microsoft.AspNetCore.Mvc;
using TodoApi.Models;
using TodoApi.Data.Repositories;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly ICarRepository _carRepository;

        // Внедряем сразу два репозитория через конструктор
        public BookingsController(IBookingRepository bookingRepository, ICarRepository carRepository)
        {
            _bookingRepository = bookingRepository;
            _carRepository = carRepository;
        }

        // 1. Начать аренду
        [HttpPost("start")]
        public async Task<ActionResult<Booking>> StartBooking(int carId)
        {
            // Используем репозиторий машин для проверки
            var car = await _carRepository.GetById(carId);
            if (car == null || car.Status != "free")
                return BadRequest("Car is not available.");

            var booking = new Booking
            {
                CarId = carId,
                StartTime = DateTime.Now,
                PaymentStatus = "Pending"
            };

            car.Status = "rented";

            await _bookingRepository.Add(booking);
            await _bookingRepository.Save();
            await _carRepository.Save(); // Сохраняем изменение статуса машины

            return Ok(booking);
        }

        // 2. Завершить аренду с расчетом цены
        [HttpPost("{id}/finish")]
        public async Task<IActionResult> FinishBooking(int id, double kms)
        {
            // Используем наш специальный метод из репозитория для подгрузки данных о машине
            var booking = await _bookingRepository.GetBookingWithCar(id);
            if (booking == null) return NotFound();

            booking.EndTime = DateTime.Now;
            booking.KilometersTraveled = kms;

            // Расчет TotalPrice произойдет автоматически в модели Booking
            booking.PaymentStatus = "Paid";

            if (booking.Car != null)
            {
                booking.Car.Status = "free";
            }

            await _bookingRepository.Save();

            return Ok(new
            {
                Message = "Booking finished",
                TotalCost = booking.TotalPrice,
                Currency = "EUR"
            });
        }

        // 3. Получить все бронирования
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookings()
        {
            return Ok(await _bookingRepository.GetAll());
        }
    }
}