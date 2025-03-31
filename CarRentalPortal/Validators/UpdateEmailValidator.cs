using CarRentalPortal.Models.DTO_s.User;
using FluentValidation;

namespace CarRentalPortal.Validators
{
    public class UpdateEmailValidator : AbstractValidator<UpdateUserEmailDTO>
    {
        public UpdateEmailValidator()
        {
            RuleFor(user => user.NewEmail)
                .NotEmpty().WithMessage("ელ. ფოსტა სავალდებულოა.")
                .Matches(@"^[^@\s]+@[^@\s]+\.[a-zA-Z]{2,}$")
                    .WithMessage("გთხოვთ შეიყვანოთ ვალიდური ელ. ფოსტა.")
                .MaximumLength(200).WithMessage("ელ. ფოსტა არ უნდა აღემატებოდეს 200 სიმბოლოს.")
                .Must(HaveValidDomain)
                    .WithMessage("გთხოვთ შეამოწმოთ ელ. ფოსტის დომენი — შესაძლოა, აკრეფის შეცდომა გაქვთ.");
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
