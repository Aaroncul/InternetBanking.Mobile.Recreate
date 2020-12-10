using Foundation;
using InternetBanking.Contracts;
using InternetBanking.iOS.SlieveGullion.Infrastructure;
using Xamarin.Forms;

[assembly: Dependency(typeof(iOSAppVersion))]

namespace InternetBanking.iOS.SlieveGullion.Infrastructure
{
    public sealed class iOSAppVersion : IAppVersion
    {
        public string GetVersion()
        {
            return NSBundle.MainBundle.ObjectForInfoDictionary("CFBundleShortVersionString").ToString();
        }

        public long GetBuild()
        {
            return int.Parse(NSBundle.MainBundle.ObjectForInfoDictionary("CFBundleVersion").ToString());
        }
    }
}