using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PCAssembly.Services;
using PCAssembly.Data;
using X.PagedList.Extensions;

namespace PCAssembly.Controllers
{
    public class UserReviewsController : UserController
    {

        public UserReviewsController(ComputerAssemblyContext context) : base(context)
        {
        }

        [HttpGet]
        public IActionResult AllReviews(int? page, string sortOrder = "ReviewText", string sortDirection = "asc", string reviewFilter = "")
        {
            int pageNumber = page ?? 1;
            int pageSize = 10;

            var reviewsQuery = _context.Reviews
                .Include(r => r.Assembly)
                .Include(r => r.User)
                .AsQueryable();

            // Фильтрация
            if (!string.IsNullOrEmpty(reviewFilter))
            {
                reviewsQuery = reviewsQuery.Where(r => r.ReviewText.Contains(reviewFilter) || r.Assembly.AssemblyName.Contains(reviewFilter));
            }

            // Сортировка
            reviewsQuery = SortingService.ReviewsSorting(reviewsQuery, sortOrder, sortDirection);

            // Преобразуем запрос в список с пагинацией
            var pagedReviews = reviewsQuery.ToPagedList(pageNumber, pageSize);

            // Передаем параметры сортировки и фильтрации в представление
            ViewBag.CurrentSortOrder = sortOrder;
            ViewBag.CurrentSortDirection = sortDirection;
            ViewBag.CurrentFilter = reviewFilter;

            return View(pagedReviews);
        }

        [HttpGet]
        public IActionResult CreateReview()
        {
            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized("User not authenticated");
            }

            // Получение сборок текущего пользователя
            var userAssemblies = _context.Assemblies
                .Where(a => a.UserId == currentUserId)
                .Select(a => new { a.AssemblyId, a.AssemblyName })
                .ToList();

            if (!userAssemblies.Any())
            {
                ModelState.AddModelError(string.Empty, "You don't have any assemblies to review.");
                return View("Error"); 
            }

            ViewBag.AssemblyId = new SelectList(userAssemblies, "AssemblyId", "AssemblyName");

            var model = new Review
            {
                UserId = currentUserId 
            };

            return View("CreateReview", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateReview(Review review)
        {
            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized("User not authenticated");
            }

            review.UserId = currentUserId;

            var existingReview = await _context.Reviews
                .FirstOrDefaultAsync(r => r.UserId == currentUserId && r.AssemblyId == review.AssemblyId);

            if (existingReview != null)
            {
                ModelState.AddModelError(string.Empty, "You have already submitted a review for this assembly.");
            }

            if (ModelState.IsValid)
            {
                var userAssemblies = _context.Assemblies
                    .Where(a => a.UserId == currentUserId)
                    .Select(a => new { a.AssemblyId, a.AssemblyName })
                    .ToList();
                ViewBag.AssemblyId = new SelectList(userAssemblies, "AssemblyId", "AssemblyName");

                return View(review);
            }

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return RedirectToAction("AllReviews");
        }
    }
}
