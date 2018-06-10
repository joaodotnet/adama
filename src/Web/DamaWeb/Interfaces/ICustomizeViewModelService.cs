using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DamaWeb.ViewModels;

namespace DamaWeb.Interfaces
{
    public interface ICustomizeViewModelService
    {
        Task<CustomizeViewModel> GetCustomizeItems(int? categoryId, int? catalogItemId);
        Task SendCustomizeService(CustomizeViewModel request);
    }
}
