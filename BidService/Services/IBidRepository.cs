using BidService.Models;

namespace BidService.Services;
public interface IBidRepository
{
    //Task<Bid> GetBidById(int bidId);
    //Task<IEnumerable<Bid>> GetAllBids();
    Task<IEnumerable<Bid>> GetBidsForAuction(int auctionId);
    //Task<IEnumerable<Bid>> GetBidsByUserId(int userId);
    //Task AddBid(Bid bid);
    //Task UpdateBid(Bid bid);
    //Task DeleteBid(int bidId);
}