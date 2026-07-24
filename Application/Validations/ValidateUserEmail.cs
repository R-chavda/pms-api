using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Application.Validations
{
    public class ValidateUserEmail : ValidationAttribute
    {
        private readonly bool _isRequired;
        // This regex requires: non-empty username, @ symbol, valid domain (with at least one dot), proper TLD (like .com, .net, etc.)
        private static readonly Regex StrictEmailRegex = new(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public ValidateUserEmail(bool isRequired = true)
        {
            _isRequired = isRequired;
        }
        
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var email = value?.ToString()?.Trim();

            if (_isRequired && string.IsNullOrEmpty(email))
            {
                return new ValidationResult("Email is required");
            }

            if (_isRequired && !StrictEmailRegex.IsMatch(email!))
                return new ValidationResult("The email address is not valid.");

            return ValidationResult.Success;
        }
    }
}
