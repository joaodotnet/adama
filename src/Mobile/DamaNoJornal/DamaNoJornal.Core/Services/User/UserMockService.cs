using DamaNoJornal.Core.Models.User;
using DamaNoJornal.Core.Services.RequestProvider;
using DamaNoJornal.Core.Services.Settings;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DamaNoJornal.Core.Services.User
{
    public class UserMockService : IUserService
    {
        private readonly ISettingsService _settingsService;

        public UserMockService(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }
        private UserInfo JueUserInfo = new UserInfo
        {
            //UserId = "612843e9-0c7c-453e-b969-fb42a88bda35",
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
            //UserId = "f672e2f8-5259-4dcb-8491-ceab99973414",
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
            //UserId = "53670bd4-9c38-49c9-8dfe-b690ec4429f5",
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
            return JueUserInfo;
            //JueUserInfo.UserId = _settingsService.JueUserId;
            //switch (authToken)
            //{
            //    case GlobalSetting.JueAuthToken:
            //        return JueUserInfo;
            //    case GlobalSetting.SueAuthToken:
            //        SueUserInfo.UserId = _settingsService.SueUserId;
            //        return SueUserInfo;
            //    case GlobalSetting.SoniaAuthToken:
            //        MotherUserInfo.UserId = _settingsService.MotherUserId;
            //        return MotherUserInfo;
            //    default:
            //        return JueUserInfo;
            //}
        }
    }
}