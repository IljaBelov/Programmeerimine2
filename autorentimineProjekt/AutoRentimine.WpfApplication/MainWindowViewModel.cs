using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoRentimine.WpfApplication.Api;
using autorentimineProjekt.ToDoApi.Application.DTOs;

namespace AutoRentimine.WpfApplication
{
    public class MainWindowViewModel : NotifyPropertyChangedBase
    {
        private readonly IApiClient _apiClient;
        private readonly IDialogProvider _dialogProvider;
        private ObservableCollection<CarModel> _data;
        private CarModel? _selectedItem;

        // Поля бланка
        private string _mark = string.Empty;
        private string _model = string.Empty;
        private string _registrationNumber = string.Empty;
        private string _dailyRate = string.Empty;
        private string _status = string.Empty;

        // Сервисный флаг для защиты от зацикливания при синхронизации
        private bool _isUpdatingFromSelection;

        public MainWindowViewModel() : this(new ApiClient(), new DialogProvider()) { }

        public MainWindowViewModel(IApiClient apiClient, IDialogProvider dialogProvider)
        {
            _apiClient = apiClient;
            _dialogProvider = dialogProvider;
            _data = new ObservableCollection<CarModel>();

            LoadCommand = new RelayCommand<object>(async _ => await LoadDataAsync());
            AddCommand = new RelayCommand<object>(async _ => await AddCarAsync());
            SaveCommand = new RelayCommand<object>(_ => ClearForm(), _ => SelectedItem != null);
            DeleteCommand = new RelayCommand<object>(async _ => await DeleteCarAsync(), _ => SelectedItem != null);
            OpenRentalCommand = new RelayCommand<object>(_ => OpenRental());
        }

        public ICommand LoadCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand OpenRentalCommand { get; }

        public ObservableCollection<CarModel> Data { get => _data; set => SetProperty(ref _data, value); }

        public CarModel? SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (SetProperty(ref _selectedItem, value))
                {
                    // Кликнули по таблице -> заполняем форму данными машины
                    _isUpdatingFromSelection = true;
                    try
                    {
                        Mark = value?.Mark ?? string.Empty;
                        Model = value?.Model ?? string.Empty;
                        RegistrationNumber = value?.RegistrationNumber ?? string.Empty;
                        DailyRate = value?.DailyRate.ToString() ?? string.Empty;
                        Status = value?.Status ?? string.Empty;
                    }
                    finally
                    {
                        _isUpdatingFromSelection = false;
                    }
                }
            }
        }

        // Свойства бланка с обратной связью в реальном времени
        public string Mark
        {
            get => _mark;
            set
            {
                if (SetProperty(ref _mark, value))
                {
                    if (!_isUpdatingFromSelection && SelectedItem != null)
                        SelectedItem.Mark = value;
                }
            }
        }

        public string Model
        {
            get => _model;
            set
            {
                if (SetProperty(ref _model, value))
                {
                    if (!_isUpdatingFromSelection && SelectedItem != null)
                        SelectedItem.Model = value;
                }
            }
        }

        public string RegistrationNumber
        {
            get => _registrationNumber;
            set
            {
                if (SetProperty(ref _registrationNumber, value))
                {
                    if (!_isUpdatingFromSelection && SelectedItem != null)
                        SelectedItem.RegistrationNumber = value;
                }
            }
        }

        public string DailyRate
        {
            get => _dailyRate;
            set
            {
                if (SetProperty(ref _dailyRate, value))
                {
                    if (!_isUpdatingFromSelection && SelectedItem != null)
                    {
                        if (decimal.TryParse(value, out decimal price))
                            SelectedItem.DailyRate = price;
                    }
                }
            }
        }

        public string Status
        {
            get => _status;
            set
            {
                if (SetProperty(ref _status, value))
                {
                    if (!_isUpdatingFromSelection && SelectedItem != null)
                        SelectedItem.Status = value;
                }
            }
        }

        public async Task LoadDataAsync()
        {
            var result = await _apiClient.GetCarsAsync();
            if (result != null && !result.HasErrors && result.Data != null)
            {
                Data.Clear();
                foreach (var d in result.Data)
                    Data.Add(new CarModel { Id = d.Id, Mark = d.Mark, Model = d.Model, RegistrationNumber = d.RegistrationNumber, Status = d.Status, DailyRate = d.DailyRate });
            }
            // ИСПРАВЛЕНИЕ 1: Добавлена обработка ошибок при загрузке
            else if (result != null && result.HasErrors)
            {
                _dialogProvider.ShowError(string.Join(Environment.NewLine, result.Errors));
            }
        }

        private async Task AddCarAsync()
        {
            // ПРОВЕРКА 1: Проверяем, что ВСЕ текстовые поля заполнены (хотя бы одно пустое — вызовет ошибку)
            if (string.IsNullOrWhiteSpace(Mark) ||
                string.IsNullOrWhiteSpace(Model) ||
                string.IsNullOrWhiteSpace(RegistrationNumber) ||
                string.IsNullOrWhiteSpace(DailyRate) ||
                string.IsNullOrWhiteSpace(Status))
            {
                _dialogProvider.ShowError("Ошибка! Невозможно добавить автомобиль. Заполните абсолютно все поля в бланке!");
                return;
            }

            // ПРОВЕРКА 2: Проверяем корректность формата цены
            if (!decimal.TryParse(DailyRate, out decimal price))
            {
                _dialogProvider.ShowError("Ошибка! Поле 'Цена' должно быть числовым значением.");
                return;
            }

            // Формируем DTO для отправки на бэкэнд
            var dto = new CarDto
            {
                Mark = Mark,
                Model = Model,
                RegistrationNumber = RegistrationNumber,
                Status = Status,
                DailyRate = price
            };

            var result = await _apiClient.AddCarAsync(dto);

            if (result != null && !result.HasErrors)
            {
                await LoadDataAsync(); // Обновляем данные с бэка
                ClearForm();           // Чистим бланк после успешного добавления
            }
            else if (result != null && result.HasErrors)
            {
                _dialogProvider.ShowError(string.Join(Environment.NewLine, result.Errors));
            }
        }

        private async Task DeleteCarAsync()
        {
            if (SelectedItem == null) return;

            // ИСПРАВЛЕНИЕ 2: Спрашиваем подтверждение удаления у пользователя перед вызовом API
            if (!_dialogProvider.Confirm("Вы уверены, что хотите удалить выбранный автомобиль?"))
            {
                return; // Если пользователь нажал "Нет", прерываем метод
            }

            var result = await _apiClient.DeleteCarAsync(SelectedItem.Id);

            if (result != null && !result.HasErrors)
            {
                await LoadDataAsync();
                SelectedItem = null; // ИСПРАВЛЕНИЕ 4: Сбрасываем выделенный элемент после успешного удаления
            }
            // ИСПРАВЛЕНИЕ 3: Выводим ошибку, если бэкенд запретил удаление
            else if (result != null && result.HasErrors)
            {
                _dialogProvider.ShowError(string.Join(Environment.NewLine, result.Errors));
            }
        }
        private void OpenRental()
        {
            var rentalWindow = new RentalWindow(_apiClient);
            rentalWindow.Show();
        }

        private void ClearForm()
        {
            SelectedItem = null;
            Mark = string.Empty;
            Model = string.Empty;
            RegistrationNumber = string.Empty;
            DailyRate = string.Empty;
            Status = string.Empty;
        }
    }
}