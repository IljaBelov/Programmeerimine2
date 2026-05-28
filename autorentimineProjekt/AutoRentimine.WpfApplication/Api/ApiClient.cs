using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using autorentimineProjekt.ToDoApi.Application.DTOs;

namespace AutoRentimine.WpfApplication.Api
{
    public interface IApiClient
    {
        Task<OperationResult<List<CarDto>>> GetCarsAsync();
        Task<OperationResult> AddCarAsync(CarDto car);
        Task<OperationResult> UpdateCarAsync(CarDto car);
        Task<OperationResult> DeleteCarAsync(int id);
        Task<OperationResult<int>> CreateBookingAsync(int carId);
        Task<OperationResult<decimal>> CancelBookingAsync(int carId, double kilometers);
    }

    public class ApiClient : IApiClient
    {
        private readonly HttpClient _client;
        private const string BaseUrl = "http://localhost:55005/api/cars";
        private const string BookingsUrl = "http://localhost:55005/api/bookings";

        public ApiClient()
        {
            _client = new HttpClient();
        }

        public async Task<OperationResult<List<CarDto>>> GetCarsAsync()
        {
            var result = new OperationResult<List<CarDto>>();
            try
            {
                var response = await _client.GetAsync(BaseUrl);
                if (response.IsSuccessStatusCode)
                {
                    result.Data = await response.Content.ReadFromJsonAsync<List<CarDto>>();
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

        public async Task<OperationResult> AddCarAsync(CarDto car)
        {
            var result = new OperationResult();
            try
            {
                var response = await _client.PostAsJsonAsync(BaseUrl, car);
                if (!response.IsSuccessStatusCode)
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

        public async Task<OperationResult> UpdateCarAsync(CarDto car)
        {
            var result = new OperationResult();
            try
            {
                var response = await _client.PutAsJsonAsync($"{BaseUrl}/{car.Id}", car);
                if (!response.IsSuccessStatusCode)
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

        public async Task<OperationResult> DeleteCarAsync(int id)
        {
            var result = new OperationResult();
            try
            {
                var response = await _client.DeleteAsync($"{BaseUrl}/{id}");
                if (!response.IsSuccessStatusCode)
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

        public async Task<OperationResult<int>> CreateBookingAsync(int carId)
        {
            var result = new OperationResult<int>();
            try
            {
                var response = await _client.PostAsJsonAsync(BookingsUrl, new { CarId = carId });
                if (response.IsSuccessStatusCode)
                {
                    result.Data = await response.Content.ReadFromJsonAsync<int>();
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
                var response = await _client.PostAsJsonAsync($"{BookingsUrl}/cancel", payload);
                if (response.IsSuccessStatusCode)
                {
                    result.Data = await response.Content.ReadFromJsonAsync<decimal>();
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