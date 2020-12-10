using Xamarin.Forms;

namespace InternetBanking.Views
{
    public partial class MessagesView : ContentPage
    {
        public MessagesView()
        {
            InitializeComponent();

            listViewMessageThreads.ItemSelected += (sender, e) =>
            {
                ((ListView)sender).SelectedItem = null;
            };
        }
    }
}
