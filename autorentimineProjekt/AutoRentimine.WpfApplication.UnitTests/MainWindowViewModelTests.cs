using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoRentimine.WpfApplication;
using AutoRentimine.WpfApplication.Api;
using autorentimineProjekt.ToDoApi.Application.DTOs;
using Moq;
using Xunit;

namespace KooliProjekt.WpfApplication.UnitTests
{
    public class MainWindowViewModelTests
    {
        private readonly Mock<IApiClient> _apiClientMock;
        private readonly Mock<IDialogProvider> _dialogProviderMock;
        private readonly MainWindowViewModel _viewModel;

        public MainWindowViewModelTests()
        {
            _apiClientMock = new Mock<IApiClient>();
            _dialogProviderMock = new Mock<IDialogProvider>();
            _viewModel = new MainWindowViewModel(_apiClientMock.Object, _dialogProviderMock.Object);
        }

        [Fact]
        public void SelectedItem_should_return_correct_item()
        {
            // Arrange
            var item = new CarModel { Id = 1, Mark = "Tesla", Model = "Model 3" };

            // Act
            _viewModel.SelectedItem = item;

            // Assert
            Assert.Equal(item, _viewModel.SelectedItem);
        }

        [Fact]
        public void SelectedItem_should_call_notify_property_changed()
        {
            // Arrange
            var item = new CarModel { Id = 1, Mark = "Tesla", Model = "Model S" };
            var propertyChangedRaised = false;
            _viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(MainWindowViewModel.SelectedItem))
                {
                    propertyChangedRaised = true;
                }
            };

            // Act
            _viewModel.SelectedItem = item;

            // Assert
            Assert.True(propertyChangedRaised);
        }

        [Fact]
        public async Task LoadData_should_load_data_from_api_client()
        {
            // Arrange
            var apiResult = new OperationResult<List<CarDto>>
            {
                Data = new List<CarDto>
                {
                    new CarDto { Id = 1, Mark = "Audi", Model = "A6" },
                    new CarDto { Id = 2, Mark = "BMW", Model = "530" }
                }
            };

            _apiClientMock.Setup(client => client.GetCarsAsync())
                .ReturnsAsync(apiResult)
                .Verifiable();

            // Act            
            await _viewModel.LoadDataAsync();

            // Assert
            _apiClientMock.VerifyAll();
            Assert.Equal(2, _viewModel.Data.Count);
            Assert.Equal(1, _viewModel.Data[0].Id);
            Assert.Equal(2, _viewModel.Data[1].Id);
        }

        [Fact]
        public async Task LoadData_should_show_error_when_api_client_fails()
        {
            // Arrange
            var apiResult = new OperationResult<List<CarDto>>
            {
                Errors = new List<string> { "Ошибка сервера при загрузке" }
            };

            _apiClientMock.Setup(client => client.GetCarsAsync())
                .ReturnsAsync(apiResult)
                .Verifiable();

            // Act            
            await _viewModel.LoadDataAsync();

            // Assert
            _apiClientMock.VerifyAll();
            Assert.Empty(_viewModel.Data);
            _dialogProviderMock.Verify(d => d.ShowError(It.IsAny<string>()), Times.Once);
        }

        // =========================================================================
        // ТЕСТЫ ДЛЯ ДОБАВЛЕНИЯ АВТОМОБИЛЯ (AddCommand) С НОВОЙ ВАЛИДАЦИЕЙ
        // =========================================================================

        [Fact]
        public void AddCommand_should_show_error_when_any_field_is_empty()
        {
            // Arrange: Оставляем поле марки пустым, остальные заполняем
            _viewModel.Mark = "";
            _viewModel.Model = "A6";
            _viewModel.RegistrationNumber = "123 ABC";
            _viewModel.DailyRate = "45";
            _viewModel.Status = "free";

            // Act
            _viewModel.AddCommand.Execute(null);

            // Assert: Должна показаться ошибка валидации пустых полей
            _dialogProviderMock.Verify(d => d.ShowError(It.Is<string>(s => s.Contains("Заполните абсолютно все поля"))), Times.Once);
            // API при этом вызываться не должно
            _apiClientMock.Verify(client => client.AddCarAsync(It.IsAny<CarDto>()), Times.Never);
        }

        [Fact]
        public void AddCommand_should_show_error_when_price_is_invalid_format()
        {
            // Arrange: Ставим текст вместо числового значения цены
            _viewModel.Mark = "Audi";
            _viewModel.Model = "A6";
            _viewModel.RegistrationNumber = "123 ABC";
            _viewModel.DailyRate = "дорого";
            _viewModel.Status = "free";

            // Act
            _viewModel.AddCommand.Execute(null);

            // Assert: Должна показаться ошибка формата цены
            _dialogProviderMock.Verify(d => d.ShowError(It.Is<string>(s => s.Contains("должно быть числовым значением"))), Times.Once);
            _apiClientMock.Verify(client => client.AddCarAsync(It.IsAny<CarDto>()), Times.Never);
        }

        [Fact]
        public void AddCommand_should_call_api_and_clear_form_when_data_is_valid()
        {
            // Arrange: Заполняем форму абсолютно корректно
            _viewModel.Mark = "Audi";
            _viewModel.Model = "A6";
            _viewModel.RegistrationNumber = "123 ABC";
            _viewModel.DailyRate = "45";
            _viewModel.Status = "free";

            var successResult = new OperationResult(); // Успешный результат без ошибок
            _apiClientMock.Setup(client => client.AddCarAsync(It.IsAny<CarDto>()))
                .ReturnsAsync(successResult);

            // Имитируем успешный вызов последующего обновления списка (LoadDataAsync)
            _apiClientMock.Setup(client => client.GetCarsAsync())
                .ReturnsAsync(new OperationResult<List<CarDto>> { Data = new List<CarDto>() });

            // Act
            _viewModel.AddCommand.Execute(null);

            // Assert: Проверяем, что запрос ушел в API с правильной конвертацией данных
            _apiClientMock.Verify(client => client.AddCarAsync(It.Is<CarDto>(dto =>
                dto.Mark == "Audi" &&
                dto.Model == "A6" &&
                dto.RegistrationNumber == "123 ABC" &&
                dto.DailyRate == 45m &&
                dto.Status == "free")), Times.Once);

            // Список машин должен обновиться с бэка
            _apiClientMock.Verify(client => client.GetCarsAsync(), Times.Once);

            // Текстовые поля бланка должны очиститься после успешного сохранения
            Assert.Equal(string.Empty, _viewModel.Mark);
            Assert.Equal(string.Empty, _viewModel.Model);
            Assert.Null(_viewModel.SelectedItem);
        }

        [Fact]
        public void AddCommand_should_show_error_when_api_returns_errors()
        {
            // Arrange: Данные валидны на клиенте, но бэкенд вернет ошибку (например, дубликат гос. номера)
            _viewModel.Mark = "Audi";
            _viewModel.Model = "A6";
            _viewModel.RegistrationNumber = "123 ABC";
            _viewModel.DailyRate = "45";
            _viewModel.Status = "free";

            var apiErrorResult = new OperationResult();
            apiErrorResult.AddError("Автомобиль с таким регистрационным номером уже существует!");

            _apiClientMock.Setup(client => client.AddCarAsync(It.IsAny<CarDto>()))
                .ReturnsAsync(apiErrorResult);

            // Act
            _viewModel.AddCommand.Execute(null);

            // Assert: Ошибка с бэка должна быть выведена на экран через диалог
            _dialogProviderMock.Verify(d => d.ShowError(It.Is<string>(s => s.Contains("уже существует"))), Times.Once);
            // Повторная загрузка списка машин вызываться не должна
            _apiClientMock.Verify(client => client.GetCarsAsync(), Times.Never);
            // Бланк НЕ должен очищаться, чтобы пользователь мог исправить опечатку
            Assert.Equal("123 ABC", _viewModel.RegistrationNumber);
        }

        // =========================================================================
        // ТЕСТЫ ОСТАЛЬНЫХ КОМАНД (Save, Delete)
        // =========================================================================

        [Fact]
        public void SaveCommand_can_execute_when_selected_item_is_not_null()
        {
            _viewModel.SelectedItem = null;
            Assert.False(_viewModel.SaveCommand.CanExecute(null));

            _viewModel.SelectedItem = new CarModel { Id = 1, Mark = "Fiat" };
            Assert.True(_viewModel.SaveCommand.CanExecute(null));
        }

        [Fact]
        public void DeleteCommand_should_return_when_no_confirmation()
        {
            var car = new CarModel { Id = 5, Mark = "Toyota" };
            _viewModel.SelectedItem = car;

            _dialogProviderMock.Setup(d => d.Confirm(It.IsAny<string>())).Returns(false);

            _viewModel.DeleteCommand.Execute(null);

            _apiClientMock.Verify(client => client.DeleteCarAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void DeleteCommand_should_load_data_if_no_errors()
        {
            var car = new CarModel { Id = 10, Mark = "Skoda" };
            _viewModel.SelectedItem = car;
            _viewModel.Data.Add(car);

            _dialogProviderMock.Setup(d => d.Confirm(It.IsAny<string>())).Returns(true);

            _apiClientMock.Setup(client => client.DeleteCarAsync(10))
                .ReturnsAsync(new OperationResult());

            _apiClientMock.Setup(client => client.GetCarsAsync())
                .ReturnsAsync(new OperationResult<List<CarDto>> { Data = new List<CarDto>() });

            _viewModel.DeleteCommand.Execute(null);

            _apiClientMock.Verify(client => client.DeleteCarAsync(10), Times.Once);
            _apiClientMock.Verify(client => client.GetCarsAsync(), Times.Once);
            Assert.Null(_viewModel.SelectedItem);
        }

        [Fact]
        public void DeleteCommand_should_return_when_api_gave_error()
        {
            var car = new CarModel { Id = 15, Mark = "Ford" };
            _viewModel.SelectedItem = car;

            _dialogProviderMock.Setup(d => d.Confirm(It.IsAny<string>())).Returns(true);

            var failedResult = new OperationResult();
            failedResult.AddError("Ошибка СУБД: Нельзя удалить объект!");

            _apiClientMock.Setup(client => client.DeleteCarAsync(15)).ReturnsAsync(failedResult);

            _viewModel.DeleteCommand.Execute(null);

            _dialogProviderMock.Verify(d => d.ShowError(It.IsAny<string>()), Times.Once);
            _apiClientMock.Verify(client => client.GetCarsAsync(), Times.Never);
        }

        [Fact]
        public void DeleteCommand_can_execute_when_selected_item_is_not_null()
        {
            // Проверка CanExecute для команды удаления
            _viewModel.SelectedItem = null;
            Assert.False(_viewModel.DeleteCommand.CanExecute(null));

            _viewModel.SelectedItem = new CarModel { Id = 1 };
            Assert.True(_viewModel.DeleteCommand.CanExecute(null));
        }
    }
}