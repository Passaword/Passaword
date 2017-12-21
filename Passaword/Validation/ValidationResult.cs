using System;
using System.Collections.Generic;
using System.Text;

namespace Passaword.Validation
{
    public class ValidationResult
    {
        public ValidationResult(bool isValid)
        {
            IsValid = isValid;
        }

        public bool IsValid { get; set; }
        public string ValidationPointOfFailure { get; set; }
        public string Error { get; set; }

        public static ValidationResult SuccessResult = new ValidationResult(true);
    }
}
