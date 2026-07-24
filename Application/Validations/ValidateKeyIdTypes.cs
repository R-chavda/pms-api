using System.ComponentModel.DataAnnotations;

namespace Application.Validations
{
    public class ValidateKeyIdTypes : ValidationAttribute
    {
        private readonly bool _isRequired;
        private readonly long _minVal;

        public ValidateKeyIdTypes(bool isRequired = true, long minVal = 1)
        {
            _isRequired = isRequired;
            _minVal = minVal;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return _isRequired
                    ? new ValidationResult($"{validationContext.DisplayName} is required")
                    : ValidationResult.Success;
            }

            long parsedValue;
            if (value is long longValue)
            {
                parsedValue = longValue;
            }
            else if (value is string strValue)
            {
                if (string.IsNullOrWhiteSpace(strValue))
                {
                    return _isRequired
                        ? new ValidationResult($"{validationContext.DisplayName} is required")
                        : ValidationResult.Success;
                }

                if (!long.TryParse(strValue, out parsedValue))
                {
                    return new ValidationResult($"{validationContext.DisplayName} must be a valid numeric value.");
                }
            }
            else
            {
                return new ValidationResult($"{validationContext.DisplayName} must be a number or string.");
            }

            if (_isRequired && parsedValue < _minVal)
            {
                return new ValidationResult($"{validationContext.DisplayName} must be at least {_minVal}.");
            }

            return ValidationResult.Success;
        }
    }
}
