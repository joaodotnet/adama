using DamaApp.Core.Models.User;
using System.Threading.Tasks;

namespace DamaApp.Core.Services.User
{
    public interface IUserService
    {
        Task<UserInfo> GetUserInfoAsync(string authToken);
    }
}
