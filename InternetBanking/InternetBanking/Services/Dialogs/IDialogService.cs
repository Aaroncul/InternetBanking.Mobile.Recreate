using Acr.UserDialogs;
using System.Threading.Tasks;

namespace InternetBanking.Services.Dialogs
{
    public interface IDialogService
    {
        Task ShowAlertAsync(string message, string title, string buttonLabel);

        Task<bool> ShowConfirmAsync(string message, string title, string okLabel, string cancelLabel = "Cancel");

        void ShowLoading(string title = null, MaskType? maskType = null);

        void HideLoading();

        Task<string> ShowTwoFactorDialog(string message);
    }
}
