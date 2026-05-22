using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace KooliProjekt.IntegrationTests.Helpers
{
    // Убираем <TStartup> вообще. Наследуемся напрямую от WebApplicationFactory<FakeStartup>
    public class TestApplicationFactory : WebApplicationFactory<FakeStartup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // Твой точный путь до папки, где лежит ToDoApi.csproj
            string apiPath = @"C:\Users\IT\MvcProjects\Programmeerimine2\autorentimineProjekt\ToDoApi";

            builder.UseContentRoot(apiPath);

            builder.ConfigureAppConfiguration((context, conf) =>
            {
                conf.AddJsonFile(Path.Combine(apiPath, "appsettings.json"), optional: true);
            });

            base.ConfigureWebHost(builder);
        }
    }
}