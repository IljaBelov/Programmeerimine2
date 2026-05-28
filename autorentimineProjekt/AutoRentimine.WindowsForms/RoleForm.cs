using System;
using System.Windows.Forms;
using AutoRentimine.WindowsForms.Api;

namespace AutoRentimine.WindowsForms
{
    // Наследуем форму от нового интерфейса IRoleView
    public partial class RoleForm : Form, IRoleView
    {
        private readonly RolePresenter _presenter;

        public RoleForm(IApiClient apiClient)
        {
            InitializeComponent();

            // Инициализируем презентер для этой формы
            _presenter = new RolePresenter(this, apiClient);

            // Привязка событий клика к кнопкам (строго по твоему дизайнеру)
            if (btnModerator != null) btnModerator.Click += btnModerator_Click;
            if (btnClient != null) btnClient.Click += btnClient_Click;
        }

        // --- Обработчики кнопок (перенаправляют команды в Презентер) ---

        private void btnModerator_Click(object sender, EventArgs e)
        {
            _presenter.HandleModeratorChoice();
        }

        private void btnClient_Click(object sender, EventArgs e)
        {
            _presenter.HandleClientChoice();
        }

        // --- Реализация методов интерфейса IRoleView (чисто визуальное переключение окон) ---

        public void OpenAdminForm(IApiClient apiClient)
        {
            Form1 adminForm = new Form1(apiClient);

            this.Hide();           // Прячем окно выбора ролей
            adminForm.ShowDialog(); // Открываем админку (Form1)
            this.Show();           // Возвращаем окно ролей после закрытия админки
        }

        public void OpenClientForm(IApiClient apiClient)
        {
            RentalForm rentalForm = new RentalForm(apiClient);

            this.Hide();            // Прячем окно выбора ролей
            rentalForm.ShowDialog(); // Открываем форму проката авто
            this.Show();            // Возвращаем окно ролей после закрытия проката
        }
    }
}