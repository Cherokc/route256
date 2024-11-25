namespace ProductService.Domain.Exceptions;

public class ProductIdModificationException : Exception
{
    public ProductIdModificationException() { }
    public ProductIdModificationException(string message) : base(message) { }
    public ProductIdModificationException(string message, Exception inner) : base(message, inner) { }
}