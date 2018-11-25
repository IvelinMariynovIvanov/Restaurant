using Microsoft.AspNetCore.Mvc.Rendering;
using Restaurant.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Restaurant.ViewModels.MenuItemVm
{
    public class MenuItemVm
    {
        public MenuItem MenuItem { get; set; }

        public List<SelectListItem> CategoryCollection { get; set; }

        public List<SelectListItem> SubCategoryCollection { get; set; }

        public string StatusMessage { get; set; }

    }
}
