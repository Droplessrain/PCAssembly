using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PCAssembly.Services;
using Microsoft.AspNetCore.Mvc.Rendering;
using X.PagedList.Extensions;
using PCAssembly.Data;
using PCAssembly.ViewModels;

namespace PCAssembly.Controllers
{
    [Authorize(Roles = "User")]
    public class UserAssembliesController : UserController
    {
        public UserAssembliesController(ComputerAssemblyContext context) : base(context)
        {
        }
        public IActionResult MyAssemblies(string sortOrder, string componentFilter, int? page)
        {
            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (currentUserId == null)
            {
                return Unauthorized();
            }
            
            int pageNumber = page ?? 1;
            int pageSize = 10;

            var assembliesQuery = _context.Assemblies
                .Where(a => a.UserId == currentUserId)
                .AsQueryable();

            switch (sortOrder)
            {
                case "rating_desc":
                    assembliesQuery = assembliesQuery.OrderByDescending(a => a.Avgrating);
                    break;
                default:
                    assembliesQuery = assembliesQuery.OrderBy(a => a.Avgrating);
                    break;
            }

            var pagedAssemblies = assembliesQuery.ToPagedList(pageNumber, pageSize);

            var assemblyIds = pagedAssemblies.Select(a => a.AssemblyId).ToList();

            var assemblyComponents = _context.AssemblyComponents
                .Where(ac => assemblyIds.Contains(ac.AssemblyId))
                .GroupBy(ac => ac.AssemblyId)
                .ToDictionary(g => g.Key, g => g.Any());

            ViewData["CurrentSort"] = sortOrder;
            ViewData["CurrentFilter"] = componentFilter;

            ViewBag.AssemblyComponents = assemblyComponents;

            return View(pagedAssemblies);
        }

        public IActionResult AllAssemblies(int? page, string sortColumn = "AssemblyName", string sortDirection = "asc", string assemblyFilter = "")
        {
            int pageNumber = page ?? 1;
            int pageSize = 10;

            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (currentUserId == null)
            {
                return Unauthorized();
            }

            var assembliesQuery = _context.Assemblies
                .Include(a => a.User)
                .AsQueryable();

            if (!string.IsNullOrEmpty(assemblyFilter))
            {
                assembliesQuery = assembliesQuery.Where(a => a.AssemblyName.Contains(assemblyFilter));
            }

            assembliesQuery = SortingService.SortAssemblies(assembliesQuery, sortColumn, sortDirection);

            var pagedAssemblies = assembliesQuery.ToPagedList(pageNumber, pageSize);

            ViewBag.CurrentSortColumn = sortColumn;
            ViewBag.CurrentSortDirection = sortDirection;
            ViewBag.CurrentFilter = assemblyFilter;

            return View(pagedAssemblies);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAssemblyToUser(int assemblyId)
        {
            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (currentUserId == null)
            {
                return Unauthorized();
            }

            var sourceComponents = await _context.AssemblyComponents
                .Where(ac => ac.AssemblyId == assemblyId)
                .Include(ac => ac.Assembly)
                .Include(ac => ac.Component)
                .ToListAsync();

            if (!sourceComponents.Any() || sourceComponents.First().Assembly == null)
            {
                return NotFound("Сборка не найдена или её компоненты отсутствуют.");
            }

            var newAssembly = new Assembly
            {
                AssemblyName = $"{sourceComponents.First().Assembly.AssemblyName}",
                UserId = currentUserId,
                Avgrating = null
            };

            _context.Assemblies.Add(newAssembly);
            await _context.SaveChangesAsync(); // Сохраняем для генерации нового AssemblyId

            // Добавляем компоненты из исходной сборки в новую
            foreach (var component in sourceComponents)
            {
                var newAssemblyComponent = new AssemblyComponent
                {
                    AssemblyId = newAssembly.AssemblyId,
                    ComponentId = component.ComponentId
                };

                _context.AssemblyComponents.Add(newAssemblyComponent);
            }

            await _context.SaveChangesAsync();

            // Возвращаемся на страницу со списком сборок
            return RedirectToAction(nameof(MyAssemblies));
        }


        [HttpGet]
        public IActionResult CreateAssembly()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAssembly([Bind("AssemblyName")] Assembly @assembly)
        {

            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (currentUserId == null)
            {
                return Unauthorized();
            }

            @assembly.UserId = currentUserId;
            @assembly.Avgrating = null;

            _context.Add(@assembly);
            await _context.SaveChangesAsync();

            Console.WriteLine($"AssemblyName: {@assembly.AssemblyName}");
            Console.WriteLine($"UserId: {@assembly.UserId}");
            return RedirectToAction(nameof(MyAssemblies));
        }

        [HttpGet]
        public async Task<IActionResult> DeleteAssembly(int id)
        {
            var assembly = await _context.Assemblies.FindAsync(id);
            if (assembly == null)
            {
                return NotFound();
            }

            _context.Assemblies.Remove(assembly);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(MyAssemblies));
        }
    }
}
