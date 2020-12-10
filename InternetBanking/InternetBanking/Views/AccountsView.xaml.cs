using Xamarin.Forms;

namespace InternetBanking.Views
{
    public partial class AccountsView : ContentPage
    {
        public AccountsView()
        {
            InitializeComponent();
        }

        protected override void OnDisappearing()
        {
            listViewAccounts.SelectedItem = null;
        }
    }
}
