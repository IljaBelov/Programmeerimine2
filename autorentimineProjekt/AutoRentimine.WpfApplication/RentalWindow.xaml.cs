using System;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using AutoRentimine.WpfApplication.Api;
using autorentimineProjekt.ToDoApi.Application.DTOs;

namespace AutoRentimine.WpfApplication
{
    public partial class RentalWindow : Window
    {
        private readonly IApiClient _apiClient;
        private readonly DispatcherTimer _timer;

        private CarDto? _selectedCar;
        private DateTime _startTime;
        private double _kilometersTraveled = 0;

        public RentalWindow(IApiClient apiClient)
        {
            InitializeComponent();
            _apiClient = apiClient;

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;

            Loaded += async (s, e) => await LoadAvailableCars();
        }

        private async System.Threading.Tasks.Task LoadAvailableCars()
        {
            var result = await _apiClient.GetCarsAsync();
            if (!result.HasErrors && result.Data != null)
            {
                var freeCars = result.Data
                    .Where(c => c.Status?.Trim().ToLower() == "free")
                    .ToList();
                DgvCars.ItemsSource = freeCars;
            }
            else
            {
                MessageBox.Show("Ошибка загрузки: " + string.Join(", ", result.Errors));
            }
        }

        private void DgvCars_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            _selectedCar = DgvCars.SelectedItem as CarDto;
        }

        private async void BtnStartRental_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCar == null)
            {
                MessageBox.Show("Пожалуйста, выберите автомобиль!");
                return;
            }

            var result = await _apiClient.CreateBookingAsync(_selectedCar.Id);
            if (!result.HasErrors)
            {
                _startTime = DateTime.Now;
                _kilometersTraveled = 0;

                LblCarInfo.Text = $"Вы арендовали: {_selectedCar.Mark} {_selectedCar.Model} [{_selectedCar.RegistrationNumber}]";
                LblTimer.Text = "Время в пути: 00:00:00";
                LblKm.Text = "Пройдено: 0.00 км";

                ShowPanel("active");
                _timer.Start();
            }
            else
            {
                MessageBox.Show("Не удалось арендовать: " + string.Join(", ", result.Errors));
            }
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            TimeSpan elapsed = DateTime.Now - _startTime;
            _kilometersTraveled += (50.0 / 3600.0);

            LblTimer.Text = $"Время в пути: {elapsed:hh\\:mm\\:ss}";
            LblKm.Text = $"Пройдено: {_kilometersTraveled:F2} км";
        }

        private async void BtnEndRental_Click(object sender, RoutedEventArgs e)
        {
            _timer.Stop();

            var result = await _apiClient.CancelBookingAsync(_selectedCar!.Id, _kilometersTraveled);
            if (!result.HasErrors)
            {
                LblFinalPrice.Text = $"{result.Data:F2} EUR";
                LblReceiptDetails.Text = $"Автомобиль: {_selectedCar.Mark} {_selectedCar.Model}\n" +
                                          $"Пройдено: {_kilometersTraveled:F2} км\n" +
                                          $"Время: {(DateTime.Now - _startTime):hh\\:mm\\:ss}";
                ShowPanel("result");
            }
            else
            {
                MessageBox.Show("Ошибка завершения: " + string.Join(", ", result.Errors));
                _timer.Start();
            }
        }

        private async void BtnBackToMenu_Click(object sender, RoutedEventArgs e)
        {
            ShowPanel("selection");
            _selectedCar = null;
            await LoadAvailableCars();
        }

        private void ShowPanel(string panel)
        {
            PanelSelection.Visibility = panel == "selection" ? Visibility.Visible : Visibility.Collapsed;
            PanelActive.Visibility = panel == "active" ? Visibility.Visible : Visibility.Collapsed;
            PanelResult.Visibility = panel == "result" ? Visibility.Visible : Visibility.Collapsed;

            Title = panel switch
            {
                "active" => "Активная поездка 🚗💨",
                "result" => "Ваш счёт за аренду",
                _ => "Аренда автомобиля"
            };
        }
    }
}