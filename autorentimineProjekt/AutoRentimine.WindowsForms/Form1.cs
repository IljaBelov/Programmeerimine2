using AutoRentimine.WindowsForms.Api;
using autorentimineProjekt.ToDoApi.Application.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace AutoRentimine.WindowsForms
{
    // Наследуем форму от нашего нового интерфейса IMainView
    public partial class Form1 : Form, IMainView
    {
        private readonly MainPresenter _presenter;
        private BindingList<CarDto> _carsList;

        public Form1(IApiClient apiClient)
        {
            InitializeComponent();

            // Инициализируем презентер, связывая форму и логику вместе
            _presenter = new MainPresenter(this, apiClient);

            // Подключаем событие изменения ячеек напрямую к логике сохранения в презентере
            this.dataGridViewCars.CellEndEdit += DataGridViewCars_CellEndEdit;
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            SetupGrid();
            // Загрузку данных теперь выполняет презентер
            await _presenter.LoadData();
        }

        // --- Реализация методов интерфейса IMainView ---

        public CarDto SelectedCar => dataGridViewCars.CurrentRow?.DataBoundItem as CarDto;

        public void DisplayCars(List<CarDto> cars)
        {
            _carsList = new BindingList<CarDto>(cars);
            dataGridViewCars.DataSource = _carsList;
        }

        public void ShowMessage(string message)
        {
            MessageBox.Show(message, "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // ТРЕБОВАНИЕ УЧИТЕЛЯ: Реализация метода ConfirmDelete в форме
        public bool ConfirmDelete(string message)
        {
            var confirmResult = MessageBox.Show(message, "Подтверждение удаления",
                                                 MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            return confirmResult == DialogResult.Yes;
        }

        public void AddNewCarRow()
        {
            var newCar = new CarDto { Id = 0, Status = "free" };
            _carsList.Add(newCar);

            int lastRowIndex = dataGridViewCars.Rows.Count - 1;
            if (dataGridViewCars.AllowUserToAddRows) lastRowIndex--;

            if (lastRowIndex >= 0)
            {
                dataGridViewCars.CurrentCell = dataGridViewCars.Rows[lastRowIndex].Cells["Mark"];
                dataGridViewCars.BeginEdit(true);
            }
        }

        // --- ОБРАБОТЧИКИ НАЖАТИЯ НА КНОПКИ (ПЕРЕНЕСЕНО В ПРЕЗЕНТЕР) ---

        private void btnAdd_Click(object sender, EventArgs e)
        {
            _presenter.Add();
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            await _presenter.Delete();
        }

        // Если у тебя на форме есть отдельная кнопка сохранения (например btnSave)
        private async void btnSave_Click(object sender, EventArgs e)
        {
            await _presenter.Save();
        }

        private async void DataGridViewCars_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            await _presenter.Save();
        }

        // Настройка колонок таблицы (остается без изменений)
        private void SetupGrid()
        {
            dataGridViewCars.AutoGenerateColumns = false;
            dataGridViewCars.Columns.Clear();

            dataGridViewCars.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Id", HeaderText = "ID", Name = "Id", ReadOnly = true });
            dataGridViewCars.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Mark", HeaderText = "Марка", Name = "Mark" });
            dataGridViewCars.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Model", HeaderText = "Модель", Name = "Model" });
            dataGridViewCars.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "DailyRate", HeaderText = "Цена в день", Name = "DailyRate" });
            dataGridViewCars.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "RegistrationNumber", HeaderText = "Номер", Name = "RegistrationNumber" });
            dataGridViewCars.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Status", HeaderText = "Статус", Name = "Status" });
        }

        private void DataGridViewCars_KeyDown(object sender, KeyEventArgs e)
        {
            // Твоя заглушка для дизайнера
        }
    }
}