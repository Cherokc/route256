using Domain;

namespace Domain.Interfaces
{
    public interface ISalesPredictionCommand : ICommand
    {
        int GetPrediction(IReadOnlyCollection<Product> products, int days);
    }
}
