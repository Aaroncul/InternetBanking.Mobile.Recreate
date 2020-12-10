using Acr.UserDialogs;
using System.Threading;
using System.Threading.Tasks;

namespace InternetBanking.Services.Dialogs
{
    public class DialogService : IDialogService
    {


        public async Task ShowAlertAsync(string message, string title, string buttonLabel)
        {
            var config = new AlertConfig
            {
                Message = message,
                Title = title,
                OkText = buttonLabel
            };

            await UserDialogs.Instance.AlertAsync(config);

            return;
        }

        public Task<bool> ShowConfirmAsync(string message, string title, string okLabel, string cancelLabel = "Cancel")
        {
            var config = new ConfirmConfig
            {
                Message = message,
                Title = title,
                OkText = okLabel,
                CancelText = cancelLabel
            };

            return UserDialogs.Instance.ConfirmAsync(config, CancellationToken.None);
        }

        public void ShowLoading(string title = null, MaskType? maskType = null)
        {
            UserDialogs.Instance.ShowLoading(title, maskType);
        }

        public void HideLoading()
        {
            UserDialogs.Instance.HideLoading();
        }

        public async Task<string> ShowTwoFactorDialog(string message)
        {
            var config = new PromptConfig
            {
                Message = message,
                Title = "User verification",
                InputType = InputType.Number,
                OkText = "Verify",
                MaxLength = 6,
                OnTextChanged = args =>
                {
                    args.IsValid = args.Value.Length == 6;
                }
            };

            var result = await UserDialogs.Instance.PromptAsync(config, CancellationToken.None);

            return result.Text;
        }
    }
}
