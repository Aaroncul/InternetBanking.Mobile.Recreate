using Foundation;
using InternetBanking.Services.Navigation;
using InternetBanking.Services.Settings;
using InternetBanking.ViewModels.Base;
using RoundedBoxView.Forms.Plugin.iOSUnified;
using System.Globalization;
using System.Threading;
using UIKit;
using Xamarin;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace InternetBanking.iOS.SlieveGullion
{
    [Register("AppDelegate")]
    public class AppDelegate : FormsApplicationDelegate
    {
        private const string SkinName = "Slieve Gullion";

        public override bool FinishedLaunching(UIApplication uiApplication, NSDictionary launchOptions)
        {
            ViewModelLocator
                .Instance
                .RegisterSingleton<INavigationService, iOSNavigationService>();

            Forms.Init();
            FormsMaps.Init();
            RoundedBoxViewRenderer.Init();

            App.ScreenWidth = (int)(UIScreen.MainScreen.Bounds.Width * UIScreen.MainScreen.NativeScale);
            App.ScreenHeight = (int)(UIScreen.MainScreen.Bounds.Height * UIScreen.MainScreen.NativeScale);

            LoadApplication(new App(SkinName, new SettingsService()));

            var cultureInfo = new CultureInfo(App.Skin.Culture);

            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;

            return base.FinishedLaunching(uiApplication, launchOptions);
        }
    }
}