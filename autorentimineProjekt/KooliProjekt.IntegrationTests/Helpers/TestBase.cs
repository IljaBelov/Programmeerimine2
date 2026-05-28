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
        private CarRentalContext _dbContext;

        public WebApplicationFactory<FakeStartup> Factory { get; private set; }
        public HttpClient Client { get; private set; }

        public TestBase()
        {
            Factory = new TestApplicationFactory<FakeStartup>();
            Client = Factory.CreateClient();
        }

        protected CarRentalContext DbContext
        {
            get
            {
                if (_dbContext != null)
                {
                    return _dbContext;
                }

                _dbContext = Factory.Services.GetService<CarRentalContext>();
                return _dbContext;
            }
        }

        public void Dispose()
        {
            using var scope = Factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CarRentalContext>();
            dbContext.Database.EnsureDeleted();

            if (Factory != null)
            {
                Factory.Dispose();
                Factory = null;
            }

            if (Client != null)
            {
                Client.Dispose();
                Client = null;
            }
        }

        // Add your other helper methods here
    }
}