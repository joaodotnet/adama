using Acr.UserDialogs;
using System.Threading.Tasks;

namespace DamaNoJornal.Services
{
    public interface IDialogService
    {
        Task ShowAlertAsync(string message, string title, string buttonLabel);
        Task<bool> ShowDialogAsync(string message, string title, string buttonOk, string buttonCancel);
        Task<string> ShowPromptAsync(string title, string buttonCancel, string[] options);
    }
}
