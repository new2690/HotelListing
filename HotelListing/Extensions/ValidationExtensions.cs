using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.ModelBinding;


namespace HotelListing.Extensions
{
    public static class ValidationExtensions
    {
        public static void AddToModelState(this ValidationResult result, ModelStateDictionary modelState)
        {
            if (!modelState.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    modelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
            }
        }
    }
}
