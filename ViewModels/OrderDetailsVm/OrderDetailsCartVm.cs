using Restaurant.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Restaurant.ViewModels.OrderDetailsVm
{
    public class OrderDetailsCartVm
    {
        public List<ShoppingCart> AllItemsInShoppingCart { get; set; }

        public OrderHeader OrderHeader { get; set; }

        //public OrderDetailsCartVm()
        //{
        //    AllItemsInShoppingCart = new List<ShoppingCart>();
        //}
    }
}
