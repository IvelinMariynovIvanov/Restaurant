using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Restaurant.Data;
using Restaurant.Models.Entities;
using Restaurant.Utility;
using Restaurant.ViewModels.OrderDetailsVm;

namespace Restaurant.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _db;

        public OrdersController(ApplicationDbContext db)
        {
            _db = db;
        }

        [Authorize]
        public async Task<IActionResult> Confirm(int id)
        {
            var claimIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            //ConfirmOrderVm confirmOrderVm = new ConfirmOrderVm()
            //{
            //    OrderHeader = await _db.OrderHeaders.SingleOrDefaultAsync(o => o.Id == id && o.UserId == claim.Value),
            //    OrderDetailsList = await _db.OrderDetails.Include(m => m.MenuItem).Where(d => d.OrderId == id).ToListAsync()
            //};

            //return View(confirmOrderVm);

            OrderHeader orderHeader = await _db.OrderHeaders.Include(d => d.OrderDetails)
                                                .Include(m => m.MenuItems)
                                                .Where(o => o.Id == id && o.UserId == claim.Value)
                                                .SingleOrDefaultAsync();

            //List<OrderDetails> orderDetails= await _db.OrderDetails.Include(m => m.MenuItem)
            //                                 .Include(h => h.OrderHeader)
            //                                 .Where(o => o.OrderId == id && o.UserId == claim.Value)
            //                                 .ToListAsync();

            return View(orderHeader);
        }

        [Authorize]
        public async Task<IActionResult> OrderHistory()
        {
            var claimIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            //List<ConfirmOrderVm> confirmOrderVmList = new List<ConfirmOrderVm>();

            //List<OrderDetails> orderDetailsList = await _db.OrderDetails.Include(h => h.OrderHeader).
            //                                            Where(o => o.OrderHeader.UserId == claim.Value).ToListAsync();

            //foreach (var order in orderHeadersList)
            //{
            //    ConfirmOrderVm confirmOrderVm = new ConfirmOrderVm()
            //    {
            //        OrderHeader = order,
            //        OrderDetailsList = order.OrderDetails.ToList()
            //    };

            //    confirmOrderVmList.Add(confirmOrderVm);
            //}

            //return View(orderHeadersList);

            //List<OrderHeader> orderHeadersList = await _db.OrderHeaders.Include(h => h.OrderDetails).Where(o => o.UserId == claim.Value).ToListAsync();
            //List<OrderDetails> orderDetails = await _db.OrderDetails.Include(m => m.MenuItem)
            //                                 .Include(h => h.OrderHeader)
            //                                 .Where( o => o.UserId == claim.Value)
            //                                 .ToListAsync();

            List<OrderHeader> orderHeaderList = await _db.OrderHeaders.Include(d => d.OrderDetails)
                                              .Include(m => m.MenuItems)
                                              .Where(o => o.UserId == claim.Value)
                                              .OrderByDescending(d => d.OrderDate)
                                              .ToListAsync();



            return View(orderHeaderList);
        }


        [Authorize(Roles = StaticDetails.AdminEndUser)]
        public async Task<IActionResult> PickUpDetails(int orderId)
        {
            OrderHeader order = await _db.OrderHeaders.Include(d => d.OrderDetails)
                                              .Include(m => m.MenuItems)
                                              .Include(u => u.ApplicationUser)
                                              .SingleOrDefaultAsync(o => o.Id == orderId);

            return View(order);
        }

        [Authorize(Roles = StaticDetails.AdminEndUser)]
        [HttpPost]
        [ActionName("PickUpDetails")]
        public async Task<IActionResult> PickUpDetailsPost(int orderId)
        {
            var order = await _db.OrderHeaders.FindAsync(orderId);
            order.OrderStatus = StaticDetails.StatusCompletted;

            await _db.SaveChangesAsync();

            return RedirectToAction("OrderPickUp", "Orders"); ;
        }

        [Authorize(Roles = StaticDetails.AdminEndUser)]
        public async Task<IActionResult> ManageOrder()
        {
            var claimIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            List<OrderHeader> orderList = await _db.OrderHeaders.Include(d => d.OrderDetails)
                                              .Include(m => m.MenuItems)
                                              .Where(o => o.OrderStatus == StaticDetails.StatusSubmitted || o.OrderStatus == StaticDetails.StatusInProgress)
                                              .OrderByDescending(o => o.PickUpTime)
                                              .ToListAsync();

            return View(orderList);
        }

        [HttpGet]
        [Authorize(Roles = StaticDetails.AdminEndUser)]
        public async Task<IActionResult>OrderPickUp(string searchOrder, string searchEmail, string searchPhone)
        {
            List<OrderHeader> orderList = new List<OrderHeader>();

            if (searchEmail != null || searchOrder != null || searchPhone != null)
            {
                if(searchOrder != null)
                {
                    orderList = await _db.OrderHeaders.Include(d => d.OrderDetails)
                                              .Include(m => m.MenuItems)
                                              .Where(o =>o.Id == int.Parse(searchOrder))
                                              .OrderByDescending(o => o.PickUpTime)
                                              .ToListAsync();
                }
                else if(searchEmail != null)
                {
                    orderList = await _db.OrderHeaders.Include(d => d.OrderDetails)
                                              .Include(m => m.MenuItems)
                                              .Where(o => o.ApplicationUser.Email == searchEmail )
                                              .OrderByDescending(o => o.PickUpTime)
                                              .ToListAsync();
                }
                else if(searchPhone != null)
                {
                    orderList = await _db.OrderHeaders.Include(d => d.OrderDetails)
                                              .Include(m => m.MenuItems)
                                              .Where(o => o.ApplicationUser.PhoneNumber == searchPhone)
                                              .OrderByDescending(o => o.PickUpTime)
                                              .ToListAsync();
                }

            }

            else 
            {
                var claimIdentity = (ClaimsIdentity)this.User.Identity;
                var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

                orderList = await _db.OrderHeaders.Include(d => d.OrderDetails)
                                             .Include(m => m.MenuItems)
                                             .Where(o => o.OrderStatus == StaticDetails.StatusReady)
                                             .OrderByDescending(o => o.PickUpTime)
                                             .ToListAsync();
            }

            return View(orderList);
        }


        [Authorize(Roles = StaticDetails.AdminEndUser)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OrderPrepare(int orderId)
        {
            var order = await _db.OrderHeaders.FindAsync(orderId);
            order.OrderStatus = StaticDetails.StatusInProgress;

            await _db.SaveChangesAsync();

            return RedirectToAction("ManageOrder", "Orders");
        }

        [Authorize(Roles = StaticDetails.AdminEndUser)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>OrderReady(int orderId)
        {
            var order = await _db.OrderHeaders.FindAsync(orderId);
            order.OrderStatus = StaticDetails.StatusReady;

            await _db.SaveChangesAsync();

            return RedirectToAction("ManageOrder", "Orders");
        }

        [Authorize(Roles = StaticDetails.AdminEndUser)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>OrderCancell(int orderId)
        {
            var order = await _db.OrderHeaders.FindAsync(orderId);
            order.OrderStatus = StaticDetails.StatusCancelled;

            await _db.SaveChangesAsync();

            return RedirectToAction("ManageOrder", "Orders");
        }
    }
}