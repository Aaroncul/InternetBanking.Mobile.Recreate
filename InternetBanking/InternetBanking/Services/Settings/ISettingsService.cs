using System;
using System.Threading.Tasks;

namespace InternetBanking.Services.Settings
{
    public interface ISettingsService
    {
        bool IsFirstRun { get; set; }
        bool IsSignedIn { get; set; }
        int MemberNumber { get; set; }
        int CustomerId { get; set; }
        int PersonId { get; set; }
        bool RememberPin { get; set; }
        string Name { get; set; }
        string Email { get; set; }
        string Mobile { get; set; }
        DateTime? DateOfBirth { get; set; }
        string TwoFactorCode { get; set; }
        DateTime SleepTime { get; set; }

        bool GetValueOrDefault(string key, bool defaultValue);

        string GetValueOrDefault(string key, string defaultValue);

        Task AddOrUpdateValue(string key, bool value);

        Task AddOrUpdateValue(string key, string value);

        void SignOut();
    }
}
