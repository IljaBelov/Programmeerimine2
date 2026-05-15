using autorentimineProjekt.ToDoApi.Application.Bookings.Commands;
using autorentimineProjekt.ToDoApi.Application.Cars.Queries;
using autorentimineProjekt.ToDoApi.Data;
using autorentimineProjekt.ToDoApi.Data.Repositories;
using autorentimineProjekt.ToDoApi.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class CancelBookingHandlerTests
{
    [Fact]
    public async Task Handle_ShouldChangeStatusToFree_WhenBookingCancelled()
    {
        // Arrange: Настройка базы в памяти для теста
        var options = new DbContextOptionsBuilder<CarRentalContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Используем Guid, чтобы тесты не мешали друг другу
            .Options;

        using var context = new CarRentalContext(options);

        var car = new Car { Id = 1, Status = "rented", DailyRate = 10m }; // Статус rented, как мы сделали в Start

        var booking = new Booking
        {
            Id = 1,
            CarId = 1, // ОБЯЗАТЕЛЬНО: привязываем бронь к ID машины
            Car = car,
            StartTime = DateTime.Now.AddHours(-1) // Ставим время на час назад, чтобы цена посчиталась
        };

        context.Cars.Add(car);
        context.Bookings.Add(booking);
        await context.SaveChangesAsync();

        var handler = new CancelBookingHandler(context);

        // Передаем CarId, так как мы переделали команду под логику Swagger
        var command = new CancelBookingCommand { CarId = 1, Kilometers = 100 };

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.True(result.IsSuccess); // Проверяем, что хендлер не выдал ошибку
        Assert.Equal("free", car.Status); // Машина освободилась
        Assert.Equal("Paid", booking.PaymentStatus); // Статус оплаты сменился
        Assert.NotNull(booking.EndTime); // Время завершения проставилось
        Assert.True(booking.TotalPrice > 0); // Цена рассчиталась по твоей формуле
    }
    [Fact]
    public async Task Handle_ShouldReturnDefault_WhenCommandIsNull()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<CarRentalContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        using var context = new CarRentalContext(options);
        var handler = new CancelBookingHandler(context);

        // Act
        // Пункт 10: передаем null в хендлер
        var result = await handler.Handle(null!);

        // Assert
        Assert.True(result.IsSuccess); // Проверка, что vigu tekkida ei tohi (ошибки нет)
        Assert.Equal(0, result.Value); // Проверка, что Value по умолчанию (0 для decimal)
    }
    [Fact]
    public async Task GetCars_ShouldReturnNull_WhenRequestIsNull()
    {
        var options = new DbContextOptionsBuilder<CarRentalContext>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
        .Options;
        using var context = new CarRentalContext(options);
        // Arrange
        var repository = new CarRepository(context);
        var handler = new GetCarsHandler(repository);

        // Act
        var result = await handler.Handle(null);

        // Assert (Пункт 10)
        Assert.True(result.IsSuccess);
        Assert.Null(result.Value); // Проверяем, что результат действительно null
    }
    [Fact]
    public async Task Handle_ShouldReturnNullValue_WhenQueryIsNull()
    {
        // Arrange: Настраиваем базу в памяти и хендлер
        var options = new DbContextOptionsBuilder<CarRentalContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new CarRentalContext(options);
        var handler = new GetBookingsHandler(context);

        // Act: Вызываем хендлер, передавая null (как просит пункт 10 задания)
        var result = await handler.Handle(null!);

        // Assert: Проверяем условия из задания
        // 1. Result.IsSuccess должен быть true (vigu tekkida ei tohi)
        Assert.True(result.IsSuccess);

        // 2. Result.Value должен быть null
        Assert.Null(result.Value);
    }
    [Fact]
    public async Task Handle_ShouldReturnCarsList_WhenCarsExistInDatabase()
    {
        // 1. Arrange: Настраиваем чистую базу в памяти
        var options = new DbContextOptionsBuilder<CarRentalContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new CarRentalContext(options);

        // Добавляем тестовую машину в базу
        var testCar = new Car { Id = 1, Mark = "Toyota", Model = "Corolla", Status = "free", DailyRate = 50 };
        context.Cars.Add(testCar);
        await context.SaveChangesAsync();

        // Создаем репозиторий и хендлер (как мы делали раньше)
        var repository = new CarRepository(context);
        var handler = new GetCarsHandler(repository);
        var query = new GetCarsQuery();

        // 2. Act: Вызываем хендлер
        var result = await handler.Handle(query);

        // 3. Assert: Проверяем, что всё дошло до DTO правильно
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Single(result.Value); // Проверяем, что в списке ровно 1 машина

        var carDto = result.Value[0];
        Assert.Equal("Toyota", carDto.Mark);
        Assert.Equal("Corolla", carDto.Model);
        Assert.Equal(50, carDto.DailyRate);
    }
}