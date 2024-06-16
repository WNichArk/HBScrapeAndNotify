namespace HiBidAPI.Models.Interfaces
{
    public interface ICommMessage
    {
        string Message { get; set; }
        DateTime TimeSent { get; set; }
    }
}
