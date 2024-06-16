using HiBidAPI.Models.HiBid;
using LiteDB;

namespace HiBidAPI.Models.Repositories
{
    public interface IHiBidAuctionItemRepository
    {
        List<HiBidAuctionItem> GetAllHiBidAuctionItems();
        HiBidAuctionItem GetHiBidAuctionItemById(int id);
        void InsertHiBidAuctionItem(HiBidAuctionItem auctionItem);
        void UpdateHiBidAuctionItem(HiBidAuctionItem auctionItem);
        void DeleteHiBidAuctionItem(int id);
    }

    public class HiBidAuctionItemRepository : IHiBidAuctionItemRepository
    {
        private ILiteDatabase _db;

        public HiBidAuctionItemRepository(ILiteDatabase db)
        {
            _db = db;
        }

        public List<HiBidAuctionItem> GetAllHiBidAuctionItems()
        {
            var auctionItemsCollection = _db.GetCollection<HiBidAuctionItem>("auctionItems");
            return auctionItemsCollection.FindAll().ToList();
        }

        public HiBidAuctionItem GetHiBidAuctionItemById(int id)
        {
            var auctionItemsCollection = _db.GetCollection<HiBidAuctionItem>("auctionItems");
            return auctionItemsCollection.FindById(id);
        }

        public void InsertHiBidAuctionItem(HiBidAuctionItem auctionItem)
        {
            var auctionItemsCollection = _db.GetCollection<HiBidAuctionItem>("auctionItems");
            auctionItemsCollection.Insert(auctionItem);
        }

        public void UpdateHiBidAuctionItem(HiBidAuctionItem auctionItem)
        {
            var auctionItemsCollection = _db.GetCollection<HiBidAuctionItem>("auctionItems");
            auctionItemsCollection.Update(auctionItem);
        }

        public void DeleteHiBidAuctionItem(int id)
        {
            var auctionItemsCollection = _db.GetCollection<HiBidAuctionItem>("auctionItems");
            auctionItemsCollection.Delete(id);
        }
    }
}
