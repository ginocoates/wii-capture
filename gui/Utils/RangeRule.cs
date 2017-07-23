using System.Windows.Controls;

namespace wiicapture.gui.Utils
{
    public class RangeRule : ValidationRule
    {
        public string Message { get; set; }

        public int Min { get; set; }
        public int Max { get; set; }

        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (string.IsNullOrEmpty((string)value))
            {
                return new ValidationResult(false, this.Message);
            }

            int parsedValue;

            if (!int.TryParse((string)value, out parsedValue))
            {
                return new ValidationResult(false, this.Message);
            }

            if (parsedValue < Min || parsedValue > Max)
            {
                return new ValidationResult(false, this.Message);
            }

            return ValidationResult.ValidResult;
        }
    }
}
