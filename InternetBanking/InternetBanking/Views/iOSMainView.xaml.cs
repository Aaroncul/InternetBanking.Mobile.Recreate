using System;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace InternetBanking.Views
{
    public partial class iOSMainView : TabbedPage
    {
        private Page _previousPage;

        public iOSMainView()
        {
            InitializeComponent();
        }

        public void AddPage(Page page, string title)
        {
            var navigationPage = new CustomNavigationView(page)
            {
                Title = title,
                IconImageSource = GetIconForPage(page),
                BarBackgroundColor = Color.FromHex(App.Skin.PrimaryColor),
                BarTextColor = Color.White
            };

            if (_previousPage == null)
            {
                _previousPage = page;
            }

            Children.Add(navigationPage);
        }

        public bool TrySetCurrentPage(Page requiredPage)
        {
            return TrySetCurrentPage(requiredPage.GetType());
        }

        public bool TrySetCurrentPage(Type requiredPageType)
        {
            CustomNavigationView page = GetTabPageWithInitial(requiredPageType);

            if (page != null)
            {
                CurrentPage = null;
                CurrentPage = page;
            }

            return page != null;
        }

        public async Task ClearNavigationForPage(Type type)
        {
            CustomNavigationView page = GetTabPageWithInitial(type);

            if (page != null)
            {
                await page.Navigation.PopToRootAsync(false);
            }
        }

        private CustomNavigationView GetTabPageWithInitial(Type type)
        {
            CustomNavigationView page = Children
                .OfType<CustomNavigationView>()
                .FirstOrDefault(p =>
                {
                    return
                        p.CurrentPage.Navigation.NavigationStack.Count > 0 &&
                        p.CurrentPage.Navigation.NavigationStack[0].GetType() == type;
                });

            return page;
        }

        private string GetIconForPage(Page page)
        {
            string icon = string.Empty;
            if (page is AccountsView)
            {
                if (Device.RuntimePlatform == Device.Android)
                {
                    icon = "wallet2.png";
                }
                else
                {
                    icon = "wallet.png";
                }
            }
            else if (page is MoreView)
            {
                if (Device.RuntimePlatform == Device.Android)
                {
                    icon = "list2.png";
                }
                else
                {
                    icon = "list.png";
                }
            }
            else if (page is SettingsView)
            {
                if (Device.RuntimePlatform == Device.Android)
                {
                    icon = "user_male2.png";
                }
                else
                {
                    icon = "user_male.png";
                }

            }
            else if (page is LoginView)
            {
                if (Device.RuntimePlatform == Device.Android)
                {
                    icon = "passwordW.png";
                }
                else
                {
                    icon = "password.png";
                }

            }

            else if (page is LoginView)
            {
                if (Device.RuntimePlatform == Device.Android)
                {
                    icon = "handshakeW.png";
                }
                else
                {
                    icon = "handshake.png";
                }
            }

            else if (page is LoginView)
            {
                if (Device.RuntimePlatform == Device.Android)
                {
                    icon = "checkedW.png";
                }
                else
                {
                    icon = "checked.png";
                }
            }
            return icon;
        }

        private void OnCurrentPageChanged(object sender, EventArgs e)
        {
            if (CurrentPage == null)
            {
                return;
            }

            if (!CurrentPage.IsEnabled)
            {
                CurrentPage = _previousPage;
            }
            else
            {
                _previousPage = CurrentPage;
            }
        }
    }
}