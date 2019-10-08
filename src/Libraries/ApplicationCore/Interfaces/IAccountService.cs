using ApplicationCore.DTOs;
using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces
{
    public interface IAccountService
    {
        Task AddOrUpdateUserAddress(string userId, string name, int? taxNumber, string contactNumber, bool useSameAsShipping, string street, string postalCode, string city, string country, AddressType addressType = AddressType.SHIPPING);
        Task<List<UserAddressDTO>> GetUserAddress(string userId);
    }
}
