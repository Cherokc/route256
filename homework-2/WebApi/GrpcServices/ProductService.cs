using Grpc.Core;
using ProductGrpcService;
using Domain.Repository;
using ProductService.WebApi.Mappers;
using FluentValidation;
using FluentValidation.Results;
using ProductService.WebApi.Exceptions;

namespace ProductService.WebApi.GrpcServices;

public class ProductService : ProductGrpcService.ProductService.ProductServiceBase
{
    private readonly IProductRepository _productRepository;
    private readonly IValidator<CreateProductRequest> _createProductRequestValidator;
    private readonly IValidator<GetProductsRequest> _getProductsRequestValidator;
    private readonly IValidator<GetProductByIdRequest> _getProductByIdRequestValidator;
    private readonly IValidator<UpdateProductPriceRequest> _updateProductPriceRequestValidator;

    public ProductService(IProductRepository productRepository,
        IValidator<CreateProductRequest> createProductRequestValidator,
        IValidator<GetProductsRequest> getProductsRequestValidator,
        IValidator<GetProductByIdRequest> getProductByIdRequestValidator,
        IValidator<UpdateProductPriceRequest> updateProductPriceRequestValidator)
    {
        _productRepository = productRepository;
        _createProductRequestValidator = createProductRequestValidator;
        _getProductsRequestValidator = getProductsRequestValidator;
        _getProductByIdRequestValidator = getProductByIdRequestValidator;
        _updateProductPriceRequestValidator = updateProductPriceRequestValidator;
    }

    public override Task<CreateProductResponse> CreateProduct(CreateProductRequest request, ServerCallContext context)
    {
        var result = _createProductRequestValidator.Validate(request);
        if(!result.IsValid)
            ThrowRequestException(result);

        var newProduct = ProductMapper.ChangeToDomainProduct(request);
        var id = _productRepository.Add(newProduct);
        return Task.FromResult(
            new CreateProductResponse() 
            { 
                Id = id.ToString()
            });
    }

    public override Task<GetProductsResponse> GetProducts(GetProductsRequest request, ServerCallContext context)
    {
        var result = _getProductsRequestValidator.Validate(request);
        if (!result.IsValid)
            ThrowRequestException(result);

        var products = _productRepository.FindBy(
                ProductMapper.ChangeToDomainCategory(request.Category),
                request.CreationDate != null ? request.CreationDate.ToDateTime() : DateTime.MinValue,
                request.WarehouseId).Skip(0);

        if (request.Cursor != "")
        {
            products = products
                .OrderBy(x => x.Id)
                .SkipWhile(x => x.Id != Guid.Parse(request.Cursor))
                .Skip(1)
                .Take(request.PageSize);
        }
        else
        {
            products = products
                .OrderBy(x => x.Id)
                .Take(request.PageSize);
        }

        var response = new GetProductsResponse()
            {
                NextCursor = products.LastOrDefault()?.Id.ToString() ?? ""
            };

        foreach (var product in products)
            response.Products.Add(ProductMapper.ChangeToProtoProduct(product));

        return Task.FromResult(response);
    }

    public override Task<GetProductByIdResponse> GetProductById(GetProductByIdRequest request, ServerCallContext context)
    {
        var result = _getProductByIdRequestValidator.Validate(request);
        if (!result.IsValid)
            ThrowRequestException(result);

        var product = _productRepository.Find(Guid.Parse(request.Id));
        var response = new GetProductByIdResponse() { Product = ProductMapper.ChangeToProtoProduct(product) };
        return Task.FromResult(response);
    }

    public override Task<UpdateProductPriceResponse> UpdateProductPrice(UpdateProductPriceRequest request, ServerCallContext context)
    {
        var result = _updateProductPriceRequestValidator.Validate(request);
        if (!result.IsValid)
            ThrowRequestException(result);

        var productId = _productRepository.UpdatePrice(Guid.Parse(request.Id), request.NewPrice);
        var response = new UpdateProductPriceResponse() { Id = productId.ToString() };
        return Task.FromResult(response);
    }

    private void ThrowRequestException(ValidationResult result)
    {
        var metadata = string.Join("; \n", result.Errors);

        throw new BadRequestException(metadata);
    }
}