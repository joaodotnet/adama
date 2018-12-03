using ApplicationCore.Interfaces;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using System.Linq;
using System.Collections.Generic;
using ApplicationCore.Specifications;
using DamaWeb.Interfaces;
using DamaWeb.ViewModels;
using DamaWeb.Extensions;

namespace DamaWeb.Services
{
    public class BasketViewModelService : IBasketViewModelService
    {
        private readonly IAsyncRepository<Basket> _basketRepository;
        private readonly IUriComposer _uriComposer;
        private readonly IRepository<CatalogItem> _itemRepository;

        public BasketViewModelService(IAsyncRepository<Basket> basketRepository,
            IRepository<CatalogItem> itemRepository,
            IUriComposer uriComposer)
        {
            _basketRepository = basketRepository;
            _uriComposer = uriComposer;
            _itemRepository = itemRepository;
        }

        public async Task<BasketViewModel> GetOrCreateBasketForUser(string userName)
        {
            var basketSpec = new BasketWithItemsSpecification(userName);
            var basket = (await _basketRepository.ListAsync(basketSpec)).LastOrDefault();

            if (basket == null)
            {
                return await CreateBasketForUser(userName);
            }
            return CreateViewModelFromBasket(basket);
        }

        private BasketViewModel CreateViewModelFromBasket(Basket basket)
        {
            var viewModel = new BasketViewModel();
            viewModel.Id = basket.Id;
            viewModel.BuyerId = basket.BuyerId;            
            viewModel.Items = basket.Items.Select(i =>
            {
                var itemModel = new BasketItemViewModel()
                {
                    Id = i.Id,
                    UnitPrice = i.UnitPrice,
                    Quantity = i.Quantity,
                    CatalogItemId = i.CatalogItemId,
                    CustomizeName = i.CustomizeName
                };
                var spec = new CatalogAttrFilterSpecification(i.CatalogItemId);
                var item = _itemRepository.GetSingleBySpec(spec);
                if (item != null)
                {
                    itemModel.PictureUrl = _uriComposer.ComposePicUri(item.PictureUri);
                    itemModel.ProductName = item.Name;

                    foreach (var attr in item.CatalogAttributes)
                    {
                        if ((i.CatalogAttribute1.HasValue && i.CatalogAttribute1 == attr.Id) ||
                            (i.CatalogAttribute2.HasValue && i.CatalogAttribute2 == attr.Id) ||
                            (i.CatalogAttribute3.HasValue && i.CatalogAttribute3 == attr.Id))
                            itemModel.Attributes.Add(new AttributeViewModel
                            {
                                Name = attr.Name,
                                Label = EnumHelper<AttributeType>.GetDisplayValue(attr.Type)
                            });
                    }
                }

                return itemModel;
            }).ToList();
            //Shipping Cost
            decimal shippingCost = 0;
            foreach (var item in viewModel.Items)
            {
                var spec = new CatalogTypeFilterSpecification(item.CatalogItemId);
                var catalogItem = _itemRepository.GetSingleBySpec(spec);                
                if (catalogItem != null && catalogItem.CatalogType.ShippingCost > shippingCost)
                    shippingCost = catalogItem.CatalogType.ShippingCost;
            }
            viewModel.DefaultShippingCost = shippingCost;

            return viewModel;
        }

        private async Task<BasketViewModel> CreateBasketForUser(string userId)
        {
            var basket = new Basket() { BuyerId = userId };
            await _basketRepository.AddAsync(basket);

            return new BasketViewModel()
            {
                BuyerId = basket.BuyerId,
                Id = basket.Id,
                Items = new List<BasketItemViewModel>()
            };
        }
    }
}
