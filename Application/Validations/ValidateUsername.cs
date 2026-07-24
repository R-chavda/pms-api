using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Application.Validations
{
    public class ValidateUsername : ValidationAttribute
    {
        private readonly bool _isRequired;
        public ValidateUsername(bool isRequired = true)
        {
            _isRequired = isRequired;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var username = value?.ToString()?.Trim();

            if (_isRequired && string.IsNullOrEmpty(username))
            {
                return new ValidationResult("Username is required");
            }

            // Skip further checks if optional and empty
            if (!_isRequired && string.IsNullOrWhiteSpace(username))
            {
                return ValidationResult.Success;
            }

            if (username!.Length < 6)
            {
                return new ValidationResult("Username must be at least 6 characters long.");
            }

            if (!Regex.IsMatch(username, @"^[a-zA-Z][a-zA-Z0-9_.]+$"))
            {
                return new ValidationResult("Username can only contain letters, digits, underscores, dots and must start with a character");
            }
            return ValidationResult.Success;
        }
    }
}
