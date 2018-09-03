using Acr.UserDialogs;
using System.Threading.Tasks;

namespace DamaNoJornal.Services
{
    public class DialogService : IDialogService
    {
        public Task ShowAlertAsync(string message, string title, string buttonLabel)
        {
            return UserDialogs.Instance.AlertAsync(message, title, buttonLabel);
        }

        public Task<bool> ShowDialogAsync(string message, string title, string buttonOk, string buttonCancel)
        {
            return UserDialogs.Instance.ConfirmAsync(message, title, buttonOk, buttonCancel);
        }

        public Task<PromptResult> ShowDialogPassword(string message, string title, string buttonOk, string buttonCancel)
        {
            return UserDialogs.Instance.PromptAsync(message, title, buttonOk, buttonCancel,string.Empty, InputType.Password);
        }

        public Task<string> ShowPromptAsync(string title, string buttonCancel,string[] options)
        {
            return UserDialogs.Instance.ActionSheetAsync(title, buttonCancel, null, null, options);
        }

        public void ShowToastMessage(string message)
        {
            UserDialogs.Instance.Toast(message);
        }
    }
}
