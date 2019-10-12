using ApplicationCore.DTOs;
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
        public async Task AddOrUpdateUserAddress(string userId, string name, int? taxNumber, string contactNumber, bool useSameAsShipping, string street, string postalCode, string city, string country, AddressType addressType = AddressType.SHIPPING)
        {
            await _accountRepository.AddOrUpdateUserAddress(userId, name, taxNumber, contactNumber, useSameAsShipping, street, postalCode, city, country, addressType);
        }

        public async Task<List<UserAddressDTO>> GetUserAddress(string userId)
        {
            return await _accountRepository.GetUserAddressAsync(userId);
        }
    }
}
