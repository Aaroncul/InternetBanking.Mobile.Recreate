using System;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace InternetBanking.Services.Settings
{
    public class SettingsService : ISettingsService
    {
        const bool DefaultIsFirstRun = true;
        const bool DefaultIsSignedIn = false;
        const int DefaultMemberNumber = 0;
        const int DefaultCustomerId = 0;
        const int DefaultPersonId = 0;
        const bool DefaultRememberPin = true;
        const string DefaultName = "";
        const string DefaultEmail = "";
        const string DefaultMobile = "";
        const string DefaultTwoFactorCode = "";
        private DateTime DefaultSleepTime = DateTime.Now;

        public bool IsFirstRun
        {
            get => GetValueOrDefault(KnownId.IsFirstRunKey, DefaultIsFirstRun);
            set => AddOrUpdateValue(KnownId.IsFirstRunKey, value);
        }

        public bool IsSignedIn
        {
            get => GetValueOrDefault(KnownId.IsSignedInKey, DefaultIsSignedIn);
            set => AddOrUpdateValue(KnownId.IsSignedInKey, value);
        }

        public int MemberNumber
        {
            get => GetValueOrDefault(KnownId.MemberNumberKey, DefaultMemberNumber);
            set => AddOrUpdateValue(KnownId.MemberNumberKey, value);
        }

        public int CustomerId
        {
            get => GetValueOrDefault(KnownId.CustomerIdKey, DefaultCustomerId);
            set => AddOrUpdateValue(KnownId.CustomerIdKey, value);
        }

        public int PersonId
        {
            get => GetValueOrDefault(KnownId.PersonIdKey, DefaultPersonId);
            set => AddOrUpdateValue(KnownId.PersonIdKey, value);
        }

        public bool RememberPin
        {
            get => GetValueOrDefault(KnownId.RememberPinKey, DefaultRememberPin);
            set => AddOrUpdateValue(KnownId.RememberPinKey, value);
        }

        public string Name
        {
            get => GetValueOrDefault(KnownId.NameKey, DefaultName);
            set => AddOrUpdateValue(KnownId.NameKey, value);
        }

        public string Email
        {
            get => GetValueOrDefault(KnownId.EmailKey, DefaultEmail);
            set => AddOrUpdateValue(KnownId.EmailKey, value);
        }

        public string Mobile
        {
            get => GetValueOrDefault(KnownId.MobileKey, DefaultMobile);
            set => AddOrUpdateValue(KnownId.MobileKey, value);
        }

        public DateTime? DateOfBirth
        {
            get
            {
                var dateOfBirth = GetValueOrDefault(KnownId.DateOfBirthKey, null);
                return dateOfBirth != null ? (DateTime?)DateTime.Parse(dateOfBirth) : null;
            }
            set => AddOrUpdateValue(KnownId.DateOfBirthKey, value.ToString());
        }

        public DateTime SleepTime
        {
            get => GetValueOrDefault(KnownId.SleepTimeKey, DefaultSleepTime);
            set => AddOrUpdateValue(KnownId.SleepTimeKey, value);
        }

        public string TwoFactorCode
        {
            get => GetValueOrDefault(KnownId.TwoFactorCodeKey, DefaultTwoFactorCode);
            set => AddOrUpdateValue(KnownId.TwoFactorCodeKey, value);
        }

        public Task AddOrUpdateValue(string key, bool value) =>
            AddOrUpdateValueImpl(key, value);

        public Task AddOrUpdateValue(string key, string value) =>
            AddOrUpdateValueImpl(key, value);

        public Task AddOrUpdateValue(string key, int value) =>
            AddOrUpdateValueImpl(key, value);

        private Task AddOrUpdateValue(string key, DateTime value) =>
            AddOrUpdateValueImpl(key, value);

        public bool GetValueOrDefault(string key, bool defaultValue) =>
            GetValueOrDefaultImpl(key, defaultValue);

        public string GetValueOrDefault(string key, string defaultValue) =>
            GetValueOrDefaultImpl(key, defaultValue);

        public int GetValueOrDefault(string key, int defaultValue) =>
            GetValueOrDefaultImpl(key, defaultValue);

        public DateTime GetValueOrDefault(string key, DateTime defaultValue) =>
           GetValueOrDefaultImpl(key, defaultValue);

        public void SignOut()
        {
            IsSignedIn = DefaultIsSignedIn;

            MemberNumber = DefaultMemberNumber;
            CustomerId = DefaultCustomerId;
            PersonId = DefaultPersonId;
            RememberPin = DefaultRememberPin;

            Name = DefaultName;
            Email = DefaultEmail;
            Mobile = DefaultMobile;
            DateOfBirth = null;
            TwoFactorCode = DefaultTwoFactorCode;
        }

        private async Task AddOrUpdateValueImpl<T>(string key, T value)
        {
            if (value == null)
            {
                await Remove(key);
            }

            Application.Current.Properties[key] = value;

            try
            {
                await Application.Current.SavePropertiesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unable to save: {key}");
                Console.WriteLine(ex.Message);
            }
        }

        private T GetValueOrDefaultImpl<T>(string key, T defaultValue = default(T))
        {
            object value = null;

            if (Application.Current.Properties.ContainsKey(key))
            {
                value = Application.Current.Properties[key];
            }

            return null != value ? (T)value : defaultValue;
        }

        private async Task Remove(string key)
        {
            try
            {
                if (Application.Current.Properties[key] != null)
                {
                    Application.Current.Properties.Remove(key);
                    await Application.Current.SavePropertiesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unable to remove: {key}");
                Console.WriteLine(ex.Message);
            }
        }
    }
}
