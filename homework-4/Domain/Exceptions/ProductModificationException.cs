namespace ProductService.Domain.Exceptions;

public class ProductModificationException : Exception
{
    public ProductModificationException() { }
    public ProductModificationException(string message) : base(message) { }
    public ProductModificationException(string message, Exception inner) : base(message, inner) { }
}