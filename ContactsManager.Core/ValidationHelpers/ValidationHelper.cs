using System.ComponentModel.DataAnnotations;

namespace Services.ValidationHelpers
{
    public class ValidationHelper
    {
        internal static void ModelValidator(object obj)
        {
            ValidationContext validationContext = new ValidationContext(obj);
            List<ValidationResult> validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(obj, validationContext, validationResults, true);
            if (!isValid)
            {
                throw new ArgumentException(validationResults.FirstOrDefault()?.ErrorMessage, nameof(obj));
            }
        }
    }
}
