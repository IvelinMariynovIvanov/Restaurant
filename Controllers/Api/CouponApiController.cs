using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Data;
using Restaurant.Models;

namespace Restaurant.Controllers.Api
{
    //[Produces("application/json")]
    [Route("api/CouponApi")]
    public class CouponApiController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CouponApiController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> Get(decimal orderTotal, string couponCode)
        {
            //Return string will have :E for error and :S for success at the end

            var result = "";

            if (couponCode == null)
            {
                result = orderTotal + ":E";

                return Ok(result);
            }

            var couponFromDb = _db.Coupons.Where(c => c.Name == couponCode).FirstOrDefault();

            if (couponFromDb == null)
            {
                result = orderTotal + ":E";
                return Ok(result);
            }
            if (couponFromDb.MinAmount > orderTotal)
            {
                result = orderTotal + ":E";
                return Ok(result);
            }

            if (Convert.ToInt32(couponFromDb.CouponType) == (int)CouponType.Dollar)
            {
                orderTotal = orderTotal - couponFromDb.Discount;
                result = orderTotal + ":S";
                return Ok(result);
            }
            else
            {
                if (Convert.ToInt32(couponFromDb.CouponType) == (int)CouponType.Percent)
                {
                    orderTotal = orderTotal - (orderTotal * couponFromDb.Discount / 100);
                    result = orderTotal + ":S";
                    return Ok(result);
                }
            }

            return Ok();
        }

    }
}
