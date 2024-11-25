using DataAccess.Exceptions;
using Domain;
using Domain.Interfaces;

namespace DataAccess.Commands
{
    public class SalesPredictionCommand : ISalesPredictionCommand
    {
        private readonly IADSCommand _ADSCommand;

        public SalesPredictionCommand(IADSCommand adsCommand)
        {
            _ADSCommand = adsCommand;
        }

        public int GetPrediction(IReadOnlyCollection<Product> products, int days)
        {
            var ads = _ADSCommand.GetADS(products);
            if (ads < 0)
                throw new InvalidCommandCalculationException(_ADSCommand, "Must be non-negative");
            return (int)Math.Round(ads * days);
        }

        public string GetName()
        {
            return "prediction";
        }
    }
}
