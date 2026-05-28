using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xunit;
using KooliProjekt.IntegrationTests.Helpers;

using autorentimineProjekt.ToDoApi.Models;
using autorentimineProjekt.ToDoApi.Application.Cars.Commands;

namespace KooliProjekt.IntegrationTests
{
    public class CarsControllerTests : TestBase
    {
        [Fact]
        public async Task Get_List_ShouldReturnSuccessAndCarsList()
        {
            var response = await Client.GetAsync("/api/cars");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var cars = await response.Content.ReadFromJsonAsync<List<Car>>();
            Assert.NotNull(cars);
        }

        [Fact]
        public async Task Get_ById_WithInvalidId_ShouldReturnBadRequest()
        {
            var response = await Client.GetAsync("/api/cars/9999");
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Save_WithValidData_ShouldCreateCarAndReturnSuccess()
        {
            var command = new CreateCarCommand
            {
                Mark = "Porsche",
                Model = "911",
                RegistrationNumber = "911 POR",
                Status = "free",
                DailyRate = 150
            };

            var response = await Client.PostAsJsonAsync("/api/cars", command);

            // Теперь транзакции не падают, сервер вернет 200 OK!
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var body = await response.Content.ReadAsStringAsync();
            var createdCarId = await response.Content.ReadFromJsonAsync<int>();
            Assert.True(createdCarId > 0);
        }

        [Fact]
        public async Task Save_WithInvalidData_ShouldReturnBadRequestWithOperationResult()
        {
            var command = new CreateCarCommand
            {
                Mark = "", // Вызовет ошибку валидации "Mark and Model are required." в хендлере
                Model = "Cayenne",
                RegistrationNumber = "ERR 123",
                Status = "free",
                DailyRate = 120
            };

            var response = await Client.PostAsJsonAsync("/api/cars", command);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Delete_WithExistingId_ShouldRemoveCarAndReturnSuccess()
        {
            var command = new CreateCarCommand
            {
                Mark = "Audi",
                Model = "A6",
                RegistrationNumber = "888 AUD",
                Status = "free",
                DailyRate = 70
            };

            // Создаем машину через POST
            var createResponse = await Client.PostAsJsonAsync("/api/cars", command);
            Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);

            var createdCarId = await createResponse.Content.ReadFromJsonAsync<int>();
            Assert.True(createdCarId > 0);

            // Удаляем созданную машину по реальному ID
            var response = await Client.DeleteAsync($"/api/cars/{createdCarId}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Delete_WithNonExistingId_ShouldReturnNotFoundOrBadRequest()
        {
            var response = await Client.DeleteAsync("/api/cars/99999");
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}