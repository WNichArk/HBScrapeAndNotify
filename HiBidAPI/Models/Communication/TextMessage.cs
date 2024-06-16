namespace HiBidAPI.Models.Communication
{
    public class TextMessage : ICommMessage
    {
        public string Message { get; set; }
        public DateTime TimeSent { get; set; }
        public string PhoneNumber { get; set; }

    }
}
