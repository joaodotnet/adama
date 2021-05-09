using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces
{
    public interface IATService
    {
        Task<bool> CreateInvoiceAsync();
    }
}
