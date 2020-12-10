using InternetBanking.Models;
using InternetBanking.ViewModels.Base;
using InternetBanking.ViewModels.Components;
using InternetBanking.ViewModels.Parameters;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace InternetBanking.ViewModels
{
    public class ContactUsViewModel : SkinnedViewModel
    {
        private ObservableCollection<Branch> _branches;
        public ObservableCollection<Branch> Branches
        {
            get { return _branches; }
            set
            {
                _branches = value;
                RaisePropertyChanged(() => _branches);
            }
        }

        private string _email;
        public string Email
        {
            get { return _email; }
            set
            {
                _email = value;
                RaisePropertyChanged(() => _email);
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

        private Branch _selectedBranch { get; set; }

        public Branch SelectedBranch
        {
            get => _selectedBranch;
            set
            {
                _selectedBranch = value;
                RaisePropertyChanged(() => SelectedBranch);
            }
        }

        public ICommand SendEmailCommand => new Command(async () =>
            await SendEmail());

        public ICommand CallPhoneNumberCommand => new Command(async (branch) =>
            await CallPhoneNumber(branch as Branch));

        public ICommand TableRowSelectedCommand => new Command(async () =>
            await NavigationService.NavigateToAsync<FindBranchViewModel>(
                new FindBranchViewModelParameter(SelectedBranch)));

        public ContactUsViewModel()
        {
            Title = "Contact Us";
            _branches = new ObservableCollection<Branch>();

            //Credit unions seem to use the same email address for all branches
            for (var i = 0; i < App.Skin.Branches.Count; i++)
            {
                if (_email == null && App.Skin.Branches[i].Email != null)
                {
                    _email = App.Skin.Branches[i].Email;
                }
            }
            foreach (var branch in App.Skin.Branches)
            {
                branch.Line1 += " " + branch.Line2;
                _branches.Add(branch);
            }
        }

        private async Task SendEmail()
        {
            IsBusy = true;

            try
            {
                if (await Launcher.CanOpenAsync(new Uri($"mailto:{_email}")))
                {
                    await Launcher.OpenAsync(new Uri($"mailto:{_email}"));
                }
                else
                {
                    throw new Exception();
                }
            }
            catch
            {
                await DialogService.ShowAlertAsync("There was a problem opening your email client.", "Error", "Ok");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task CallPhoneNumber(Branch branch)
        {
            IsBusy = true;
            var number = branch.Phone.Replace(" ", "");
            try
            {
                PhoneDialer.Open(number);
            }
            catch (FeatureNotSupportedException)
            {
                await DialogService.ShowAlertAsync("Phone Dialer is not supported on this device.", "Not supported", "Ok");
            }
            catch
            {
                await DialogService.ShowAlertAsync("There was a problem opening your phone book.", "Error", "Ok");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
