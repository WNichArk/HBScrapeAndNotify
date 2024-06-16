using HiBidAPI.Models.Communication;
using HiBidAPI.Models.Utility;

namespace HiBidAPI.Services
{
    public interface ICommService
    {
        ResponseMessage SendNotification(ICommMessage message);
    }
}
