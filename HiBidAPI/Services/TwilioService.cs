using HiBidAPI.Models.Interfaces;
using HiBidAPI.Models.Repositories;
using HiBidAPI.Models.Utility;
using HiBidAPI.Services.Interfaces;
using Twilio.Types;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace HiBidAPI.Services
{
    public class TwilioService : ICommService
    {
        ISettingsRepository _settingsRepository;
        ICommunicationRepository _commRepository;

        public TwilioService(ISettingsRepository settingsRepository, ICommunicationRepository communicationRepository)
        {
            _settingsRepository = settingsRepository;
            _commRepository = communicationRepository;
        }
        public ResponseMessage SendNotification(ICommMessage message)
        {
            var settings = _settingsRepository.GetAllSettings();
            var accountSid = settings.FirstOrDefault(s => s.Name.ToLower() == "twilioaccountsid")?.Value;
            var authToken = settings.FirstOrDefault(s => s.Name.ToLower() == "twilioauthtoken")?.Value;
            var fromNumber = settings.FirstOrDefault(s => s.Name.ToLower() == "twiliofromnumber")?.Value;

            if (accountSid == null || authToken == null || fromNumber == null)
            {
                throw new InvalidOperationException("Twilio configuration settings are missing.");
            }

            TwilioClient.Init(accountSid, authToken);

            var messageOptions = new CreateMessageOptions(new PhoneNumber(message.ContactString))
            {
                From = new PhoneNumber(fromNumber),
                Body = message.Message
            };

            var result = MessageResource.Create(messageOptions);
            _commRepository.AddCommunication(message);
            return new ResponseMessage
            {
                Message = result.Status.ToString(),
                Success = result.Status == MessageResource.StatusEnum.Queued
            };
        }
    }
}
