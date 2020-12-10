using InternetBanking.Dto;
using InternetBanking.Services.API;
using InternetBanking.Services.Settings;
using InternetBanking.Validation;
using InternetBanking.ViewModels.Base;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace InternetBanking.ViewModels
{
    public class WithdrawalViewModel : SkinnedViewModel
    {
        private readonly IAbacusApiService _abacusApiService;
        private readonly ISettingsService _settingsService;

        private List<AccountDto> _fromAccounts;

        public List<AccountDto> FromAccounts
        {
            get => _fromAccounts;
            set
            {
                _fromAccounts = value;
                RaisePropertyChanged(() => FromAccounts);
            }
        }

        private List<BankAccountDto> _toAccounts;

        public List<BankAccountDto> ToAccounts
        {
            get => _toAccounts;
            set
            {
                _toAccounts = value;
                RaisePropertyChanged(() => ToAccounts);
            }
        }

        private ValidatableObject<BankAccountDto> _selectedToAccount;

        public ValidatableObject<BankAccountDto> SelectedToAccount
        {
            get => _selectedToAccount;
            set
            {
                _selectedToAccount = value;

                RaisePropertyChanged(() => SelectedToAccount);
            }
        }

        private ValidatableObject<AccountDto> _selectedFromAccount;

        public AccountDto SelectedFromAccount
        {
            get => _selectedFromAccount.Value;
            set
            {
                _selectedFromAccount.Value = value;
                if (_selectedFromAccount.Value != null)
                {
                    _selectedFromAccount.Value.AvailableBalance =
                        GetAvailableBalance(_selectedFromAccount.Value);
                }

                RaisePropertyChanged(() => SelectedFromAccount);
            }
        }

        public ObservableCollection<string> SelectedFromAccountErrors
        {
            get => new ObservableCollection<string>(_selectedFromAccount.Errors);
        }

        private ValidatableObject<decimal> _amount;

        public ValidatableObject<decimal> Amount
        {
            get => _amount;
            set
            {
                _amount = Amount;
                RaisePropertyChanged(() => Amount);
            }
        }

        private ValidatableObject<string> _reference;

        public ValidatableObject<string> Reference
        {
            get => _reference;
            set
            {
                _reference = value;
                RaisePropertyChanged(() => Reference);
            }
        }

        public ICommand WithdrawalCommand => new Command(async () => await OnWithdrawalAsync());

        public WithdrawalViewModel(
            IAbacusApiService abacusApiService,
            ISettingsService settingsService)
        {
            Title = "Withdraw";

            _abacusApiService = abacusApiService;
            _settingsService = settingsService;

            FromAccounts = new List<AccountDto>();
            ToAccounts = new List<BankAccountDto>();

            _selectedFromAccount = new ValidatableObject<AccountDto>();
            _selectedFromAccount.Validations.Add(new IsNotNullOrEmptyRule<AccountDto>
            {
                ValidationMessage = "Select a credit union account to withdraw from."
            });

            _selectedToAccount = new ValidatableObject<BankAccountDto>();
            _selectedToAccount.Validations.Add(new IsNotNullOrEmptyRule<BankAccountDto>
            {
                ValidationMessage = "Select a bank account to send funds to."
            });

            _amount = new ValidatableObject<decimal>();

            _amount.Validations.Add(new IsGreaterThanZero<decimal>
            {
                ValidationMessage = "Amount must be greater than zero."
            });

            _reference = new ValidatableObject<string>();

            _reference.Validations.Add(new IsNotNullOrEmptyRule<string>
            {
                ValidationMessage = "A reference is required."
            });

            _reference.Validations.Add(new IsNotMoreThan20<string>
            {
                ValidationMessage = "Reference must not exceed 20 characters"
            });

        }

        public override async Task InitializeAsync(object navigationData)
        {
            ThreadPool.QueueUserWorkItem(async (state) =>
            {
                await RefreshAsync();
            });

            await base.InitializeAsync(navigationData);
        }

        private async Task RefreshAsync()
        {
            IsBusy = true;

            try
            {
                DialogService.ShowLoading();

                var response = await _abacusApiService.GetAsync(
                    $"customers/{_settingsService.CustomerId}/accounts?sort=description&paged=false")
                    .ConfigureAwait(false);

                response.EnsureSuccessStatusCode();

                var accounts = JsonConvert.DeserializeObject<List<AccountDto>>(
                    await response.Content.ReadAsStringAsync().ConfigureAwait(false));

                _fromAccounts = new List<AccountDto>();
                _fromAccounts = accounts.Where(x => x.ProductTypeId == 1).ToList();
                RaisePropertyChanged(() => FromAccounts);

                _toAccounts = await GetBankAccounts();
                RaisePropertyChanged(() => ToAccounts);

                _amount.Value = 0;
                _reference.Value = string.Empty;
                _selectedFromAccount.Value = null;
                _selectedToAccount.Value = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                DialogService.HideLoading();
                IsBusy = false;
            }
        }

        private async Task<List<BankAccountDto>> GetBankAccounts()
        {
            var response = await _abacusApiService.GetAsync(
                    $"customers/{_settingsService.CustomerId}/bankaccounts?sort=name&paged=false");

            response.EnsureSuccessStatusCode();

            var accounts = JsonConvert.DeserializeObject<List<BankAccountDto>>(await
                response.Content.ReadAsStringAsync().ConfigureAwait(false))
                .Where(x => !x.DateClosed.HasValue)
                .ToList();

            var observable = new List<BankAccountDto>();

            foreach (var account in accounts)
            {
                observable.Add(account);
            }

            return observable;
        }

        private async Task OnWithdrawalAsync()
        {
            if (!ValidateWithdrawal())
            {
                return;
            }

            var dto = new WithdrawalDto
            {
                CustomerId = _settingsService.CustomerId,
                SavingsAccountId = _selectedFromAccount.Value.Id,
                Amount = _amount.Value,
                BankAccountId = _selectedToAccount.Value.BankAccountId,
                Reference = _reference.Value
            };

            var response = await _abacusApiService.PostAsync("withdrawals", JsonConvert.SerializeObject(dto));

            if (response.StatusCode == System.Net.HttpStatusCode.Created)
            {
                await DialogService.ShowAlertAsync(
                        "Withdrawal Request is successful!",
                        string.Empty,
                        "OK");
                await RefreshAsync();
            }

            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                await DialogService.ShowAlertAsync(
                        "Opps! Something went wrong, please double check the information provided",
                        string.Empty,
                        "OK");
            }

            if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                await DialogService.ShowAlertAsync(
                        "Opps! Something went wrong, please double check the information provided",
                        string.Empty,
                        "OK");
            }
        }

        private bool ValidateWithdrawal()
        {
            var fromAccountIsValid = ValidateFromAccount();
            var toAccountIsValid = ValidateToAccount();
            var amountIsValid = ValidateAmount();
            var referenceIsValid = ValidateReference();

            return amountIsValid && referenceIsValid && fromAccountIsValid && toAccountIsValid;
        }

        private bool ValidateFromAccount()
        {
            return _selectedFromAccount.Validate();
        }

        private bool ValidateToAccount()
        {
            return SelectedToAccount.Validate();
        }

        private bool ValidateAmount()
        {
            var valid = _amount.Validate();

            if (_selectedFromAccount.Value == null)
            {
                return false;
            }

            if (_amount == null)
            {
                _amount.Errors.Add("Enter an amount to withdraw.");
                _amount.IsValid = false;
                RaisePropertyChanged(() => Amount);
                return false;
            }

            if (_amount.Value > _selectedFromAccount.Value.AvailableBalance)
            {
                _amount.Errors.Add("Amount exceeds your available balance.");
                _amount.IsValid = false;
                RaisePropertyChanged(() => Amount);

                return false;
            }

            return valid;
        }

        private bool ValidateReference()
        {
            return Reference.Validate();
        }

        private decimal GetAvailableBalance(AccountDto account)
        {
            var minimumShareBalance =
                Math.Max(account.MinimumShareBalance, account.OverdraftLimit);

            return account.Balance -
                account.BalanceLocked -
                account.UnclearedAmount -
                minimumShareBalance;
        }
    }
}