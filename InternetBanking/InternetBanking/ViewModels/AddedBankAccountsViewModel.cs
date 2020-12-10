using InternetBanking.Dto;
using InternetBanking.Services.API;
using InternetBanking.Services.Settings;
using InternetBanking.ViewModels.Base;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace InternetBanking.ViewModels
{
    public class AddedBankAccountsViewModel : SkinnedViewModel
    {
        private readonly IAbacusApiService _abacusApiService;
        private readonly ISettingsService _settingsService;

        private ObservableCollection<BankAccountDto> _bankAccounts;

        private bool _hasBankAccount;

        public bool HasBankAccount
        {
            get { return _hasBankAccount; }
            set
            {
                _hasBankAccount = value;
                RaisePropertyChanged(() => HasBankAccount);
            }
        }

        public ICommand AddBankAccountCommand => new Command(async () =>
            await NavigationService.NavigateToAsync<AddBankAccountViewModel>());

        public ICommand ToggleActiveCommand => new Command<BankAccountDto>(
            async (BankAccountDto account) => await ToggleActive(account));

        public ICommand RefreshCommand => new Command(async () =>
            await OnPullToRefreshAsync());

        public ObservableCollection<BankAccountDto> BankAccounts
        {
            get => _bankAccounts;
            set
            {
                _bankAccounts = value;
                RaisePropertyChanged(() => BankAccounts);
            }
        }

        public AddedBankAccountsViewModel(
            IAbacusApiService abacusApiService,
            ISettingsService settingsService)
        {
            Title = "Bank Accounts";

            _abacusApiService = abacusApiService;
            _settingsService = settingsService;

            _bankAccounts = new ObservableCollection<BankAccountDto>();
        }

        public override async Task InitializeAsync(object navigationData)
        {
            ThreadPool.QueueUserWorkItem(async (state) =>
            {
                await OnPullToRefreshAsync();
            });

            

            await base.InitializeAsync(navigationData);
        }

        private async Task OnPullToRefreshAsync()
        {
            IsBusy = true;

            try
            {
                BankAccounts = await GetBankAccounts();
                HasBankAccount = BankAccounts.Count > 0;
                RaisePropertyChanged(() => HasBankAccount);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task<ObservableCollection<BankAccountDto>> GetBankAccounts()
        {
            var response = await _abacusApiService.GetAsync(
                    $"customers/{_settingsService.CustomerId}/bankaccounts?sort=name&paged=false");

            response.EnsureSuccessStatusCode();

            var accounts = JsonConvert.DeserializeObject<List<BankAccountDto>>(await
                response.Content.ReadAsStringAsync().ConfigureAwait(false))
                .ToList();

            var observable = new ObservableCollection<BankAccountDto>();

            foreach (var account in accounts)
            {
                account.Name = FormatName(account.Name);
                observable.Add(account);
            }

            return observable;
        }

        private string FormatName(string name)
        {
            var formattedName = name.Trim();
            formattedName = formattedName.ToUpper();

            var regex = new Regex("[ ]{2,}", RegexOptions.None);
            formattedName = regex.Replace(formattedName, " ");

            return formattedName;
        }

        public async Task ToggleActive(BankAccountDto account)
        {
            try
            {
                var response = await _abacusApiService.GetAsync($"bankaccounts/{account.Id}");

                response.EnsureSuccessStatusCode();

                var bankAccount = JsonConvert.DeserializeObject<BankAccountDto>(
                    await response.Content.ReadAsStringAsync().ConfigureAwait(false));

                bankAccount.DateClosed = bankAccount.DateClosed == null ? (DateTime?)DateTime.UtcNow : null;

                response = await _abacusApiService.PutAsync($"bankaccounts/{bankAccount.Id}", JsonConvert.SerializeObject(bankAccount));
                BankAccounts = await GetBankAccounts();
            }
            catch
            {
                await DialogService.ShowAlertAsync(
                "An error occurred, please try again later.",
                string.Empty,
                "OK");
            }
        }
    }
}