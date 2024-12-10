using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PCAssembly.Data;

namespace PCAssembly.Controllers
{
    public class UserAssemblyComponentsController : UserController
    {
        public UserAssemblyComponentsController(ComputerAssemblyContext context) : base(context) { }
        public async Task<IActionResult> AssemblyComponents(int id)
        {
            var components = await _context.AssemblyComponents
                                            .Where(ac => ac.AssemblyId == id)
                                            .Include(ac => ac.Component)
                                            .Select(ac => new ComponentViewModel
                                            {
                                                ComponentId = ac.Component.ComponentId,
                                                Name = ac.Component.Name,
                                                Description = ac.Component.Description,
                                                Price = ac.Component.Price,
                                                AssemblyComponentId = ac.AssemblyComponentId
                                            })
                                            .ToListAsync();

            ViewBag.AssemblyId = id;

            return View(components);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveComponentFromAssembly(int assemblyComponentId)
        {
            var assemblyComponent = await _context.AssemblyComponents.FindAsync(assemblyComponentId);

            if (assemblyComponent == null)
            {
                return NotFound("The selected component connection was not found.");
            }

            _context.AssemblyComponents.Remove(assemblyComponent);
            await _context.SaveChangesAsync();

            return RedirectToAction("AssemblyComponents", new { id = assemblyComponent.AssemblyId });
        }
    }
    public class ComponentViewModel
    {
        public int ComponentId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int AssemblyComponentId { get; set; }
    }
}
