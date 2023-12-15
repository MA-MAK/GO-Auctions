using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using AuctionService.Models;
using AuctionService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using System.Diagnostics;

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

        [ActivatorUtilitiesConstructor]
        public AuctionController(
            ILogger<AuctionController> logger,
            IConfiguration configuration,
            IAuctionRepository auctionRepository,
            IItemRepository itemRepository,
            IBidRepository bidRepository
        )
        {
            _logger = logger;
            _auctionRepository = auctionRepository;
            _itemRepository = itemRepository;
            _bidRepository = bidRepository;
        }

        public AuctionController(
            ILogger<AuctionController> logger,
            IConfiguration configuration,
            IAuctionRepository auctionRepository,
            IItemRepository itemRepository
        )
        {
            _logger = logger;
            _auctionRepository = auctionRepository;
            _itemRepository = itemRepository;
        }

        // GET: api/auction
        [HttpGet]
        [Authorize]
        public IActionResult Get()
        {
            return Ok(_auctions);
        }

        // GET: api/auction/all
        [HttpGet("all")]
        [Authorize]
        public IActionResult GetAllAuctions()
        {
            var auctions = _auctionRepository.GetAllAuctions().Result;
            foreach (var auction in auctions)
            {
                auction.Item = _itemRepository.GetItemById(auction.Item.Id).Result;
                auction.Bids = _bidRepository.GetBidsForAuction(auction.Id).Result.ToList();
            }
            return Ok(auctions);
        }

        // GET: api/auction/{id}
        [HttpGet("{id}")]
        [Authorize]
        public IActionResult GetAuctionById(string id)
        {
            Auction auction = _auctionRepository.GetAuctionById(id).Result;
            _logger.LogInformation($"### GetAuctionById: {auction.Title}");
            auction.Item = _itemRepository.GetItemById(auction.Item.Id).Result;
            _logger.LogInformation($"### GetAuctionById: {auction.Item.Title}");
            auction.Bids = _bidRepository.GetBidsForAuction(id).Result.ToList();
            return Ok(auction);
        }

        // POST: api/auction
        [HttpPost]
        [Authorize]
        public Task<IActionResult> PostAuction([FromBody] Auction auction)
        {
            _logger.LogInformation($"PostAuction: {auction.Title}");
            auction.Item = _itemRepository.GetItemById(auction.Item.Id).Result;
            _logger.LogInformation($"PostAuction: {auction.Item.Title}");
            _auctionRepository.PostAuction(auction);
            _logger.LogInformation("posting..");
            return Task.FromResult<IActionResult>(
                CreatedAtAction(nameof(GetAuctionById), new { id = auction.Id }, auction)
            );
        }

        // PUT: api/auction/{id}
        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult> PutAuction(string id, [FromBody] Auction auction)
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
        [Authorize]
        public IActionResult Delete(string id)
        {
            var auction = _auctions.Find(a => a.Id == id);
            if (auction == null)
            {
                return NotFound();
            }
            _auctions.Remove(auction);
            return NoContent();
        }

        [HttpGet("version")]
        [Authorize]
        public async Task<Dictionary<string, string>> GetVersion()
        {
            _logger.LogInformation("posting..");
            var properties = new Dictionary<string, string>();
            var assembly = typeof(Program).Assembly;
            properties.Add("service", "GOAuctions");
            var ver =
                FileVersionInfo.GetVersionInfo(typeof(Program).Assembly.Location).ProductVersion
                ?? "N/A";
            properties.Add("version", ver);
            var hostName = System.Net.Dns.GetHostName();
            var ips = await System.Net.Dns.GetHostAddressesAsync(hostName);
            var ipa = ips.First().MapToIPv4().ToString() ?? "N/A";
            properties.Add("ip-address", ipa);
            return properties;
        }
    }
}
