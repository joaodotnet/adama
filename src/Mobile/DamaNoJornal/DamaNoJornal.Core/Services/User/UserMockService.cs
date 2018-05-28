using DamaNoJornal.Core.Models.User;
using System;
using System.Threading.Tasks;

namespace DamaNoJornal.Core.Services.User
{
    public class UserMockService : IUserService
    {
        private UserInfo MockUserInfo = new UserInfo
        {
            UserId = "joaofbbg@gmail.com",
            Name = "Jhon",
            LastName = "Doe",
            PreferredUsername = "Jdoe",
            Email = "joaofbbg@gmail.com",
            EmailVerified = true,
            PhoneNumber = "202-555-0165",
            PhoneNumberVerified = true,
            Address = "Seattle, WA",
            Street = "Feira de Loulé",
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
            return MockUserInfo;
        }
    }
}