using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using AuctionService.Models;
using AuctionService.Services;

namespace AuctionService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuctionController : ControllerBase
    {
        private readonly List<Auction> _auctions;
        private readonly ILogger<AuctionController> _logger;
        private readonly IAuctionRepository _auctionRepository;
        private readonly IBidRepository _bidRepository;
        private readonly IItemRepository _itemRepository;


        public AuctionController()
        {
            _auctions = new List<Auction>();
        }

        public AuctionController(ILogger<AuctionController> logger, IConfiguration configuration, IAuctionRepository auctionRepository, IItemRepository itemRepository, IBidRepository bidRepository)
        {
            _logger = logger;
            _auctionRepository = auctionRepository;
            _itemRepository = itemRepository;
            _bidRepository = bidRepository;
        }

        public AuctionController(ILogger<AuctionController> logger, IConfiguration configuration, IAuctionRepository auctionRepository, IItemRepository itemRepository)
        {
            _logger = logger;
            _auctionRepository = auctionRepository;
            _itemRepository = itemRepository;
        }

        // GET: api/auction
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_auctions);
        }

        // GET: api/auction/{id}
        [HttpGet("{id}")]
        public IActionResult GetAuctionById(int id)
        {
            Auction auction = _auctionRepository.GetAuctionById(id).Result;
            auction.Item = _itemRepository.GetItemById(auction.Item.Id).Result;
            auction.Bids = _bidRepository.GetBidsForAuction(auction.Id).Result.ToList();
            return Ok(auction);
        }

        // POST: api/auction
        [HttpPost]
        public async Task<ActionResult> PostAuction([FromBody] Auction auction)
        {
            auction.Item = _itemRepository.GetItemById(auction.Item.Id).Result;
            await _auctionRepository.PostAuction(auction);
            _logger.LogInformation("posting..");
            return CreatedAtAction(nameof(GetAuctionById), new { id = auction.Id }, auction);
        }

        // PUT: api/auction/{id}

        // PUT: api/auction/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> PutAuction(int id, [FromBody] Auction auction)
        {
            var existingAuction = await _auctionRepository.GetAuctionById(id);
            if (existingAuction == null)
            {
                return NotFound();
            }

            existingAuction.Title = auction.Title;
            existingAuction.Description = auction.Description;

            // Opdater auktionen, hvis metoden ikke returnerer noget (void)
            await _auctionRepository.UpdateAuction(existingAuction);
            return NoContent(); // Returnerer NoContent uanset hvad

            //return NoContent(); // Returnerer NoContent uanset hvad
        }


        /*
                [HttpPut("{id}")]
                public IActionResult PutAuction(int id, [FromBody] Auction auction)
                {
                    var existingAuction = _auctions.Find(a => a.Id == id);
                    if (existingAuction == null)
                    {
                        return NotFound();
                    }
                    existingAuction.Title = auction.Title;
                    existingAuction.Description = auction.Description;
                    return NoContent();
                }
                */


        // DELETE: api/auction/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var auction = _auctions.Find(a => a.Id == id);
            if (auction == null)
            {
                return NotFound();
            }
            _auctions.Remove(auction);
            return NoContent();
        }
    }
}