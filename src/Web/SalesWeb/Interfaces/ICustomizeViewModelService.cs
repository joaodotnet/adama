using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SalesWeb.ViewModels;

namespace SalesWeb.Interfaces
{
    public interface ICustomizeViewModelService
    {
        Task<CustomizeViewModel> GetCustomizeItems(int? categoryId, int? catalogItemId);
        Task SendCustomizeService(CustomizeViewModel request);
    }
}
