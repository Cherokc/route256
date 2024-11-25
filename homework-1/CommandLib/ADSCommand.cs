using Domain;
using Domain.Interfaces;

namespace CommandLibrary
{
    public class ADSCommand : IADSCommand
    {

        public double GetADS(IReadOnlyCollection<Product> products)
        {
            var speciesProducts = products.Where(p => p.Sales > 0).ToArray();
            var salesSum = speciesProducts.Sum(p => p.Sales);
            var countDays = speciesProducts.Count();
            return (double)salesSum / countDays;
        }

        public string GetName()
        {
            return "ads";
        }
    }
}
