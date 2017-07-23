using System.Windows.Controls;

namespace wiicapture.gui.Utils
{
    public class RequiredRule : ValidationRule
    {
        public string Message { get; set;}
        
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (string.IsNullOrWhiteSpace((string)value))
            {
                return new ValidationResult(false, this.Message);
            }

            return ValidationResult.ValidResult;
        }
    }
}
