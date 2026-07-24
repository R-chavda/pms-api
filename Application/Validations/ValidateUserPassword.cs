using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Application.Validations
{
    public class ValidateUserPassword : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var password = value != null ? value.ToString() : string.Empty;

            if (string.IsNullOrEmpty(password))
            {
                return new ValidationResult("Password field cannot be empty");
            }

            if (password.Length < 6)
            {
                return new ValidationResult("The Password must be of atleast 6 characters");
            }

            if (!Regex.IsMatch(password, @"\d") || !Regex.IsMatch(password, @"[\W_]"))
            {
                return new ValidationResult("The password must contain at least one digit and one special character.");
            }

            return ValidationResult.Success;
        }
    }
}
