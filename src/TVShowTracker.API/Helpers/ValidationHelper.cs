using System.ComponentModel.DataAnnotations;

namespace TVShowTracker.API.Helpers;

public static class ValidationHelper
{
    public static IResult ValidateParameters<T>(T parameters) where T : class
    {
        var context = new ValidationContext(parameters);
        var validationResults = new List<ValidationResult>();

        if (!Validator.TryValidateObject(parameters, context, validationResults, true))
        {
            var errors = validationResults.Select(r => r.ErrorMessage).ToList();
            return Results.BadRequest(new { Errors = errors });
        }

        var properties = typeof(T).GetProperties();
        foreach (var property in properties)
        {
            if (property.PropertyType == typeof(string))
            {
                var value = property.GetValue(parameters) as string;
                if (string.IsNullOrWhiteSpace(value))
                {
                    property.SetValue(parameters, null);
                }
            }
        }

        return null!;
    }
}
