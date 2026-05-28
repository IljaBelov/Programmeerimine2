using System.Collections.Generic;
using autorentimineProjekt.ToDoApi.Application.DTOs;

namespace AutoRentimine.WindowsForms
{
    public interface IMainView
    {
        // Возвращает текущую выбранную машину в таблице DataGridView
        CarDto SelectedCar { get; }

        // Заполняет таблицу данными из списка машин
        void DisplayCars(List<CarDto> cars);

        // Показывает любые сообщения или ошибки пользователю (MessageBox)
        void ShowMessage(string message);

        // Добавляет новую пустую строку в таблицу для ввода данных
        void AddNewCarRow();

        // ТРЕБОВАНИЕ УЧИТЕЛЯ: Метод подтверждения удаления
        bool ConfirmDelete(string message);
    }
}