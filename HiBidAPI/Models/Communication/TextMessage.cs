using HiBidAPI.Models.Interfaces;

namespace HiBidAPI.Models.Communication
{
    public class TextMessage : ICommMessage
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public bool Sent { get; set; }
        public DateTime TimeSent { get; set; }
        public string PhoneNumber { get; set; }
        public string ContactString
        {
            get
            {
                return PhoneNumber;
            }
        }
        public ContactType ContactType
        {
            get
            {
                return ContactType.Text;
            }
        }

    }
}
