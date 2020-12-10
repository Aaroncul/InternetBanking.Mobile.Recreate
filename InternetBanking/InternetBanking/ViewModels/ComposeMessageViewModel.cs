using InternetBanking.Dto;
using InternetBanking.Services.API;
using InternetBanking.Services.Settings;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace InternetBanking.ViewModels
{
    public class ComposeMessageViewModel : BaseViewModel
    {
        private readonly ISettingsService _settingsService;
        private readonly IAbacusApiService _abacusApiService;

        private bool _isSendEnabled;

        public bool IsSendEnabled
        {
            get { return _isSendEnabled; }
            set
            {
                _isSendEnabled = value;

                RaisePropertyChanged(() => IsSendEnabled);
            }
        }

        private string _threadTitle;

        public string ThreadTitle
        {
            get { return _threadTitle; }
            set
            {
                _threadTitle = value;

                RaisePropertyChanged(() => ThreadTitle);

                UpdateSendEnabled();
            }
        }

        private string _content;

        public string Content
        {
            get { return _content; }
            set
            {
                _content = value;

                RaisePropertyChanged(() => Content);

                UpdateSendEnabled();
            }
        }

        public ICommand SendCommand => new Command(async () =>
            await OnSendAsync());

        public ComposeMessageViewModel(IAbacusApiService abacusApiService, ISettingsService settingsService)
        {
            _abacusApiService = abacusApiService;
            _settingsService = settingsService;

            Content = "";
        }

        private void UpdateSendEnabled()
        {
            IsSendEnabled = !string.IsNullOrEmpty(ThreadTitle) &&
                            !string.IsNullOrEmpty(Content) &&
                            Content.Length <= 500 && Content.Length > 0;
        }

        private async Task OnSendAsync()
        {
            try
            {
                var messageThreadDto = new MessageThreadDto
                {
                    CustomerId = _settingsService.CustomerId,
                    MessageTypeId = 1,
                    Subject = ThreadTitle
                };

                var response = await _abacusApiService.PostAsync(
                    "messagethreads",
                    JsonConvert.SerializeObject(messageThreadDto));

                response.EnsureSuccessStatusCode();

                messageThreadDto = JsonConvert.DeserializeObject<MessageThreadDto>(
                    await response.Content.ReadAsStringAsync().ConfigureAwait(false));

                var messageDto = new MessageDto
                {
                    MesageThreadId = messageThreadDto.Id,
                    Content = Content,
                    FromCustomer = true
                };

                response = await _abacusApiService.PostAsync($"messagethreads/{messageThreadDto.Id}/messages",
                    JsonConvert.SerializeObject(messageDto));

                response.EnsureSuccessStatusCode();

                messageDto = JsonConvert.DeserializeObject<MessageDto>(
                    await response.Content.ReadAsStringAsync().ConfigureAwait(false));

                await NavigationService.NavigateToAsync<MessagesViewModel>();
                await NavigationService.RemoveLastFromBackStackAsync();
                await NavigationService.RemoveLastFromBackStackAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}