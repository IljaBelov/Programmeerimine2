using autorentimineProjekt.ToDoApi.Application.Bookings.Commands;
using autorentimineProjekt.ToDoApi.Application.Cars.Commands;
using autorentimineProjekt.ToDoApi.Application.Cars.Queries;
using autorentimineProjekt.ToDoApi.Data;
using autorentimineProjekt.ToDoApi.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using MediatR;
using FluentValidation;
using autorentimineProjekt.ToDoApi.Application.Behaviors;

var builder = WebApplication.CreateBuilder(args);

// 1. НАСТРОЙКА CORS (Разрешаем Blazor с любого порта)
// 1. НАСТРОЙКА CORS — Теперь с правильным портом Blazor
builder.Services.AddCors();

// 2. РЕГИСТРАЦИЯ MEDIATR
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    cfg.AddOpenBehavior(typeof(ErrorHandlingBehavior<,>));
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
    cfg.AddOpenBehavior(typeof(TransactionalBehavior<,>));
});

// 3. РЕГИСТРАЦИЯ ВАЛИДАТОРОВ И РЕПОЗИТОРИЕВ (Без дубликатов)
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddScoped<ICarRepository, CarRepository>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 4. НАСТРОЙКА БАЗЫ ДАННЫХ (In-Memory)
builder.Services.AddDbContext<CarRentalContext>(opt => opt.UseInMemoryDatabase("CarRental"));

var app = builder.Build();

// 5. ПОДКЛЮЧЕНИЕ SWAGGER В DEVELOPMENT-РЕЖИМЕ
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseRouting();
// 6. ВКЛЮЧЕНИЕ CORS (Строго перед MapControllers!)
app.UseCors(options => options
    .AllowAnyOrigin()
    .AllowAnyHeader()
    .AllowAnyMethod());

app.MapControllers();

// 7. ИНИЦИАЛИЗАЦИЯ ДЕМО-ДАННЫХ В БАЗУ
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<CarRentalContext>();
    if (!context.Cars.Any())
    {
        context.Cars.AddRange(
            new autorentimineProjekt.ToDoApi.Models.Car { Mark = "Tesla", Model = "Model 3", RegistrationNumber = "777 TES", Status = "free", DailyRate = 60 },
            new autorentimineProjekt.ToDoApi.Models.Car { Mark = "Audi", Model = "A6", RegistrationNumber = "123 ABC", Status = "rented", DailyRate = 45 }
        );
        context.SaveChanges();
    }
}

app.Run();

public partial class Program { }