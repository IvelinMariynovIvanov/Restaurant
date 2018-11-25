using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Restaurant.Data;
using Restaurant.Models;
using Restaurant.Models.Entities;
using Restaurant.Utility;
using Restaurant.ViewModels.OrderDetailsVm;

namespace Restaurant.Controllers
{
    public class CartsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        [BindProperty]
        private OrderDetailsCartVm OrderDetailsCartVm { get; set; }

        public CartsController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
           
            _db = db;
            _userManager = userManager;

            //hgg

          
        }

        public async Task<IActionResult> Index()
        {
            OrderDetailsCartVm = new OrderDetailsCartVm()
            {
                OrderHeader = new OrderHeader()
            };

            OrderDetailsCartVm.OrderHeader.OrderTotalPrice = 0;

            var currentLoginuser = await _userManager.GetUserAsync(HttpContext.User);

            var allItemsInUserCart = await _db.ShoppingCarts.Where(s => s.ApplicationUserId == currentLoginuser.Id).ToListAsync();

            if(allItemsInUserCart != null)
            {
                OrderDetailsCartVm.AllItemsInShoppingCart = allItemsInUserCart;
            }

            foreach (ShoppingCart shoppingCartItem in OrderDetailsCartVm.AllItemsInShoppingCart)
            {
                shoppingCartItem.MenuItem = await _db.MenuItems.SingleOrDefaultAsync(m => m.Id == shoppingCartItem.MenuItemId);

                OrderDetailsCartVm.OrderHeader.OrderTotalPrice += shoppingCartItem.MenuItem.Price * shoppingCartItem.Count;

                if(shoppingCartItem.MenuItem.Description != null && shoppingCartItem.MenuItem.Description.Length > 100)
                {
                    shoppingCartItem.MenuItem.Description = shoppingCartItem.MenuItem.Description.Substring(0, 90) + "......";
                }
            }

            OrderDetailsCartVm.OrderHeader.PickUpTime = DateTime.Now;

            return View(OrderDetailsCartVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public async Task<IActionResult> IndexPost(OrderDetailsCartVm vm)
        {
            var currentLoginuser = await _userManager.GetUserAsync(HttpContext.User);

            vm.AllItemsInShoppingCart = await _db.ShoppingCarts.Include(m => m.MenuItem)
                                                        .Where(c => c.ApplicationUserId == currentLoginuser.Id).ToListAsync();



            vm.OrderHeader.OrderDate = DateTime.Now;
            //OrderDetailsCartVm.OrderHeader.UserId = currentLoginuser.Id;
            vm.OrderHeader.UserId = vm.AllItemsInShoppingCart[0].ApplicationUserId;
            vm.OrderHeader.OrderStatus = StaticDetails.StatusSubmitted;

            _db.OrderHeaders.Add(vm.OrderHeader);
            await _db.SaveChangesAsync();

            var orderDetailsCollection = new List<OrderDetails>();
            foreach (var item in vm.AllItemsInShoppingCart)
            {
               // item.MenuItem = await _db.MenuItems.SingleOrDefaultAsync(m => m.Id == item.MenuItemId);

                OrderDetails orderDetails = new OrderDetails()
                {
                    MenuItemId = item.MenuItemId,
                    OrderId = vm.OrderHeader.Id,
                    Description = item.MenuItem.Description,
                    Name = item.MenuItem.Name,
                    Price = item.MenuItem.Price,
                    Count = item.Count,
                    UserId = currentLoginuser.Id
                };
                //await _db.OrderDetails.AddAsync(orderDetails);
                orderDetailsCollection.Add(orderDetails);
            }

            try
            {
                await _db.OrderDetails.AddRangeAsync(orderDetailsCollection);

                _db.ShoppingCarts.RemoveRange(vm.AllItemsInShoppingCart);

                await _db.SaveChangesAsync();

                HttpContext.Session.SetInt32("CartCount", 0);

                //return RedirectToAction("Index", "Home");
                return RedirectToAction("Confirm", "Orders", new { id = vm.OrderHeader.Id});
            }
            catch (Exception)
            {
                return View(vm);

                throw;    
            }
           
        }

        public async Task<IActionResult> Plus (int cartId)
        {
            var cartItem = await _db.ShoppingCarts.SingleAsync(c => c.Id == cartId);

            cartItem.Count++;

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> Minus(int cartId)
        {
            var cartItem = await _db.ShoppingCarts.SingleAsync(c => c.Id == cartId);

            if(cartItem.Count == 1)
            {
                _db.ShoppingCarts.Remove(cartItem);
                await _db.SaveChangesAsync();

                ///// update items in shopping carts
                var count = await _db.ShoppingCarts.Where(c => c.ApplicationUserId == cartItem.ApplicationUserId).CountAsync();

                HttpContext.Session.SetInt32("CartCount", count);
            }
            else
            {
                cartItem.Count--;
                await _db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));

        }
    }
}