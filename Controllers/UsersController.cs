using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Restaurant.Data;
using Restaurant.Models;
using Restaurant.Utility;

namespace Restaurant.Controllers
{
    [Authorize(Roles = StaticDetails.AdminEndUser)]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var currentLoginuser = await _userManager.GetUserAsync(HttpContext.User);

            var allUsersFromDb = await _db.Users.ToListAsync();

            allUsersFromDb.Remove(currentLoginuser);

            return View(allUsersFromDb);
        }

        public async Task<IActionResult>Edit(string id)
        {
            var userFromDb = await _db.Users.SingleOrDefaultAsync(u => u.Id == id);

            if (userFromDb == null)
                return NotFound();

            return View(userFromDb);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, ApplicationUser model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var userFromDb = await _db.Users.Where(u => u.Id == id).FirstOrDefaultAsync();

                userFromDb.FristName = model.FristName;
                userFromDb.Lastname = model.Lastname;
                userFromDb.PhoneNumber = model.PhoneNumber;
                userFromDb.LockoutEnd = model.LockoutEnd;
                userFromDb.LockoutEnabled = model.LockoutEnabled;
                userFromDb.AccessFailedCount = model.AccessFailedCount;
                userFromDb.LockoutReason = model.LockoutReason;

                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task<IActionResult> Lock(string id)
        {
            var userFromDb = await _db.Users.SingleOrDefaultAsync(u => u.Id == id);

            if (userFromDb == null)
                return NotFound();

            return View(userFromDb);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Lock(string id, ApplicationUser model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var userFromDb = await _db.Users.Where(u => u.Id == id).FirstOrDefaultAsync();

                //userFromDb.FristName = model.FristName;
                //userFromDb.Lastname = model.Lastname;
                //userFromDb.PhoneNumber = model.PhoneNumber;
                //userFromDb.LockoutEnabled = model.LockoutEnabled;
                //userFromDb.AccessFailedCount = model.AccessFailedCount;
                //userFromDb.LockoutReason = model.LockoutReason;

                userFromDb.LockoutEnd = DateTime.Now.AddYears(1);

                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
    }
}