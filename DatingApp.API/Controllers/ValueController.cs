using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [EnableCors("EnableCORS")]
    public class ValueController : Controller
    {
        private ApplicationDbContext _db;

        public ValueController(ApplicationDbContext db)
        {
            _db = db;
        }
        
        [HttpGet]
        [EnableCors]
        [AllowAnonymous]
        public IActionResult GetValues()
        {
            var value = _db.Values.ToList();
            return Ok(value);
        }

        [AllowAnonymous]
        //[Route("GetValue")]
        [HttpGet ("{id}")]
        public IActionResult GetValue(int id)
        {
            var value = _db.Values.FirstOrDefault(c => c.Id == id);
            if (value==null)
            {
                return NotFound();
            }
            return Ok(value);
        }
    }
}