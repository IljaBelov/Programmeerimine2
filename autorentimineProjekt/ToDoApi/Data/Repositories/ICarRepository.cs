using autorentimineProjekt.ToDoApi.Models;

namespace autorentimineProjekt.ToDoApi.Data.Repositories
{
    public interface ICarRepository
    {
        Task<List<Car>> GetAll();
        Task<Car?> GetById(int id);
        Task Add(Car car);
        Task Save();
    }
}