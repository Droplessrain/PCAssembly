using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PCAssembly.Data;
using PCAssembly.Services;

namespace PCAssembly.Controllers
{
    public class UserAssemblyComponentsController : UserController
    {
        private readonly AssemblyService _assemblyService;
        private readonly AuthorizationService _authorizationService;

        public UserAssemblyComponentsController(ComputerAssemblyContext context)
            : base(context)
        {
            _assemblyService = new AssemblyService(context);
            _authorizationService = new AuthorizationService();
        }

        public async Task<IActionResult> AssemblyComponents(int id)
        {
            var assembly = await _assemblyService.GetAssemblyByIdAsync(id);

            if (assembly == null)
            {
                return NotFound("Сборка не найдена.");
            }

            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var isOwner = _authorizationService.IsOwner(assembly, currentUserId);

            var components = await _assemblyService.GetComponentsAsync(id);
            var reviews = await _assemblyService.GetReviewsAsync(id);

            var model = Tuple.Create(assembly.AssemblyName, components, reviews, isOwner);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveComponentFromAssembly(int assemblyComponentId)
        {
            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (currentUserId == null)
            {
                return Unauthorized();
            }

            var assemblyComponent = await _context.AssemblyComponents
                                                  .Include(ac => ac.Assembly)
                                                  .FirstOrDefaultAsync(ac => ac.AssemblyComponentId == assemblyComponentId);

            if (assemblyComponent == null)
            {
                return NotFound("The selected component connection was not found.");
            }

            if (!_authorizationService.IsOwner(assemblyComponent.Assembly, currentUserId))
            {
                return Forbid("You do not have permission to modify this assembly.");
            }

            _context.AssemblyComponents.Remove(assemblyComponent);
            await _context.SaveChangesAsync();

            return RedirectToAction("AssemblyComponents", new { id = assemblyComponent.AssemblyId });
        }
    }
}
