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
        public async Task UpdateAuthConfigAsync(SageApplicationType applicationId, string accessToken, string refreshToken)
        {
            var appConfig = await _context.AuthConfigs.FindAsync(applicationId);
            if(appConfig != null)
            {               
                appConfig.AccessToken = accessToken;
                appConfig.RefreshToken = refreshToken;
                await _context.SaveChangesAsync();
            }
            
        }

        public async Task<AuthConfig> GetAuthConfigAsync(SageApplicationType applicationId)
        {
            return await _context.AuthConfigs.FindAsync(applicationId);
        }
    }
}
