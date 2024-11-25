using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ProductService.Domain.Dao;
using ProductService.WebApi.Controllers.Dao;
using ProductService.WebApi.Controllers.Dao.Extensions;
using ProductService.Domain.Exceptions;
using ProductService.Domain.Repository;

namespace ProductService.WebApi.Controllers;

[ApiController]
[Route("/api/v1/product")]
public class ProductController : ControllerBase
{

    private readonly ILogger<ProductController> _logger;
    private readonly IProductRepository _productRepository;
    private readonly IValidator<ProductDto> _productValidator;
    private readonly IValidator<Filter> _filterValidator;
    private readonly IValidator<UpdatePriceRequest> _updatePriceRequestValidator;

    public ProductController(ILogger<ProductController> logger,
        IProductRepository goodsRepository,
        IValidator<ProductDto> productValidator,
        IValidator<Filter> filterValidator,
        IValidator<UpdatePriceRequest> updatePriceRequestValidator)
    {
        _logger = logger;
        _productRepository = goodsRepository;
        _productValidator = productValidator;
        _filterValidator = filterValidator;
        _updatePriceRequestValidator = updatePriceRequestValidator;
    }

    [HttpPost("create")]
    public IActionResult Create(ProductDto productDto)
    {
        try
        {
            var result = _productValidator.Validate(productDto);
            if (!result.IsValid)
                return BadRequest(string.Join("; \n", result.Errors));

            var product = productDto.ConvertToDomainProduct();

            return Ok(_productRepository.Add(product));
        }
        catch (AlreadyExistsException ex)
        {
            return Conflict("Id is already occupied.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An internal error occurred. Please try again later.");
        }
    }

    [HttpGet("find-by-id")]
    public IActionResult FindById(Guid id)
    {
        try
        {
            var value = _productRepository.Find(id);
            return Ok(value);
        }
        catch(NotFoundException ex)
        {
            return NotFound("No products with the specified ID.");
        }
        catch(Exception ex)
        {
            return StatusCode(500, "An internal error occurred. Please try again later.");
        }
    }

    [HttpGet("select-by")]
    public IActionResult SelectBy([FromQuery] Filter filter)
    {
        try
        {
            var result = _filterValidator.Validate(filter);
            if (!result.IsValid)
                return BadRequest(string.Join("; \n", result.Errors));

            if(filter.Cursor != default(Guid))
                _productRepository.Find(filter.Cursor);

            var value = _productRepository.FindBy(filter.Category, filter.CreationDate, filter.WarehouseId);

            IEnumerable<Product> pageList = new List<Product>();

            if (filter.Cursor != default(Guid))
                pageList = value
                    .OrderBy(x => x.Id)
                    .SkipWhile(x => x.Id != filter.Cursor)
                    .Skip(1)
                    .Take(filter.PageSize);
            else
                pageList = value
                    .OrderBy(x => x.Id)
                    .Take(filter.PageSize);

            var response = new SelectByResponse()
            {
                List = pageList,
                Next = pageList.LastOrDefault()?.Id.ToString()
            };

            return Ok(response);
        }
        catch (NotFoundException ex)
        {
            return StatusCode(404, "No products found with the specified cursor");
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An internal error occurred. Please try again later.");
        }
    }

    [HttpPatch("update-price")]
    public IActionResult UpdatePrice(UpdatePriceRequest request)
    {
        try
        {
            var result = _updatePriceRequestValidator.Validate(request);
            if (!result.IsValid)
                return BadRequest(string.Join("; \n", result.Errors));

            return Ok(_productRepository.UpdatePrice(request.Id, request.NewPrice));
        }
        catch (NotFoundException ex)
        {
            return NotFound("No products with the specified ID.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An internal error occurred. Please try again later.");
        }
    }
}
