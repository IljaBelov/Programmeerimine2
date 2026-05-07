using TodoApi.Models;
namespace TodoApi.Data.Repositories
{
    public interface ICarRepository
    {
        Task<List<Car>> GetAll();
        Task<PagedResult<Car>> GetAllPaged(int page, int pageSize);
        Task Add(Car car);
        Task Delete(int id);
        Task Save();
        Task<Car?> GetById(int id);
    }
}