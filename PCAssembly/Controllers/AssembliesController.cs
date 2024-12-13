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
using X.PagedList;
using X.PagedList.Extensions;
using X.PagedList.Mvc.Core;

namespace PCAssembly.Controllers
{
    public class AssembliesController : Controller
    {
        private readonly ComputerAssemblyContext _context;

        public AssembliesController(ComputerAssemblyContext context)
        {
            _context = context;
        }
        public IActionResult Index(int? page, string sortColumn = "AssemblyName", string sortDirection = "asc")
        {
            int pageNumber = page ?? 1;
            int pageSize = 10;

            // Получаем данные из базы
            var assembliesQuery = _context.Assemblies.Include(a => a.User).AsQueryable();

            // Применяем сортировку
            assembliesQuery = SortingService.SortAssemblies(assembliesQuery, sortColumn, sortDirection);

            // Пагинация
            var pagedAssemblies = assembliesQuery.ToPagedList(pageNumber, pageSize);

            // Передаем текущую сортировку в представление
            ViewBag.CurrentSortColumn = sortColumn;
            ViewBag.CurrentSortDirection = sortDirection;

            return View(pagedAssemblies);
        }



        // GET: Assemblies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @assembly = await _context.Assemblies
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.AssemblyId == id);
            if (@assembly == null)
            {
                return NotFound();
            }

            return View(@assembly);
        }

        // GET: Assemblies/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AssemblyId,UserId,AssemblyName,Avgrating")] Assembly @assembly)
        {
            _context.Add(@assembly);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }



        // GET: Assemblies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @assembly = await _context.Assemblies.FindAsync(id);
            if (@assembly == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", @assembly.UserId);
            return View(@assembly);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AssemblyId,UserId,AssemblyName,Avgrating")] Assembly @assembly)
        {
            if (id != @assembly.AssemblyId)
            {
                return NotFound();
            }

            try
            {
                _context.Update(@assembly);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AssemblyExists(@assembly.AssemblyId))
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

        // GET: Assemblies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @assembly = await _context.Assemblies
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.AssemblyId == id);
            if (@assembly == null)
            {
                return NotFound();
            }

            return View(@assembly);
        }

        // POST: Assemblies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @assembly = await _context.Assemblies.FindAsync(id);
            if (@assembly != null)
            {
                _context.Assemblies.Remove(@assembly);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AssemblyExists(int id)
        {
            return _context.Assemblies.Any(e => e.AssemblyId == id);
        }
    }
}
