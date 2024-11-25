using Domain.Interfaces;

namespace DataAccess
{
    public class SalesService : ISalesService
    {
        public SalesService(IADSCommand aDSCommand, ISalesPredictionCommand salesPredictionCommand, IDemandCommand demandComand, IProductRepository productRepository)
        {
            ADSCommand = aDSCommand;
            SalesPredictionCommand = salesPredictionCommand;
            DemandCommand = demandComand;
            ProductRepository = productRepository; 
        }

        public IADSCommand ADSCommand { get; }
        public ISalesPredictionCommand SalesPredictionCommand { get; }
        public IDemandCommand DemandCommand { get; }

        public IReadOnlyCollection<string> AvailableCommands
        {
            get => new List<string>()
            {
                ADSCommand.GetName(),
                DemandCommand.GetName(),
                SalesPredictionCommand.GetName()
            }.AsReadOnly();
        }

        public IProductRepository ProductRepository { get; }
    }
}
