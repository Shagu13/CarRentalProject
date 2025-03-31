using CarRentalPortal.Models.DTO_s.Admin;
using CarRentalPortal.Models.DTO_s.Auth;
using FluentValidation;

namespace CarRentalPortal.Validators
{
    public class AdminUserPasswordChangeValidator : AbstractValidator<ChangePasswordDTO>
    {
        public AdminUserPasswordChangeValidator()
        {

            RuleFor(user => user.NewPassword)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("პაროლი სავალდებულოა.")
                .Length(8, 50).WithMessage("პაროლი უნდა იყოს 8-დან 50 სიმბოლომდე.")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$")
                .WithMessage("პაროლი უნდა შეიცავდეს ერთ დიდ ასოს, ერთ პატარა ასოს, ერთ ციფრს და ერთ სპეციალურ სიმბოლოს.");
        }
    }
}
