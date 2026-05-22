using Xunit;
using autorentimineProjekt.ToDoApi.Application.Cars.Queries;
using autorentimineProjekt.ToDoApi.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace autorentimineProjekt.UnitTests.Features
{
    public class GetCarsHandlerTests : TestBase
    {
        [Fact]
        public async Task Handle_ShouldReturnEmptyPagedResult_WhenNoCarsInDatabase()
        {
            // Arrange
            var handler = new GetCarsHandler(DbContext);
            var query = new GetCarsQuery();

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            // Проверяем, что список результатов внутри PagedResult пустой
            Assert.Empty(result.Value.Results);
            Assert.Equal(0, result.Value.RowCount);
        }

        [Fact]
        public async Task Handle_ShouldReturnCars_WhenQueryIsNotNull()
        {
            // Arrange
            var car = new Car { Id = 1, Mark = "BMW", Model = "M5", Status = "free", DailyRate = 100m, RegistrationNumber = "777 AAA" };
            DbContext.Cars.Add(car);
            await DbContext.SaveChangesAsync();

            var handler = new GetCarsHandler(DbContext);

            // Act
            var result = await handler.Handle(new GetCarsQuery(), CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            // Проверяем, что в Results лежит наша добавленная машина
            Assert.Single(result.Value.Results);
            Assert.Equal(1, result.Value.RowCount);
        }

        [Fact]
        public async Task Handle_ShouldReturnFilteredCars_WhenSearchParameterIsProvided()
        {
            // Arrange — Создаем две разные машины в тестовой базе данных
            var car1 = new Car { Id = 1, Mark = "BMW", Model = "M5", Status = "free", DailyRate = 100m, RegistrationNumber = "777 AAA" };
            var car2 = new Car { Id = 2, Mark = "Audi", Model = "A6", Status = "free", DailyRate = 90m, RegistrationNumber = "123 XYZ" };

            DbContext.Cars.Add(car1);
            DbContext.Cars.Add(car2);
            await DbContext.SaveChangesAsync();

            var handler = new GetCarsHandler(DbContext);

            // Ищем только "Audi"
            var query = new GetCarsQuery { Search = "Audi" };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);

            // Должна вернуться только одна машина (Audi) внутри свойства Results
            Assert.Single(result.Value.Results);
            Assert.Equal("Audi", result.Value.Results[0].Mark);
            Assert.Equal(1, result.Value.RowCount);
        }
    }
}