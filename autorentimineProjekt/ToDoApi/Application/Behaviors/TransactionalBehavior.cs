using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using autorentimineProjekt.ToDoApi.Data; // Твой CarRentalContext
using MediatR;

namespace autorentimineProjekt.ToDoApi.Application.Behaviors
{
    // Объявляем интерфейс-маркер тут, чтобы его видели хендлеры
    public interface ITransactional { }

    public class TransactionalBehavior<TRequest, TResponse> :
        IPipelineBehavior<TRequest, TResponse>
        where TRequest : ITransactional
        where TResponse : class
    {
        private readonly CarRentalContext _dbContext;

        public TransactionalBehavior(CarRentalContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            TResponse response = default;

            try
            {
                Debug.WriteLine("Begin transaction");
                await _dbContext.Database.BeginTransactionAsync(cancellationToken);

                response = await next();

                Debug.WriteLine("Commit transaction");
                await _dbContext.Database.CommitTransactionAsync(cancellationToken);
            }
            catch
            {
                Debug.WriteLine("Rollback transaction");
                await _dbContext.Database.RollbackTransactionAsync(cancellationToken);
                throw;
            }

            return response;
        }
    }
}