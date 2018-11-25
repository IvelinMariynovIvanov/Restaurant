using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Restaurant.Data;
using Restaurant.Models.Entities;
using Restaurant.Utility;
using Restaurant.ViewModels.SubCategoryVm;

namespace Restaurant.Controllers
{
    [Authorize(Roles = StaticDetails.AdminEndUser)]
    public class SubCategoriesController : Controller
    {
        private readonly ApplicationDbContext _db;

        [TempData]
        private string StatusMessage { get; set; }

        public SubCategoriesController(ApplicationDbContext db)
        {
            _db = db;    
        }

        public async Task<IActionResult> Index()
        {
            var subCategories = await _db.SubCategories.Include(s => s.Category).ToListAsync();

            return View(subCategories);
        }

        public async Task<IActionResult> Create()
        {
            var categories = await _db.Categories.ToListAsync();

            SubCategoryVm subCategoryVm = new SubCategoryVm
            {
                CategoryList = categories.Select(c => new SelectListItem { Text = c.Name, Value = c.Id.ToString()}).ToList(),             
                SubCategory = new SubCategory(),
                SubCategoryList = await _db.SubCategories.OrderBy(s => s.Name).Select(s => s.Name).Distinct().ToListAsync(),

            };

           // subCategoryVm.SelectedCategoryid = -6;
            subCategoryVm.CategoryList.Insert(0, new SelectListItem { Text = "--Select Category--", Value = "" });

            return View(subCategoryVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubCategoryVm model)
        {
        
            if(ModelState.IsValid)
            {
                var doesSubCategoryExists = await _db.SubCategories.Where(s => s.Name == model.SubCategory.Name).CountAsync();
                var doesPairSubcategoryAndcategoryExists = 
                                                           await _db.SubCategories.Where
                                                           (s => s.Name == model.SubCategory.Name &&
                                                            s.CategoryId == model.SubCategory.CategoryId).CountAsync();
                if(doesSubCategoryExists >0 && model.IsNew)
                {
                    // error
                    StatusMessage = "Error : SubCategory alredy exists";
                }
                else
                {
                    if(doesSubCategoryExists == 0 && !model.IsNew)
                    {
                        StatusMessage = "Error : SubCategory does not exist";
                    }
                    else
                    {
                        if(doesPairSubcategoryAndcategoryExists > 0)
                        {
                            StatusMessage = "Error: Category and Subcategory already exist";
                        }
                        else
                        {
                            _db.Add(model.SubCategory);
                            await _db.SaveChangesAsync();

                            return RedirectToAction(nameof(Index));
                        }
                    }
                }

               // return View(subCategoryVm);
            }

            var categories = await _db.Categories.ToListAsync();

            SubCategoryVm subCategoryVm = new SubCategoryVm
            {
                CategoryList = categories.Select(c => new SelectListItem { Text = c.Name, Value = c.Id.ToString() }).ToList(),

                SubCategory = model.SubCategory,
                SubCategoryList = await _db.SubCategories.OrderBy(s => s.Name).Select(s => s.Name).Distinct().ToListAsync(),

                StatusMessage = StatusMessage

            };
            return View(subCategoryVm);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var subCategoryFromDb = await _db.SubCategories.SingleOrDefaultAsync(s => s.Id == id);

            if (subCategoryFromDb == null)
                return NotFound();

            SubCategoryVm subCategoryVm = new SubCategoryVm()
            {
                CategoryList = await _db.Categories.
                                Select(c => new SelectListItem { Text = c.Name, Value = c.Id.ToString() }).ToListAsync(),

                SubCategoryList = await _db.SubCategories.Select(c => c.Name).Distinct().ToListAsync(),
                SubCategory = subCategoryFromDb
            };

            return View(subCategoryVm);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Edit(int id, SubCategoryVm model)
        {
            if(ModelState.IsValid)
            {
                var doesSubCategoryExists = await _db.SubCategories.Where(s => s.Name == model.SubCategory.Name).CountAsync();
                var doesPairSubcategoryAndcategoryExists =
                                                           await _db.SubCategories.Where
                                                           (s => s.Name == model.SubCategory.Name &&
                                                            s.CategoryId == model.SubCategory.CategoryId).CountAsync();
                if(doesSubCategoryExists == 0)
                {
                    StatusMessage = "Error: SubCategory does not exist. You can not add it here";
                }
                else
                {
                    if(doesPairSubcategoryAndcategoryExists > 0)
                    {
                        StatusMessage = "Error: Category and SubCategory alredy exists";
                    }
                    else
                    {
                        var subCategoryFromDb = await _db.SubCategories.FindAsync(id);

                        subCategoryFromDb.Name = model.SubCategory.Name;
                        subCategoryFromDb.CategoryId = model.SubCategory.CategoryId;

                        await _db.SaveChangesAsync();

                        return RedirectToAction(nameof(Index));
                    }
                }
               
            }
            var categories = await _db.Categories.ToListAsync();

            SubCategoryVm subCategoryVm = new SubCategoryVm
            {
                CategoryList = categories.Select(c => new SelectListItem { Text = c.Name, Value = c.Id.ToString() }).ToList(),

                SubCategory = model.SubCategory,
                SubCategoryList = await _db.SubCategories.OrderBy(s => s.Name).Select(s => s.Name).Distinct().ToListAsync(),

                StatusMessage = StatusMessage

            };
            return View(subCategoryVm);
        }

        public async Task<IActionResult>Details(int? id)
        {
            if (id == null)
                return NotFound();

            var subCategory = await _db.SubCategories.Include(c=>c.Category).SingleOrDefaultAsync(s => s.Id == id);

            if (subCategory == null)
                return NotFound();

            return View(subCategory);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var subCategory = await _db.SubCategories.Include(c => c.Category).SingleOrDefaultAsync(s => s.Id == id);

            if (subCategory == null)
                return NotFound();

            return View(subCategory);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeletePost(int? id)
        {
            var subCategory = await _db.SubCategories.SingleOrDefaultAsync(s => s.Id == id);

            _db.SubCategories.Remove(subCategory);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}