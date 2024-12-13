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
    public class ReviewsController : Controller
    {
        private readonly ComputerAssemblyContext _context;

        public ReviewsController(ComputerAssemblyContext context)
        {
            _context = context;
        }

        // GET: Reviews
        public async Task<IActionResult> Index(int? page, string sortOrder = "ReviewText", string sortDirection = "asc")
        {
            int pageNumber = page ?? 1;
            int pageSize = 10;

            IQueryable<Review> reviewsQuery = _context.Reviews.Include(r => r.Assembly).Include(r => r.User);

            // Применяем сортировку
            reviewsQuery = SortingService.ReviewsSorting(reviewsQuery.AsQueryable(), sortOrder, sortDirection);

            var reviewsList = await reviewsQuery.ToListAsync(); // Материализуем запрос в список

            var pagedReviews = reviewsList.ToPagedList(pageNumber, pageSize);

            // Передаем текущую сортировку в представление
            ViewBag.CurrentSortColumn = sortOrder;
            ViewBag.CurrentSortDirection = sortDirection;

            return View(pagedReviews);
        }

        // GET: Reviews/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews
                .Include(r => r.Assembly)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.ReviewId == id);
            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

        // GET: Reviews/Create
        public IActionResult Create()
        {
            ViewData["AssemblyId"] = new SelectList(_context.Assemblies, "AssemblyId", "AssemblyId");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Reviews/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReviewId,AssemblyId,UserId,ReviewText,Rating")] Review review)
        {
            _context.Add(review);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Reviews/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }
            ViewData["AssemblyId"] = new SelectList(_context.Assemblies, "AssemblyId", "AssemblyId", review.AssemblyId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", review.UserId);
            return View(review);
        }

        // POST: Reviews/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ReviewId,AssemblyId,UserId,ReviewText,Rating")] Review review)
        {
            if (id != review.ReviewId)
            {
                return NotFound();
            }

            try
            {
                _context.Update(review);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReviewExists(review.ReviewId))
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

        // GET: Reviews/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews
                .Include(r => r.Assembly)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.ReviewId == id);
            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

        // POST: Reviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review != null)
            {
                _context.Reviews.Remove(review);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReviewExists(int id)
        {
            return _context.Reviews.Any(e => e.ReviewId == id);
        }
    }
}
