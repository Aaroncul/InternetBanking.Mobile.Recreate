using InternetBanking.Dto;
using InternetBanking.Helpers;
using InternetBanking.Services.API;
using InternetBanking.Services.Dialogs;
using InternetBanking.Services.Settings;
using InternetBanking.ViewModels.Base;
using InternetBanking.ViewModels.Components;
using InternetBanking.ViewModels.Parameters;
using Microcharts;
using Newtonsoft.Json;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace InternetBanking.ViewModels
{
    public class AccountViewModel : SkinnedViewModel
    {
        private readonly IAbacusApiService _abacusApiService;
        private readonly IDialogService _dialogService;
        private readonly ISettingsService _settingsService;

        private List<TransactionDto> _transactions;
        private DateTime LastButtonPress { get; set; }

        private AccountDto _account;

        public AccountDto Account
        {
            get { return _account; }
            set
            {
                _account = value;
                RaisePropertyChanged(() => Account);
            }
        }

        private List<TransactionSection> _transactionSections;

        public List<TransactionSection> TransactionSections
        {
            get { return _transactionSections; }
            set
            {
                _transactionSections = value;
                RaisePropertyChanged(() => TransactionSections);
            }
        }

        private bool _hasTransaction;

        public bool HasTransaction
        {
            get { return _hasTransaction; }
            set
            {
                _hasTransaction = value;
                RaisePropertyChanged(() => HasTransaction);
            }
        }

        private bool _isSearching;

        public bool IsSearching
        {
            get { return _isSearching; }
            set
            {
                _isSearching = value;
                RaisePropertyChanged(() => IsSearching);
            }
        }

        private bool _showVerificationPrompt;
        public bool ShowVerificationPrompt
        {
            get { return _showVerificationPrompt; }
            set
            {
                _showVerificationPrompt = value;
                RaisePropertyChanged(() => ShowVerificationPrompt);
            }
        }

        private DateTime _fromDate;

        public DateTime FromDate
        {
            get { return _fromDate; }
            set
            {
                _fromDate = value;
                RaisePropertyChanged(() => FromDate);
            }
        }

        private DateTime _untilDate;

        public DateTime UntilDate
        {
            get { return _untilDate; }
            set
            {
                _untilDate = value;
                RaisePropertyChanged(() => UntilDate);
            }
        }

        private AccountChartMode _mode;

        public AccountChartMode Mode
        {
            get { return _mode; }
            set
            {
                _mode = value;
                RaisePropertyChanged(() => Mode);
            }
        }

        private string _searchIcon;

        public string SearchIcon
        {
            get { return _searchIcon; }
            set
            {
                _searchIcon = value;
                RaisePropertyChanged(() => SearchIcon);
            }
        }

        public Chart ThisMonthChart
        {
            get
            {
                var entries = CreateEntriesByDays(14);

                return new LineChart
                {
                    Entries = entries,
                    LineMode = LineMode.Straight,
                    PointMode = PointMode.Circle,
                    PointSize = 20,
                    MinValue = entries.Min(x => x.Value) - 0.1f,
                    BackgroundColor = SKColors.Transparent,
                    LabelTextSize = 40
                };
            }
        }

        public Chart Last6MonthsChart
        {
            get
            {
                var entries = CreateEntriesByMonths(6);

                return new LineChart
                {
                    Entries = entries,
                    LineMode = LineMode.Straight,
                    PointMode = PointMode.Circle,
                    PointSize = 20,
                    MinValue = entries.Min(x => x.Value) - 0.1f,
                    BackgroundColor = SKColors.Transparent,
                    LabelTextSize = 40
                };
            }
        }

        public Chart Last12MonthsChart
        {
            get
            {
                var entries = CreateEntriesByMonths(12);

                return new LineChart
                {
                    Entries = entries,
                    LineMode = LineMode.Straight,
                    PointMode = PointMode.Circle,
                    PointSize = 20,
                    MinValue = entries.Min(x => x.Value) - 0.1f,
                    BackgroundColor = SKColors.Transparent,
                    LabelTextSize = 40
                };
            }
        }

        public ICommand SearchToggleCommand => new Command(() =>
        {
            IsSearching = !IsSearching;
            SearchIcon = IsSearching ? "\uf00d" : "\uf002";
            if (!IsSearching)
            {
                ResetSearchDates();
            }
        });

        public ICommand SelectModeCommand =>
            new Command<AccountChartMode>((AccountChartMode mode) => Mode = mode);

        public ICommand DateSelectedCommand => new Command(() => GroupTransactions());

        public ICommand OpenVerificationDialogCommand => new Command(async () => await OpenVerificationDialog());

        public ICommand RefreshCommand => new Command(async () => await OnPullToRefreshAsync());
        public AccountViewModel(
            IAbacusApiService abacusApiService,
            IDialogService dialogService,
            ISettingsService settingsService)
        {
            _abacusApiService = abacusApiService;
            _dialogService = dialogService;
            _settingsService = settingsService;

            _transactions = new List<TransactionDto>();
            TransactionSections = new List<TransactionSection>();

            Account = new AccountDto();
            Mode = AccountChartMode.Overview;
            IsSearching = false;
            SearchIcon = "\uf002";
            ResetSearchDates();
            _hasTransaction = true;

            LastButtonPress = DateTime.Now.AddSeconds(-60);

            if (string.IsNullOrEmpty(settingsService.TwoFactorCode))
            {
                ShowVerificationPrompt = true;
            }
            else
            {
                ShowVerificationPrompt = false;
            }
        }

        public override async Task InitializeAsync(object navigationData)
        {
            Account = ((AccountViewModelParameter)navigationData).Account;
            Title = Account.Description;

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
                /* Previously included code for 90 day limits without verification. Temporarily removed as it is not currently required.
                if (_settingsService.TwoFactorCode != string.Empty)
                {
                    response = await _abacusApiService.GetAsync(
                        $"accounts/{Account.Id}/alltransactionsverified?twoFactorCode={_settingsService.TwoFactorCode}&paged=false&sort=valuedate&ascending=false");
                }
                else
                {
                    response = await _abacusApiService.GetAsync(
                        $"accounts/{Account.Id}/transactions?paged=false&sort=valuedate&ascending=false&daysOld=90");
                }

                var startOfToday = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
                var endOfToday = startOfToday.AddDays(1).AddTicks(-1);

                if (_settingsService.TwoFactorCode != string.Empty)
                {
                    ShowVerificationPrompt = false;
                }
                */

                var response = await _abacusApiService.GetAsync(
                        $"accounts/{Account.Id}/transactions?paged=false&sort=valuedate&ascending=false");
                response.EnsureSuccessStatusCode();

                var transactions = JsonConvert.DeserializeObject<List<TransactionDto>>(
                    await response.Content.ReadAsStringAsync());

                _transactions = new List<TransactionDto>();
                foreach (var transaction in transactions)
                {
                    _transactions.Add(transaction);
                }

                GroupTransactions();

                RaisePropertyChanged(() => ThisMonthChart);
                RaisePropertyChanged(() => Last6MonthsChart);
                RaisePropertyChanged(() => Last12MonthsChart);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                IsBusy = false;
                _hasTransaction = _transactions.Count > 0;
            }
        }

        private void GroupTransactions()
        {
            var list = new List<TransactionSection>();

            var groupedTransactions =
                from t in _transactions
                    .Where(x => x.ValueDate >= FromDate && x.ValueDate <= UntilDate.AddDays(1).AddTicks(1))
                let dt = t.ValueDate
                group t by new
                {
                    dt.Year,
                    dt.Month,
                    dt.Day
                } into g
                select g;

            foreach (var group in groupedTransactions)
            {
                var key = group.Key;
                var date = new DateTime(key.Year, key.Month, key.Day);

                list.Add(new TransactionSection(group.ToList())
                {
                    Heading = date.ToString("ddd d MMM yyyy")
                });
            }

            TransactionSections = list;
            HasTransaction = TransactionSections.Count > 0;
        }

        private List<ChartEntry> CreateEntriesByDays(int days)
        {
            var entries = new List<ChartEntry>();

            for (var i = 0; i < days; i++)
            {
                var day = DateTime.Now.AddDays(-1 * i);
                var startOfDay = new DateTime(day.Year, day.Month, day.Day, 0, 0, 0);
                var endOfDay = startOfDay.AddDays(1).AddTicks(-1);

                var value = 0.0f;

                foreach (var transaction in _transactions.Where(x => x.ValueDate >= startOfDay && x.ValueDate <= endOfDay))
                {
                    if (transaction.Credit)
                    {
                        value += (float)transaction.Amount;
                    }
                    else
                    {
                        value -= (float)transaction.Amount;
                    }
                }

                entries.Add(new ChartEntry(value)
                {
                    Label = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedDayName(day.DayOfWeek),
                    ValueLabel = value <= 0 ? string.Empty : value.ToString("C0", CultureInfo.CreateSpecificCulture("en-GB")),
                    TextColor = SKColors.White,
                    Color = SKColor.Parse(SecondaryColor)
                });
            }

            entries.Reverse();

            return entries;
        }

        private List<ChartEntry> CreateEntriesByMonths(int months)
        {
            var entries = new List<ChartEntry>();

            for (var i = 0; i < months; i++)
            {
                var dayOfMonth = DateTime.Now.AddMonths(-1 * i);
                var firstDayOfMonth = new DateTime(dayOfMonth.Year, dayOfMonth.Month, 1);
                var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddTicks(-1);

                var value = 0.0f;

                foreach (var transaction in _transactions.Where(x => x.ValueDate >= firstDayOfMonth && x.ValueDate <= lastDayOfMonth))
                {
                    if (transaction.Credit)
                    {
                        value += (float)transaction.Amount;
                    }
                    else
                    {
                        value -= (float)transaction.Amount;
                    }
                }

                entries.Add(new ChartEntry(value)
                {
                    Label = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(dayOfMonth.Month),
                    ValueLabel = value <= 0 ? string.Empty : value.ToString("C0", CultureInfo.CreateSpecificCulture("en-GB")),
                    TextColor = SKColors.White,
                    Color = SKColor.Parse(SecondaryColor)
                });
            }

            entries.Reverse();

            return entries;
        }

        private async Task OpenVerificationDialog()
        {
            //Send code
            await SendVerificationCodeAsync();

            _settingsService.TwoFactorCode = await _dialogService.ShowTwoFactorDialog("We've just sent an sms with a unique code, enter it below.");

            var response = await _abacusApiService.GetAsync(
                        $"accounts/{Account.Id}/alltransactionsverified?twoFactorCode={_settingsService.TwoFactorCode}&paged=false&sort=valuedate&ascending=false");

            if (response.IsSuccessStatusCode)
            {
                ShowVerificationPrompt = false;
                await OnPullToRefreshAsync();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                _settingsService.TwoFactorCode = string.Empty;
                await _dialogService.ShowTwoFactorDialog("Verification code invalid. Please check and try again.");
            }
            else if (!response.IsSuccessStatusCode)
            {
                _settingsService.TwoFactorCode = string.Empty;
                await _dialogService.ShowAlertAsync("Verification currently unavailable, try again later", "Error", "Ok");
            }
        }

        private async Task SendVerificationCodeAsync()
        {
            IsBusy = true;

            if ((DateTime.Now - LastButtonPress).TotalSeconds > 60)
            {
                LastButtonPress = DateTime.Now;

                var dto = new SmsAuthenticationSendRequestDto
                {
                    CustomerId = _settingsService.CustomerId,
                    MobileNumber = _settingsService.Mobile
                };

                await _abacusApiService.PostAsync("bankaccounts/verification", JsonConvert.SerializeObject(dto));
            }
            else
            {
                await _dialogService.ShowAlertAsync("A verification code was sent less than one minute ago.",
                    "Code already sent.",
                    "Ok");
            }

            IsBusy = false;
        }

        private void ResetSearchDates()
        {
            var now = DateTime.Now;

            UntilDate = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0).AddDays(1).AddTicks(-1);
            FromDate = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0).AddDays(-300);
        }
    }
}