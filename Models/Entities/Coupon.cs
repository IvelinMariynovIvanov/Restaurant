using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Restaurant.Models.Entities
{
    public class Coupon
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public CouponType CouponType { get; set; }

        [Required]
        public decimal Discount { get; set; }

        [Required]
        public decimal MinAmount { get; set; }

        public byte[] Picture { get; set; }

        [Required]
        public bool IsActive { get; set; }
    }
}
