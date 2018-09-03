using DamaNoJornal.Core.Models.Token;
using DamaNoJornal.Core.Models.User;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DamaNoJornal.Core.Services.Identity
{
    public interface IIdentityService
    {
        string CreateAuthorizationRequest();
        string CreateLogoutRequest(string token);
        Task<UserToken> GetTokenAsync(string code);
        Task<List<UserInfo>> GetStaffUsersAsync();
        Task<UserInfo> LoginStaffAsync(string username, string password);
    }
}