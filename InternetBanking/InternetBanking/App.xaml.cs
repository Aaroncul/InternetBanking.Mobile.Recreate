using InternetBanking.Models;
using InternetBanking.Services.API;
using InternetBanking.Services.Navigation;
using InternetBanking.Services.Settings;
using InternetBanking.Services.Skins;
using InternetBanking.ViewModels;
using InternetBanking.ViewModels.Base;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace InternetBanking
{
    public partial class App : Application
    {
        public static Skin Skin { get; set; }

        public static int ScreenWidth { get; set; }
        public static int ScreenHeight { get; set; }

        public static DateTime SleepTime { get; set; }
        private readonly ISettingsService _settingsService;
        public App(string skinName, ISettingsService settingsService)
        {
            InitializeComponent();

            Skin = ViewModelLocator
                .Instance
                .Resolve<ISkinService>()
                .GetSkin(skinName);

            _settingsService = settingsService;

            ViewModelLocator
                .Instance
                .Register<IAbacusApiService>(new AbacusApiService(Skin.ApiToken));
        }

        protected override async void OnStart()
        {
            base.OnStart();

            await ViewModelLocator
            .Instance
            .Resolve<INavigationService>()
            .InitializeAsync();
        }

        protected override void OnSleep()
        {
            ViewModelLocator
                .Instance
                .Resolve<ISettingsService>()
                .SleepTime = DateTime.Now;
        }

        protected override async void OnResume()
        {
            var sleepTime = ViewModelLocator
                .Instance
                .Resolve<ISettingsService>()
                .SleepTime;

            if ((DateTime.Now - sleepTime).TotalSeconds > 10 && _settingsService.IsSignedIn)
            {
                await ViewModelLocator
                    .Instance
                    .Resolve<INavigationService>()
                    .NavigateToAsync<LoginViewModel>();

                await Current.MainPage.DisplayAlert("Security Alert", "Your session timed out.", "Ok");

            }
        }
    }
}
