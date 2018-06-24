using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces
{
    public interface IAuthConfigRepository
    {
        Task<AuthConfig> GetAuthConfigAsync(DamaApplicationId applicationId);
        Task AddOrUpdateAuthConfigAsync(DamaApplicationId applicationId, string accessToken, string refreshToken);
    }
}
