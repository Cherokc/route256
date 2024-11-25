using Domain;
using Domain.Interfaces;

namespace DataAccess.Commands
{
    public class ADSCommand : IADSCommand
    {

        public double GetADS(IReadOnlyCollection<Product> products)
        {
            var speciesProducts = products.Where(p => p.Sales + p.Stock > 0).ToArray();
            var salesSum = speciesProducts.Sum(p => p.Sales);
            var countDays = speciesProducts.Count();
            var ads = countDays == 0 ? 1 : (double)salesSum / countDays;
            if (ads < 0)
                throw new ArgumentException("Incorrect input data");
            return ads;
        }

        public string GetName()
        {
            return "ads";
        }
    }
}
