using System.Globalization;
using System.Windows.Controls;

namespace GraduationProject.Converters
{
    public class InputValidationRule : ValidationRule
    {
        private string _errorMessage;
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { _errorMessage = value; }
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            double convertToDouble;
            if (!double.TryParse((string)value, out convertToDouble))
            {
                return new ValidationResult(false, "Ошибка");
            }
            else
            {
                return new ValidationResult(true, null);
            }
        }
    }
}