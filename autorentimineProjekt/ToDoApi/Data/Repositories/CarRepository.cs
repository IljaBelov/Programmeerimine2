using Microsoft.EntityFrameworkCore;
using autorentimineProjekt.ToDoApi.Models;

namespace autorentimineProjekt.ToDoApi.Data.Repositories
{
    public class CarRepository : ICarRepository
    {
        private readonly CarRentalContext _context;
        public CarRepository(CarRentalContext context) => _context = context;

        public async Task<List<Car>> GetAll() => await _context.Cars.ToListAsync();
        public async Task<Car?> GetById(int id) => await _context.Cars.FindAsync(id);
        public async Task Add(Car car) => await _context.Cars.AddAsync(car);
        public async Task Save() => await _context.SaveChangesAsync();
    }
}