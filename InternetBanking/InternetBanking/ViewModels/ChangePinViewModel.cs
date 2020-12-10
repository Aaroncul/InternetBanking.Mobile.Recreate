using InternetBanking.Dto;
using InternetBanking.Services.API;
using InternetBanking.Services.Settings;
using InternetBanking.ViewModels.Base;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace InternetBanking.ViewModels
{
    public class ChangePinViewModel : SkinnedViewModel
    {
        private readonly IAbacusApiService _abacusApiService;
        private readonly ISettingsService _settingsService;

        private string _currentPin;

        public string CurrentPin
        {
            get { return _currentPin; }
            set
            {
                _currentPin = value;
                RaisePropertyChanged(() => CurrentPin);
            }
        }

        private string _pin1;

        public string Pin1
        {
            get { return _pin1; }
            set
            {
                _pin1 = value;
                RaisePropertyChanged(() => Pin1);
            }
        }

        private string _pin2;

        public string Pin2
        {
            get { return _pin2; }
            set
            {
                _pin2 = value;
                RaisePropertyChanged(() => Pin2);
            }
        }

        private bool _staySignedIn;

        public bool StaySignedIn
        {
            get { return _staySignedIn; }
            set
            {
                _staySignedIn = value;
                RaisePropertyChanged(() => StaySignedIn);
            }
        }

        public ICommand SaveCommand => new Command(async () =>
            await OnSaveAsync());

        public ChangePinViewModel(
            IAbacusApiService abacusApiService,
            ISettingsService settingsService)
        {
            Title = "Change PIN";

            _abacusApiService = abacusApiService;
            _settingsService = settingsService;

            StaySignedIn = true;
        }

        private async Task OnSaveAsync()
        {
            try
            {
                IsBusy = true;

                if (Pin1 != Pin2)
                {
                    await DialogService.ShowAlertAsync(
                    "Your new PIN and confirmation PIN did not match.",
                    string.Empty,
                    "OK");

                    return;
                }

                var response = await _abacusApiService.PutAsync(
                    $"customers/{_settingsService.CustomerId}/pin",
                    JsonConvert.SerializeObject(new PinUpdateRequestDto
                    {
                        CurrentPin = CurrentPin,
                        Pin = Pin1
                    }));

                response.EnsureSuccessStatusCode();

                var success = response.StatusCode == System.Net.HttpStatusCode.NoContent;

                if (!success)
                {
                    await DialogService.ShowAlertAsync(
                    "Sorry you cannot change your new pin to your exisiting one. Please try again.",
                    string.Empty,
                    "OK");
                }
                else
                {
                    if (CurrentPin == Pin1)
                    {
                        await DialogService.ShowAlertAsync(
                        "Your new PIN can't be the same as your existing PIN!",
                        string.Empty,
                        "OK");
                    }
                    else
                    {
                        await DialogService.ShowAlertAsync(
                        "PIN changed successfully!",
                        string.Empty,
                        "OK");
                    }
                }
            }
            catch
            {
                await DialogService.ShowAlertAsync(
                    "There was a problem changing your PIN, try again shortly.",
                    string.Empty,
                    "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}