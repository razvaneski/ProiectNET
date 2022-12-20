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
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ProductsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
        )
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public IActionResult Index()
        {
            int _perPage = 3;

            var products = db.Products.Include("Category");

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
            return View();

        }

        [Authorize(Roles = "Colaborator,Admin")]
        public IActionResult Add()
        {
            var categories = db.Categories;
            ViewBag.Categories = categories;
            return View();
        }

        [Authorize(Roles = "Colaborator,Admin")]
        [HttpPost]
        public IActionResult Add(Product product)
        {
            if (User.IsInRole("Admin") || _userManager.GetUserId(User) == product.UserID)
            {
                try
                {
                    product.ProductID = Guid.NewGuid().ToString();
                    product.UserID = _userManager.GetUserId(User);
                    db.Products.Add(product);
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

        public IActionResult Show(string id)
        {
            var product = db.Products
                .Include("User")
                .Include("Category")
                .Include("Reviews")
                .FirstOrDefault(prod => prod.ProductID == id);
 
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [Authorize(Roles = "Colaborator,Admin")]
        [HttpPost]
        public IActionResult Delete(string id)
        {
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            if(User.IsInRole("Admin") || (User.IsInRole("Colaborator") && _userManager.GetUserId(User) == product.UserID))
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

        [Authorize(Roles = "Colaborator,Admin")]
        public IActionResult Edit(string id)
        {
            var categories = db.Categories;
            ViewBag.Categories = categories;

            Product product = db.Products.Include("Category").First(prod => prod.ProductID == id);
            if (User.IsInRole("Admin") || (User.IsInRole("Colaborator") && _userManager.GetUserId(User) == product.UserID))
            {
                if (product == null)
                {
                    return NotFound();
                }
                return View(product);
            }
            else
            {
                return Unauthorized();
            }
        }

        [Authorize(Roles = "Colaborator,Admin")]
        [HttpPost]
        public IActionResult Edit(string id, Product newProduct)
        {
            Product oldProduct = db.Products.Find(id);
            if (User.IsInRole("Admin") || (User.IsInRole("Colaborator") && _userManager.GetUserId(User) == oldProduct.UserID))
            {
                try
                {
                    oldProduct.Name = newProduct.Name;
                    oldProduct.Price = newProduct.Price;
                    oldProduct.Description = newProduct.Description;
                    oldProduct.CategoryID = newProduct.CategoryID;
                    db.SaveChanges();
                    TempData["Success"] = "Produs modificat cu succes!";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "A aparut o eroare, va rugam reincercati.";
                    return RedirectToAction("Edit/{product.ProductID}");
                }
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}
