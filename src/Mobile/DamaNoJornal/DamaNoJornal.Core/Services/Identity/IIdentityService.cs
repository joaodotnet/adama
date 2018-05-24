using DamaNoJornal.Core.Models.Token;
using System.Threading.Tasks;

namespace DamaNoJornal.Core.Services.Identity
{
    public interface IIdentityService
    {
        string CreateAuthorizationRequest();
        string CreateLogoutRequest(string token);
        Task<UserToken> GetTokenAsync(string code);
    }
}