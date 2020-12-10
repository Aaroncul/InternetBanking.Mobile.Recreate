using Xamarin.Forms;

namespace InternetBanking.Views
{
    public partial class LoginView : TabbedPage
    {
        public LoginView()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }
    }
}
