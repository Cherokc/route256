using Domain;

namespace Domain.Interfaces
{
    public interface IDemandCommand : ICommand
    {
        int GetDemand(IReadOnlyCollection<Product> products, int days);
    }
}
