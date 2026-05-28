using AutoRentimine.WindowsForms.Api;

namespace AutoRentimine.WindowsForms
{
    public class RolePresenter
    {
        private readonly IRoleView _view;
        private readonly IApiClient _apiClient;

        public RolePresenter(IRoleView view, IApiClient apiClient)
        {
            _view = view;
            _apiClient = apiClient;
        }

        // Логика нажатия на кнопку Модератора
        public void HandleModeratorChoice()
        {
            // Презентер говорит виду: "Открывай окно админа и держи API-клиент"
            _view.OpenAdminForm(_apiClient);
        }

        // Логика нажатия на кнопку Клиента
        public void HandleClientChoice()
        {
            // Презентер говорит виду: "Открывай окно проката и держи API-клиент"
            _view.OpenClientForm(_apiClient);
        }
    }
}