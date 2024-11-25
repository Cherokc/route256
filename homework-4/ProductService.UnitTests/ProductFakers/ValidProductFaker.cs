﻿using Bogus;
using ProductService.Domain.Dao;

namespace ProductService.UnitTests.ProductFakers;

public class ValidProductFaker : Faker<Product>
{
    public ValidProductFaker()
    {
        RuleFor(p => p.Id, f => Guid.Empty);
        RuleFor(p => p.Name, f => f.Commerce.ProductName());
        RuleFor(p => p.Price, f => f.Random.Double(1, 1000));
        RuleFor(p => p.Weight, f => f.Random.Double(1, 1000));
        RuleFor(p => p.Category, f => f.PickRandom<ProductCategory>());
        RuleFor(p => p.CreationDate, f => f.Date.Past(1));
        RuleFor(p => p.WarehouseId, f => f.Random.Int(1, 1000));
    }
}