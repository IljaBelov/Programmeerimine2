using autorentimineProjekt.ToDoApi.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.Diagnostics; // Обязательно для InMemoryEventId
using System;
using System.Linq;
using System.Net.Http;
using autorentimineProjekt.ToDoApi;

namespace KooliProjekt.IntegrationTests.Helpers
{
    public abstract class TestBase : IDisposable
    {
        private IServiceScope _scope;
        private CarRentalContext _dbContext;

        public WebApplicationFactory<Program> Factory { get; }
        public HttpClient Client { get; }

        public TestBase()
        {
            // Настраиваем фабрику с динамической подменой конфигурации базы данных
            Factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {
                // Используем правильный путь к API проекта
                builder.UseContentRoot(@"C:\Users\IT\MvcProjects\Programmeerimine2\autorentimineProjekt\ToDoApi");

                builder.ConfigureServices(services =>
                {
                    // Находим базовую регистрацию контекста и удаляем её на время тестов
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<CarRentalContext>));
                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    // Регистрируем контекст заново, принудительно заставляя InMemory-базу игнорировать вызовы транзакций
                    services.AddDbContext<CarRentalContext>(options =>
                    {
                        options.UseInMemoryDatabase("CarRentalTest")
                               .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
                    });
                });
            });

            Client = Factory.CreateClient();

            // Создаем Scope, который будет жить на протяжении одного тестового метода
            _scope = Factory.Services.CreateScope();
        }

        // Теперь это свойство полностью безопасно и доступно в любом тесте
        protected CarRentalContext DbContext
        {
            get
            {
                if (_dbContext == null)
                {
                    _dbContext = _scope.ServiceProvider.GetRequiredService<CarRentalContext>();
                }
                return _dbContext;
            }
        }

        public void Dispose()
        {
            if (_dbContext != null)
            {
                _dbContext.Database.EnsureDeleted();
            }

            _scope?.Dispose();
            Client?.Dispose();
            Factory?.Dispose();
        }
    }
}