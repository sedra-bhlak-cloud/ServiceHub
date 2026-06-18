using Microsoft.AspNetCore.Mvc;
using ServiceHub.Infrastructure.Data;
using ServiceHub.Domain.Entities;
using System.Linq;

namespace ServiceHub.Web.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. MAIN LIST VIEW
        public IActionResult Index()
        {
            var categories = _context.Categories.ToList();
            return View(categories);
        }

        // 2. ADD NEW: Open blank form
        public IActionResult Create()
        {
            return View();
        }

        // 3. ADD NEW: Save submission
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Categories.Add(category);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // 4. EDIT: Open the form with the existing item's data loaded
        public IActionResult Edit(int id)
        {
            var category = _context.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // 5. EDIT: Save the updated changes back to the database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Categories.Update(category);
                _context.SaveChanges(); // Saves the changes!
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // 6. DELETE: Wipe out a category
        public IActionResult Delete(int id)
        {
            var category = _context.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }

            _context.Categories.Remove(category);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}