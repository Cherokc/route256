using Domain;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandLibrary
{
    public class SalesPredictionCommand : ISalesPredictionCommand
    {
        private readonly IADSCommand _ADSCommand;

        public SalesPredictionCommand(ADSCommand adsCommand)
        {
            _ADSCommand = adsCommand;
        }

        public int GetPrediction(IReadOnlyCollection<Product> products, int days)
        {
            var ads = _ADSCommand.GetADS(products);
            return (int)Math.Round(ads * days);
        }

        public string GetName()
        {
            return "prediction";
        }
    }
}
