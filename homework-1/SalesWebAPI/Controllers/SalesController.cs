using DataAccess.Exceptions;
using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;

namespace SalesWebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SalesController : ControllerBase
    {
        private readonly ILogger<SalesController> _logger;
        private readonly ISalesService _service;

        public SalesController(ILogger<SalesController> logger, ISalesService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpGet("ads")]
        public IActionResult GetADS([FromHeader] string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    _logger.LogWarning("Invalid ID provided for GetADS.");
                    throw new ArgumentNullException("id");
                }

                var productHistory = _service.ProductRepository.Get(id);
                if (productHistory == null || productHistory.Count == 0)
                {
                    _logger.LogWarning($"Product history not found for ID: {id}");
                    throw new ProductNotFoundException(id);
                }

                var ads = _service.ADSCommand.GetADS(productHistory);
                return Ok(ads);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ProductNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting ADS.");
                return StatusCode(500, "An internal error occurred. Please try again later.");
            }
        }

        [HttpGet("prediction")]
        public IActionResult GetPrediction([FromQuery] string id, int days)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    _logger.LogWarning("Invalid ID provided for GetPrediction.");
                    throw new ArgumentNullException("id");
                }

                if (days <= 0)
                {
                    _logger.LogWarning("Invalid number of days provided for GetPrediction.");
                    throw new InvalidCountOfDaysException();
                }

                var productHistory = _service.ProductRepository.Get(id);
                if (productHistory == null || productHistory.Count == 0)
                {
                    _logger.LogWarning($"Product history not found for ID: {id}");
                    throw new ProductNotFoundException(id);
                }

                var prediction = _service.SalesPredictionCommand.GetPrediction(productHistory, days);
                return Ok(prediction);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidCountOfDaysException ex)
            {
                return BadRequest("Count of days must be greater than zero.");
            }
            catch (ProductNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting prediction.");
                return StatusCode(500, "An internal error occurred. Please try again later.");
            }
        }

        [HttpGet("demand")]
        public IActionResult GetDemand([FromQuery] string id, int days)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    _logger.LogWarning("Invalid ID provided for GetDemand.");
                    throw new ArgumentNullException("id");
                }

                if (days <= 0)
                {
                    _logger.LogWarning("Invalid number of days provided for GetDemand.");
                    throw new InvalidCountOfDaysException();
                }

                var productHistory = _service.ProductRepository.Get(id);
                if (productHistory == null || productHistory.Count == 0)
                {
                    _logger.LogWarning($"Product history not found for ID: {id}");
                    throw new ProductNotFoundException(id);
                }

                var demand = _service.DemandCommand.GetDemand(productHistory, days);
                return Ok(demand);
            }
            catch(ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidCountOfDaysException ex)
            {
                return BadRequest("Count of days must be greater than zero.");
            }
            catch(ProductNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting demand.");
                return StatusCode(500, "An internal error occurred. Please try again later.");
            }
        }
    }
}