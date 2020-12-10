using Xamarin.Forms;

namespace InternetBanking.Views
{
    public partial class MoreView : ContentPage
    {
        public MoreView()
        {
            InitializeComponent();
        }

        protected override void OnDisappearing()
        {
            listView.SelectedItem = null;
        }
    }
}
