using HiBidAPI.Models.Interfaces;

namespace HiBidAPI.Models.Communication
{
    public class EmailMessage : ICommMessage
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public bool Sent { get; set; }
        public DateTime TimeSent { get; set; }
        public string EmailAddress { get; set; }
        public string ContactString
        {
            get
            {
                return EmailAddress;
            }
        }
        public ContactType ContactType
        {
            get
            {
                return ContactType.Email;
            }
        }

    }
}
