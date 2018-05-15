using System.Collections.Generic;

namespace Dama.API.ViewModels
{
    public class BasketViewModel
    {
        public string BuyerId { get; set; }
        public List<BasketItemViewModel> Items { get; set; }
    }
}
