using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces
{
    public interface IAuthConfigRepository
    {
        Task<AuthConfig> GetAuthConfigAsync(SageApplicationType applicationId);
        Task UpdateAuthConfigAsync(SageApplicationType applicationId, string accessToken, string refreshToken);
    }
}
