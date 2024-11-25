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
            CreationDate = Timestamp.FromDateTime(product.CreationDate.ToUniversalTime()),
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
            Domain.Dao.ProductCategory.HouseholdChemical => ProductGrpcService.ProductCategory.ProductHouseholdChemical,
            Domain.Dao.ProductCategory.Appliance => ProductGrpcService.ProductCategory.ProductAppliance,
            Domain.Dao.ProductCategory.Food => ProductGrpcService.ProductCategory.ProductFood,
            Domain.Dao.ProductCategory.General => ProductGrpcService.ProductCategory.ProductGeneral,
            _ => ProductGrpcService.ProductCategory.ProductUnspecified
        };
    }

    public static Domain.Dao.ProductCategory ChangeToDomainCategory(ProductGrpcService.ProductCategory category)
    {
        return category switch
        {
            ProductGrpcService.ProductCategory.ProductHouseholdChemical => Domain.Dao.ProductCategory.HouseholdChemical,
            ProductGrpcService.ProductCategory.ProductAppliance => Domain.Dao.ProductCategory.Appliance,
            ProductGrpcService.ProductCategory.ProductFood => Domain.Dao.ProductCategory.Food,
            ProductGrpcService.ProductCategory.ProductGeneral => Domain.Dao.ProductCategory.General,
            _ => Domain.Dao.ProductCategory.Unspecified
        };
    }
}

