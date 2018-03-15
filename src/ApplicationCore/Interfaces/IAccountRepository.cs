using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces
{
    public interface IAccountRepository
    {
        Task AddOrUpdateUserAddress(string userId, bool useSameAsShipping, string street, string postalCode, string city, string country, AddressType addressType);
    }
}
