using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using AutoRentimine.WindowsForms.Api;
using autorentimineProjekt.ToDoApi.Application.DTOs;

namespace AutoRentimine.WindowsForms
{
    public partial class RentalForm : Form, IRentalView
    {
        private readonly RentalPresenter _presenter;
        private System.Windows.Forms.Timer _tripTimer;

        // Твои компоненты формы
        private DataGridView _dgvAvailableCars;
        private Button _btnStartRental;
        private Label _lblCarInfo;
        private Label _lblTimer;
        private Label _lblKm;
        private Button _btnEndRental;
        private Label _lblFinalPrice;
        private Label _lblReceiptDetails;
        private Button _btnBackToMenu;

        public RentalForm(IApiClient apiClient)
        {
            InitializeComponent();

            // Твоя отрисовка интерфейса
            InitializeCustomComponents();

            // Привязываем таймер формы к тикам презентера
            _tripTimer = new System.Windows.Forms.Timer();
            _tripTimer.Interval = 1000;
            _tripTimer.Tick += TripTimer_Tick;

            // Создаем MVP презентер
            _presenter = new RentalPresenter(this, apiClient);

            this.Load += RentalForm_Load;
        }

        private void InitializeCustomComponents()
        {
            panelSelection.Controls.Clear();
            panelActive.Controls.Clear();
            panelResult.Controls.Clear();

            // ==========================================
            // ЭКРАН 1: Выбор машины (panelSelection)
            // ==========================================
            _dgvAvailableCars = new DataGridView
            {
                Location = new Point(15, 15),
                Size = new Size(750, 240),
                AutoGenerateColumns = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };

            _btnStartRental = new Button
            {
                Text = "Арендовать выбранный автомобиль",
                Location = new Point(15, 275),
                Size = new Size(320, 45),
                Font = new Font("Arial", 10, FontStyle.Bold),
                BackColor = Color.LightGreen
            };
            _btnStartRental.Click += btnStartRental_Click;

            panelSelection.Controls.Add(_dgvAvailableCars);
            panelSelection.Controls.Add(_btnStartRental);

            // ==========================================
            // ЭКРАН 2: Активная поездка (panelActive)
            // ==========================================
            _lblCarInfo = new Label { Location = new Point(20, 20), Size = new Size(500, 30), Font = new Font("Arial", 12, FontStyle.Bold) };
            _lblTimer = new Label { Text = "Время в пути: 00:00:00", Location = new Point(20, 60), Size = new Size(300, 25), Font = new Font("Arial", 11) };
            _lblKm = new Label { Text = "Пройдено: 0.00 км", Location = new Point(20, 90), Size = new Size(300, 25), Font = new Font("Arial", 11) };

            _btnEndRental = new Button
            {
                Text = "Завершить поездку",
                Location = new Point(20, 140),
                Size = new Size(200, 45),
                Font = new Font("Arial", 11, FontStyle.Bold),
                BackColor = Color.Tomato,
                ForeColor = Color.White
            };
            _btnEndRental.Click += btnEndRental_Click;

            panelActive.Controls.Add(_lblCarInfo);
            panelActive.Controls.Add(_lblTimer);
            panelActive.Controls.Add(_lblKm);
            panelActive.Controls.Add(_btnEndRental);

            // ==========================================
            // ЭКРАН 3: Финальный чек (panelResult)
            // ==========================================
            Label lblTitle = new Label { Text = "Поездка завершена! К оплате:", Location = new Point(25, 25), Size = new Size(300, 25), Font = new Font("Arial", 12, FontStyle.Bold) };
            _lblFinalPrice = new Label { Text = "0.00 EUR", Location = new Point(25, 60), Size = new Size(400, 40), Font = new Font("Arial", 20, FontStyle.Bold), ForeColor = Color.DarkGreen };
            _lblReceiptDetails = new Label { Location = new Point(25, 120), Size = new Size(550, 100), Font = new Font("Arial", 11) };
            _btnBackToMenu = new Button { Text = "Вернуться к списку авто", Location = new Point(25, 240), Size = new Size(200, 40), Font = new Font("Arial", 10) };
            _btnBackToMenu.Click += btnBackToMenu_Click;

            panelResult.Controls.Add(lblTitle);
            panelResult.Controls.Add(_lblFinalPrice);
            panelResult.Controls.Add(_lblReceiptDetails);
            panelResult.Controls.Add(_btnBackToMenu);
        }

        private async void RentalForm_Load(object sender, EventArgs e)
        {
            ShowPanel(panelSelection);
            await _presenter.LoadAvailableCars();
        }

        private void ShowPanel(Panel panelToShow)
        {
            panelSelection.Visible = (panelToShow == panelSelection);
            panelActive.Visible = (panelToShow == panelActive);
            panelResult.Visible = (panelToShow == panelResult);

            if (panelToShow == panelSelection) this.Text = "Выбор автомобиля";
            if (panelToShow == panelActive) this.Text = "Активная поездка 🚗💨";
            if (panelToShow == panelResult) this.Text = "Ваш счет за аренду";
        }

        // --- Реализация интерфейса IRentalView ---

        public CarDto SelectedCar => _dgvAvailableCars?.CurrentRow?.DataBoundItem as CarDto;

        public void DisplayAvailableCars(List<CarDto> cars)
        {
            if (_dgvAvailableCars != null)
            {
                _dgvAvailableCars.DataSource = null;
                _dgvAvailableCars.DataSource = cars;
            }
        }

        public void ShowMessage(string message)
        {
            MessageBox.Show(message);
        }

        public void ShowSelectionScreen() => ShowPanel(panelSelection);

        public void ShowActiveScreen(string carInfoText)
        {
            _lblCarInfo.Text = carInfoText;
            _lblTimer.Text = "Время в пути: 00:00:00";
            _lblKm.Text = "Пройдено: 0.00 км";
            ShowPanel(panelActive);
        }

        public void ShowResultScreen(string finalPrice, string receiptDetails)
        {
            _lblFinalPrice.Text = finalPrice;
            _lblReceiptDetails.Text = receiptDetails;
            ShowPanel(panelResult);
        }

        public void UpdateTripMetrics(string timeText, string kmText)
        {
            _lblTimer.Text = timeText;
            _lblKm.Text = kmText;
        }

        public void StartTimer() => _tripTimer.Start();
        public void StopTimer() => _tripTimer.Stop();

        // --- Обработчики UI событий (Перенаправление в Презентер) ---

        private async void btnStartRental_Click(object sender, EventArgs e)
        {
            await _presenter.StartRental();
        }

        private void TripTimer_Tick(object sender, EventArgs e)
        {
            _presenter.ProcessTimerTick();
        }

        private async void btnEndRental_Click(object sender, EventArgs e)
        {
            await _presenter.EndRental();
        }

        private async void btnBackToMenu_Click(object sender, EventArgs e)
        {
            await _presenter.BackToMenu();
        }
    }
}