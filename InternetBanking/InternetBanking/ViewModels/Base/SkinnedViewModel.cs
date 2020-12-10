using Xamarin.Forms;

namespace InternetBanking.ViewModels.Base
{
    public abstract class SkinnedViewModel : BaseViewModel
    {
        protected const string WhiteColor = "#FFFFFF";
        protected const string BlackColor = "#000000";

        public string PrimaryTextColor => Color.FromHex(App.Skin.PrimaryColor).Luminosity > 0.7 ? BlackColor : WhiteColor;
        public string SecondaryTextColor => Color.FromHex(App.Skin.SecondaryColor).Luminosity > 0.7 ? BlackColor : WhiteColor;
        public string AccentTextColor => Color.FromHex(App.Skin.AccentColor).Luminosity > 0.7 ? BlackColor : WhiteColor;

        public string SkinName => App.Skin.Name;
        public string SkinDescription => App.Skin.Description;
        public string Culture => App.Skin.Culture;

        public string PrimaryColor => App.Skin.PrimaryColor;
        public string SecondaryColor => App.Skin.SecondaryColor;
        public string AccentColor => App.Skin.AccentColor;

        public int PinLength => App.Skin.PinLength;
        public int MemberNumberLength => App.Skin.MemberNumberLength;

        #region Feature Set
        public bool DisplayWelcomeImage => App.Skin.DisplayWelcomeImage;
        public bool WelcomeButtonBorderWidth => App.Skin.WelcomeButtonBorderWidth;
        public bool EnableBankAccounts => App.Skin.EnableBankAccounts;
        public bool EnableMessaging => App.Skin.EnableMessaging;
        public bool EnableTransfers => App.Skin.EnableTransfers;
        public bool EnableWithdrawals => App.Skin.EnableWithdrawals;
        #endregion Feature Set

        protected SkinnedViewModel()
        { }
    }
}