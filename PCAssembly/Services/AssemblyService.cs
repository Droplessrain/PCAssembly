using Microsoft.EntityFrameworkCore;
using PCAssembly.Data;
using PCAssembly.ViewModels;

namespace PCAssembly.Services
{
    public class AssemblyService
    {
        private readonly ComputerAssemblyContext _context;

        public AssemblyService(ComputerAssemblyContext context)
        {
            _context = context;
        }

        public async Task<Assembly> GetAssemblyByIdAsync(int id)
        {
            return await _context.Assemblies
                                 .Include(a => a.User)
                                 .FirstOrDefaultAsync(a => a.AssemblyId == id);
        }

        public async Task<List<ComponentViewModel>> GetComponentsAsync(int assemblyId)
        {
            return await _context.AssemblyComponents
                                 .Where(ac => ac.AssemblyId == assemblyId)
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
        }

        public async Task<List<Review>> GetReviewsAsync(int assemblyId)
        {
            return await _context.Reviews
                                 .Where(r => r.AssemblyId == assemblyId)
                                 .Include(r => r.User)
                                 .ToListAsync();
        }
    }
}
