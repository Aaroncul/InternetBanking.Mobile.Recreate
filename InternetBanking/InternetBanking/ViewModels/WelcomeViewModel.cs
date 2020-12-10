using InternetBanking.ViewModels.Base;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace InternetBanking.ViewModels
{
    public class WelcomeViewModel : SkinnedViewModel
    {
        private int _frameWidth;
        public int FrameWidth
        {
            get { return _frameWidth; }
            set
            {
                _frameWidth = value;

                RaisePropertyChanged(() => FrameWidth);
            }
        }

        private int _frameHeight;
        public int FrameHeight
        {
            get { return _frameHeight; }
            set
            {
                _frameHeight = value;

                RaisePropertyChanged(() => FrameHeight);
            }
        }

        private int _frameRadius;
        public int FrameRadius
        {
            get { return _frameRadius; }
            set
            {
                _frameRadius = value;

                RaisePropertyChanged(() => FrameRadius);
            }
        }

        public bool ShowSkinName
        {
            get
            {
                return SkinName != "Slieve Gullion";
            }
        }

        public ICommand GetStartedCommand => new Command(async () => await GetStarted());

        public WelcomeViewModel()
        {
            FrameWidth = 200;
            FrameHeight = 200;
            FrameRadius = 120;
        }

        public async Task GetStarted()
        {
            await NavigationService.NavigateToAsync<LoginViewModel>();
        }
    }
}
