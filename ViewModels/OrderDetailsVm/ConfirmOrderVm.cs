using Restaurant.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Restaurant.ViewModels.OrderDetailsVm
{
    public class ConfirmOrderVm
    {
        public OrderHeader OrderHeader { get; set; }

        public List<OrderDetails> OrderDetailsList { get; set; }
    }
}
