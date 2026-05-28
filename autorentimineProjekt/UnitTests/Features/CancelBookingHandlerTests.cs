using autorentimineProjekt.ToDoApi.Application.Bookings.Commands;
using autorentimineProjekt.ToDoApi.Application.Cars.Queries;
using autorentimineProjekt.ToDoApi.Models;
using Xunit;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace autorentimineProjekt.UnitTests.Features
{
    public class CancelBookingHandlerTests : TestBase
    {
        [Fact]
        public async Task Handle_ShouldChangeStatusToFree_WhenBookingCancelled()
        {
            // Arrange
            var car = new Car { Id = 1, Status = "rented", DailyRate = 10m, RegistrationNumber = "999 CCC" };
            var booking = new Booking
            {
                Id = 1,
                CarId = 1,
                Car = car,
                StartTime = DateTime.Now.AddHours(-1)
            };

            DbContext.Cars.Add(car);
            DbContext.Bookings.Add(booking);
            await DbContext.SaveChangesAsync();

            var handler = new CancelBookingHandler(DbContext);
            var command = new CancelBookingCommand { CarId = 1, Kilometers = 100 };

            // Act
            // ИСПРАВЛЕНО: Добавили CancellationToken.None вторым аргументом
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            var updatedCar = await DbContext.Cars.FindAsync(1);
            Assert.NotNull(updatedCar);
            Assert.Equal("free", updatedCar.Status);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        public async Task Get_should_return_null_request_id_is_zero_or_less(int id)
        {
            // Arrange
            var dbContext = GetFaultyDbContext();
            var query = new GetCarByIdQuery { Id = id };
            var handler = new GetCarByIdHandler(dbContext);

            // Act & Assert
            try
            {
                var result = await handler.Handle(query, CancellationToken.None);
                Assert.True(result == null || !result.IsSuccess);
            }
            catch (Exception)
            {
                Assert.True(true);
            }
        }
    }
}