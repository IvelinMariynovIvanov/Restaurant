using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Restaurant.Data;
using Restaurant.Models.Entities;
using Restaurant.Utility;

namespace Restaurant.Controllers
{
    [Authorize(Roles = StaticDetails.AdminEndUser)]
    public class CouponsController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CouponsController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {

            return View(await _db.Coupons.ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>Create(Coupon coupon)
        {
            if (!ModelState.IsValid)
                return View(coupon);

            var filesFromRequest = HttpContext.Request.Form.Files;
            var pic = filesFromRequest[0];

            if(pic != null && pic.Length > 0)
            {
                byte[] uploadedPic = null;

                using (var fs = pic.OpenReadStream())
                {
                    using (var ms = new MemoryStream())
                    {
                        fs.CopyTo(ms);

                        uploadedPic = ms.ToArray();
                    }
                }
                coupon.Picture = uploadedPic; 
            }

            _db.Coupons.Add(coupon);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var coupon = await _db.Coupons.SingleOrDefaultAsync(c => c.Id == id);

            if (coupon == null)
                return NotFound();

            return View(coupon);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>Edit(int? id, Coupon coupon)
        {
            if (id != coupon.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(coupon);

            var couponFromDb = await _db.Coupons.SingleOrDefaultAsync(c => c.Id == id);

            if (couponFromDb == null)
                return NotFound();

            var filesFromRequest = HttpContext.Request.Form.Files;
            var pic = filesFromRequest[0];

            if (pic != null && pic.Length > 0)
            {
                byte[] uploadedPic = null;

                using (var fs = pic.OpenReadStream())
                {
                    using (var ms = new MemoryStream())
                    {
                        fs.CopyTo(ms);

                        uploadedPic = ms.ToArray();
                    }
                }
                couponFromDb.Picture = uploadedPic;
            }

            couponFromDb.MinAmount = coupon.MinAmount;
            couponFromDb.Name = coupon.Name;
            couponFromDb.Discount = coupon.Discount;
            couponFromDb.CouponType = coupon.CouponType;
            couponFromDb.IsActive = coupon.IsActive;

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var coupon = await _db.Coupons.SingleOrDefaultAsync(c => c.Id == id);

            if (coupon == null)
                return NotFound();

            return View(coupon);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult>DeletePost(int id)
        {
            var coupon = await _db.Coupons.SingleOrDefaultAsync(c => c.Id == id);

            _db.Coupons.Remove(coupon);

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}