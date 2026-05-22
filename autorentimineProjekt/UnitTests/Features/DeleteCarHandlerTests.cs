using autorentimineProjekt.ToDoApi.Application.Cars.Commands;
using autorentimineProjekt.ToDoApi.Models;
using Xunit;
using System.Threading;
using System.Threading.Tasks;

namespace autorentimineProjekt.UnitTests.Features
{
    public class DeleteCarHandlerTests : TestBase
    {
        [Fact]
        public async Task Handle_ShouldSuccessfullyDeleteCar_WhenCarExists()
        {
            // Arrange
            var car = new Car { Id = 10, Mark = "BMW", Model = "X5", Status = "free", DailyRate = 100m, RegistrationNumber = "555 BBB" };
            DbContext.Cars.Add(car);
            await DbContext.SaveChangesAsync();

            var handler = new DeleteCarHandler(DbContext);
            var command = new DeleteCarCommand { Id = 10 };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            var deletedCar = await DbContext.Cars.FindAsync(10);
            Assert.Null(deletedCar);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenCarDoesNotExist()
        {
            // Arrange
            var handler = new DeleteCarHandler(DbContext);
            var command = new DeleteCarCommand { Id = 999 };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert — Проверяем просто то, что операция завершилась ошибкой
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Error);
        }
    }
}