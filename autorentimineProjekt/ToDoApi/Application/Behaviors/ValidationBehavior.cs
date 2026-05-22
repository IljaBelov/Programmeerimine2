using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;

namespace autorentimineProjekt.ToDoApi.Application.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse> :
        IPipelineBehavior<TRequest, TResponse> where TResponse : class, new()
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request,
                                            RequestHandlerDelegate<TResponse> next,
                                            CancellationToken cancellationToken)
        {
            if (!_validators.Any()) return await next();

            var context = new ValidationContext<TRequest>(request);

            var validationFailures = await Task.WhenAll(
                _validators.Select(validator => validator.ValidateAsync(context, cancellationToken)));

            var errors = validationFailures
                .Where(validationResult => !validationResult.IsValid)
                .SelectMany(validationResult => validationResult.Errors)
                .ToList();

            if (errors.Any())
            {
                var firstError = errors.First().ErrorMessage;
                var response = new TResponse();

                var errorProperty = typeof(TResponse).GetProperty("Error");
                var successProperty = typeof(TResponse).GetProperty("IsSuccess");

                if (errorProperty != null && successProperty != null)
                {
                    errorProperty.SetValue(response, firstError);
                    successProperty.SetValue(response, false);
                }

                return response;
            }

            return await next();
        }
    }
}