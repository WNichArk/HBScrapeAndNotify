using HiBidAPI.Models.Interfaces;
using LiteDB;

namespace HiBidAPI.Models.Repositories
{
    public interface ICommunicationRepository
    {
        void AddCommunication(ICommMessage communication);
        List<ICommMessage> GetAllCommunications();
    }

    public class CommunicationRepository : ICommunicationRepository
    {
        private ILiteDatabase _db;

        public CommunicationRepository(ILiteDatabase db)
        {
            _db = db;
        }

        public void AddCommunication(ICommMessage communication)
        {
            var communicationCollection = _db.GetCollection<ICommMessage>("communications");
            communicationCollection.Insert(communication);
        }

        public List<ICommMessage> GetAllCommunications()
        {
            var communicationCollection = _db.GetCollection<ICommMessage>("communications");
            return communicationCollection.FindAll().ToList();
        }

        public List<ICommMessage> GetCommMessagesByContactString(string contactString)
        {
            var communicationCollection = _db.GetCollection<ICommMessage>("communications");
            return communicationCollection.Find(x => x.ContactString == contactString).ToList();
        }
    }
}
