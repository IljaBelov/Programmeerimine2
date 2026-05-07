using TodoApi.Models;
namespace TodoApi.Data.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(TodoContext context) : base(context) { }
    }
}