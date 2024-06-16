namespace HiBidAPI.Models.HiBid
{
    public class HiBidSearch
    {
        public string SearchTerm { get; set; }
        public string ZipCode { get; set; }
        public int Radius { get; set; }
        public bool ShippingOffered { get; set; }
    }
}
