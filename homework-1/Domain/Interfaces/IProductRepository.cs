namespace Domain.Interfaces
{
    public interface IProductRepository
    {
        IReadOnlyCollection<Product> Get(string id);
        IReadOnlyCollection<Product> GetAll();
    }
}
