using System.Collections.Generic;
using autorentimineProjekt.ToDoApi.Application.DTOs;

namespace AutoRentimine.WindowsForms
{
    public interface IRentalView
    {
        // Какая машина сейчас выделена в таблице
        CarDto SelectedCar { get; }

        // Показ машин в таблице
        void DisplayAvailableCars(List<CarDto> cars);

        // Показ сообщений и ошибок
        void ShowMessage(string message);

        // Методы управления экранами (панелями)
        void ShowSelectionScreen();
        void ShowActiveScreen(string carInfoText);
        void ShowResultScreen(string finalPrice, string receiptDetails);

        // Обновление таймера и километров на экране
        void UpdateTripMetrics(string timeText, string kmText);

        // Управление таймером формы
        void StartTimer();
        void StopTimer();
    }
}