using System.Threading.Tasks;
using System.Windows.Input;

using Xamarin.Forms;

using InternetBanking.Dto;
using InternetBanking.Services.Authentication;
using InternetBanking.Validation;
using InternetBanking.ViewModels.Base;

namespace InternetBanking.ViewModels
{
    public class ForgotPinViewModel : SkinnedViewModel
    {
        public ICommand ForgotPinCommand => new Command(async () => await OnForgotPinAsync());

        private readonly IAuthenticationService _authenticationService;

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

        public ForgotPinViewModel(IAuthenticationService authenticationService)
        {
            Title = "Forgot PIN";

            _authenticationService = authenticationService;

            _memberNumber = new ValidatableObject<string>();
            _mobileNumber = new ValidatableObject<string>();

            _memberNumber.Validations.Add(new IsNotNullOrEmptyRule<string>
            {
                ValidationMessage = "Member ID is required"
            });

            var memberNumberLength = App.Skin.MemberNumberLength;

            _memberNumber.Validations.Add(new ExactLengthRule<string>(memberNumberLength)
            {
                ValidationMessage = $"Member ID must be {memberNumberLength} characters"
            });

            _mobileNumber.Validations.Add(new IsNotNullOrEmptyRule<string>()
            {
                ValidationMessage = "Mobile number is required"
            });

            _mobileNumber.Validations.Add(new IsValidMobileNumberRule<string>(11)
            {
                ValidationMessage = "Mobile number is not valid"
            });
        }

        private async Task OnForgotPinAsync()
        {
            if (!ValidateForgot())
            {
                return;
            }

            IsBusy = true;

            var result = await _authenticationService.ForgotPinAsync(new ForgotPinRequestDto
            {
                MemberId = MemberNumber.Value,
                Phone = MobileNumber.Value
            });

            if (result)
            {
                await DialogService.ShowAlertAsync("New PIN been sent!", "", "OK");
                await NavigationService.NavigateToAsync<LoginViewModel>();
            }
            else
            {
                await DialogService.ShowAlertAsync(
                    "Your member ID or PIN was incorrect",
                    string.Empty,
                    "OK");
            }

            IsBusy = false;
        }

        private bool ValidateForgot()
        {
            return
                ValidateMemberNumber() &
                ValidateMobileNumber();
        }

        private bool ValidateMemberNumber()
        {
            return MemberNumber.Validate();
        }

        private bool ValidateMobileNumber()
        {
            return MobileNumber.Validate();
        }
    }
}