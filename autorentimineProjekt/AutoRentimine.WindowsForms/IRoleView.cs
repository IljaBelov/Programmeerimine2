using AutoRentimine.WindowsForms.Api;

namespace AutoRentimine.WindowsForms
{
    public interface IRoleView
    {
        // Метод для открытия панели модератора/админа (Form1)
        void OpenAdminForm(IApiClient apiClient);

        // Метод для открытия панели обычного клиента (RentalForm)
        void OpenClientForm(IApiClient apiClient);
    }
}