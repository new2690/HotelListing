using FluentValidation;
using HotelListing.Models.DTOs;

namespace HotelListing.Validations
{
    public class HotelValidator:AbstractValidator<CreateHotelDTO>
    {
        public HotelValidator()
        {
            RuleFor(h => h.Name).NotNull()
                .Length(min:1, max: 200)
                .WithMessage("The minimum and maximum length of the property {PropertyName} is {MinLength} and {MaxLength}");

            RuleFor(h => h.Address).NotNull()
                .Length(min:1, max: 300)
                .WithMessage("The minimum and maximum length of the property {PropertyName} is {MinLength} and {MaxLength}");

            RuleFor(h => h.Rating)
                .InclusiveBetween(1.0, 5.0)
                .WithMessage("The range {PropertyName} is between {From}.0 and {To}.0");

            RuleFor(h => h.CountryId).NotNull();
        }
    }
}
