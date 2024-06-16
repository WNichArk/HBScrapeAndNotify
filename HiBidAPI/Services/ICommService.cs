using HiBidAPI.Models.Interfaces;
using HiBidAPI.Models.Utility;

namespace HiBidAPI.Services
{
    public interface ICommService
    {
        ResponseMessage SendNotification(ICommMessage message);
    }
}
