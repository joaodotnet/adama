using DamaNoJornal.Core.Models.User;
using System;
using System.Threading.Tasks;

namespace DamaNoJornal.Core.Services.User
{
    public class UserMockService : IUserService
    {
        private UserInfo JueUserInfo = new UserInfo
        {
            UserId = "jue@damanojornal.com",
            Name = "João",
            LastName = "da Dama",
            PreferredUsername = "jue@damanojornal.com",
            Email = "jue@damanojornal.com",
            EmailVerified = true,
            City = "Loulé",
            Street = "Feira de Loulé",
            PostalCode = "8100",
            Country = "Portugal",
        };

        private UserInfo SueUserInfo = new UserInfo
        {
            UserId = "sue@damanojornal.com",
            Name = "Susana",
            LastName = "Mendez",
            PreferredUsername = "sue@damanojornal.com",
            Email = "sue@damanojornal.com",
            EmailVerified = true,
            City = "Loulé",
            Street = "Feira de Loulé",
            PostalCode = "8100",
            Country = "Portugal",
        };

        private UserInfo MotherUserInfo = new UserInfo
        {
            UserId = "sonia@damanojornal.com",
            Name = "Sonia",
            LastName = "Mendez",
            PreferredUsername = "sonia@damanojornal.com",
            Email = "sonia@damanojornal.com",
            EmailVerified = true,
            City = "Loulé",
            Street = "Feira de Loulé",
            PostalCode = "8100",
            Country = "Portugal",
        };

        public async Task<UserInfo> GetUserInfoAsync(string authToken)
        {
            await Task.Delay(10);
            switch (authToken)
            {
                case GlobalSetting.JueAuthToken:
                    return JueUserInfo;
                case GlobalSetting.SueAuthToken:
                    return SueUserInfo;
                case GlobalSetting.SoniaAuthToken:
                    return MotherUserInfo;
                default:
                    return JueUserInfo;
            }
        }
    }
}