using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using SaleService.Models;
using SaleService.Services;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.Extensions.Configuration;

namespace SaleService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SaleController : ControllerBase
    {
        private readonly ISaleRepository _saleRepository;

        private readonly ILogger<SaleController> _logger;
        private readonly IConfiguration _configuration;

        public SaleController(
            ISaleRepository saleRepository,
            ILogger<SaleController> logger,
            IConfiguration configuration
        )
        {
            _saleRepository = saleRepository;
            _logger = logger;
            _configuration = configuration;
        }

        [HttpGet("{id}")]
        public IActionResult GetSaleById(string id)
        {
            Sale sale = _saleRepository.GetSaleById(id).Result;
            sale.Item = _itemRepository.GetItemById(sale.Item.Id).Result;
            return Ok(auction);
        }
        /*
        [HttpPost]
        public async Task<IActionResult> PostSaleForItem(string itemId, Sale sale)
        {
            try
            {
                // Hent varen fra itemId
                var item = await _saleRepository.GetItemById(itemId);

                if (item == null)
                {
                    return NotFound("Item not found");
                }

                // Tilknyt varenummeret til salget
                sale.ItemId = itemId;

                // Opret salget
                var createdSale = await _saleRepository.CreateSale(sale);

                return CreatedAtAction(nameof(GetSaleForItem), new { id = createdSale.Id }, createdSale);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create sale for item");
                return StatusCode(500, "Internal server error");
            }
        }
     */
    }
}