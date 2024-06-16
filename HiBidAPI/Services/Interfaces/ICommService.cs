using HiBidAPI.Models.Interfaces;
using HiBidAPI.Models.Utility;

namespace HiBidAPI.Services.Interfaces
{
    public interface ICommService
    {
        ResponseMessage SendNotification(ICommMessage message);
    }
}
