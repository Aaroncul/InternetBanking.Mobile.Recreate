using Xamarin.Forms;

namespace InternetBanking.Views
{
    public partial class HelpView : ContentPage
    {
        public HelpView()
        {
            InitializeComponent();
        }

        protected override void OnDisappearing()
        {
            listView.SelectedItem = null;
        }
    }
}
