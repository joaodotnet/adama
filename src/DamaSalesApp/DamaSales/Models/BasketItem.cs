using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DamaSales.Models
{
    //public class Basket
    //{
    //    public string CreateUser { get; set; }
    //    public DateTime CreateDate { get; set; }
    //    public BasketStateType State{ get; set; }
    //    public List<BasketItem> Items { get; set; }
    //    public decimal Total { get; set; }
    //}

    public class BasketItem
    {
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public string DisplayName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }

    //public enum BasketStateType
    //{
    //    IN_PROGRESS,
    //    DONE,
    //    CANCEL
    //}
}
