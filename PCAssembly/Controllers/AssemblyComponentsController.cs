using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PCAssembly;
using PCAssembly.Data;
using PCAssembly.Services;
using X.PagedList.Extensions;

namespace PCAssembly.Controllers
{
    public class AssemblyComponentsController : Controller
    {
        private readonly ComputerAssemblyContext _context;

        public AssemblyComponentsController(ComputerAssemblyContext context)
        {
            _context = context;
        }

        // GET: AssemblyComponents
        public async Task<IActionResult> Index(int? page, string sortColumn = "AssemblyName", string sortDirection = "asc")
        {
            int pageNumber = page ?? 1; // Номер страницы, если не задан, то по умолчанию 1
            int pageSize = 10; // Количество записей на странице

            // Получаем данные из базы
            var computerAssemblyContext = _context.AssemblyComponents
                .Include(a => a.Assembly)
                .Include(a => a.Component)
                .AsQueryable();

            // Применяем сортировку
            computerAssemblyContext = SortingService.SortAssemblyComponents(computerAssemblyContext, sortColumn, sortDirection);

            // Материализуем запрос в список
            var assembliesList = await computerAssemblyContext.ToListAsync();

            // Применяем пагинацию
            var assembliesPaged = assembliesList.ToPagedList(pageNumber, pageSize);

            // Передаем текущую сортировку в представление
            ViewBag.CurrentSortColumn = sortColumn;
            ViewBag.CurrentSortDirection = sortDirection;

            return View(assembliesPaged);
        }

        // GET: AssemblyComponents/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assemblyComponent = await _context.AssemblyComponents
                .Include(a => a.Assembly)
                .Include(a => a.Component)
                .FirstOrDefaultAsync(m => m.AssemblyComponentId == id);
            if (assemblyComponent == null)
            {
                return NotFound();
            }

            return View(assemblyComponent);
        }

        // GET: AssemblyComponents/Create
        public IActionResult Create()
        {
            ViewData["AssemblyId"] = new SelectList(_context.Assemblies, "AssemblyId", "AssemblyId");
            ViewData["ComponentId"] = new SelectList(_context.Components, "ComponentId", "ComponentId");
            return View();
        }

        // POST: AssemblyComponents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AssemblyComponentId,AssemblyId,ComponentId")] AssemblyComponent assemblyComponent)
        {
            _context.Add(assemblyComponent);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: AssemblyComponents/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assemblyComponent = await _context.AssemblyComponents.FindAsync(id);
            if (assemblyComponent == null)
            {
                return NotFound();
            }
            ViewData["AssemblyId"] = new SelectList(_context.Assemblies, "AssemblyId", "AssemblyId", assemblyComponent.AssemblyId);
            ViewData["ComponentId"] = new SelectList(_context.Components, "ComponentId", "ComponentId", assemblyComponent.ComponentId);
            return View(assemblyComponent);
        }

        // POST: AssemblyComponents/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AssemblyComponentId,AssemblyId,ComponentId")] AssemblyComponent assemblyComponent)
        {
            if (id != assemblyComponent.AssemblyComponentId)
            {
                return NotFound();
            }

            try
            {
                _context.Update(assemblyComponent);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AssemblyComponentExists(assemblyComponent.AssemblyComponentId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: AssemblyComponents/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assemblyComponent = await _context.AssemblyComponents
                .Include(a => a.Assembly)
                .Include(a => a.Component)
                .FirstOrDefaultAsync(m => m.AssemblyComponentId == id);
            if (assemblyComponent == null)
            {
                return NotFound();
            }

            return View(assemblyComponent);
        }

        // POST: AssemblyComponents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var assemblyComponent = await _context.AssemblyComponents.FindAsync(id);
            if (assemblyComponent != null)
            {
                _context.AssemblyComponents.Remove(assemblyComponent);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AssemblyComponentExists(int id)
        {
            return _context.AssemblyComponents.Any(e => e.AssemblyComponentId == id);
        }
    }
}
