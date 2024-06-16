using LiteDB;

namespace HiBidAPI.Models.Interfaces
{
    public interface ICommMessage
    {
        [BsonId]
        int Id { get; set; }
        string Message { get; set; }
        bool Sent { get; set; }
        DateTime TimeSent { get; set; }
        string ContactString { get; }
        ContactType ContactType { get; }

    }

    public enum ContactType : int
    {
        Email = 0,
        Text = 1
    }
}
