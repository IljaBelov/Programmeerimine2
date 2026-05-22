using Xunit;
using autorentimineProjekt.ToDoApi.Application.Bookings.Commands;
using autorentimineProjekt.ToDoApi.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace autorentimineProjekt.UnitTests.Features
{
    public class SaveBookingCommandHandlerTests : TestBase
    {
        [Fact]
        public async Task Handle_ShouldCreateBooking_WhenCarExistsAndIdIsZero()
        {
            // Arrange
            var car = new Car { Id = 1, Mark = "Toyota", Model = "Corolla", RegistrationNumber = "123 TOY", DailyRate = 50m };
            DbContext.Cars.Add(car);
            await DbContext.SaveChangesAsync();

            var handler = new SaveBookingCommandHandler(DbContext);
            var command = new SaveBookingCommand
            {
                Id = 0,
                CarId = 1,
                StartTime = DateTime.UtcNow,
                PaymentStatus = "Pending"
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.HasErrors);
            var bookingInDb = await DbContext.Bookings.FirstOrDefaultAsync(b => b.CarId == 1);
            Assert.NotNull(bookingInDb);
        }

        [Fact]
        public async Task Handle_ShouldUpdateCarInBooking_WhenIdGreaterThanZero()
        {
            // Arrange — Создаем две машины
            var car1 = new Car { Id = 1, Mark = "Toyota", Model = "Corolla", RegistrationNumber = "123 TOY", DailyRate = 50m };
            var car2 = new Car { Id = 2, Mark = "Nissan", Model = "Leaf", RegistrationNumber = "456 NIS", DailyRate = 60m };
            DbContext.Cars.AddRange(car1, car2);

            // Создаем бронирование, изначально привязанное к машине 1
            var booking = new Booking { Id = 5, CarId = 1, StartTime = DateTime.UtcNow, PaymentStatus = "Pending" };
            DbContext.Bookings.Add(booking);
            await DbContext.SaveChangesAsync();

            DbContext.Entry(booking).State = EntityState.Detached;

            var handler = new SaveBookingCommandHandler(DbContext);
            var command = new SaveBookingCommand
            {
                Id = 5,
                CarId = 2, // МЕНЯЕМ машину в бронировании на ID 2!
                StartTime = booking.StartTime,
                PaymentStatus = "Paid"
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.HasErrors);
            var updatedBooking = await DbContext.Bookings.FindAsync(5);
            Assert.NotNull(updatedBooking);
            Assert.Equal(2, updatedBooking.CarId); // Машина успешно перезаписалась!
            Assert.Equal("Paid", updatedBooking.PaymentStatus);
        }

        [Fact]
        public async Task Handle_ShouldReturnError_WhenCarDoesNotExist()
        {
            // Arrange — Пытаемся сохранить бронь на несуществующую машину с ID 9999
            var handler = new SaveBookingCommandHandler(DbContext);
            var command = new SaveBookingCommand { Id = 0, CarId = 9999, StartTime = DateTime.UtcNow };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.HasErrors);
        }
    }
}