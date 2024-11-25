using Google.Protobuf.WellKnownTypes;
using ProductGrpcService;

namespace ProductService.WebApi.Mappers;

public static class ProductMapper
{
    public static ProductGrpcService.Product ChangeToProtoProduct(Domain.Dao.Product product)
    {
        if (product == null)
            return null;

        return new ProductGrpcService.Product()
        {
            Id = product.Id.ToString(),
            Name = product.Name,
            Price = product.Price,
            Weight = product.Weight,
            Category = ChangeToProtoCategory(product.Category),
            CreationDate = Timestamp.FromDateTime(product.CreationDate),
            WarehouseId = product.WarehouseId
        };
    }

    public static Domain.Dao.Product ChangeToDomainProduct(ProductGrpcService.Product protoProduct)
    {
        if (protoProduct == null)
            return null;

        return new Domain.Dao.Product(
            Guid.Parse(protoProduct.Id),
            protoProduct.Name,
            protoProduct.Price,
            protoProduct.Weight,
            ChangeToDomainCategory(protoProduct.Category),
            protoProduct.CreationDate.ToDateTime(),
            protoProduct.WarehouseId);
    }
    public static Domain.Dao.Product ChangeToDomainProduct(CreateProductRequest request)
    {
        if (request == null)
            return null;

        return new Domain.Dao.Product(
            request.Name,
            request.Price,
            request.Weight,
            ChangeToDomainCategory(request.Category),
            request.CreationDate != null ? request.CreationDate.ToDateTime() : DateTime.MinValue,
            request.WarehouseId);
    }

    public static ProductGrpcService.ProductCategory ChangeToProtoCategory(Domain.Dao.ProductCategory category)
    {
        return category switch
        {
            Domain.Dao.ProductCategory.HouseholdChemical => ProductGrpcService.ProductCategory.HouseholdChemical,
            Domain.Dao.ProductCategory.Appliance => ProductGrpcService.ProductCategory.Appliance,
            Domain.Dao.ProductCategory.Food => ProductGrpcService.ProductCategory.Food,
            Domain.Dao.ProductCategory.General => ProductGrpcService.ProductCategory.General,
            _ => ProductGrpcService.ProductCategory.Unspecified
        };
    }

    public static Domain.Dao.ProductCategory ChangeToDomainCategory(ProductGrpcService.ProductCategory category)
    {
        return category switch
        {
            ProductGrpcService.ProductCategory.HouseholdChemical => Domain.Dao.ProductCategory.HouseholdChemical,
            ProductGrpcService.ProductCategory.Appliance => Domain.Dao.ProductCategory.Appliance,
            ProductGrpcService.ProductCategory.Food => Domain.Dao.ProductCategory.Food,
            ProductGrpcService.ProductCategory.General => Domain.Dao.ProductCategory.General,
            _ => Domain.Dao.ProductCategory.Unspecified
        };
    }
}

