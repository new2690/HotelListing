using FluentValidation;
using HotelListing.Models.DTOs;

namespace HotelListing.Validations
{
    public class CountryValidator: AbstractValidator<CreateCountryDTO>
    {
        public CountryValidator()
        {
            RuleFor(h => h.Name).NotNull()
                .Length(min: 1, max: 50)
                .WithMessage("The minimum and maximum length of the property {PropertyName} is {MinLength} and {MaxLength}");

            RuleFor(h => h.ShortName).NotNull()
                .Length(min: 1, max: 3)
                .WithMessage("The minimum and maximum length of the property {PropertyName} is {MinLength} and {MaxLength}");


        }
    }
}
