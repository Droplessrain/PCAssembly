using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PCAssembly;
using PCAssembly.Data;
using X.PagedList.Extensions;

namespace PCAssembly.Controllers
{
    public class ComponentsController : Controller
    {
        private readonly ComputerAssemblyContext _context;

        public ComponentsController(ComputerAssemblyContext context)
        {
            _context = context;
        }

        // GET: Components
        public async Task<IActionResult> Index(int? page)
        {
            int pageNumber = page ?? 1; // Номер страницы, если не задан, то по умолчанию 1
            int pageSize = 10;         // Количество записей на странице

            var componentsQuery = _context.Components
                                          .Include(c => c.TypeComponents); // Запрос компонентов с типами

            var componentsList = await componentsQuery.ToListAsync();      // Материализуем запрос в список

            var pagedComponents = componentsList.ToPagedList(pageNumber, pageSize); // Применяем пагинацию

            return View(pagedComponents);
        }

        // GET: Components/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var component = await _context.Components
                .Include(c => c.TypeComponents)
                .FirstOrDefaultAsync(m => m.ComponentId == id);
            if (component == null)
            {
                return NotFound();
            }

            return View(component);
        }

        // GET: Components/Create
        public IActionResult Create()
        {
            ViewBag.TypeComponentsId = new SelectList(_context.TypeComponents, "TypeComponentsId", "TypeComponentsId");
            return View();
        }

        // POST: Components/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ComponentId,TypeComponentsId,Name,Description,Price")] Component component)
        {
            _context.Add(component);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Components/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var component = await _context.Components.FindAsync(id);
            if (component == null)
            {
                return NotFound();
            }
            
            ViewBag.TypeComponentsId = new SelectList(_context.TypeComponents, "TypeComponentsId", "TypeComponentsId", component.TypeComponentsId);
            return View(component);
        }

        // POST: Components/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ComponentId,TypeComponentsId,Name,Description,Price")] Component component)
        {
            if (id != component.ComponentId)
            {
                return NotFound();
            }

            try
            {
                _context.Update(component);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ComponentExists(component.ComponentId))
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

        // GET: Components/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var component = await _context.Components
                .Include(c => c.TypeComponents)
                .FirstOrDefaultAsync(m => m.ComponentId == id);
            if (component == null)
            {
                return NotFound();
            }

            return View(component);
        }

        // POST: Components/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var component = await _context.Components.FindAsync(id);
            if (component != null)
            {
                _context.Components.Remove(component);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ComponentExists(int id)
        {
            return _context.Components.Any(e => e.ComponentId == id);
        }
    }
}
