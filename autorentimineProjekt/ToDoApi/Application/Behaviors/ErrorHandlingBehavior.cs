using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using autorentimineProjekt.ToDoApi.Application.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace autorentimineProjekt.ToDoApi.Application.Behaviors
{
    public class ErrorHandlingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TResponse : class, new()
    {
        private readonly ILogger<ErrorHandlingBehavior<TRequest, TResponse>> _logger;

        public ErrorHandlingBehavior(ILogger<ErrorHandlingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            try
            {
                return await next();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
                _logger.LogError(ex.Message, ex);

                // Создаем дефолтный объект ответа ошибки
                var response = new TResponse();

                // Проверяем, если это наш Result<T>, записываем туда текст ошибки
                var errorProperty = typeof(TResponse).GetProperty("Error");
                var successProperty = typeof(TResponse).GetProperty("IsSuccess");

                if (errorProperty != null && successProperty != null)
                {
                    errorProperty.SetValue(response, ex.Message);
                    successProperty.SetValue(response, false);
                }

                return response;
            }
        }
    }
}