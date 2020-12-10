using InternetBanking.Models;
using System;

using Xamarin.Forms;

namespace InternetBanking.Views
{
    public partial class WelcomeView : ContentPage
    {
        public bool DisplayWelcomeImage => App.Skin.DisplayWelcomeImage;
        public WelcomeView()
        {
            InitializeComponent();

            NavigationPage.SetHasNavigationBar(this, false);

            Appearing += OnAppearing;
            Disappearing += OnDisappearing;
        }

        private void OnAppearing(object sender, EventArgs e)
        {
            base.OnAppearing();

            if (App.Skin.HasLightWelcomeStatusBar())
            {
                if (Application.Current.MainPage is CustomNavigationView mainPage)
                {
                    mainPage.BarBackgroundColor = Color.Transparent;
                    mainPage.BarTextColor = Color.White;
                }
            }
        }

        private void OnDisappearing(object sender, EventArgs e)
        {
            base.OnDisappearing();

            if (App.Skin.HasLightWelcomeStatusBar())
            {
                if (Application.Current.MainPage is CustomNavigationView mainPage)
                {
                    mainPage.BarBackgroundColor = Color.Default;
                    mainPage.BarTextColor = Color.Default;
                }
            }
        }
    }
}
