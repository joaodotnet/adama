using DamaNoJornal.Core.Models.Catalog;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace DamaNoJornal.Core.Services.Catalog
{
    public interface ICatalogService
    {
        Task<ObservableCollection<CatalogBrand>> GetCatalogCategoryAsync();
        Task<ObservableCollection<CatalogItem>> FilterAsync(int? catalogBrandId, int? catalogTypeId);
        Task<ObservableCollection<CatalogType>> GetCatalogTypeAsync(int? catalogCategoryId = null);
        Task<ObservableCollection<CatalogItem>> GetCatalogAsync();
    }
}
