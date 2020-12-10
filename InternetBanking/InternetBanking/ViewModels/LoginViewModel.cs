using System;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Input;

using Xamarin.Essentials;
using Xamarin.Forms;

using InternetBanking.Dto;
using InternetBanking.Services.Authentication;
using InternetBanking.Services.Dialogs;
using InternetBanking.Services.Settings;
using InternetBanking.Validation;
using InternetBanking.ViewModels.Base;

namespace InternetBanking.ViewModels
{
    public class LoginViewModel : SkinnedViewModel
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IDialogService _dialogService;
        private readonly ISettingsService _settingsService;

        public string HeaderBackgroundColor => App.Skin.InvertedColor ? App.Skin.PrimaryColor : WhiteColor;

        private ValidatableObject<string> _memberNumber;

        public ValidatableObject<string> MemberNumber
        {
            get => _memberNumber;
            set
            {
                _memberNumber = value;
                RaisePropertyChanged(() => MemberNumber);
            }
        }

        private ValidatableObject<string> _pin;

        public ValidatableObject<string> Pin
        {
            get => _pin;
            set
            {
                _pin = value;
                RaisePropertyChanged(() => Pin);
            }
        }

        private ValidatableObject<string> _mobileNumber;

        public ValidatableObject<string> MobileNumber
        {
            get => _mobileNumber;
            set
            {
                _mobileNumber = value;
                RaisePropertyChanged(() => MobileNumber);
            }
        }

        private ValidatableObject<DateTime> _dateOfBirth;

        public ValidatableObject<DateTime> DateOfBirth
        {
            get => _dateOfBirth;
            set
            {
                _dateOfBirth = value;
                RaisePropertyChanged(() => DateOfBirth);
            }
        }

        private ValidatableObject<string> _smsActivationCode;

        public ValidatableObject<string> SmsActivationCode
        {
            get => _smsActivationCode;
            set
            {
                _smsActivationCode = value;
                RaisePropertyChanged(() => SmsActivationCode);
            }
        }

        private ValidatableObject<string> _pin1;

        public ValidatableObject<string> Pin1
        {
            get => _pin1;
            set
            {
                _pin1 = value;
                RaisePropertyChanged(() => Pin1);
            }
        }

        private ValidatableObject<string> _pin2;

        public ValidatableObject<string> Pin2
        {
            get => _pin2;
            set
            {
                _pin2 = value;
                RaisePropertyChanged(() => Pin2);
            }
        }

        public DateTime MaximumDateOfBirth => DateTime.UtcNow;
        public ValidatableObject<string> _smsTwoFactorAuthenticationLoginCode;
        public ValidatableObject<string> SmsTwoFactorAuthenticationLoginCode
        {
            get => _smsTwoFactorAuthenticationLoginCode;
            set
            {
                _smsTwoFactorAuthenticationLoginCode = value;
                RaisePropertyChanged(() => SmsTwoFactorAuthenticationLoginCode);
            }
        }

        public ICommand LoginCommand => new Command(async () => await OnLoginAsync());
        public ICommand ForgotPinCommand => new Command(async () => await OnForgotPin());
        public ICommand RegisterCommand => new Command(async () => await OnRegisterAsync());
        public ICommand ActivateCommand => new Command(async () => await OnActivateAsync());

        public ICommand ValidateMemberNumberCommand => new Command(() => ValidateMemberNumber());
        public ICommand ValidatePinCommand => new Command(() => ValidatePin());
        public ICommand ValidateMobileNumberCommand => new Command(() => ValidateMobileNumber());
        public ICommand ValidateDateOfBirthCommand => new Command(() => ValidateDateOfBirth());
        public ICommand ValidateSmsActivationCodeCommand => new Command(() => ValidateSmsActivationCode());
        public ICommand ValidatePin1Command => new Command(() => ValidatePin1());
        public ICommand ValidatePin2Command => new Command(() => ValidatePin2());
        public LoginViewModel(IAuthenticationService authenticationService,
            IDialogService dialogService, 
            ISettingsService settingsService)
        {
            Title = "Sign in";
            _authenticationService = authenticationService;
            _dialogService = dialogService;
            //Make sure the user was correctly logged out.
            authenticationService.LogoutAsync();

            _memberNumber = new ValidatableObject<string>();
            _pin = new ValidatableObject<string>();
            _mobileNumber = new ValidatableObject<string>();
            _dateOfBirth = new ValidatableObject<DateTime>
            {
                Value = new DateTime(2000, 1, 1)
            };
            _pin1 = new ValidatableObject<string>();
            _pin2 = new ValidatableObject<string>();
            _smsActivationCode = new ValidatableObject<string>();

            _pin1.Validations.Add(new IsNotNullOrEmptyRule<string>
            {
                ValidationMessage = "PIN is required"
            });

            _pin2.Validations.Add(new MustMatch<string>(_pin1)
            {
                ValidationMessage = "PINs must match"
            });

            _memberNumber.Validations.Add(new IsNotNullOrEmptyRule<string>
            {
                ValidationMessage = "Member ID is required"
            });

            var memberNumberLength = App.Skin.MemberNumberLength;

            _memberNumber.Validations.Add(new ExactLengthRule<string>(memberNumberLength)
            {
                ValidationMessage = $"Member ID must be {memberNumberLength} characters"
            });

            _pin.Validations.Add(new IsNotNullOrEmptyRule<string>
            {
                ValidationMessage = "PIN is required"
            });

            _mobileNumber.Validations.Add(new IsValidMobileNumberRule<string>(11)
            {
                ValidationMessage = "Mobile number is not valid"
            });

            _dateOfBirth.Validations.Add(new MustBe18OrOlderRule<DateTime>
            {
                ValidationMessage = "You must be 18 years or older"
            });

            _smsActivationCode.Validations.Add(new IsNotNullOrEmptyRule<string>
            {
                ValidationMessage = "Activation code is required"
            });

            if (_authenticationService.IsAuthenticated)
            {
                _settingsService = settingsService;
                _memberNumber.Value = _settingsService.MemberNumber.ToString();

                if (_memberNumber.Value == "0")
                {
                    _memberNumber.Value = "";
                }

                _authenticationService.LogoutAsync();
            };
        }

        public override async Task InitializeAsync(object navigationData)
        {
            await base.InitializeAsync(navigationData);
        }

        private async Task OnLoginAsync()
        {
            if (!ValidateLogin())
            {
                return;
            }

            var current = Connectivity.NetworkAccess;
            if (current != NetworkAccess.Internet)
            {
                await DialogService.ShowAlertAsync(
                    "You are currently offline. Please establish a network connection and try again.",
                    string.Empty,
                    "OK");

                Pin.Value = string.Empty;
                return;
            }

            IsBusy = true;

            var loginAttempt = new LoginAttemptDto
            {
                UserName = MemberNumber.Value,
                Password = Pin.Value
            };

            var loginResponse = await _authenticationService.LoginAsync(loginAttempt);

            switch (loginResponse.StatusCode)
            {
                case HttpStatusCode.OK:                    
                    await NavigationService.NavigateToAsync<MainViewModel>();
                    break;

                case HttpStatusCode.Unauthorized:
                    await DialogService.ShowAlertAsync(
                        "Your member number or PIN was incorrect",
                        string.Empty,
                        "OK");

                    Pin.Value = string.Empty;
                    break;

                case HttpStatusCode.PreconditionFailed:
                    var twoFactorResponse = await OpenAuthenticationDialog(loginResponse,MemberNumber.Value);

                    if (twoFactorResponse == HttpStatusCode.OK || twoFactorResponse == HttpStatusCode.Created)
                    { 
                        await NavigationService.NavigateToAsync<MainViewModel>();
                        break;
                    }
                    else if (twoFactorResponse == HttpStatusCode.BadRequest) // No code entered given, do nothing
                    {
                        Pin.Value = string.Empty;
                        break;
                    }

                    await DialogService.ShowAlertAsync(
                        "We could not authenticate your account, please try again.",
                        string.Empty,
                        "OK");

                    Pin.Value = string.Empty;
                    break;
                    

                case HttpStatusCode.InternalServerError:
                    await DialogService.ShowAlertAsync(
                        "We're experiencing some technical issues, please try again later.",
                        string.Empty,
                        "OK");

                    Pin.Value = string.Empty;
                    break;

                default:
                    await DialogService.ShowAlertAsync(
                        "An error occurred, please try again later.",
                        string.Empty,
                        "OK");

                    Pin.Value = string.Empty;
                    break;
            }

            IsBusy = false;
        }
        private async Task OnForgotPin()
        {
            await NavigationService.NavigateToAsync<ForgotPinViewModel>();
        }
        private async Task OnRegisterAsync()
        {
            if (!ValidateRegistration())
            {
                return;
            }

            IsBusy = true;

            var result = await _authenticationService.RegisterAsync(new RegisterDto
            {
                CustomerNumber = MemberNumber.Value,
                PhoneNumber = MobileNumber.Value.Replace(" ", ""),
                DOB = _dateOfBirth.Value.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture)
            });

            if (result == HttpStatusCode.Created)
            {

                await DialogService.ShowAlertAsync(
                    "Success! You should get your activation code by SMS shortly.",
                    string.Empty,
                    "OK");

                await InitializeAsync(this);

                //todo: Go to activate tab
            }
            else if (result == HttpStatusCode.Conflict)
            {
                await DialogService.ShowAlertAsync(
                    "This account has already been registered!",
                    string.Empty,
                    "OK");
            }
            else if (result == HttpStatusCode.NotAcceptable)
            {
                await DialogService.ShowAlertAsync(
                    "The details you provided were not correct.",
                    string.Empty,
                    "OK");
            }
            else
            {
                await DialogService.ShowAlertAsync(
                    "There was a problem registering your account.",
                    string.Empty,
                    "OK");
            }

            IsBusy = false;
        }
        private async Task OnActivateAsync()
        {
            if (!ValidateActivation())
            {
                return;
            }

            IsBusy = true;

            var result = await _authenticationService.ActivateAsync(new ActivationDto
            {
                MembershipNumber = MemberNumber.Value,
                SmsActivationCode = SmsActivationCode.Value,
                Pin1 = Pin1.Value,
                Pin2 = Pin2.Value
            });

            if (result == HttpStatusCode.Created)
            {
                await DialogService.ShowAlertAsync(
                    "Success! Your account has been activated.",
                    string.Empty,
                    "OK");

                Pin.Value = Pin1.Value;

                await OnLoginAsync();
            }
            else if (result == HttpStatusCode.Conflict)
            {
                await DialogService.ShowAlertAsync(
                    "Your account has already been activated!",
                    string.Empty,
                    "OK");
            }
            else if (result == HttpStatusCode.NotAcceptable)
            {
                await DialogService.ShowAlertAsync(
                    "The details you provided were not correct. Ensure that your account has been registered and the SMS code has been entered correctly.",
                    string.Empty,
                    "OK");
            }
            else if (result == HttpStatusCode.InternalServerError)
            {
                await DialogService.ShowAlertAsync(
                    "There was a problem activating your account, please try again later.",
                    "",
                    "OK");
            }
            else
            {
                await DialogService.ShowAlertAsync(
                    "There was a problem activating your account, ensure that your account has been registered.",
                    string.Empty,
                    "OK");
            }

            IsBusy = false;
        }
        private async Task<HttpStatusCode> OpenAuthenticationDialog(LoginResponseDto login, string memberNumber)
        {
            //Send code
            var response = await _authenticationService.SendAuthenticationSmsAsync(login.CustomerId, login.Phone);

            if (response != HttpStatusCode.OK)
            {
                return HttpStatusCode.InternalServerError;
            }

            var code =
                    await _dialogService.ShowTwoFactorDialog("We've just sent an sms with a unique code, enter it below.");

            var result = await _authenticationService.AuthenticateSmsAsync(login, memberNumber, code);

            return result;
        }
        private bool ValidateLogin()
        {
            return
                ValidateMemberNumber() &
                ValidatePin();
        }
        private bool ValidateRegistration()
        {
            return
                ValidateMemberNumber() &
                ValidateMobileNumber() &
                ValidateDateOfBirth();
        }
        private bool ValidateActivation()
        {
            return
                ValidateMemberNumber() &
                ValidateSmsActivationCode() &
                ValidatePin1() &
                ValidatePin2();
        }
        private bool ValidateMemberNumber()
        {
            return MemberNumber.Validate();
        }
        private bool ValidateSmsActivationCode()
        {
            return SmsActivationCode.Validate();
        }
        private bool ValidatePin()
        {
            return Pin.Validate();
        }
        private bool ValidatePin1()
        {
            return Pin1.Validate();
        }
        private bool ValidatePin2()
        {
            return Pin2.Validate();
        }
        private bool ValidateMobileNumber()
        {
            return MobileNumber.Validate();
        }
        private bool ValidateDateOfBirth()
        {
            return DateOfBirth.Validate();
        }
    }
}