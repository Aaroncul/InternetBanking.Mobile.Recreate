using System.Collections.Generic;

namespace InternetBanking.Models
{
    public class Skin
    {
        public string AccentColor { get; set; }
        public string ApiToken { get; set; }
        public List<Branch> Branches { get; set; }
        public string Culture { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public string FacebookId { get; set; }
        public string FacebookUrl { get; set; }
        public bool HasLightWelcomeStatusBar()
        {
            return Name.ToLower().Equals("enterprise");
        }
        public int Id { get; set; }
        public string InstagramUrl { get; set; }
        public bool InvertedColor { get; set; }
        public int MemberNumberLength { get; set; }
        public string Name { get; set; }
        public int PinLength { get; set; }
        public string PrimaryColor { get; set; }
        public string SecondaryColor { get; set; }
        public string TwitterId { set; get; }
        public string TwitterUrl { get; set; }
        public string Url { get; set; }

        # region Feature Sets
        public bool DisplayWelcomeImage { get; set; }
        public bool WelcomeButtonBorderWidth { get; set; }
        public bool EnableBankAccounts { get; set; }
        public bool EnableMessaging { get; set; }
        public bool EnableTransfers { get; set; }
        public bool EnableWithdrawals { get; set; }

        #endregion Feature Sets
    }
}