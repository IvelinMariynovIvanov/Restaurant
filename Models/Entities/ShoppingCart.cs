using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Restaurant.Models.Entities
{
    public class ShoppingCart
    {
        
        public int Id { get; set; }

        public string ApplicationUserId { get; set; }

        public int MenuItemId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value greter than 0")]
        public int Count { get; set; }

        [NotMapped]
        public string StatusMessage { get; set; }


        [ForeignKey("MenuItemId")]
        public virtual MenuItem MenuItem { get; set; }

        [ForeignKey("ApplicationUserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }

    }
}
