using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Data.Repositories
{
    public class CarRepository : BaseRepository<Car>, ICarRepository
    {
        public CarRepository(TodoContext context) : base(context) { }

        // Метод Delete должен быть ЗДЕСЬ, внутри фигурных скобок класса
        public async Task Delete(int id)
        {
            var car = await _context.Cars.FindAsync(id); // Используем _context из базового репозитория
            if (car != null)
            {
                _context.Cars.Remove(car);
            }
        }
    }
}