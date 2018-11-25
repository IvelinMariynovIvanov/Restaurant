using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Restaurant.Data;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Restaurant.Controllers.Api
{
    //[Route("api/[controller]")]
    [Route("Api/Users/")]
    public class UsersApiController : Controller
    {
        private readonly ApplicationDbContext _db;

        public UsersApiController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: api/<controller>
        [HttpGet]
        public IActionResult Get(string type, string query)
        {
            if(query != null && type == "email")
            {
                var userList = _db.Users.Where(u => u.Email.Contains(query)).ToList();

                return Ok(userList);
            }

            return NotFound();
          
        }

       
    }
}
