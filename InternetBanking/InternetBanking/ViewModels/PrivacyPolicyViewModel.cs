using InternetBanking.ViewModels.Base;

namespace InternetBanking.ViewModels
{
    public class PrivacyPolicyViewModel : SkinnedViewModel
    {
        private string _privacyPolicyIntroduction;

        public string PrivacyPolicyIntroduction
        {
            get => _privacyPolicyIntroduction;
            set
            {
                _privacyPolicyIntroduction = value;
                RaisePropertyChanged(() => PrivacyPolicyIntroduction);
            }
        }

        private string _privacyPolicyContactUs;

        public string PrivacyPolicyContactUs
        {
            get => _privacyPolicyContactUs;
            set
            {
                _privacyPolicyContactUs = value;
                RaisePropertyChanged(() => PrivacyPolicyContactUs);
            }
        }

        public PrivacyPolicyViewModel()
        {
            Title = "Privacy Policy";

            PrivacyPolicyIntroduction = $"{App.Skin.Name} built the {App.Skin.Name} app as a Commercial app. This SERVICE is provided by {App.Skin.Name} and is intended for use as is.";
            PrivacyPolicyContactUs = $"If you have any questions or suggestions about our Privacy Policy, do not hesitate to contact us at: {App.Skin.Email}";
        }
    }
}