namespace Domain.Interfaces
{
    public interface ISalesService
    {
        IADSCommand ADSCommand { get; }
        ISalesPredictionCommand SalesPredictionCommand { get; }
        IDemandCommand DemandCommand { get; }
        IReadOnlyCollection<string> AvailableCommands { get; }
        IProductRepository ProductRepository { get; }
    }
}
