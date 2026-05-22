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

// РЕГИСТРАЦИЯ MEDIATR ЧЕРЕЗ PROGRAM (теперь он точно найдет все хендлеры в проекте)
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);

    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ErrorHandlingBehavior<,>));
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(TransactionalBehavior<,>));
});

// Автоматически регистрируем все валидаторы FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Настройка базы (в памяти для тестов)
builder.Services.AddDbContext<CarRentalContext>(opt => opt.UseInMemoryDatabase("CarRental"));

// Регистрация репозиториев
builder.Services.AddScoped<ICarRepository, CarRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

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