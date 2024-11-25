using Domain;

namespace Domain.Interfaces
{
    public interface IADSCommand : ICommand
    {
        double GetADS(IReadOnlyCollection<Product> products);
    }
}
