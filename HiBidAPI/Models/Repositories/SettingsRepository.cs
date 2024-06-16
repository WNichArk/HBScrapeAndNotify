using HiBidAPI.Models.Utility;
using LiteDB;

namespace HiBidAPI.Models.Repositories
{
    public interface ISettingsRepository
    {
        List<Setting> GetAllSettings();
    }

    public class SettingsRepository : ISettingsRepository
    {
        private ILiteDatabase _db;

        public SettingsRepository(ILiteDatabase db)
        {
            _db = db;
        }

        public List<Setting> GetAllSettings()
        {
            var settingsCollection = _db.GetCollection<Setting>("settings");
            return settingsCollection.FindAll().ToList();
        }
    }

}
