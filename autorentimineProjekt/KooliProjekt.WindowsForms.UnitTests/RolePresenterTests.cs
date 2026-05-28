using AutoRentimine.WindowsForms;
using AutoRentimine.WindowsForms.Api;
using Moq;
using Xunit;

namespace KooliProjekt.WindowsForms.UnitTests
{
    public class RolePresenterTests
    {
        private readonly Mock<IRoleView> _viewMock;
        private readonly Mock<IApiClient> _apiClientMock;
        private readonly RolePresenter _presenter;

        public RolePresenterTests()
        {
            _viewMock = new Mock<IRoleView>();
            _apiClientMock = new Mock<IApiClient>();

            // Инициализируем тестируемый презентер, подкидывая ему моки
            _presenter = new RolePresenter(_viewMock.Object, _apiClientMock.Object);
        }

        [Fact]
        public void HandleModeratorChoice_ShouldCallOpenAdminForm_WithCorrectApiClient()
        {
            // Act
            _presenter.HandleModeratorChoice();

            // Assert
            // Проверяем, что презентер приказал форме открыть окно админа 
            // и передал в него именно наш рабочий экземпляр apiClient
            _viewMock.Verify(v => v.OpenAdminForm(_apiClientMock.Object), Times.Once);

            // Проверяем, что форму клиента при этом никто не трогал
            _viewMock.Verify(v => v.OpenClientForm(It.IsAny<IApiClient>()), Times.Never);
        }

        [Fact]
        public void HandleClientChoice_ShouldCallOpenClientForm_WithCorrectApiClient()
        {
            // Act
            _presenter.HandleClientChoice();

            // Assert
            // Проверяем, что презентер приказал форме открыть окно клиента
            _viewMock.Verify(v => v.OpenClientForm(_apiClientMock.Object), Times.Once);

            // Проверяем, что админку при этом никто случайно не открыл
            _viewMock.Verify(v => v.OpenAdminForm(It.IsAny<IApiClient>()), Times.Never);
        }
    }
}