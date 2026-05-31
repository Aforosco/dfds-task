using System.ComponentModel.DataAnnotations;

namespace DFDSVisitManagementAPI.Domain.src.Validation
{
    public class UpperCaseNoSpacesAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(
            object? value, ValidationContext validationContext)
        {
            if (value is string str)
            {
                if (str.Contains(' '))
                    return new ValidationResult(
                        $"{validationContext.DisplayName} must not contain whitespace.");

                if (str != str.ToUpper())
                    return new ValidationResult(
                        $"{validationContext.DisplayName} must be uppercase.");
            }

            return ValidationResult.Success;
        }
    }
}