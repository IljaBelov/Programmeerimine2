using TodoApi.Models;
namespace TodoApi.Data.Repositories
{
    public interface IUserRepository
    {
        Task<List<User>> GetAll();
        Task<User?> GetById(int id);
        Task Add(User user);
        Task Save();
    }
}