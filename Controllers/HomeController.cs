using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Restaurant.Data;
using Restaurant.Models;
using Restaurant.Models.Entities;
using Restaurant.ViewModels.HomeVm;

namespace Restaurant.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            IndexVm vm = new IndexVm()
            {
                MenuItems = await _db.MenuItems.Include(c=> c.Category).Include(s=> s.SubCategory).ToListAsync(),
                Categories = await _db.Categories.ToListAsync(),
                Coupons = await _db.Coupons.Where(c=>c.IsActive ==true).ToListAsync()

            };

            return View(vm);
        }

        [Authorize]
        public async Task<IActionResult>Details(int id)
        {
            var menuItem = await _db.MenuItems.Include(c => c.Category).Include(s => s.SubCategory).SingleOrDefaultAsync(m => m.Id == id);

            ShoppingCart shoppingCart = new ShoppingCart()
            {
                MenuItem = menuItem,
                MenuItemId = id
            };

            return View(shoppingCart);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(ShoppingCart cardObj)
        {
            var currentLoginuser = await _userManager.GetUserAsync(HttpContext.User);

            cardObj.Id = 0;
            cardObj.ApplicationUserId = currentLoginuser.Id;

            if (!ModelState.IsValid)
            {
                var menuItem = await _db.MenuItems.Include(c => c.Category).Include(s => s.SubCategory).SingleOrDefaultAsync(m => m.Id == cardObj.MenuItemId);

                cardObj.MenuItem = menuItem;

                return View(cardObj);
            }

            var cartFromDb = await _db.ShoppingCarts.Where(s => s.ApplicationUserId == cardObj.ApplicationUserId && 
                                                           s.MenuItemId == cardObj.MenuItemId).SingleOrDefaultAsync();
           
            if(cartFromDb == null)
            {
                // this menuitem does not exist           
                _db.ShoppingCarts.Add(cardObj);
            }
            else
            {
                // this menuitem does  exist and update  the count
                cartFromDb.Count += cardObj.Count;
            }
            await _db.SaveChangesAsync();

            /// all items for logged user
            var shoppingItems = await _db.ShoppingCarts.Where(s => s.ApplicationUserId == currentLoginuser.Id).CountAsync();
            // CartCount - session name
            HttpContext.Session.SetInt32("CartCount", shoppingItems);


            return RedirectToAction(nameof(Index));
        }




        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
