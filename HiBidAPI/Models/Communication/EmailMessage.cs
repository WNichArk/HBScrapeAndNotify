using HiBidAPI.Models.Interfaces;

namespace HiBidAPI.Models.Communication
{
    public class EmailMessage : ICommMessage
    {
        public string Message { get; set; }
        public DateTime TimeSent { get; set; }
        public string EmailAddress { get; set; }

    }
}
