using DamaApp.Core.Models.User;
using System;
using System.Threading.Tasks;

namespace DamaApp.Core.Services.User
{
    public class UserMockService : IUserService
    {
        private UserInfo JGUserInfo = new UserInfo
        {
            UserId = Guid.NewGuid().ToString(),
            Name = "João",
            LastName = "Gonçalves",
            PreferredUsername = "João",
            Email = "joaofbbg@gmail.com",
            EmailVerified = true,
            PhoneNumber = "202-555-0165",
            PhoneNumberVerified = true,
            Address = "Seattle, WA",
            Street = "120 E 87th Street",
            ZipCode = "98101",
            Country = "United States",
            State = "Seattle",
            CardNumber = "378282246310005",
            CardHolder = "American Express",
            CardSecurityNumber = "1234"
        };

        private UserInfo SusanaUserInfo = new UserInfo
        {
            UserId = Guid.NewGuid().ToString(),
            Name = "Susana",
            LastName = "Mendez",
            PreferredUsername = "Susana",
            Email = "susana.m.mendez@gmail.com",
            EmailVerified = true,
            PhoneNumber = "202-555-0165",
            PhoneNumberVerified = true,
            Address = "Seattle, WA",
            Street = "120 E 87th Street",
            ZipCode = "98101",
            Country = "United States",
            State = "Seattle",
            CardNumber = "378282246310005",
            CardHolder = "American Express",
            CardSecurityNumber = "1234"
        };

        private UserInfo SoniaUserInfo = new UserInfo
        {
            UserId = Guid.NewGuid().ToString(),
            Name = "Sônia",
            LastName = "Mendez",
            PreferredUsername = "Sônia",
            Email = "sonia.mendez.artesa@gmail.com",
            EmailVerified = true,
            PhoneNumber = "202-555-0165",
            PhoneNumberVerified = true,
            Address = "Seattle, WA",
            Street = "120 E 87th Street",
            ZipCode = "98101",
            Country = "United States",
            State = "Seattle",
            CardNumber = "378282246310005",
            CardHolder = "American Express",
            CardSecurityNumber = "1234"
        };

        public async Task<UserInfo> GetUserInfoAsync(string authToken)
        {
            await Task.Delay(10);

            if (authToken == GlobalSetting.JGToken)
                return JGUserInfo;
            else if (authToken == GlobalSetting.SusanaToken)
                return SusanaUserInfo;
            else if (authToken == GlobalSetting.SoniaToken)
                return SoniaUserInfo;

            return JGUserInfo;
        }
    }
}