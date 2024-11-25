using DataAccess.Exceptions;
using Domain;
using Domain.Interfaces;

namespace DataAccess.Commands
{
    public class DemandCommand : IDemandCommand
    {

        private readonly ISalesPredictionCommand _salesPredictionCommand;

        public DemandCommand(ISalesPredictionCommand salesPredictionCommand)
        {
            _salesPredictionCommand = salesPredictionCommand;
        }

        public int GetDemand(IReadOnlyCollection<Product> products, int days)
        {
            var prediction = _salesPredictionCommand.GetPrediction(products, days);
            if (prediction < 0)
                throw new InvalidCommandCalculationException(_salesPredictionCommand, "Must be non-negative");
            var availableStock = products.Last().Stock;
            return prediction - availableStock;
        }

        public string GetName()
        {
            return "demand";
        }
    }
}
