using System;
using System.Windows.Forms;
using AutoRentimine.WindowsForms.Api;
// Подключаем нашу новую папку

namespace AutoRentimine.WindowsForms
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Создаем экземпляр API-клиента
            IApiClient apiClient = new ApiClient();

            // Передаем его в конструктор формы (Пункт 3)
            Application.Run(new RoleForm(apiClient)); 
        }
    }
}