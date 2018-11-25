using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Restaurant.Models.Entities
{
    public class OrderHeader
    {
        [Required]
        public int Id { get; set; }

        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        [Required]
        public decimal OrderTotalPrice { get; set; }

        [Required]
        public DateTime PickUpTime { get; set; }

        public string CouponCode { get; set; }

        public string OrderStatus { get; set; }

        public string Comments { get; set; }

        public virtual ICollection<OrderDetails> OrderDetails { get; set; }

        public virtual ICollection<MenuItem> MenuItems { get; set; }

    }
}
