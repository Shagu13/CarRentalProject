using CarRentalPortal.Models.DTO_s.Auth;
using FluentValidation;

namespace CarRentalPortal.Validators
{
    public class LoginValidator : AbstractValidator<UserLoginDTO>
    {
        public LoginValidator()
        {
            RuleFor(user => user.PhoneNumber)
                .NotEmpty().WithMessage("ტელეფონის ნომერი სავალდებულოა.")
                .InclusiveBetween(500000000, 599999999).WithMessage("ტელეფონის ნომერი უნდა იყოს 9 ციფრი და დაიწყოს 5-ით.");

            RuleFor(user => user.Password)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("პაროლი სავალდებულოა.")
                .Length(8, 50).WithMessage("პაროლი უნდა იყოს 8-დან 50 სიმბოლომდე.")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$")
                .WithMessage("პაროლი უნდა შეიცავდეს ერთ დიდ ასოს, ერთ პატარა ასოს, ერთ ციფრს და ერთ სპეციალურ სიმბოლოს.");
        }
    }
}
