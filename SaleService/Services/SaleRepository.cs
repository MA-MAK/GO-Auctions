using System.Collections.Generic;
using SaleService.Models;

namespace SaleService.Services
{
    public class SaleRepository : ISaleRepository
    {
        private List<Sale> sales;

        public SaleRepository()
        {
            sales = new List<Sale>();
        }

        public Task<Sale> GetSaleForAuction(int auctionId)
        {
            return Task.FromResult<Sale>(sales.Where(b => b.AuctionId == auctionId).FirstOrDefault());
        }

    }
}