using Microsoft.EntityFrameworkCore;
using autorentimineProjekt.ToDoApi.Data;
using autorentimineProjekt.ToDoApi.Data.Repositories;
using autorentimineProjekt.ToDoApi.Application.Cars.Queries;
using autorentimineProjekt.ToDoApi.Application.Bookings.Commands;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<GetBookingsHandler>();
builder.Services.AddScoped<CreateBookingHandler>();
builder.Services.AddScoped<CancelBookingHandler>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Настройка базы (в памяти для тестов или SQL)
builder.Services.AddDbContext<CarRentalContext>(opt => opt.UseInMemoryDatabase("CarRental"));

// Регистрация репозиториев и хендлеров
builder.Services.AddScoped<ICarRepository, CarRepository>();
builder.Services.AddScoped<GetCarsHandler>();

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
            new autorentimineProjekt.ToDoApi.Models.Car { Mark = "Toyota", Model = "Corolla", RegistrationNumber = "123 ABC", Status = "free", DailyRate = 40 }
        );
        context.SaveChanges();
    }
}
app.Run();