using CarRentalPortal.Models.DTO_s.Auth;
using FluentValidation;

namespace CarRentalPortal.Validators
{
    public class RegisterValidator : AbstractValidator<UserRegisterDTO>
    {
        public RegisterValidator()
        {
            RuleFor(user => user.FirstName)
                .NotEmpty().WithMessage("სახელის ველი სავალდებულოა.")
                .Length(1, 50).WithMessage("სახელი უნდა შეიცავდეს 1-დან 50 სიმბოლომდე.")
                .Matches(@"^[a-zA-Z]+$").WithMessage("სახელი უნდა შეიცავდეს მხოლოდ ასოებს."); // Optional: use @"^[ა-ჰa-zA-Z]+$" for Georgian too

            RuleFor(user => user.LastName)
                .NotEmpty().WithMessage("გვარის ველი სავალდებულოა.")
                .Length(1, 50).WithMessage("გვარი უნდა შეიცავდეს 1-დან 50 სიმბოლომდე.")
                .Matches(@"^[a-zA-Z]+$").WithMessage("გვარი უნდა შეიცავდეს მხოლოდ ასოებს.");

            RuleFor(user => user.PhoneNumber)
                .NotEmpty().WithMessage("ტელეფონის ნომრის ველი სავალდებულოა.")
                .InclusiveBetween(500000000, 599999999)
                .WithMessage("ტელეფონის ნომერი უნდა იყოს 9 ციფრი და დაიწყოს 5-ით.");

            RuleFor(user => user.Email)
                .NotEmpty().WithMessage("ელ. ფოსტა სავალდებულოა.")
                .Matches(@"^[^@\s]+@[^@\s]+\.[a-zA-Z]{2,}$")
                    .WithMessage("გთხოვთ შეიყვანოთ ვალიდური ელ. ფოსტა.")
                .MaximumLength(200).WithMessage("ელ. ფოსტა არ უნდა აღემატებოდეს 200 სიმბოლოს.")
                .Must(HaveValidDomain)
                    .WithMessage("გთხოვთ შეამოწმოთ ელ. ფოსტის დომენი — შესაძლოა, აკრეფის შეცდომა გაქვთ.");

            RuleFor(user => user.Password)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("პაროლი სავალდებულოა.")
                .Length(8, 50).WithMessage("პაროლი უნდა იყოს 8-დან 50 სიმბოლომდე.")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$")
                .WithMessage("პაროლი უნდა შეიცავდეს ერთ დიდ ასოს, ერთ პატარა ასოს, ერთ ციფრს და ერთ სპეციალურ სიმბოლოს.");
        }


        private bool HaveValidDomain(string email)
        {
            var suspiciousDomains = new List<string>
            {
            "gml.com", "gmal.com", "gnail.com", "hotmial.com", "yaho.com",
            "gmail.con", "gmail.co", "gmail.c", "gmail.cm", "outlok.com", "outllook.com",
            "yahoo.con", "hotmail.c", "example.com", "test.com"
             };

            try
            {
                var domain = email.Split('@')[1].ToLower();
                return !suspiciousDomains.Contains(domain);
            }
            catch
            {
                return false;
            }
        }
    }
}
