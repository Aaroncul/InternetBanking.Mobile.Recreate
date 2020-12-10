using InternetBanking.Database.Entities;
using InternetBanking.Dto;
using InternetBanking.Services.API;
using InternetBanking.Services.Settings;
using InternetBanking.ViewModels.Base;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace InternetBanking.ViewModels
{
    public class MessagesViewModel : SkinnedViewModel
    {
        private readonly IAbacusApiService _abacusApiService;
        private readonly ISettingsService _settingsService;

        private bool _isRefreshing;

        public bool IsRefreshing
        {
            get { return _isRefreshing; }
            set
            {
                _isRefreshing = value;
                RaisePropertyChanged(() => IsRefreshing);
            }
        }

        private ObservableCollection<MessageThread> _messageThreads;

        public ObservableCollection<MessageThread> MessageThreads
        {
            get { return _messageThreads; }
            set
            {
                _messageThreads = value;
                RaisePropertyChanged(() => MessageThreads);
            }
        }

        private MessageThread _selectedThread;

        public MessageThread SelectedThread
        {
            get { return _selectedThread; }
            set
            {
                _selectedThread = value;
                RaisePropertyChanged(() => SelectedThread);
            }
        }

        public ICommand RefreshCommand => new Command(async () =>
            await OnPullToRefreshAsync(true));

        public ICommand ComposeCommand => new Command(async () =>
            await NavigationService.NavigateToAsync<ComposeMessageViewModel>());

        public MessagesViewModel(IAbacusApiService abacusApiService, ISettingsService settingsService)
        {
            _abacusApiService = abacusApiService;
            _settingsService = settingsService;

            Title = "Messages";

            MessageThreads = new ObservableCollection<MessageThread>();

            /*
            ThreadPool.QueueUserWorkItem(async (object state) =>
            {
                var messageThreads = await App.Database.GetMessageThreadsAsync();

                var observable = new ObservableCollection<MessageThread>();

                foreach (var messageThread in messageThreads)
                {
                    observable.Add(messageThread);
                }

                MessageThreads = observable;
            });
            */

            ThreadPool.QueueUserWorkItem(async (object state) =>
            {
                await OnPullToRefreshAsync(false);
            });
        }

        public override async Task InitializeAsync(object navigationData)
        {
            Title = "Messages";

            /*
            var messageThreads = await App.Database.GetMessageThreadsAsync();

            var observable = new ObservableCollection<MessageThread>();

            foreach (var messageThread in messageThreads)
            {
                observable.Add(messageThread);
            }

            MessageThreads = observable;

            ThreadPool.QueueUserWorkItem(async (object state) =>
            {
                await OnPullToRefreshAsync(false);
            });
            */

            await base.InitializeAsync(navigationData);
        }

        private async Task OnPullToRefreshAsync(bool showLoading)
        {
            try
            {
                IsRefreshing |= showLoading;

                var response = await _abacusApiService.GetAsync(
                    $"customers/{_settingsService.CustomerId}/messagethreads?sort=opendate&ascending=false&paged=false")
                    .ConfigureAwait(false);

                response.EnsureSuccessStatusCode();

                var dtos = JsonConvert.DeserializeObject<List<MessageThreadDto>>(
                    await response.Content.ReadAsStringAsync().ConfigureAwait(false));

                var messageThreads = dtos.Select(x => new MessageThread
                {
                    Id = x.Id,
                    Created = x.OpenDate ?? DateTime.Now,
                    Subject = x.Subject,
                    MessageTypeId = x.MessageTypeId,
                    Closed = x.Closed ?? false,
                    ClosedDate = x.Closed.HasValue ? x.ClosedDate : null
                });

                await Synchronize(messageThreads);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                IsRefreshing &= !showLoading;
            }
        }

        private Task Synchronize(IEnumerable<MessageThread> remoteEntities)
        {
            throw new Exception();
            /*
            var comparator = new MessageThreadComparator();

            var entitiesToDelete = _messageThreads.Where(x => remoteEntities.All(y => y.Id != x.Id)).ToList();
            var entitiesToAdd = remoteEntities.Except(_messageThreads, comparator).ToList();

            foreach (var messageThread in entitiesToDelete)
            {
                await App.Database.DeleteMessageThreadAsync(messageThread);
            }

            foreach (var messageThread in entitiesToAdd)
            {
                await App.Database.SaveMessageThreadAsync(messageThread);
            }

            var observable = new ObservableCollection<MessageThread>();

            foreach (var messageThread in await App.Database.GetMessageThreadsAsync())
            {
                observable.Add(messageThread);
            }

            MessageThreads = observable;
            */
        }
    }
}