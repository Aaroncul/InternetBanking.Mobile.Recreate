using InternetBanking.Services.API;
using InternetBanking.Services.Authentication;
using InternetBanking.Services.Dialogs;
using InternetBanking.Services.Settings;
using InternetBanking.Services.Skins;
using System;
using Unity;
using Unity.Lifetime;

namespace InternetBanking.ViewModels.Base
{
    public class ViewModelLocator
    {
        private readonly IUnityContainer _container;

        public static ViewModelLocator Instance { get; } = new ViewModelLocator();

        protected ViewModelLocator()
        {
            _container = new UnityContainer();

            _container.RegisterType<ISkinService, SkinService>();
            _container.RegisterType<IAbacusApiService, AbacusApiService>();
            _container.RegisterType<ISettingsService, SettingsService>();
            _container.RegisterType<IAuthenticationService, AuthenticationService>();
            _container.RegisterType<IDialogService, DialogService>();

            _container.RegisterType<WelcomeViewModel>();
            _container.RegisterType<LoginViewModel>();
            _container.RegisterType<ForgotPinViewModel>();
            _container.RegisterType<MainViewModel>();
            _container.RegisterType<SettingsViewModel>();
            _container.RegisterType<AccountsViewModel>();
            _container.RegisterType<AccountViewModel>();
            _container.RegisterType<MoreViewModel>();
        }

        public void Register<TInterface, T>() where T : TInterface
        {
            _container.RegisterType<TInterface, T>();
        }

        public void RegisterSingleton<TInterface, T>() where T : TInterface
        {
            _container.RegisterType<TInterface, T>(new ContainerControlledLifetimeManager());
        }

        public T Resolve<T>() where T : class
        {
            return _container.Resolve<T>();
        }

        public object Resolve(Type type)
        {
            return _container.Resolve(type);
        }

        public void Register<T>(T instance)
        {
            _container.RegisterInstance(instance);
        }
    }
}