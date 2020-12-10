using InternetBanking.Services.Authentication;
using InternetBanking.Services.Settings;
using InternetBanking.ViewModels.Components;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using MenuItem = InternetBanking.ViewModels.Components.MenuItem;

namespace InternetBanking.ViewModels
{
    public class MenuViewModel : BaseViewModel
    {
        private readonly ISettingsService _settingsService;

        private string _name { get; set; }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = _settingsService.Name;
                RaisePropertyChanged(() => _name);
            }
        }

        public string FirstName
        {
            get { return GetFirstName(); }
            set
            {
                var name = _settingsService.Name;
                RaisePropertyChanged(() => name);
            }
        }

        private ObservableCollection<MenuItem> _menuItems = new ObservableCollection<MenuItem>();

        public ObservableCollection<MenuItem> MenuItems
        {
            get => _menuItems;
            set
            {
                _menuItems = value;
                RaisePropertyChanged(() => MenuItems);
            }
        }

        public ICommand ItemSelectedCommand => new Command<MenuItem>(OnItemSelected);
        public ICommand SettingsCommand => new Command(async () =>
            await NavigationService.NavigateToAsync<SettingsViewModel>());
        public ICommand HelpCommand => new Command(async () =>
           await NavigationService.NavigateToAsync<ContactUsViewModel>());

        public MenuViewModel(IAuthenticationService authenticationService,
            ISettingsService settingsService)
        {
            _settingsService = settingsService;
            Name = _settingsService.Name;

            InitializeMenuItems();
        }

        public override Task InitializeAsync(object navigationData)
        {
            return base.InitializeAsync(navigationData);
        }

        private void InitializeMenuItems()
        {
            MenuItems.Add(new MenuItem
            {
                Title = "Accounts",
                Icon = new AwesomeIcon("\uf2bd", AwesomeIcon.Packages.Solid),
                ViewModelType = typeof(AccountsViewModel),
                IsEnabled = true
            });

            MenuItems.Add(new MenuItem
            {
                Title = "Transfers",
                Icon = new AwesomeIcon("\uf362", AwesomeIcon.Packages.Solid),
                ViewModelType = typeof(TransferViewModel),
                IsEnabled = true
            });

            MenuItems.Add(new MenuItem
            {
                Title = "Withdrawals",
                Icon = new AwesomeIcon("\uf53a", AwesomeIcon.Packages.Solid),
                ViewModelType = typeof(WithdrawalViewModel),
                IsEnabled = true
            });

            MenuItems.Add(new MenuItem
            {
                Title = "More",
                Icon = new AwesomeIcon("\uf35d", AwesomeIcon.Packages.Solid),
                ViewModelType = typeof(MoreViewModel),
                IsEnabled = true
            });

            MenuItems.Add(new MenuItem
            {
                Title = "Contact Us",
                Icon = new AwesomeIcon("\uf879", AwesomeIcon.Packages.Solid),
                ViewModelType = typeof(ContactUsViewModel),
                IsEnabled = true
            });
        }

        private async void OnItemSelected(MenuItem item)
        {
            if (!item.IsEnabled)
            {
                return;
            }

            await NavigationService.NavigateToAsync(item.ViewModelType);
        }

        private string GetFirstName()
        {
            //Separate Surname from rest of name
            var name = Name.Split(',')[1].Trim();

            //Separate first name from middle names
            var firstName = name.Split(' ')[0];

            //Format capital letters.
            return firstName.Substring(0, 1).ToUpper() + firstName.Substring(1, firstName.Length - 1).ToLower();
        }

    }
}