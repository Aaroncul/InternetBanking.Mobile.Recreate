using InternetBanking.Dto;
using InternetBanking.Services.API;
using InternetBanking.Services.Settings;
using InternetBanking.ViewModels.Base;
using InternetBanking.ViewModels.Parameters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace InternetBanking.ViewModels
{
    public class AccountsViewModel : SkinnedViewModel
    {
        private readonly IAbacusApiService _abacusApiService;
        private readonly ISettingsService _settingsService;

        private ObservableCollection<AccountDto> _accounts;

        public ObservableCollection<AccountDto> Accounts
        {
            get => _accounts;
            set
            {
                _accounts = value;
                RaisePropertyChanged(() => Accounts);
            }
        }

        private ObservableCollection<PromotionDto> _promotions;

        public ObservableCollection<PromotionDto> Promotions
        {
            get => _promotions;
            set
            {
                _promotions = value;
                RaisePropertyChanged(() => Promotions);
            }
        }

        private AccountDto _selectedAccount { get; set; }

        public AccountDto SelectedAccount
        {
            get => _selectedAccount;
            set
            {
                _selectedAccount = value;
                RaisePropertyChanged(() => SelectedAccount);
            }
        }

        private bool _hasAccount { get; set; }

        public bool HasAccount
        {
            get => _hasAccount;
            set
            {
                _hasAccount = value;
                RaisePropertyChanged(() => HasAccount);
            }
        }

        public string TransferButtonText
        {
            get
            {
                if(App.Skin.Name.ToUpper().Contains("SLIEVE"))
                {
                    return "Internal Transfer";
                }
                else
                {
                    return "Transfer";
                }
            }
        }

        public ICommand RefreshCommand => new Command(async () =>
            await OnPullToRefreshAsync());

        public ICommand TableRowSelectedCommand => new Command(async () =>
            await NavigationService.NavigateToAsync<AccountViewModel>(new AccountViewModelParameter(SelectedAccount)));

        public ICommand WithdrawCommand => new Command(async () =>
          await NavigationService.NavigateToAsync<WithdrawalViewModel>());

        public ICommand TransferCommand => new Command(async () =>
                 await NavigationService.NavigateToAsync<TransferViewModel>());

        public AccountsViewModel(IAbacusApiService abacusApiService, ISettingsService settingsService)
        {
            Title = "Accounts";

            _abacusApiService = abacusApiService;
            _settingsService = settingsService;

            Accounts = new ObservableCollection<AccountDto>();
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
                Accounts = await GetAccounts();
                //Promotions = await GetPromotions();

                HasAccount = Accounts.Count > 0;
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

        private async Task<ObservableCollection<AccountDto>> GetAccounts()
        {
            var response = await _abacusApiService.GetAsync(
                $"customers/{_settingsService.CustomerId}/accounts?sort=description&paged=false")
                .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var accounts = JsonConvert.DeserializeObject<List<AccountDto>>(
                await response.Content.ReadAsStringAsync().ConfigureAwait(false));

            var observable = new ObservableCollection<AccountDto>();

            foreach (var account in accounts)
            {
                account.Description = FormatName(account.Description);
                observable.Add(account);

            }

            return observable;
        }

        private string FormatName(string name)
        {
            var formattedName = name.Trim();
            formattedName = formattedName.ToUpper();
            return formattedName;
        }

        private async Task<ObservableCollection<PromotionDto>> GetPromotions()
        {
            var response = await _abacusApiService.GetAsync("promotions?sort=created&paged=false")
                   .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var promotions = JsonConvert.DeserializeObject<List<PromotionDto>>(
                await response.Content.ReadAsStringAsync().ConfigureAwait(false));

            var observable = new ObservableCollection<PromotionDto>();

            foreach (var promotion in promotions)
            {
                observable.Add(promotion);
            }

            return observable;
        }
    }
}