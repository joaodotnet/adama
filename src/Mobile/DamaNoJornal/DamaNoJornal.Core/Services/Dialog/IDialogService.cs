using Acr.UserDialogs;
using System.Threading.Tasks;

namespace DamaNoJornal.Services
{
    public interface IDialogService
    {
        Task ShowAlertAsync(string message, string title, string buttonLabel);
        Task<PromptResult> ShowDialogAsync(string message, string title, string buttonOk, string buttonCancel);
    }
}
