using autorentimineProjekt.ToDoApi.Application.Cars.Commands;
using autorentimineProjekt.ToDoApi.Models;
using Xunit;
using System.Threading;
using System.Threading.Tasks;

namespace autorentimineProjekt.UnitTests.Features
{
    public class CreateCarHandlerTests : TestBase
    {
        // 1. Тест на полный успех всей команды (Пункт 3 из задания за 06.02)
        [Fact]
        public async Task Save_should_successfully_create_and_store_car_when_command_is_valid()
        {
            // Arrange
            var command = new CreateCarCommand
            {
                Mark = "Audi",
                Model = "A6",
                Status = "free",
                DailyRate = 85m,
                RegistrationNumber = "123 XYZ"
            };

            var handler = new CreateCarHandler(DbContext);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);

            int generatedId = result.Value;
            Assert.True(generatedId > 0);

            var carInDb = await DbContext.Cars.FindAsync(generatedId);
            Assert.NotNull(carInDb);
            Assert.Equal("Audi", carInDb.Mark);
        }

        // 2. Тестирование правила валидации поля "Mark" (Пункт 2 из задания)
        // Проверяем несколько невалидных вариантов (Theory)
        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public async Task Save_should_return_failure_result_when_Mark_is_invalid(string invalidMark)
        {
            // Arrange
            var command = new CreateCarCommand
            {
                Mark = invalidMark, // Подставляем плохую марку
                Model = "A6",
                Status = "free",
                DailyRate = 85m,
                RegistrationNumber = "123 XYZ"
            };
            var handler = new CreateCarHandler(DbContext);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert — Валидация внутри хендлера должна отклонить команду
            Assert.False(result.IsSuccess);
            Assert.Equal("Mark and Model are required.", result.Error);
        }

        // 3. Тестирование правила валидации поля "Model" (Пункт 2 из задания)
        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public async Task Save_should_return_failure_result_when_Model_is_invalid(string invalidModel)
        {
            // Arrange
            var command = new CreateCarCommand
            {
                Mark = "Audi",
                Model = invalidModel, // Подставляем плохую модель
                Status = "free",
                DailyRate = 85m,
                RegistrationNumber = "123 XYZ"
            };
            var handler = new CreateCarHandler(DbContext);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert — Валидация внутри хендлера должна отклонить команду
            Assert.False(result.IsSuccess);
            Assert.Equal("Mark and Model are required.", result.Error);
        }
    }
}