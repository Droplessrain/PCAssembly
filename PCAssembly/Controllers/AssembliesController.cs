using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PCAssembly;
using PCAssembly.Data;
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
        public async Task<IActionResult> Index(int? page)
        {
            int pageNumber = page ?? 1; // Номер страницы, если не задан, то по умолчанию 1
            int pageSize = 10; // Количество записей на странице

            var computerAssemblyContext = _context.Assemblies.Include(a => a.User);

            var assembliesList = await computerAssemblyContext.ToListAsync(); // Материализуем запрос в список

            var assembliesPaged = assembliesList.ToPagedList(pageNumber, pageSize);

            return View(assembliesPaged); // Передаем пагинированные данные в представление
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

        // POST: Assemblies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
