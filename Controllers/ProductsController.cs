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

            var products = db.Products.Include("Category").Where(p => p.IsAvailable == true);

            var search = "";
            // MOTOR DE CAUTARE
            if (Convert.ToString(HttpContext.Request.Query["search"]) != null)
            {
                search = Convert.ToString(HttpContext.Request.Query["search"]).Trim();

                // Cautare in articol (nume & descriere & categorie)
                List<string> productIDs = db.Products.Where(prod => prod.Name.Contains(search) || prod.Category.Name.Contains(search) || prod.Description.Contains(search)).Select(p => p.ProductID).ToList();

                products = db.Products.Where(p => productIDs.Contains(p.ProductID) && p.IsAvailable == true).Include("Category");
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
            if (User.IsInRole("Admin") || User.IsInRole("Colaborator"))
            {
                try
                {
                    product.ProductID = Guid.NewGuid().ToString();
                    product.UserID = _userManager.GetUserId(User);
                    product.IsAvailable = User.IsInRole("Admin");
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
            if(product.IsAvailable)
            {
                return View(product);
            }
            else if(User.IsInRole("Admin") || (User.IsInRole("Colaborator") && _userManager.GetUserId(User) == product.UserID))
            {
                return View(product);
            }
            return Unauthorized();
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
                    db.CartItems.RemoveRange(db.CartItems.Where(ci => ci.ProductID == product.ProductID));
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
                    oldProduct.Stock = newProduct.Stock;
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

        [Authorize(Roles = "User,Colaborator,Admin")]
        [HttpPost]
        public IActionResult AddToCart(string id)
        {
            var product = db.Products.FirstOrDefault(prod => prod.ProductID == id);
            if (product == null)
            {
                return NotFound();
            }
            if(product.Stock <= 0 || product.IsAvailable == false)
            {
                TempData["Error"] = "Produs indisponibil";
                return RedirectToAction("Index");
            }
            var cart = db.Carts.FirstOrDefault(c => c.UserID == _userManager.GetUserId(User));
            if (cart == null)
            {
                cart = new Cart();
                cart.CartID = Guid.NewGuid().ToString();
                cart.UserID = _userManager.GetUserId(User);
                db.Carts.Add(cart);
                db.SaveChanges();
            }
            var cartItem = db.CartItems.FirstOrDefault(ci => ci.CartID == cart.CartID && ci.ProductID == product.ProductID);
            if(cartItem != null)
            {
                cartItem.Quantity += 1;
                db.SaveChanges();
                TempData["Success"] = "Produs adaugat in cos.";
            }
            else
            {
                cartItem = new CartItem();
                cartItem.CartID = cart.CartID;
                cartItem.Quantity = 1;
                cartItem.ProductID = product.ProductID;
                cartItem.CartItemID = Guid.NewGuid().ToString();
                db.CartItems.Add(cartItem);
                db.SaveChanges();
                TempData["Success"] = "Produs adaugat in cos.";
            }
            return RedirectToAction("Show", new { id = id });
        }
    }
}
