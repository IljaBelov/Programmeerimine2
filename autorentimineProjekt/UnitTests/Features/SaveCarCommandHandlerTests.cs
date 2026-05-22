using Xunit;
using autorentimineProjekt.ToDoApi.Application.Cars.Commands;
using autorentimineProjekt.ToDoApi.Models;
using autorentimineProjekt.ToDoApi.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace autorentimineProjekt.UnitTests.Features
{
    public class SaveCarCommandHandlerTests : TestBase
    {
        [Fact]
        public async Task Handle_ShouldCreateNewCar_WhenIdIsZero()
        {
            // Arrange
            var repository = new CarRepository(DbContext);
            var handler = new SaveCarCommandHandler(repository);
            var command = new SaveCarCommand
            {
                Id = 0,
                Mark = "Tesla",
                Model = "Model S",
                RegistrationNumber = "999 TES",
                Status = "free",
                DailyRate = 150m
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.HasErrors);

            var carInDb = await DbContext.Cars.FirstOrDefaultAsync(c => c.RegistrationNumber == "999 TES");
            Assert.NotNull(carInDb);
            Assert.Equal("Tesla", carInDb.Mark);
            Assert.Equal(150m, carInDb.DailyRate);
        }

        [Fact]
        public async Task Handle_ShouldUpdateExistingCar_WhenIdGreaterThanZero()
        {
            // Arrange
            var car = new Car { Id = 10, Mark = "Ford", Model = "Focus", RegistrationNumber = "111 FFF", Status = "free", DailyRate = 40m };
            DbContext.Cars.Add(car);
            await DbContext.SaveChangesAsync();

            // Отвязываем сущность от трекера, чтобы сымитировать чистый запрос к БД без кэша памяти
            DbContext.Entry(car).State = EntityState.Detached;

            var repository = new CarRepository(DbContext);
            var handler = new SaveCarCommandHandler(repository);
            var command = new SaveCarCommand
            {
                Id = 10,
                Mark = "Ford",
                Model = "Focus",
                RegistrationNumber = "222 NEW", // Проверяем смену регистрационного номера
                Status = "free",
                DailyRate = 55m                  // Проверяем изменение цены (DailyRate)
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.HasErrors);

            var updatedCar = await DbContext.Cars.FindAsync(10);
            Assert.NotNull(updatedCar);
            Assert.Equal("222 NEW", updatedCar.RegistrationNumber);
            Assert.Equal(55m, updatedCar.DailyRate);
        }

        [Fact]
        public async Task Handle_ShouldReturnError_WhenCarNotFound()
        {
            // Arrange
            var repository = new CarRepository(DbContext);
            var handler = new SaveCarCommandHandler(repository);
            var command = new SaveCarCommand { Id = 999, Mark = "Ghost", Model = "Car" };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.HasErrors);
        }
    }
}