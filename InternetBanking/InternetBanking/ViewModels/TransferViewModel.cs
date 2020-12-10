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
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace InternetBanking.ViewModels
{
    public class TransferViewModel : SkinnedViewModel
    {
        private readonly IAbacusApiService _abacusApiService;
        private readonly ISettingsService _settingsService;

        private ObservableCollection<AccountDto> _allAccounts;

        private ObservableCollection<AccountDto> _fromAccounts;

        public ObservableCollection<AccountDto> FromAccounts
        {
            get => _fromAccounts;
            set
            {
                _fromAccounts = value;
                RaisePropertyChanged(() => FromAccounts);
            }
        }

        private ObservableCollection<AccountDto> _toAccounts;

        public ObservableCollection<AccountDto> ToAccounts
        {
            get => _toAccounts;
            set
            {
                _toAccounts = value;
                RaisePropertyChanged(() => ToAccounts);
            }
        }

        private ValidatableObject<AccountDto> _selectedToAccount;

        public AccountDto SelectedToAccount
        {
            get => _selectedToAccount.Value;
            set
            {
                _selectedToAccount.Value = value;

                if (_selectedToAccount.Value != null)
                {
                    _selectedToAccount.Value.AvailableBalance = GetAvailableBalance(_selectedToAccount.Value);
                }

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
                    _selectedFromAccount.Value.AvailableBalance = GetAvailableBalance(_selectedFromAccount.Value);
                    _toAccounts = _allAccounts;
                    _toAccounts.Remove(_toAccounts.Where(x => x.Id == _selectedFromAccount.Value.Id).First());

                    RaisePropertyChanged(() => ToAccounts);

                    if (_selectedToAccount.Value != null &&
                        _selectedToAccount.Value.Id == _selectedFromAccount.Value.Id)
                    {
                        _selectedToAccount.Value = null;
                        RaisePropertyChanged(() => SelectedToAccount);
                    }
                }

                RaisePropertyChanged(() => SelectedFromAccount);
            }
        }

        public ObservableCollection<string> SelectedFromAccountErrors
        {
            get => new ObservableCollection<string>(_selectedFromAccount.Errors);
        }

        public ObservableCollection<string> SelectedToAccountErrors
        {
            get => new ObservableCollection<string>(_selectedToAccount.Errors);
        }

        private ValidatableObject<decimal> _amount;

        public ValidatableObject<decimal> Amount
        {
            get => _amount;
            set
            {
                _amount = value;
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

        public ICommand TransferCommand => new Command(async () => await OnTransferAsync());

        public TransferViewModel(
            IAbacusApiService abacusApiService,
            ISettingsService settingsService)
        {
            Title = "Transfer";

            _abacusApiService = abacusApiService;
            _settingsService = settingsService;

            FromAccounts = new ObservableCollection<AccountDto>();
            ToAccounts = new ObservableCollection<AccountDto>();

            _selectedFromAccount = new ValidatableObject<AccountDto>();
            _selectedFromAccount.Validations.Add(new IsNotNullOrEmptyRule<AccountDto>
            {
                ValidationMessage = "Select a credit union account to withdraw from."
            });

            _selectedToAccount = new ValidatableObject<AccountDto>();
            _selectedToAccount.Validations.Add(new IsNotNullOrEmptyRule<AccountDto>
            {
                ValidationMessage = "Select a credit union account to withdraw to."
            });

            _amount = new ValidatableObject<decimal>();

            _amount.Validations.Add(new IsGreaterThanZero<decimal>
            {
                ValidationMessage = "Amount must be greater than zero"
            });

            _reference = new ValidatableObject<string>();

            _reference.Validations.Add(new IsNotNullOrEmptyRule<string>
            {
                ValidationMessage = "A reference is required"
            });

            _reference.Validations.Add(new IsNotMoreThan20<string>
            {
                ValidationMessage = "Reference must be no more than 20 characters"
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

                var accountsRequest = JsonConvert.DeserializeObject<List<AccountDto>>(
                    await response.Content.ReadAsStringAsync().ConfigureAwait(false));

                _allAccounts = new ObservableCollection<AccountDto>(accountsRequest);

                _toAccounts = _allAccounts;
                RaisePropertyChanged(() => ToAccounts);

                _fromAccounts = new ObservableCollection<AccountDto>(_allAccounts.Where(x => x.ProductTypeId == 1));
                RaisePropertyChanged(() => FromAccounts);

                _amount.Value = 0;
                _reference.Value = string.Empty;

                _selectedFromAccount.Value = null;
                RaisePropertyChanged(() => SelectedFromAccount);

                _selectedToAccount.Value = null;
                RaisePropertyChanged(() => SelectedToAccount);
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

        private async Task OnTransferAsync()
        {
            if (!ValidateTransfer())
            {
                return;
            }

            var dto = new InternalTransferDto
            {
                CustomerId = _settingsService.CustomerId,
                Amount = _amount.Value,
                Reference = _reference.Value
            };

            var response = await _abacusApiService.PostAsync($"accounts/{_selectedFromAccount.Value.Id}/internaltransfer/{_selectedToAccount.Value.Id}",
                JsonConvert.SerializeObject(dto));

            if (response.StatusCode == System.Net.HttpStatusCode.Created)
            {
                await DialogService.ShowAlertAsync(
                    "Transfer complete!",
                    string.Empty,
                    "OK");

                await RefreshAsync();
            }
            else
            {
                await DialogService.ShowAlertAsync(
                    "Invalid. Please check the information provided.",
                    string.Empty,
                    "OK");
            }
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

        //NOTE - Checking if both Amount and Reference are both validated
        private bool ValidateTransfer()
        {
            var fromAccountIsValid = ValidateFromAccount();
            RaisePropertyChanged(() => SelectedFromAccountErrors);

            var toAccountIsValid = false;
            if (fromAccountIsValid)
            {
                toAccountIsValid = ValidateToAccount();
                RaisePropertyChanged(() => SelectedToAccountErrors);
            }

            var amountIsValid = ValidateAmount();
            var referenceIsValid = ValidateReference();

            return amountIsValid && referenceIsValid && fromAccountIsValid && toAccountIsValid;
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

        private bool ValidateToAccount()
        {
            if (_selectedToAccount.Value == null)
            {
                _selectedToAccount.Errors.Add("Select an account to transfer to.");
                _selectedToAccount.IsValid = false;

                return false;
            }

            if (_selectedToAccount.Value.Id == _selectedFromAccount.Value.Id)
            {
                _selectedToAccount.Errors.Add("Error: Cannot transfer money to the same account");
                _selectedToAccount.IsValid = false;

                return false;
            }

            return _selectedToAccount.Validate();
        }

        private bool ValidateFromAccount()
        {
            if (_selectedFromAccount.Value == null)
            {
                _selectedFromAccount.Errors.Add("Select an account to transfer from.");
                _selectedFromAccount.IsValid = false;

                return false;
            }

            return _selectedFromAccount.Validate();
        }

        private bool ValidateReference()
        {
            return Reference.Validate();
        }
    }
}