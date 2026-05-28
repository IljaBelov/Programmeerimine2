using System;
using System.Linq;
using System.Threading.Tasks;
using AutoRentimine.WindowsForms.Api;
using autorentimineProjekt.ToDoApi.Application.DTOs;

namespace AutoRentimine.WindowsForms
{
    public class RentalPresenter
    {
        private readonly IRentalView _view;
        private readonly IApiClient _apiClient;

        private CarDto _selectedCar;
        private DateTime _startTime;
        private double _kilometersTraveled = 0;

        public RentalPresenter(IRentalView view, IApiClient apiClient)
        {
            _view = view;
            _apiClient = apiClient;
        }

        // Загрузка свободных машин с фильтрацией
        public async Task LoadAvailableCars()
        {
            var result = await _apiClient.GetCarsAsync();
            if (result.Data != null)
            {
                var freeCars = result.Data
                    .Where(c => c.Status != null && c.Status.Trim().ToLower() == "free")
                    .ToList();

                _view.DisplayAvailableCars(freeCars);
            }
            else
            {
                _view.ShowMessage("Не удалось загрузить авто: " + string.Join(", ", result.Errors));
            }
        }

        // Старт аренды автомобиля
        public async Task StartRental()
        {
            _selectedCar = _view.SelectedCar;
            if (_selectedCar == null)
            {
                _view.ShowMessage("Пожалуйста, выберите автомобиль из списка!");
                return;
            }

            var result = await _apiClient.CreateBookingAsync(_selectedCar.Id);
            if (!result.HasErrors)
            {
                _startTime = DateTime.Now;
                _kilometersTraveled = 0;

                string carInfo = $"Вы арендовали: {_selectedCar.Mark} {_selectedCar.Model} [{_selectedCar.RegistrationNumber}]";

                _view.ShowActiveScreen(carInfo);
                _view.StartTimer();
            }
            else
            {
                string errorMsg = (result.Errors != null && result.Errors.Any())
                    ? string.Join(", ", result.Errors)
                    : "Сервер отклонил запрос без объяснения причин (проверь логи бэкенда).";

                _view.ShowMessage("Не удалось арендовать: " + errorMsg);
            }
        }

        // Каждую секунду обновляем метрики пути
        public void ProcessTimerTick()
        {
            TimeSpan elapsed = DateTime.Now - _startTime;
            _kilometersTraveled += (50.0 / 3600.0);

            string timeText = $"Время в пути: {elapsed:hh\\:mm\\:ss}";
            string kmText = $"Пройдено: {_kilometersTraveled:F2} км";

            _view.UpdateTripMetrics(timeText, kmText);
        }

        // Завершение поездки и расчет счета
        public async Task EndRental()
        {
            _view.StopTimer();

            var result = await _apiClient.CancelBookingAsync(_selectedCar.Id, _kilometersTraveled);
            if (!result.HasErrors)
            {
                string finalPrice = $"{result.Data:F2} EUR";
                string receiptDetails = $"Автомобиль: {_selectedCar.Mark} {_selectedCar.Model}\n" +
                                         $"Пройденное расстояние: {_kilometersTraveled:F2} км\n" +
                                         $"Время в прокате: {(DateTime.Now - _startTime):hh\\:mm\\:ss}";

                _view.ShowResultScreen(finalPrice, receiptDetails);
            }
            else
            {
                _view.ShowMessage("Ошибка при закрытии аренды: " + string.Join(", ", result.Errors));
                _view.StartTimer(); // Возвращаем таймер, если сервер упал с ошибкой
            }
        }

        // Возврат назад к выбору машин
        public async Task BackToMenu()
        {
            await LoadAvailableCars();
            _view.ShowSelectionScreen();
        }
    }
}