using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Versioning;
using StoreProject.Data;
using StoreProject.Models;
using System.Text.RegularExpressions;

namespace StoreProject.Controllers
{
    public class RequestsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RequestsController(
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
            var products = db.Products.Include("Category").Where(p => p.Status == "in asteptare");

            var search = "";
            // MOTOR DE CAUTARE
            if (Convert.ToString(HttpContext.Request.Query["search"]) != null)
            {
                search = Convert.ToString(HttpContext.Request.Query["search"]).Trim();

                // Cautare in articol (nume & descriere & categorie)
                List<string> productIDs = db.Products.Where(prod => prod.Name.Contains(search) || prod.Category.Name.Contains(search) || prod.Description.Contains(search)).Select(p => p.ProductID).ToList();

                products = db.Products.Where(p => productIDs.Contains(p.ProductID) && p.Status == "in asteptare").Include("Category");
            }

            ViewBag.SearchString = search;

            int _perPage = 3;

            int totalItems = products.Count();

            var currentPage = Convert.ToInt32(HttpContext.Request.Query["page"]);

            var offset = 0;

            if (!currentPage.Equals(0))
            {
                offset = (currentPage - 1) * _perPage;
            }

            var paginatedProducts = products.Skip(offset).Take(_perPage);

            ViewBag.LastPage = Math.Ceiling((float)totalItems / (float)_perPage);

            ViewBag.Products = paginatedProducts;

            if (search != "")
            {
                ViewBag.PaginationBaseUrl = "/Products/Index/?search=" + search + "&page";
            }
            else
            {
                ViewBag.PaginationBaseUrl = "/Products/Index/?page";
            }

            return View();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Show(string id)
        {
            var product = db.Products
                .Include("User")
                .Include("Category")
                .FirstOrDefault(prod => prod.ProductID == id);


            if (product == null)
            {
                return NotFound();
            }

            ApplicationUser user = db.Users.FirstOrDefault(u => u.Id == product.UserID);

            if (user == null) 
            { 
                return NotFound(); 
            }

            ViewBag.Email = user.Email;

            return View(product);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Add()
        {
            var categories = db.Categories;
            ViewBag.Categories = categories;
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Add(string id)
        {
            Product product = db.Products.Find(id);

            if (User.IsInRole("Admin"))
            {
                try
                {
                    product.Status = "acceptat";
                    db.SaveChanges();
                    TempData["Success"] = "Produs adaugat cu succes!";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "A aparut o eroare, va rugam reincercati.";
                    return RedirectToAction("Add");
                }
            }
            else
            {
                return Unauthorized();
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Delete(string id)
        {
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            if (User.IsInRole("Admin"))
            {
                try
                {
                    db.Reviews.RemoveRange(db.Reviews.Where(r => r.ProductID == product.ProductID));
                    db.Remove(product);
                    db.SaveChanges();
                    TempData["Success"] = "Produs sters cu succes!";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "A aparut o eroare, va rugam reincercati.";
                    return RedirectToAction("Show", new { id = id });
                }
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}
