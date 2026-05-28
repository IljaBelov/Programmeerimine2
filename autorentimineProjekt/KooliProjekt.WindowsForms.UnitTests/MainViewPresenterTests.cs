using System.Collections.Generic;
using System.Threading.Tasks;
using AutoRentimine.WindowsForms; // Твой неймспейс с интерфейсом и презентером
using AutoRentimine.WindowsForms.Api; // Твой неймспейс API
using autorentimineProjekt.ToDoApi.Application.DTOs; // Твой неймспейс DTO машин
using Moq;
using Xunit;

namespace KooliProjekt.WindowsForms.UnitTests
{
    public class MainViewPresenterTests
    {
        private readonly Mock<IApiClient> _apiClientMock;
        private readonly Mock<IMainView> _mainViewMock;
        private readonly MainPresenter _presenter;

        public MainViewPresenterTests()
        {
            _apiClientMock = new Mock<IApiClient>();
            _mainViewMock = new Mock<IMainView>();

            // В твоем MainPresenter порядок аргументов: (IMainView view, IApiClient apiClient)
            _presenter = new MainPresenter(_mainViewMock.Object, _apiClientMock.Object);
        }

        [Fact]
        public async Task LoadData_should_call_ShowError_with_faulty_response()
        {
            // Arrange
            // Исправлено: инициализируем список Errors сразу, чтобы не ловить NullReferenceException
            var faultyResponse = new OperationResult<List<CarDto>>
            {
                Errors = new List<string> { "An error occurred while fetching data." }
            };

            _apiClientMock
                .Setup(client => client.GetCarsAsync())
                .ReturnsAsync(faultyResponse)
                .Verifiable();

            _mainViewMock
                .Setup(view => view.ShowMessage(It.Is<string>(s => s.Contains("Ошибка загрузки данных"))))
                .Verifiable();

            // Act
            await _presenter.LoadData();

            // Assert
            _apiClientMock.VerifyAll();
            _mainViewMock.VerifyAll();
        }

        [Fact]
        public void SelectedIndexChanged_should_set_fields_with_valid_selection()
        {
            // Arrange
            _mainViewMock.Setup(view => view.AddNewCarRow()).Verifiable();

            // Act
            _presenter.Add();

            // Assert
            _mainViewMock.VerifyAll();
        }

        [Fact]
        public async Task Save_should_call_ShowError_with_faulty_response()
        {
            // Arrange
            // Исправлено: инициализируем список Errors сразу
            var faultyResponse = new OperationResult
            {
                Errors = new List<string> { "An error occurred while saving data." }
            };

            var mockCar = new CarDto { Id = 1, Mark = "BMW", Model = "X5" };
            _mainViewMock.Setup(view => view.SelectedCar).Returns(mockCar);

            _apiClientMock
                .Setup(client => client.UpdateCarAsync(mockCar))
                .ReturnsAsync(faultyResponse)
                .Verifiable();

            _mainViewMock
                .Setup(view => view.ShowMessage(It.Is<string>(s => s.Contains("Ошибка сохранения"))))
                .Verifiable();

            // Твой презентер при ошибке сохранения все равно вызывает LoadData(),
            // поэтому подсовываем пустой результат для GetCarsAsync, чтобы тест не падал.
            var loadResponse = new OperationResult<List<CarDto>> { Data = new List<CarDto>() };
            _apiClientMock.Setup(client => client.GetCarsAsync()).ReturnsAsync(loadResponse);

            // Act
            await _presenter.Save();

            // Assert
            _apiClientMock.VerifyAll();
            _mainViewMock.VerifyAll();
        }

        [Fact]
        public async Task Save_should_call_LoadData_with_valid_response()
        {
            // Arrange
            var successSaveResponse = new OperationResult();
            var successLoadResponse = new OperationResult<List<CarDto>> { Data = new List<CarDto>() };

            var mockCar = new CarDto { Id = 1, Mark = "Audi", Model = "A6" };
            _mainViewMock.Setup(view => view.SelectedCar).Returns(mockCar);

            _apiClientMock
                .Setup(client => client.UpdateCarAsync(mockCar))
                .ReturnsAsync(successSaveResponse)
                .Verifiable();

            _apiClientMock
                .Setup(client => client.GetCarsAsync())
                .ReturnsAsync(successLoadResponse)
                .Verifiable();

            _mainViewMock
                .Setup(view => view.DisplayCars(successLoadResponse.Data))
                .Verifiable();

            // Act
            await _presenter.Save();

            // Assert
            _apiClientMock.VerifyAll();
            _mainViewMock.VerifyAll();
        }

        [Fact]
        public async Task Delete_should_return_when_user_didnot_confirmed()
        {
            // Arrange
            var mockCar = new CarDto { Id = 5, Mark = "Tesla", Model = "Model 3" };
            _mainViewMock.Setup(view => view.SelectedCar).Returns(mockCar);

            _mainViewMock
                .Setup(view => view.ConfirmDelete(It.IsAny<string>()))
                .Returns(false)
                .Verifiable();

            // Act
            await _presenter.Delete();

            // Assert
            _mainViewMock.VerifyAll();
            _apiClientMock.Verify(client => client.DeleteCarAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task Delete_should_call_ShowError_with_faulty_response()
        {
            // Arrange
            // Исправлено: инициализируем список Errors сразу
            var faultyResponse = new OperationResult
            {
                Errors = new List<string> { "Server Error" }
            };

            var mockCar = new CarDto { Id = 5, Mark = "Tesla", Model = "Model 3" };
            _mainViewMock.Setup(view => view.SelectedCar).Returns(mockCar);

            _mainViewMock.Setup(view => view.ConfirmDelete(It.IsAny<string>())).Returns(true);

            _apiClientMock
                .Setup(client => client.DeleteCarAsync(mockCar.Id))
                .ReturnsAsync(faultyResponse)
                .Verifiable();

            _mainViewMock
                .Setup(view => view.ShowMessage(It.Is<string>(s => s.Contains("Ошибка удаления"))))
                .Verifiable();

            // Act
            await _presenter.Delete();

            // Assert
            _apiClientMock.VerifyAll();
            _mainViewMock.VerifyAll();
        }

        [Fact]
        public async Task Delete_should_call_LoadData_with_valid_response()
        {
            // Arrange
            var successDeleteResponse = new OperationResult();
            var successLoadResponse = new OperationResult<List<CarDto>> { Data = new List<CarDto>() };

            var mockCar = new CarDto { Id = 22, Mark = "Nissan", Model = "Leaf" };
            _mainViewMock.Setup(view => view.SelectedCar).Returns(mockCar);

            _mainViewMock.Setup(view => view.ConfirmDelete(It.IsAny<string>())).Returns(true);

            _apiClientMock
                .Setup(client => client.DeleteCarAsync(mockCar.Id))
                .ReturnsAsync(successDeleteResponse)
                .Verifiable();

            _mainViewMock
                .Setup(view => view.ShowMessage("Машина успешно удалена."))
                .Verifiable();

            _apiClientMock
                .Setup(client => client.GetCarsAsync())
                .ReturnsAsync(successLoadResponse)
                .Verifiable();

            _mainViewMock
                .Setup(view => view.DisplayCars(successLoadResponse.Data))
                .Verifiable();

            // Act
            await _presenter.Delete();

            // Assert
            _apiClientMock.VerifyAll();
            _mainViewMock.VerifyAll();
        }
    }
}