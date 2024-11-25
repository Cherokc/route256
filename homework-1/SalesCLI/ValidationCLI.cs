using System.Globalization;

namespace SalesCLI
{
    internal partial class CLI
    {
        private void ValidateString(string input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            if (input.Length == 0)
                throw new FormatException("Input string must be non empty");
        }

        private int ValidateToInt(string input)
        {
            ValidateString(input);

            if (!int.TryParse(input, out int value))
                throw new FormatException($"Input '{input}' is not in correct format");

            return value;
        }

        private string ValidateToADS(string[] arguments)
        {
            if (arguments.Length != 2)
                throw new FormatException("Expected one argument after command in input line");

            var id = arguments[1];
            ValidateString(id);

            var productHistory = _service.ProductRepository.Get(id);
            return _service.ADSCommand.GetADS(productHistory).ToString("#.00", CultureInfo.InvariantCulture);
        }

        private string ValidateToDemand(string[] arguments)
        {
            if (arguments.Length != 3)
                throw new FormatException("Expected two arguments after command in input line");

            var id = arguments[1];
            ValidateString(id);

            var daysString = arguments[2];
            var daysCount = ValidateToInt(daysString);
            if (daysCount < 0)
                throw new FormatException("Days count must be non negative");

            var productHistory = _service.ProductRepository.Get(id);

            return _service.DemandCommand.GetDemand(productHistory, daysCount).ToString();
        }

        private string ValidateToPrediction(string[] arguments)
        {
            if (arguments.Length != 3)
                throw new FormatException("Expected two arguments after command in input line");

            var id = arguments[1];
            ValidateString(id);

            var daysString = arguments[2];
            var daysCount = ValidateToInt(daysString);
            if (daysCount < 0)
                throw new FormatException("Days count must be non negative");

            var productHistory = _service.ProductRepository.Get(id);

            return _service.SalesPredictionCommand.GetPrediction(productHistory, daysCount).ToString();
        }
    }
}
