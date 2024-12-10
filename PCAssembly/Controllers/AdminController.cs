using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PCAssembly;
using PCAssembly.Data;
using System.Threading.Tasks;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ComputerAssemblyContext _context;

    public AdminController(ComputerAssemblyContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }
}
