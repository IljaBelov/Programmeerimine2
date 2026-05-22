using autorentimineProjekt.ToDoApi.Data;
using Microsoft.EntityFrameworkCore;
using System;

namespace autorentimineProjekt.UnitTests
{
    public abstract class TestBase : IDisposable
    {
        private CarRentalContext _dbContext;
        private bool disposedValue;

        protected CarRentalContext DbContext
        {
            get
            {
                if (_dbContext != null)
                {
                    return _dbContext;
                }

                var options = new DbContextOptionsBuilder<CarRentalContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options;
                _dbContext = new CarRentalContext(options);
                return _dbContext;
            }
        }

        protected CarRentalContext GetFaultyDbContext()
        {
            var options = new DbContextOptionsBuilder<CarRentalContext>();
            return new CarRentalContext(options.Options);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _dbContext?.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}