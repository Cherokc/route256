using Domain;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandLibrary
{
    public class DemandCommand : IDemandCommand
    {

        private readonly ISalesPredictionCommand _ISalesPredictionCommand;

        public DemandCommand(ISalesPredictionCommand salesPredictionCommand)
        {
            _ISalesPredictionCommand = salesPredictionCommand;
        }

        public int GetDemand(IReadOnlyCollection<Product> products, int days)
        {
            var prediction = _ISalesPredictionCommand.GetPrediction(products, days);
            var availableStock = products.Last().Stock;
            return prediction - availableStock;
        }

        public string GetName()
        {
            return "demand";
        }

        private double GetADDInRecentDays(IReadOnlyCollection<Product> products, int days)
        {
            var productsForTheLastDays = products.Skip(Math.Max(products.Count - days, 0));
            var salesSum = productsForTheLastDays.Sum(p => p.Sales);
            var count = productsForTheLastDays.Count();
            return salesSum / count;
        }
    }
}
