using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreProject.Data;
using StoreProject.Models;

namespace StoreProject.Controllers
{

    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public CategoriesController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
        )
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            var categories = db.Categories.ToList();
            return View(categories);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Remove(string id)
        {
            var category = db.Categories.FirstOrDefault(c => c.CategoryID == id);
            if(category != null)
            {
                var products = db.Products.Where(p => p.CategoryID == category.CategoryID);
                if (products != null)
                {
                    foreach (var product in products)
                    {
                        var reviews = db.Reviews.Where(r => r.ProductID == product.ProductID);
                        if(reviews != null)
                        {
                            db.Reviews.RemoveRange(reviews);
                        }
                        db.Products.Remove(product);
                    }
                }
                db.Categories.Remove(category);
                db.SaveChanges();
                TempData["Success"] = "Categoria a fost stearsa cu succes.";
            }
            else
            {
                TempData["Error"] = "Categoria nu a fost gasita.";
            }
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult New()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult New(Category category)
        {
            category.CategoryID = new Guid().ToString();
            db.Categories.Add(category);
            db.SaveChanges();
            TempData["Success"] = "Categoria a fost adaugata cu succes.";
            return RedirectToAction("Index");
        }
    }
}
