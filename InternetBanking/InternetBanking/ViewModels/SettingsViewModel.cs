using InternetBanking.Contracts;
using InternetBanking.Services.Authentication;
using InternetBanking.Services.Settings;
using InternetBanking.ViewModels.Base;
using InternetBanking.ViewModels.Components;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
namespace InternetBanking.ViewModels
{
    public class SettingsViewModel : SkinnedViewModel
    {
        private readonly ISettingsService _settingsService;
        private readonly IAuthenticationService _authenticationService;

        private ObservableCollection<ListViewSection> _sections;

        public ObservableCollection<ListViewSection> Sections
        {
            get { return _sections; }
            set
            {
                _sections = value;
                RaisePropertyChanged(() => _sections);
            }
        }

        private ListViewSectionItem _selectedSectionItem;

        public ListViewSectionItem SelectedSectionItem
        {
            get { return _selectedSectionItem; }
            set
            {
                _selectedSectionItem = value;
                RaisePropertyChanged(() => SelectedSectionItem);
            }
        }

        public string Name
        {
            get { return _settingsService.Name; }
            set
            {
                _settingsService.Name = value;
                RaisePropertyChanged(() => Name);
            }
        }

        public string FirstName
        {
            get
            {
                return GetFirstName();
            }
        }

        public string Email
        {
            get { return _settingsService.Email; }
            set
            {
                _settingsService.Email = value;
                RaisePropertyChanged(() => Email);
            }
        }

        public string Mobile
        {
            get { return _settingsService.Mobile; }
            set
            {
                _settingsService.Mobile = value;
                RaisePropertyChanged(() => Mobile);
            }
        }

        public string DateOfBirth
        {
            get
            {
                return _settingsService.DateOfBirth.HasValue
                    ? _settingsService.DateOfBirth.Value.ToString("dd/MM/yyyy")
                    : string.Empty;
            }
            set
            {
                _settingsService.DateOfBirth = DateTime.Parse(value);
                RaisePropertyChanged(() => DateOfBirth);
            }
        }

        public bool RememberPin
        {
            get { return _settingsService.RememberPin; }
            set
            {
                _settingsService.RememberPin = value;
                RaisePropertyChanged(() => RememberPin);
            }
        }
        public ICommand ChangePinCommand => new Command(async () => await OnChangePinAsync());
        public ICommand LogoutCommand => new Command(async () => await OnLogoutAsync());

        public ICommand NavigateToAddedBankAccountsCommand => new Command(async () => await NavigateToAddedBankAccounts());

        public ICommand NavigateToAddBankAccountCommand => new Command(async () => await NavigateToAddBankAccount());

        public SettingsViewModel(ISettingsService settingsService, IAuthenticationService authenticationService)
        {
            Title = "Profile";
            _settingsService = settingsService;
            _authenticationService = authenticationService;

            Sections = new ObservableCollection<ListViewSection>();

            if (App.Skin.EnableBankAccounts)
            {
                var bankAccountList = new List<ListViewSectionItem>
                {
                    new ListViewSectionItem
                    {
                        Icon = new AwesomeIcon("\uf19c",AwesomeIcon.Packages.Solid),
                        Label = $"View All",
                        ViewModelType = typeof(AddedBankAccountsViewModel)
                    },
                    new ListViewSectionItem
                    {
                        Icon = new AwesomeIcon( "\uf067",AwesomeIcon.Packages.Solid),
                        Label = "Add Bank Account",
                        ViewModelType = typeof(AddBankAccountViewModel)
                    }
                };

                Sections.Add(new ListViewSection(bankAccountList)
                {
                    Heading = "Bank Accounts"
                });
            }
            
            var securityList = new List<ListViewSectionItem>
            {
                new ListViewSectionItem
                {
                    Icon = new AwesomeIcon("\uf023",AwesomeIcon.Packages.Solid),
                    Label = $"Change PIN",
                    ViewModelType = typeof(ChangePinViewModel)
                },
                new ListViewSectionItem
                {
                    Icon = new AwesomeIcon( "\uf2f5",AwesomeIcon.Packages.Solid),
                    Label = "Log Out"
                }
            };

            Sections.Add(new ListViewSection(securityList)
            {
                Heading = "Security"
            });
        }

        public ICommand TableRowSelectedCommand => new Command<ListViewSectionItem>(async (ListViewSectionItem item) =>
        {
            if (item.Label == "Log Out")
            {
                await OnLogoutAsync();
            }
            else
            {
                await NavigationService.NavigateToAsync(item.ViewModelType);
            }

            SelectedSectionItem = null;
        });

        private string GetFirstName()
        {
            //Separate Surname from rest of name
            var name = Name.Split(',')[1].Trim();

            //Separate first name from middle names
            var firstName = name.Split(' ')[0];

            //Format capital letters.
            return firstName.Substring(0, 1).ToUpper() + firstName.Substring(1, firstName.Length - 1).ToLower();
        }

        private async Task OnChangePinAsync()
        {
            await NavigationService.NavigateToAsync<ChangePinViewModel>();
        }

        private async Task OnLogoutAsync()
        {
            var wantsToSignOut = await DialogService.ShowConfirmAsync(
                "Are you sure?",
                "Log Out",
                "Yes");

            if (wantsToSignOut)
            {
                await _authenticationService.LogoutAsync();
                await NavigationService.NavigateToAsync<LoginViewModel>();
            }
        }

        private async Task NavigateToAddedBankAccounts()
        {
            await NavigationService.NavigateToAsync(typeof(AddedBankAccountsViewModel));
        }

        private async Task NavigateToAddBankAccount()
        {
            await NavigationService.NavigateToAsync(typeof(AddBankAccountViewModel));
        }
    }
}