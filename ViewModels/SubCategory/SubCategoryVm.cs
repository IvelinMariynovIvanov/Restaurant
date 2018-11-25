using Microsoft.AspNetCore.Mvc.Rendering;
using Restaurant.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace Restaurant.ViewModels.SubCategoryVm
{
    public class SubCategoryVm
    {
        public SubCategory SubCategory { get; set; }

        public List<SelectListItem> CategoryList { get; set; }
        
        [Required(ErrorMessage = "Select a Category")]
        [Display(Name = "Category")]
        public int SelectedCategoryid { get; set; }

        public List<string> SubCategoryList { get; set; }

        [Display(Name = "New SubCategory")]
        public bool IsNew { get; set; }

        public string StatusMessage { get; set; }
    }
}
