using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Restaurant.Models.Entities;

namespace Restaurant.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public string FristName { get; set; }

        public string Lastname { get; set; }

        public string LockoutReason { get; set; }

        //public virtual ShoppingCart ShoppingCart { get; set; }
    }
}
