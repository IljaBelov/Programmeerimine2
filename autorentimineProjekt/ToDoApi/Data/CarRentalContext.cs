using Microsoft.EntityFrameworkCore;
using autorentimineProjekt.ToDoApi.Models;

namespace autorentimineProjekt.ToDoApi.Data
{
    public class CarRentalContext : DbContext
    {
        public CarRentalContext(DbContextOptions<CarRentalContext> options) : base(options) { }

        public DbSet<Car> Cars { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<User> Users { get; set; }
    }
}