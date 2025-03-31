using CarRentalPortal.Models.DTO_s.User;
using FluentValidation;

namespace CarRentalPortal.Validators
{
    public class UpdateUserPhoneNumberValidator : AbstractValidator<UpdateUserPhoneNumberDTO>
    {
        public UpdateUserPhoneNumberValidator()
        {
            RuleFor(user => user.NewPhoneNumber)
                .NotEmpty().WithMessage("ტელეფონის ნომრის ველი სავალდებულოა.")
                .InclusiveBetween(500000000, 599999999).WithMessage("ტელეფონის ნომერი უნდა იყოს 9 ციფრი და დაიწყოს 5-ით.");
        }
    }
}
