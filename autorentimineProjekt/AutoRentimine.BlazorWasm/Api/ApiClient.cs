using AutoRentimine.BlazorWasm.Api;
using AutoRentimine.BlazorWasm.DTOs;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace AutoRentimine.BlazorWasm
{
    public interface IApiClient
    {
        Task<OperationResult<List<CarDto>>> GetCarsAsync();
        Task<OperationResult> AddCarAsync(CarDto car);
        Task<OperationResult> UpdateCarAsync(CarDto car);
        Task<OperationResult> SaveCarAsync(CarDto car);
        Task<OperationResult> DeleteCarAsync(int id);
        Task<OperationResult<int>> CreateBookingAsync(int carId);
        Task<OperationResult<decimal>> CancelBookingAsync(int carId, double kilometers);
    }

    public class ApiClient : IApiClient
    {
        private readonly HttpClient _client;

        // Теперь мы просто берем готовый HttpClient, который настроен на бэкенд
        public ApiClient(HttpClient client)
        {
            _client = client;
        }

        public async Task<OperationResult<List<CarDto>>> GetCarsAsync()
        {
            var result = new OperationResult<List<CarDto>>();
            try
            {
                // Запрос пойдет на https://localhost:55004/api/cars
                var response = await _client.GetAsync("api/cars");
                if (response.IsSuccessStatusCode)
                {
                    result.Value = await response.Content.ReadFromJsonAsync<List<CarDto>>();
                }
                else
                {
                    var content = await response.Content.ReadAsStringAsync();
                    result.AddError(content);
                }
            }
            catch (Exception ex)
            {
                result.AddError($"Ошибка сети: {ex.Message}");
            }
            return result;
        }

        public async Task<OperationResult> SaveCarAsync(CarDto car)
        {
            var result = new OperationResult();
            try
            {
                HttpResponseMessage response;
                if (car.Id == 0)
                {
                    response = await _client.PostAsJsonAsync("api/cars", car);
                }
                else
                {
                    response = await _client.PutAsJsonAsync($"api/cars/{car.Id}", car);
                }

                if (!response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    result.AddError(content);
                }
            }
            catch (Exception ex)
            {
                result.AddError($"Ошибка при сохранении: {ex.Message}");
            }
            return result;
        }

        public async Task<OperationResult> AddCarAsync(CarDto car) => await SaveCarAsync(car);
        public async Task<OperationResult> UpdateCarAsync(CarDto car) => await SaveCarAsync(car);

        public async Task<OperationResult> DeleteCarAsync(int id)
        {
            var result = new OperationResult();
            try
            {
                var response = await _client.DeleteAsync($"api/cars/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    result.AddError(content);
                }
            }
            catch (Exception ex)
            {
                result.AddError($"Ошибка при удалении: {ex.Message}");
            }
            return result;
        }

        public async Task<OperationResult<int>> CreateBookingAsync(int carId)
        {
            var result = new OperationResult<int>();
            try
            {
                var response = await _client.PostAsJsonAsync("api/bookings", new { CarId = carId });
                if (response.IsSuccessStatusCode)
                {
                    result.Value = await response.Content.ReadFromJsonAsync<int>();
                }
                else
                {
                    var content = await response.Content.ReadAsStringAsync();
                    result.AddError(content);
                }
            }
            catch (Exception ex)
            {
                result.AddError($"Ошибка сети: {ex.Message}");
            }
            return result;
        }

        public async Task<OperationResult<decimal>> CancelBookingAsync(int carId, double kilometers)
        {
            var result = new OperationResult<decimal>();
            try
            {
                var payload = new { CarId = carId, Kilometers = kilometers };
                var response = await _client.PostAsJsonAsync("api/bookings/cancel", payload);
                if (response.IsSuccessStatusCode)
                {
                    result.Value = await response.Content.ReadFromJsonAsync<decimal>();
                }
                else
                {
                    var content = await response.Content.ReadAsStringAsync();
                    result.AddError(content);
                }
            }
            catch (Exception ex)
            {
                result.AddError($"Ошибка сети: {ex.Message}");
            }
            return result;
        }
    }
}