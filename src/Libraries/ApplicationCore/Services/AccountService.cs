using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;

        public AccountService(IAccountRepository repository)
        {
            this._accountRepository = repository;
        }
        public async Task AddOrUpdateUserAddress(string userId, bool useSameAsShipping, string street, string postalCode, string city, string country, AddressType addressType = AddressType.SHIPPING)
        {
            await _accountRepository.AddOrUpdateUserAddress(userId, useSameAsShipping, street, postalCode, city, country, addressType);
        }
    }
}
