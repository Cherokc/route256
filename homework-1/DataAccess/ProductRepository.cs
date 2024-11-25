using DataAccess.Exceptions;
using Domain;
using Domain.Interfaces;

namespace DataAccess
{
    public class ProductRepository : IProductRepository
    {
        private readonly IReadOnlyCollection<Product> _products;

        public ProductRepository(string dataPath)
        {
            _products = ReadData(dataPath);
        }

        public IReadOnlyCollection<Product> Get(string id)
        {
            var productHistory = _products.Where(p => p.Id == id).OrderBy(p => p.Date).ToList();
            if (productHistory.Count == 0)
                throw new ProductNotFoundException(id);
            return productHistory;
        }

        public IReadOnlyCollection<Product> GetAll()
        {
            if (_products.Count == 0)
                throw new ProductNotFoundException();
            return _products;
        }

        private IReadOnlyCollection<Product> ReadData(string dataPath)
        {
            if(!File.Exists(dataPath))
                throw new FileNotFoundException(dataPath);

            var fileContent = File.ReadAllText(dataPath).Split('\n');

            if(fileContent.Length <= 1)
                throw new FileLoadException($"File {dataPath} must be not empty");

            var dataList = new List<Product>(fileContent.Length);

            for(int i = 1; i < fileContent.Length; i++)
            {
                try
                {
                    var product = ValidateProduct(fileContent[i]);
                    dataList.Add(product);
                }
                catch (Exception e)
                {
                    throw new FileLoadException($"File {dataPath}: line {i + 1} is in incorrect format");
                }
            }

            return dataList;
        }

        private Product ValidateProduct(string fileContent)
        {
            var line = fileContent.Split(", ");
            if (line.Length != 4)
                throw new FormatException("Input string was not in a correct format.");

            var id = line[0];
            var date = DateTime.Parse(line[1]);

            if (!int.TryParse(line[2], out int sales) || !int.TryParse(line[2], out int stock))
                throw new FormatException("Input string was not in a correct format.");

            if(sales < 0 ||  stock < 0)
                throw new FormatException("Input string was not in a correct format.");

            return new Product(id, date, sales, stock);
        }
    }
}
