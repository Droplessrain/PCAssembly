using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PCAssembly.Services;
using PCAssembly.Data;
using X.PagedList.Extensions;

namespace PCAssembly.Controllers
{
    public class UserComponentsController : UserController
    {
        public UserComponentsController(ComputerAssemblyContext context) : base(context)
        {
        }
        public IActionResult AllComponents(int? page, string sortColumn = "Name", string sortDirection = "asc")
        {
            int pageNumber = page ?? 1;
            int pageSize = 10;

            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (currentUserId == null)
            {
                return Unauthorized();
            }

            var componentsQuery = _context.Components.Include(c => c.TypeComponents).AsQueryable();
            componentsQuery = SortingService.SortComponents(componentsQuery, sortColumn, sortDirection);

            var pagedComponents = componentsQuery.ToPagedList(pageNumber, pageSize);

            var assemblies = _context.Assemblies
                .Where(a => a.UserId == currentUserId)
                .ToList();

            ViewBag.Assemblies = assemblies;
            ViewBag.CurrentSortColumn = sortColumn;
            ViewBag.CurrentSortDirection = sortDirection;

            return View(pagedComponents);
        }


        [HttpGet]
        public async Task<IActionResult> AddToAssembly()
        {
            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (currentUserId == null)
            {
                return Unauthorized();
            }

            var userAssemblies = await _context.Assemblies
                                               .Where(a => a.User.Id == currentUserId)
                                               .ToListAsync();

            ViewBag.AssemblyId = new SelectList(userAssemblies, "AssemblyId", "AssemblyName");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToAssembly(int componentId, int assemblyId)
        {
            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (currentUserId == null)
            {
                return Unauthorized();
            }

            var userAssembly = await _context.Assemblies
                                              .Where(a => a.AssemblyId == assemblyId && a.User.Id == currentUserId)
                                              .FirstOrDefaultAsync();

            if (userAssembly == null)
            {
                return NotFound("The selected assembly does not exist or does not belong to the current user.");
            }

            var component = await _context.Components.FindAsync(componentId);
            if (component == null)
            {
                return NotFound();
            }

            var assemblyComponent = new AssemblyComponent
            {
                AssemblyId = assemblyId,
                ComponentId = componentId
            };
            _context.AssemblyComponents.Add(assemblyComponent);

            await _context.SaveChangesAsync();

            return RedirectToAction("MyAssemblies", "UserAssemblies");
        }
    }
}
