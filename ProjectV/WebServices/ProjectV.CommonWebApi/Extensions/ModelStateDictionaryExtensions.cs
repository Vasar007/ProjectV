using System.Linq;
using Acolyte.Assertions;
using Acolyte.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ProjectV.CommonWebApi.Extensions
{
    public static class ModelStateDictionaryExtensions
    {
        public static (bool isValid, string? error) ValidateModel(
            this ModelStateDictionary modelState)
        {
            modelState.ThrowIfNull(nameof(modelState));

            if (modelState.IsValid)
            {
                return (isValid: true, error: null);
            }

            var errors = modelState.Values
                .SelectMany(x => x.Errors.Select(c => c.ErrorMessage))
                .ToReadOnlyList();

            string combinedError = $"{string.Join(",", errors)}";

            return (isValid: false, error: combinedError);
        }
    }
}
