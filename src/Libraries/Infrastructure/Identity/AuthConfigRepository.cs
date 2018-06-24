using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Identity
{
    public class AuthConfigRepository : IAuthConfigRepository
    {
        private readonly AppIdentityDbContext _context;

        public AuthConfigRepository(AppIdentityDbContext context)
        {
            _context = context;
        }
        public async Task AddOrUpdateAuthConfigAsync(DamaApplicationId applicationId, string accessToken, string refreshToken)
        {
            var appConfig = await _context.AuthConfigs.FindAsync(applicationId);
            if(appConfig == null)
            {
                _context.AuthConfigs.Add(new AuthConfig
                {
                    ApplicationId = applicationId,
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                });
            }
            else
            {
                appConfig.AccessToken = accessToken;
                appConfig.RefreshToken = refreshToken;
            }
            await _context.SaveChangesAsync();
        }

        public async Task<AuthConfig> GetAuthConfigAsync(DamaApplicationId applicationId)
        {
            return await _context.AuthConfigs.FindAsync(applicationId);
        }
    }
}
