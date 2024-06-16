namespace HiBidAPI.Models.HiBid
{
    public class HiBidAuctionItem
    {
        public string Title { get; set; }
        public string CurrentBid { get; set; }
        public string NumberOfBids { get; set; }
        public string Href { get; set; }
        public string TimeLeftStr { get; set; }
        public TimeSpan TimeLeft { get; set; }
        public DateTime EndTime { get; set; }
    }
}
