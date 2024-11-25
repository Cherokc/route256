using DataAccess.Commands;
using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesCLI
{
    internal class Startup
    {
        public static CLI ConfigureAndGetCLI()
        {
            var adsCommand = new ADSCommand();
            var predictionCommand = new SalesPredictionCommand(adsCommand);
            var demandCommand = new DemandCommand(predictionCommand);

            var dataPath = "../../../../salesData.txt";
            var repository = new ProductRepository(dataPath);

            var service = new SalesService(adsCommand,
                predictionCommand,
                demandCommand,
                repository);

            var console = new CLI(service);

            return console;
        }
    }
}
