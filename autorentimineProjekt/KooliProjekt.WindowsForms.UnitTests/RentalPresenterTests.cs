using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoRentimine.WindowsForms; // Твой неймспейс проекта
using AutoRentimine.WindowsForms.Api; // Твой неймспейс API
using autorentimineProjekt.ToDoApi.Application.DTOs; // Твой неймспейс DTO
using Moq;
using Xunit;

namespace KooliProjekt.WindowsForms.UnitTests
{
    public class RentalPresenterTests
    {
        private readonly Mock<IRentalView> _viewMock;
        private readonly Mock<IApiClient> _apiClientMock;
        private readonly RentalPresenter _presenter;

        public RentalPresenterTests()
        {
            _viewMock = new Mock<IRentalView>();
            _apiClientMock = new Mock<IApiClient>();
            _presenter = new RentalPresenter(_viewMock.Object, _apiClientMock.Object);
        }

        [Fact]
        public async Task LoadAvailableCars_ShouldDisplayOnlyFreeCars_WhenResponseIsValid()
        {
            // Arrange
            var cars = new List<CarDto>
            {
                new CarDto { Id = 1, Mark = "BMW", Status = "Free" },
                new CarDto { Id = 2, Mark = "Audi", Status = "Busy" },
                new CarDto { Id = 3, Mark = "Opel", Status = "free " }
            };

            var response = new OperationResult<List<CarDto>>
            {
                Data = cars,
                Errors = new List<string>()
            };

            _apiClientMock.Setup(c => c.GetCarsAsync()).ReturnsAsync(response);

            // Act
            await _presenter.LoadAvailableCars();

            // Assert
            _viewMock.Verify(v => v.DisplayAvailableCars(It.Is<List<CarDto>>(list => list.Count == 2)), Times.Once);
        }

        [Fact]
        public async Task LoadAvailableCars_ShouldShowMessage_WhenResponseIsFaulty()
        {
            // Arrange
            var response = new OperationResult<List<CarDto>>
            {
                Data = null,
                Errors = new List<string> { "Ошибка подключения к серверу" }
            };

            _apiClientMock.Setup(c => c.GetCarsAsync()).ReturnsAsync(response);

            // Act
            await _presenter.LoadAvailableCars();

            // Assert
            _viewMock.Verify(v => v.ShowMessage(It.Is<string>(s => s.Contains("Ошибка подключения"))), Times.Once);
        }

        [Fact]
        public async Task StartRental_ShouldShowMessage_WhenNoCarIsSelected()
        {
            // Arrange
            _viewMock.Setup(v => v.SelectedCar).Returns((CarDto)null);

            // Act
            await _presenter.StartRental();

            // Assert
            _viewMock.Verify(v => v.ShowMessage("Пожалуйста, выберите автомобиль из списка!"), Times.Once);
            _apiClientMock.Verify(c => c.CreateBookingAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task StartRental_ShouldStartTimerAndShowActiveScreen_WhenBookingSucceeds()
        {
            // Arrange
            var selectedCar = new CarDto { Id = 10, Mark = "Toyota", Model = "Corolla", RegistrationNumber = "777AAA" };
            _viewMock.Setup(v => v.SelectedCar).Returns(selectedCar);

            // Исправлено: используем OperationResult<int> и убрали HasErrors
            var response = new OperationResult<int>
            {
                Errors = new List<string>()
            };
            _apiClientMock.Setup(c => c.CreateBookingAsync(10)).ReturnsAsync(response);

            // Act
            await _presenter.StartRental();

            // Assert
            _viewMock.Verify(v => v.ShowActiveScreen(It.Is<string>(s => s.Contains("Toyota") && s.Contains("777AAA"))), Times.Once);
            _viewMock.Verify(v => v.StartTimer(), Times.Once);
        }

        [Fact]
        public async Task StartRental_ShouldShowErrorMessage_WhenBookingFails()
        {
            // Arrange
            var selectedCar = new CarDto { Id = 10, Mark = "Toyota" };
            _viewMock.Setup(v => v.SelectedCar).Returns(selectedCar);

            // Исправлено: используем OperationResult<int> и убрали HasErrors
            var response = new OperationResult<int>
            {
                Errors = new List<string> { "Машина уже забронирована другим пользователем" }
            };
            _apiClientMock.Setup(c => c.CreateBookingAsync(10)).ReturnsAsync(response);

            // Act
            await _presenter.StartRental();

            // Assert
            _viewMock.Verify(v => v.ShowMessage(It.Is<string>(s => s.Contains("Машина уже забронирована"))), Times.Once);
            _viewMock.Verify(v => v.StartTimer(), Times.Never);
        }

        [Fact]
        public void ProcessTimerTick_ShouldUpdateMetrics()
        {
            // Act
            _presenter.ProcessTimerTick();

            // Assert
            _viewMock.Verify(v => v.UpdateTripMetrics(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task EndRental_ShouldShowResultScreen_WhenCancellationSucceeds()
        {
            // Arrange
            var selectedCar = new CarDto { Id = 55, Mark = "Tesla", Model = "Model 3" };
            _viewMock.Setup(v => v.SelectedCar).Returns(selectedCar);

            _apiClientMock.Setup(c => c.CreateBookingAsync(55)).ReturnsAsync(new OperationResult<int> { Errors = new List<string>() });
            await _presenter.StartRental();

            var response = new OperationResult<decimal>
            {
                Data = 15.75m,
                Errors = new List<string>()
            };
            _apiClientMock.Setup(c => c.CancelBookingAsync(55, It.IsAny<double>())).ReturnsAsync(response);

            // Динамически получаем, как "15.75" выглядит в текущей системе (будет "15,75" или "15.75")
            string expectedPriceString = 15.75m.ToString("F2");

            // Act
            await _presenter.EndRental();

            // Assert
            _viewMock.Verify(v => v.StopTimer(), Times.Once);

            // Теперь проверяем именно системную строку!
            _viewMock.Verify(v => v.ShowResultScreen(It.Is<string>(s => s.Contains(expectedPriceString)), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task EndRental_ShouldShowErrorMessageAndRestartTimer_WhenCancellationFails()
        {
            // Arrange
            var selectedCar = new CarDto { Id = 55, Mark = "Tesla", Model = "Model 3" };
            _viewMock.Setup(v => v.SelectedCar).Returns(selectedCar);

            // Исправлено: OperationResult<int>
            _apiClientMock.Setup(c => c.CreateBookingAsync(55)).ReturnsAsync(new OperationResult<int> { Errors = new List<string>() });
            await _presenter.StartRental();

            // Исправлено: используем OperationResult<decimal> и убрали HasErrors
            var response = new OperationResult<decimal>
            {
                Errors = new List<string> { "Ошибка сервера при фиксации пробега" }
            };
            _apiClientMock.Setup(c => c.CancelBookingAsync(55, It.IsAny<double>())).ReturnsAsync(response);

            // Act
            await _presenter.EndRental();

            // Assert
            _viewMock.Verify(v => v.StopTimer(), Times.Once);
            _viewMock.Verify(v => v.ShowMessage(It.Is<string>(s => s.Contains("Ошибка сервера"))), Times.Once);
            _viewMock.Verify(v => v.StartTimer(), Times.Exactly(2));
        }

        [Fact]
        public async Task BackToMenu_ShouldLoadCarsAndShowSelectionScreen()
        {
            // Arrange
            var response = new OperationResult<List<CarDto>> { Data = new List<CarDto>(), Errors = new List<string>() };
            _apiClientMock.Setup(c => c.GetCarsAsync()).ReturnsAsync(response);

            // Act
            await _presenter.BackToMenu();

            // Assert
            _viewMock.Verify(v => v.ShowSelectionScreen(), Times.Once);
        }
    }
}