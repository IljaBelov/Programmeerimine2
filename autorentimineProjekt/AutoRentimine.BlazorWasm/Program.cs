using AutoRentimine.BlazorWasm;
using AutoRentimine.BlazorWasm.Api;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Регистрируем HttpClient с базовым адресом бэкенда
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("http://localhost:55005/")
});

// Регистрируем ApiClient
builder.Services.AddScoped<IApiClient, ApiClient>();

await builder.Build().RunAsync();