using System.Text.Json;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace EmployeeManagement.Api.Filters;

/// <summary>
/// Runs the FluentValidation validator registered for each action argument and
/// short-circuits with a 400 validation problem before the action executes.
/// </summary>
internal sealed class ValidationFilter(IServiceProvider services, ProblemDetailsFactory problemDetailsFactory)
    : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        List<ValidationFailure> failures = [];

        foreach (object? argument in context.ActionArguments.Values)
        {
            if (argument is null)
            {
                continue;
            }

            Type validatorType = typeof(IValidator<>).MakeGenericType(argument.GetType());

            if (services.GetService(validatorType) is not IValidator validator)
            {
                continue;
            }

            ValidationResult result = await validator.ValidateAsync(
                new ValidationContext<object>(argument),
                context.HttpContext.RequestAborted);

            failures.AddRange(result.Errors);
        }

        if (failures.Count == 0)
        {
            await next();
            return;
        }

        // A fresh dictionary keeps error keys camelCased; the bound ModelState already
        // holds PascalCase keys and matches case-insensitively.
        ModelStateDictionary errors = new();

        foreach (ValidationFailure failure in failures)
        {
            errors.AddModelError(JsonNamingPolicy.CamelCase.ConvertName(failure.PropertyName), failure.ErrorMessage);
        }

        ValidationProblemDetails problem = problemDetailsFactory.CreateValidationProblemDetails(
            context.HttpContext,
            errors,
            StatusCodes.Status400BadRequest);

        context.Result = new BadRequestObjectResult(problem);
    }
}
