using Microsoft.EntityFrameworkCore;
using TodoApi.Data;

namespace TodoApi.Models // проверь, чтобы namespace совпадал с твоим проектом
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new TodoContext(
                serviceProvider.GetRequiredService<DbContextOptions<TodoContext>>()))
            {
                // Если в базе уже есть машины, не добавляем ничего
                if (context.Cars.Any())
                {
                    return;
                }

                context.Cars.AddRange(
                    new Car { CarNumber = "001 ABC", Status = "free", HourlyRate = 12, KilometerRate = 0.5m },
                    new Car { CarNumber = "002 XYZ", Status = "free", HourlyRate = 15, KilometerRate = 0.6m },
                    new Car { CarNumber = "003 TES", Status = "free", HourlyRate = 20, KilometerRate = 0.8m },
                    new Car { CarNumber = "004 BMW", Status = "free", HourlyRate = 25, KilometerRate = 1.0m },
                    new Car { CarNumber = "005 AUD", Status = "free", HourlyRate = 22, KilometerRate = 0.9m },
                    new Car { CarNumber = "006 MER", Status = "free", HourlyRate = 30, KilometerRate = 1.2m },
                    new Car { CarNumber = "007 JLB", Status = "free", HourlyRate = 10, KilometerRate = 0.4m },
                    new Car { CarNumber = "008 KIA", Status = "free", HourlyRate = 12, KilometerRate = 0.5m },
                    new Car { CarNumber = "009 HYU", Status = "free", HourlyRate = 11, KilometerRate = 0.45m },
                    new Car { CarNumber = "010 FRD", Status = "free", HourlyRate = 14, KilometerRate = 0.55m }
                );

                context.SaveChanges();
            }
        }
    }
}