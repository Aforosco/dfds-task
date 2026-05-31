using DFDSVisitManagementAPI.Domain.src.Validation;
using System.ComponentModel.DataAnnotations;

namespace DFDSVisitManagementAPI.Tests
{
    public class ValidationTests
    {
        private ValidationResult? Validate(string value, string memberName)
        {
            var attr = new UpperCaseNoSpacesAttribute();
            var ctx = new ValidationContext(new object())
            {
                DisplayName = memberName
            };
            return attr.GetValidationResult(value, ctx);
        }

        [Fact]
        public void UnitNumber_Lowercase_FailsValidation()
        {
            var result = Validate("dfds12345", "UnitNumber");
            Assert.NotEqual(ValidationResult.Success, result);
        }

        [Fact]
        public void UnitNumber_WithSpaces_FailsValidation()
        {
            var result = Validate("DFDS 12345", "UnitNumber");
            Assert.NotEqual(ValidationResult.Success, result);
        }

        [Fact]
        public void UnitNumber_ValidUpperCase_PassesValidation()
        {
            var result = Validate("DFDS12345", "UnitNumber");
            Assert.Equal(ValidationResult.Success, result);
        }

        [Fact]
        public void LicensePlate_Lowercase_FailsValidation()
        {
            var result = Validate("dk-ab-12345", "TruckLicensePlate");
            Assert.NotEqual(ValidationResult.Success, result);
        }
    }
}