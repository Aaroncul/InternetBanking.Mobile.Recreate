using InternetBanking.Dto;
using InternetBanking.Helpers;
using InternetBanking.Services.API;
using InternetBanking.Services.Dialogs;
using InternetBanking.Services.Settings;
using InternetBanking.Validation;
using InternetBanking.ViewModels.Base;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace InternetBanking.ViewModels
{
    public class AddBankAccountViewModel : SkinnedViewModel
    {
        private readonly IDialogService _dialogService;
        private readonly IAbacusApiService _abacusApiService;
        private readonly ISettingsService _settingsService;

        public ICommand ValidateAccountNameCommand => new Command(() => ValidateAccountName());
        public ICommand ValidateAccountNumberCommand => new Command(() => ValidateAccountNumber());
        public ICommand ValidateSortCodeCommand => new Command(() => ValidateSortCode());
        public ICommand ValidateVerificationCodeCommand => new Command(() => ValidateVerificationCode());
        public ICommand SaveCommand => new Command(async () => await OnSaveAsync());
        public ICommand SendVerificationCodeCommand => new Command(async () => await OnSendVerificationCodeAsync());
        public ICommand NavigateToAddedBankAccountsCommand => new Command(async () => await NavigateToAddedBankAccounts());

        private async Task NavigateToAddedBankAccounts()
        {
            await NavigationService.NavigateToAsync(typeof(AddedBankAccountsViewModel));
        }

        private bool _smsSent { get; set; }
        public bool SmsSent
        {
            get { return _smsSent; }
            set
            {
                _smsSent = value;
                RaisePropertyChanged(() => SmsSent);
            }
        }

        private ValidatableObject<string> _accountNumber;

        public ValidatableObject<string> AccountNumber
        {
            get { return _accountNumber; }
            set
            {
                _accountNumber = value;
                RaisePropertyChanged(() => AccountNumber);
            }
        }

        private ValidatableObject<string> _accountName { get; set; }

        public ValidatableObject<string> AccountName
        {
            get { return _accountName; }
            set
            {
                _accountName = value;
                RaisePropertyChanged(() => AccountName);
            }
        }

        private ValidatableObject<string> _sortCode { get; set; }

        public ValidatableObject<string> SortCode
        {
            get { return _sortCode; }
            set
            {
                _sortCode = value;
                RaisePropertyChanged(() => SortCode);
            }
        }

        private ValidatableObject<string> _verificationCode { get; set; }

        public ValidatableObject<string> VerificationCode
        {
            get { return _verificationCode; }
            set
            {
                _verificationCode = value;
                RaisePropertyChanged(() => VerificationCode);
            }
        }

        private ObservableCollection<BankAccountDto> _bankAccounts;

        public ObservableCollection<BankAccountDto> BankAccounts
        {
            get => _bankAccounts;
            set
            {
                _bankAccounts = value;
                RaisePropertyChanged(() => BankAccounts);
            }
        }

        private async Task OnSendVerificationCodeAsync()
        {
            IsBusy = true;

            var dto = new SmsAuthenticationSendRequestDto
            {
                CustomerId = _settingsService.CustomerId,
                MobileNumber = _settingsService.Mobile
            };

            await _abacusApiService.PostAsync("bankaccounts/verification", JsonConvert.SerializeObject(dto));

            SmsSent = true;
            IsBusy = false;
        }

        public AddBankAccountViewModel(
            IDialogService dialogService,
            IAbacusApiService abacusApiService,
            ISettingsService settingsService)
        {
            Title = "Add Bank Account";

            _dialogService = dialogService;
            _abacusApiService = abacusApiService;
            _settingsService = settingsService;

            _accountName = new ValidatableObject<string>();
            _accountNumber = new ValidatableObject<string>();
            _sortCode = new ValidatableObject<string>();
            _verificationCode = new ValidatableObject<string>();
            _bankAccounts = new ObservableCollection<BankAccountDto>();
            SmsSent = false;

            _accountName.Validations.Add(new IsNotNullOrEmptyRule<string>
            {
                ValidationMessage = "Please provide a bank account name."
            });

            _accountNumber.Validations.Add(new IsNotNullOrEmptyRule<string>
            {
                ValidationMessage = "Please provide a bank account number."
            });

            _accountNumber.Validations.Add(new IsValidBankAccountNumber<string>
            {
                ValidationMessage = "Account number invalid."
            });

            _sortCode.Validations.Add(new IsNotNullOrEmptyRule<string>
            {
                ValidationMessage = "Please provide a sort code."
            });

            _sortCode.Validations.Add(new IsValidSortCode<string>
            {
                ValidationMessage = "Sort code should contain numbers only."
            });

            _verificationCode.Validations.Add(new IsNotNullOrEmptyRule<string>
            {
                ValidationMessage = "Please enter your sms verification code."
            });
        }

        public override async Task InitializeAsync(object navigationData)
        {
            BankAccounts = await GetBankAccounts();

            _accountName.Validations.Add(new IsUniqueAccountName<string>(BankAccounts.ToList())
            {
                ValidationMessage = "This account name is already in use."
            });

            _accountNumber.Validations.Add(new IsUniqueAccountNumber<string>(BankAccounts.ToList())
            {
                ValidationMessage = "Account has already been linked."
            });

            await base.InitializeAsync(navigationData);
        }

        private async Task<ObservableCollection<BankAccountDto>> GetBankAccounts()
        {
            var response = await _abacusApiService.GetAsync(
                    $"customers/{_settingsService.CustomerId}/bankaccounts?sort=name&paged=false");

            response.EnsureSuccessStatusCode();

            var accounts = JsonConvert.DeserializeObject<List<BankAccountDto>>(await
                response.Content.ReadAsStringAsync().ConfigureAwait(false))
                .Where(x => !x.DateClosed.HasValue)
                .ToList();

            var observable = new ObservableCollection<BankAccountDto>();

            foreach (var account in accounts)
            {
                observable.Add(account);
            }

            return observable;
        }

        private async Task OnSaveAsync()
        {
            if (!ValidateBankAccountAdd())
            {
                return;
            }
            try
            {
                IsBusy = true;

                var response = await _abacusApiService.PostAsync(
                    $"bankaccounts/add",
                    JsonConvert.SerializeObject(new BankAccountAddDto
                    {
                        CustomerId = _settingsService.CustomerId,
                        PersonId = _settingsService.PersonId,
                        TwoFactorCode = VerificationCode.Value,
                        AccountNumber = AccountNumber.Value,
                        SortCode = SortCode.Value,
                        Name = AccountName.Value
                    }));

                if (response.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    await _dialogService.ShowAlertAsync(
                    "Bank account added!",
                    string.Empty,
                    "OK");

                    await InitializeAsync(this);
                    ResetForm();
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    await _dialogService.ShowAlertAsync(
                    "Verification code entered was invalid.",
                    string.Empty,
                    "OK");
                }
                else
                {
                    await _dialogService.ShowAlertAsync(
                    "There was a problem adding your bank account, try again shortly.",
                    string.Empty,
                    "OK");

                    //await NavigationService.NavigateBackAsync();
                }
            }
            catch
            {
                await _dialogService.ShowAlertAsync(
                    "There was a problem adding your bank account, try again shortly.",
                    string.Empty,
                    "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void ResetForm()
        {
            SmsSent = false;

            AccountName.Value = "";
            AccountName.Errors = new List<string>();

            AccountNumber.Value = "";
            AccountNumber.Errors = new List<string>();

            SortCode.Value = "";
            SortCode.Errors = new List<string>();

            VerificationCode.Value = "";
            VerificationCode.Errors = new List<string>();
        }

        private bool ValidateBankAccountAdd()
        {
            return ValidateAccountName() &
                ValidateAccountNumber() &
                ValidateSortCode() &
                ValidateVerificationCode();
        }

        private bool ValidateVerificationCode()
        {
            return VerificationCode.Validate();
        }

        private bool ValidateAccountName()
        {
            return AccountName.Validate();
        }

        private bool ValidateAccountNumber()
        {
            return AccountNumber.Validate();
        }

        private bool ValidateSortCode()
        {
            return SortCode.Validate();
        }
    }
}