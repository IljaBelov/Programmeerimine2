using System;
using System.Threading.Tasks;
using AutoRentimine.WindowsForms.Api;
using autorentimineProjekt.ToDoApi.Application.DTOs;

namespace AutoRentimine.WindowsForms
{
    public class MainPresenter
    {
        private readonly IMainView _view;
        private readonly IApiClient _apiClient;

        public MainPresenter(IMainView view, IApiClient apiClient)
        {
            _view = view;
            _apiClient = apiClient;
        }

        // Метод для первоначальной загрузки данных
        public async Task LoadData()
        {
            var result = await _apiClient.GetCarsAsync();
            if (!result.HasErrors)
            {
                _view.DisplayCars(result.Data);
            }
            else
            {
                _view.ShowMessage("Ошибка загрузки данных: " + string.Join(", ", result.Errors));
            }
        }

        // ТРЕБОВАНИЕ УЧИТЕЛЯ: Логика команды Добавления строки (AddCommand)
        public void Add()
        {
            _view.AddNewCarRow();
        }

        // ТРЕБОВАНИЕ УЧИТЕЛЯ: Логика команды Сохранения (SaveCommand)
        public async Task Save()
        {
            var selectedCar = _view.SelectedCar;
            if (selectedCar == null) return;

            OperationResult result;
            if (selectedCar.Id == 0)
            {
                // Если Id == 0, машина новая — создаем её через API
                result = await _apiClient.AddCarAsync(selectedCar);
            }
            else
            {
                // Если Id уже есть — обновляем существующую
                result = await _apiClient.UpdateCarAsync(selectedCar);
            }

            if (!result.HasErrors)
            {
                _view.ShowMessage("Данные успешно сохранены!");
                await LoadData(); // Перезагружаем таблицу свежими данными
            }
            else
            {
                _view.ShowMessage("Ошибка сохранения: " + string.Join(", ", result.Errors));

                // ВОТ ЭТА СТРОЧКА ВОЗВРАЩАЕТ ВСЁ НА МЕСТА:
                // Если API отклонил изменения, мы принудительно качаем старые данные и сбрасываем ввод в исходное состояние
                await LoadData();
            }
        }

        // ТРЕБОВАНИЕ УЧИТЕЛЯ: Логика команды Удаления (DeleteCommand) с подтверждением
        public async Task Delete()
        {
            var selectedCar = _view.SelectedCar;
            if (selectedCar == null) return;

            // 1. Вызываем метод ConfirmDelete из интерфейса, который спросит пользователя через диалог
            bool isConfirmed = _view.ConfirmDelete($"Удалить выбранную машину {selectedCar.Mark}?");
            if (!isConfirmed) return;

            if (selectedCar.Id == 0)
            {
                // Если строку создали, но в базу еще не сохранили, просто обновляем таблицу
                await LoadData();
                return;
            }

            // 2. Если пользователь подтвердил, делаем запрос на удаление в API
            var result = await _apiClient.DeleteCarAsync(selectedCar.Id);
            if (!result.HasErrors)
            {
                _view.ShowMessage("Машина успешно удалена.");
                await LoadData();
            }
            else
            {
                _view.ShowMessage("Ошибка удаления: " + string.Join(", ", result.Errors));
            }
        }
    }
}