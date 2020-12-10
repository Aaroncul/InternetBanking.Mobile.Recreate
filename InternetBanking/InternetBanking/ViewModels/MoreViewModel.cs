using InternetBanking.Contracts;
using InternetBanking.ViewModels.Base;
using InternetBanking.ViewModels.Components;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

using Xamarin.Essentials;
using Xamarin.Forms;

namespace InternetBanking.ViewModels
{
    public class MoreViewModel : SkinnedViewModel
    {
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

        public ICommand HelpCommand => new Command(async () =>
            await NavigationService.NavigateToAsync<ContactUsViewModel>());

        public ICommand SettingsCommand => new Command(async () =>
            await NavigationService.NavigateToAsync<SettingsViewModel>());

        public ICommand TableRowSelectedCommand => new Command<ListViewSectionItem>(async (ListViewSectionItem item) =>
        {
            if (item.Label == "Facebook")
            {
                if (await Launcher.CanOpenAsync("fb://page/" + App.Skin.FacebookId))
                {
                    await Launcher.OpenAsync("fb://page/" + App.Skin.FacebookId);
                }
                else
                {
                    await Launcher.OpenAsync(new Uri(App.Skin.FacebookUrl));
                }
            }
            else if (item.Label == "Twitter")
            {
                if (await Launcher.CanOpenAsync("twitter://user?user_id=" + App.Skin.TwitterId))
                {
                    await Launcher.OpenAsync("twitter://user?user_id=" + App.Skin.TwitterId);
                }
                else
                {
                    await Launcher.OpenAsync(new Uri(App.Skin.TwitterUrl));
                }
            }
            else if (item.Label == "Instagram")
            {
                await Browser.OpenAsync(App.Skin.InstagramUrl, BrowserLaunchMode.SystemPreferred);
            }
            else
            {
                await NavigationService.NavigateToAsync(item.ViewModelType);
            }

            SelectedSectionItem = null;
        });

        public MoreViewModel()
        {
            Title = "More";

            Sections = new ObservableCollection<ListViewSection>();

            var connectWithUsList = new List<ListViewSectionItem>
            {
                new ListViewSectionItem
                {
                    Icon = new AwesomeIcon("\uf87b", AwesomeIcon.Packages.Solid),
                    Label = "Contact a branch",
                    ViewModelType = typeof(ContactUsViewModel)
                }
            };


            if (!string.IsNullOrEmpty(App.Skin.FacebookUrl))
            {
                connectWithUsList.Add(new ListViewSectionItem
                {
                    Icon = new AwesomeIcon("\uf082", AwesomeIcon.Packages.Brands),
                    Label = "Facebook",
                    ViewModelType = null
                });
            }

            if (!string.IsNullOrEmpty(App.Skin.TwitterUrl))
            {
                connectWithUsList.Add(new ListViewSectionItem
                {
                    Icon = new AwesomeIcon("\uf081", AwesomeIcon.Packages.Brands),
                    Label = "Twitter",
                    ViewModelType = null
                });
            }

            if (!string.IsNullOrEmpty(App.Skin.InstagramUrl))
            {
                connectWithUsList.Add(new ListViewSectionItem
                {
                    Icon = new AwesomeIcon("\uf16d", AwesomeIcon.Packages.Brands),
                    Label = "Instagram",
                    ViewModelType = null
                });
            }

            var additionalInformationList = new List<ListViewSectionItem>
            {
                new ListViewSectionItem
                {
                    Icon = new AwesomeIcon("\uf05a",AwesomeIcon.Packages.Solid),
                    Label = $"About",
                    ViewModelType = typeof(AboutViewModel)
                },
                new ListViewSectionItem
                {
                    Icon = new AwesomeIcon( "\uf3ed",AwesomeIcon.Packages.Solid),
                    Label = "Privacy policy",
                    ViewModelType = typeof(PrivacyPolicyViewModel)
                }
            };

            Sections.Add(new ListViewSection(connectWithUsList)
            {
                Heading = "Connect With Us"
            });

            Sections.Add(new ListViewSection(additionalInformationList)
            {
                Heading = "Additional Information"
            });

            var appVersion = DependencyService.Get<IAppVersion>();

            Sections.Add(new ListViewSection(new List<ListViewSectionItem>())
            {
                Heading = $"Version {appVersion.GetVersion()}.{appVersion.GetBuild()}"
            });
        }
    }
}