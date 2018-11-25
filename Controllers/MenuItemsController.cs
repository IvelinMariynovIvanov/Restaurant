using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Restaurant.Data;
using Restaurant.Models.Entities;
using Restaurant.Utility;
using Restaurant.ViewModels.MenuItemVm;

namespace Restaurant.Controllers
{
    [Authorize(Roles = StaticDetails.AdminEndUser)]
    public class MenuItemsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IHostingEnvironment _hostingEnv;

        // retrive from anu post request and send to the view
        [BindProperty]
        private MenuItemVm MenuItemVm { get; set; }

        public MenuItemsController(ApplicationDbContext db, IHostingEnvironment hostingEnv)
        {
            _db = db;
            _hostingEnv = hostingEnv;

            MenuItemVm = new MenuItemVm
            {
                MenuItem = new MenuItem(),
                CategoryCollection =  _db.Categories. Select(i => new SelectListItem { Text = i.Name, Value = i.Id.ToString() }).ToList(),
                

                // do not initialize SubCategoryCollection because we use cascade dropdown
            };
        }

        public JsonResult GetSubCategory(int categoryId)
        {
            List<SubCategory> subCategories = new List<SubCategory>();


            subCategories = _db.SubCategories.Where(s => s.CategoryId == categoryId).ToList();
            //return Json(new SelectList(subCategories, "Id", "Name"));

            return Json(subCategories.Select(s => new SelectListItem { Text = s.Name, Value = s.Id.ToString() }));
        }

        public async Task<IActionResult> Index()
        {
            var menuItems = await _db.MenuItems.Include(m => m.Category).Include(m => m.SubCategory).ToListAsync();

           
            return View(menuItems);
        }

        public IActionResult Create()
        {
            return View(MenuItemVm);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [ActionName("Create")]
        public async Task<IActionResult>CreatePost(MenuItemVm MenuItemVm) 
        {
            if (!ModelState.IsValid)
                return base.View(this.MenuItemVm);

            this.MenuItemVm.MenuItem = MenuItemVm.MenuItem;

            // use javascript in form to populate it use NAME tag - not id

            this.MenuItemVm.MenuItem.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"]);
            this.MenuItemVm.MenuItem.CategoryId = Convert.ToInt32(Request.Form["CategoryId"]);

            _db.MenuItems.Add(this.MenuItemVm.MenuItem);
            await _db.SaveChangesAsync();

            // save Image
            string wwwRootPath = _hostingEnv.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            var menuItemFromDb = _db.MenuItems.Find(this.MenuItemVm.MenuItem.Id);

            if (files[0] != null && files[0].Length > 0)
            {
                //when user uploads an image
                var uploads = Path.Combine(wwwRootPath, "images");
                var extension = files[0].FileName.Substring(files[0].FileName.LastIndexOf("."), files[0].FileName.Length - files[0].FileName.LastIndexOf("."));

                using (var filestream = new FileStream(Path.Combine(uploads, this.MenuItemVm.MenuItem.Id + extension), FileMode.Create))
                {
                    files[0].CopyTo(filestream);
                }
                menuItemFromDb.Image = @"\images\" + this.MenuItemVm.MenuItem.Id + extension;
            }
            else
            {
                //when user does not upload image
                var uploads = Path.Combine(wwwRootPath, @"images\" + StaticDetails.DefaultFoodImage);
                System.IO.File.Copy(uploads, wwwRootPath + @"\images\" + this.MenuItemVm.MenuItem.Id + ".png");
                menuItemFromDb.Image = @"\images\" + this.MenuItemVm.MenuItem.Id + ".png";
            }

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult>Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var dbMenuItem = await _db.MenuItems.Include(c => c.Category).Include(s => s.SubCategory).SingleOrDefaultAsync(m => m.Id == id);

            if (dbMenuItem == null)
                return NotFound();

            MenuItemVm.MenuItem = dbMenuItem;

            MenuItemVm.SubCategoryCollection =  
               await _db.SubCategories.Where(c=>c.CategoryId == MenuItemVm.MenuItem.CategoryId)
               .Select(i=> new SelectListItem { Text = i.Name, Value =i.Id.ToString()}).ToListAsync();

            return View(MenuItemVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>Edit(int id, MenuItemVm vm/*, List<Category> CategoryCollection*/)
        {
            MenuItemVm.MenuItem = vm.MenuItem;

            MenuItemVm.MenuItem.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"]);
            MenuItemVm.MenuItem.CategoryId = Convert.ToInt32(Request.Form["CategoryId"]);

            //var temp = (Request.Form["Model.CategoryCollection"]).ToList();
          


            if (id != MenuItemVm.MenuItem.Id)
            {
                return NotFound();
            }

            if(!ModelState.IsValid)
            {
                MenuItemVm.SubCategoryCollection = _db.SubCategories.Where(s => s.CategoryId == MenuItemVm.MenuItem.CategoryId).
                Select(i => new SelectListItem { Text = i.Name, Value = i.Id.ToString() }).ToList();

                return View(MenuItemVm);
            }
            else
            {
                try
                {
                    string webRootPath = _hostingEnv.WebRootPath;
                    var files = HttpContext.Request.Form.Files;
                    var menuItemFromDb = _db.MenuItems.Where(m => m.Id == MenuItemVm.MenuItem.Id).FirstOrDefault();

                    if (files[0].Length > 0 && files[0] != null)
                    {
                        //if user uploads a new image
                        var uploads = Path.Combine(webRootPath, "images");

                        var extension_New = files[0].FileName.Substring(files[0].FileName.LastIndexOf("."), files[0].FileName.Length - files[0].FileName.LastIndexOf("."));

                        var extension_Old = menuItemFromDb.Image.Substring(menuItemFromDb.Image.LastIndexOf("."), menuItemFromDb.Image.Length - menuItemFromDb.Image.LastIndexOf("."));

                        if (System.IO.File.Exists(Path.Combine(uploads, MenuItemVm.MenuItem.Id + extension_Old)))
                        {
                            System.IO.File.Delete(Path.Combine(uploads, MenuItemVm.MenuItem.Id + extension_Old));
                        }
                        using (var filestream = new FileStream(Path.Combine(uploads, MenuItemVm.MenuItem.Id + extension_New), FileMode.Create))
                        {
                            files[0].CopyTo(filestream);
                        }
                        MenuItemVm.MenuItem.Image = @"\images\" + MenuItemVm.MenuItem.Id + extension_New;
                    }
                 
                    menuItemFromDb.Name = MenuItemVm.MenuItem.Name;
                    menuItemFromDb.Description = MenuItemVm.MenuItem.Description;
                    menuItemFromDb.Price = MenuItemVm.MenuItem.Price;
                    menuItemFromDb.SpicyType = MenuItemVm.MenuItem.SpicyType;
                    menuItemFromDb.CategoryId = MenuItemVm.MenuItem.CategoryId;
                    menuItemFromDb.SubCategoryId = MenuItemVm.MenuItem.SubCategoryId;

                    if (MenuItemVm.MenuItem.Image != null)
                    {
                        menuItemFromDb.Image = MenuItemVm.MenuItem.Image;
                    }


                    await _db.SaveChangesAsync();
                }
                catch (Exception ex)
                {

                }
                return RedirectToAction(nameof(Index));
            }
           
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var dbMenuItem = await _db.MenuItems.Include(c => c.Category).Include(s => s.SubCategory).SingleOrDefaultAsync(m => m.Id == id);

            if (dbMenuItem == null)
                return NotFound();

            MenuItemVm.MenuItem = dbMenuItem;

            return View(MenuItemVm);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var dbMenuItem = await _db.MenuItems.Include(c => c.Category).Include(s => s.SubCategory).SingleOrDefaultAsync(m => m.Id == id);

            if (dbMenuItem == null)
                return NotFound();

            MenuItemVm.MenuItem = dbMenuItem;

            return View(MenuItemVm);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            string webRootPath = _hostingEnv.WebRootPath;
            MenuItem menuItem = await _db.MenuItems.FindAsync(id);

            if (menuItem != null)
            {
                var uploads = Path.Combine(webRootPath, "images");
                var extension = menuItem.Image.Substring(menuItem.Image.LastIndexOf("."), menuItem.Image.Length - menuItem.Image.LastIndexOf("."));

                var imagePath = Path.Combine(uploads, menuItem.Id + extension);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
                _db.MenuItems.Remove(menuItem);
                await _db.SaveChangesAsync();

            }

            return RedirectToAction(nameof(Index));
        }

    }
}