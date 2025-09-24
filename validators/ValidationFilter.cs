using FluentValidation;

namespace AutoInsightAPI.Validators
{
    public class ValidationFilter<T> : IEndpointFilter where T : class
    {
        private readonly IValidator<T> _validator;

        public ValidationFilter(IValidator<T> validator)
        {
            _validator = validator;
        }

        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            var argument = context.Arguments.FirstOrDefault(a => a?.GetType() == typeof(T)) as T;

            if (argument is null)
            {
                return Results.ValidationProblem(new Dictionary<string, string[]>
                {
                    [""] = new[] { "The request body is empty or invalid." }
                });
            }

            var validationResult = await _validator.ValidateAsync(argument);

            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            return await next(context);
        }
    }
}
