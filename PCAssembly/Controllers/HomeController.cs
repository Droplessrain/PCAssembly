using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PCAssembly.Data;
using PCAssembly.Models;
using System.Diagnostics;

namespace PCAssembly.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ComputerAssemblyContext _context;

        public HomeController(ILogger<HomeController> logger, ComputerAssemblyContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var components = await _context.Components.ToListAsync();
            return View(components);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
