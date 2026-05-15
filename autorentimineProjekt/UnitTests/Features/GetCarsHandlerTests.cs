using Moq;
using Xunit;
using autorentimineProjekt.ToDoApi.Application.Cars.Queries;
using autorentimineProjekt.ToDoApi.Data.Repositories;
using autorentimineProjekt.ToDoApi.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace autorentimineProjekt.UnitTests.Features
{
    public class GetCarsHandlerTests
    {
        private readonly Mock<ICarRepository> _repositoryMock;
        private readonly GetCarsHandler _handler;

        public GetCarsHandlerTests()
        {
            // Создаем "подделку" репозитория
            _repositoryMock = new Mock<ICarRepository>();
            // Передаем её в хендлер
            _handler = new GetCarsHandler(_repositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnNullValue_WhenQueryIsNull()
        {
            // Arrange: Пункт 10 задания - подготавливаем null
            GetCarsQuery? nullQuery = null;

            // Act: Вызываем хендлер
            var result = await _handler.Handle(nullQuery);

            // Assert: Проверяем, что ошибок нет, и Value внутри действительно null
            Assert.True(result.IsSuccess);
            Assert.Null(result.Value);
        }

        [Fact]
        public async Task Handle_ShouldReturnCars_WhenQueryIsNotNull()
        {
            // Arrange: Подготавливаем фейковый список машин
            var cars = new List<Car>
            {
                new Car { Id = 1, Mark = "BMW", Model = "M5", Status = "free" }
            };
            _repositoryMock.Setup(repo => repo.GetAll()).ReturnsAsync(cars);

            // Act
            var result = await _handler.Handle(new GetCarsQuery());

            // Assert
            Assert.NotNull(result.Value);
            Assert.Single(result.Value);
            Assert.Equal("BMW", result.Value[0].Mark);
        }
    }
}