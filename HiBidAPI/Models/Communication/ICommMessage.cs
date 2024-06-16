namespace HiBidAPI.Models.Communication
{
    public interface ICommMessage
    {
        string Message { get; set; }
        DateTime TimeSent { get; set; }
    }
}
