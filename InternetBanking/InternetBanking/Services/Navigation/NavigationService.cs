using InternetBanking.Services.Authentication;
using InternetBanking.ViewModels;
using InternetBanking.ViewModels.Base;
using InternetBanking.Views;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace InternetBanking.Services.Navigation
{
    public class NavigationService : INavigationService
    {
        private readonly IAuthenticationService _authenticationService;

        protected readonly Dictionary<Type, Type> Mappings;

        public NavigationService(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;

            Mappings = new Dictionary<Type, Type>();

            CreatePageViewModelMappings();
        }

        private void CreatePageViewModelMappings()
        {
            Mappings.Clear();

            Mappings.Add(typeof(AboutViewModel), typeof(AboutView));
            Mappings.Add(typeof(AccountViewModel), typeof(AccountView));
            Mappings.Add(typeof(AccountsViewModel), typeof(AccountsView));
            Mappings.Add(typeof(AddBankAccountViewModel), typeof(AddBankAccountView));
            Mappings.Add(typeof(AddedBankAccountsViewModel), typeof(AddedBankAccountsView));

            Mappings.Add(typeof(ContactUsViewModel), typeof(ContactUsView));
            Mappings.Add(typeof(ChangePinViewModel), typeof(ChangePinView));
            Mappings.Add(typeof(FindBranchViewModel), typeof(FindBranchView));
            Mappings.Add(typeof(ForgotPinViewModel), typeof(ForgotPinView));

            Mappings.Add(typeof(LoginViewModel), typeof(LoginView));
            Mappings.Add(typeof(MainViewModel), typeof(MainView));
            Mappings.Add(typeof(MoreViewModel), typeof(MoreView));

            Mappings.Add(typeof(PrivacyPolicyViewModel), typeof(PrivacyPolicyView));
            Mappings.Add(typeof(SettingsViewModel), typeof(SettingsView));
            Mappings.Add(typeof(TransferViewModel), typeof(TransferView));
            Mappings.Add(typeof(WelcomeViewModel), typeof(WelcomeView));
            Mappings.Add(typeof(WithdrawalViewModel), typeof(WithdrawalView));

        }

        public Task InitializeAsync()
        {
            if (_authenticationService.IsAuthenticated)
            {
                return NavigateToAsync<LoginViewModel>();
            }

            return NavigateToAsync<WelcomeViewModel>();
        }

        public Task NavigateToAsync<TViewModel>() where TViewModel : BaseViewModel
        {
            return InternalNavigateToAsync(typeof(TViewModel), null);
        }

        public Task NavigateToAsync<TViewModel>(object parameter) where TViewModel : BaseViewModel
        {
            return InternalNavigateToAsync(typeof(TViewModel), parameter);
        }

        public Task NavigateToAsync(Type viewModelType)
        {
            return InternalNavigateToAsync(viewModelType, null);
        }

        public Task NavigateToAsync(Type viewModelType, object parameter)
        {
            return InternalNavigateToAsync(viewModelType, parameter);
        }

        public async Task NavigateBackAsync()
        {
            if (Application.Current.MainPage is MainView)
            {
                var mainPage = Application.Current.MainPage as MainView;
                await mainPage.Detail.Navigation.PopAsync();
            }
            else if (Application.Current.MainPage != null)
            {
                await Application.Current.MainPage.Navigation.PopAsync();
            }
        }

        public Task RemoveLastFromBackStackAsync()
        {
            if (Application.Current.MainPage is MainView mainPage)
            {
                mainPage.Detail.Navigation.RemovePage(
                    mainPage.Detail.Navigation.NavigationStack[mainPage.Detail.Navigation.NavigationStack.Count - 2]);
            }

            return Task.FromResult(true);
        }

        protected virtual async Task InternalNavigateToAsync(Type viewModelType, object parameter)
        {
            Page page = CreateAndBindPage(viewModelType, parameter);

            if (page is MainView)
            {
                Application.Current.MainPage = page;
            }
            else if (page is LoginView)
            {
                Application.Current.MainPage = new CustomNavigationView(page);
            }
            else if (Application.Current.MainPage is MainView)
            {
                var mainPage = Application.Current.MainPage as MainView;

                if (mainPage.Detail is CustomNavigationView navigationPage)
                {
                    await navigationPage.PushAsync(page);
                }
                else
                {
                    navigationPage = new CustomNavigationView(page);
                    mainPage.Detail = navigationPage;
                }

                mainPage.IsPresented = false;
            }
            else
            {
                if (Application.Current.MainPage is CustomNavigationView navigationPage)
                {
                    await navigationPage.PushAsync(page);
                }
                else
                {
                    Application.Current.MainPage = new CustomNavigationView(page);
                }
            }

            await (page.BindingContext as BaseViewModel).InitializeAsync(parameter);
        }

        protected Type GetPageTypeForViewModel(Type viewModelType)
        {
            if (!Mappings.ContainsKey(viewModelType))
            {
                throw new KeyNotFoundException($"No map for ${viewModelType} was found on navigation mappings");
            }

            return Mappings[viewModelType];
        }

        protected Page CreateAndBindPage(Type viewModelType, object parameter)
        {
            Type pageType = GetPageTypeForViewModel(viewModelType);

            if (pageType == null)
            {
                throw new Exception($"Mapping type for {viewModelType} is not a page");
            }

            Page page = Activator.CreateInstance(pageType) as Page;
            BaseViewModel viewModel = ViewModelLocator.Instance.Resolve(viewModelType) as BaseViewModel;

            page.BindingContext = viewModel;

            if (page is IViewWithParameter)
            {
                ((IViewWithParameter)page).InitializeWith(parameter);
            }

            return page;
        }

    }
}