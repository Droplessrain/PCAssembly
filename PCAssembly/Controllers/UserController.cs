using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PCAssembly.Data;
using PCAssembly.Services;
using X.PagedList.Extensions;
using X.PagedList;
using X.PagedList.Mvc.Core;
using System;


namespace PCAssembly.Controllers
{
    [Authorize(Roles = "User")]
    public class UserController : Controller
    {
        protected readonly ComputerAssemblyContext _context;

        public UserController(ComputerAssemblyContext context)
        {
            _context = context;
        }

        public IActionResult Profile()
        {
            return View();
        }

        

          
    }
}
