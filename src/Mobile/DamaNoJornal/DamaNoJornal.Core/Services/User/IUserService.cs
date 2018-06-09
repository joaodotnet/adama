using DamaNoJornal.Core.Models.User;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DamaNoJornal.Core.Services.User
{
    public interface IUserService
    {
        Task<UserInfo> GetUserInfoAsync(string authToken);        
    }
}
