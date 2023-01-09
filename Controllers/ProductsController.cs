using Ganss.Xss;
using Humanizer;
using MessagePack;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Versioning;
using StoreProject.Data;
using StoreProject.Models;
using System.Collections.Immutable;
using System.Text.RegularExpressions;
using System.Web;

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
        public IActionResult Index(string sortOrder, string searchString, string categoryID)
        {
            var products = db.Products.Include("Category").Include("Reviews").Where(p => p.IsAvailable == true);

            ViewBag.PriceSortOrdDesc = "price_desc";

            ViewBag.PriceSortOrdAsc = "price_asc";

            ViewBag.RatingSortOrd = "rating";

            ViewBag.Categories = db.Categories;

            var search = "";

            // MOTOR DE CAUTARE
            if (Convert.ToString(HttpContext.Request.Query["search"]) != null)
            {
                search = Convert.ToString(HttpContext.Request.Query["search"]).Trim();

                // Cautare in articol (nume & descriere & categorie)
                List<string> productIDs = db.Products.Where(prod => prod.Name.Contains(search) || prod.Category.Name.Contains(search) || prod.Description.Contains(search)).Select(p => p.ProductID).ToList();

                products = db.Products.Include("Category").Include("Reviews").Where(p => productIDs.Contains(p.ProductID) && p.IsAvailable == true);
            }

            if (sortOrder != null)
            {
                if (sortOrder == "price_desc")
                {
                    products = products.OrderByDescending(p => p.Price);
                }
                else if (sortOrder == "price_asc")
                {
                    products = products.OrderBy(p => p.Price);

                }
                else if(sortOrder == "rating")
                {
                    products = products.OrderByDescending(p => p.Reviews.Average(r => r.Rating));
                }
            }

            if(categoryID != null)
            {
                var category = db.Categories.FirstOrDefault(c => c.CategoryID == categoryID);
                if (category != null)
                {
                    products = products.Where(p => p.CategoryID == category.CategoryID);
                }
            }

            ViewBag.SearchString = search;

            int _perPage = 3;

            if(products != null)
            {
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

                if (search != "" && sortOrder == null && categoryID == null)
                {
                    ViewBag.PaginationBaseUrl = "/Products/Index/?search=" + search + "&page";
                }
                else if (sortOrder != null && search == "" && categoryID == null)
                {
                    ViewBag.PaginationBaseUrl = "/Products/Index/?sortOrder=" + sortOrder + "&page";
                }
                else if (categoryID != null && sortOrder == null && search == "")
                {
                    ViewBag.PaginationBaseUrl = "/Products/Index/?categoryID=" + categoryID + "&page";
                }
                else if (search == "" && sortOrder == null && categoryID == null)
                {
                    ViewBag.PaginationBaseUrl = "/Products/Index/?page";
                }

            }

            return View();
        }

        
        [Authorize(Roles = "Colaborator,Admin")]
        public IActionResult Add()
        {
            var categories = db.Categories;
            ViewBag.Categories = categories;
            var product = new Product();
            return View(product);
        }

        [Authorize(Roles = "Colaborator,Admin")]
        [HttpPost]
        public IActionResult Add(Product product)
        {
            product.ProductID = Guid.NewGuid().ToString();
            product.UserID = _userManager.GetUserId(User);
            product.IsAvailable = User.IsInRole("Admin");

            ModelState.ClearValidationState(nameof(product));
            if (!TryValidateModel(product, nameof(product)))
            {
                if (User.IsInRole("Admin") || User.IsInRole("Colaborator"))
                {
                    try
                    {
                        if(product.Description != null)
                        {
                            var sanitizer = new HtmlSanitizer();
                            product.Description = sanitizer.Sanitize(product.Description);
                        }

                        db.Products.Add(product);
                        db.SaveChanges();
                        TempData["Success"] = "Produs adaugat cu succes!";
                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        TempData["Error"] = "A aparut o eroare, va rugam reincercati.";
                        ViewBag.Categories = db.Categories;
                        return View(product);
                    }
                }
                else
                {
                    return Unauthorized();
                }
            }
            else
            {
                ViewBag.Categories = db.Categories;
                return View(product);
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
        public IActionResult Edit(string id, Product newProduct, [FromForm] IFormFile imageFile)
        {
            Product oldProduct = db.Products.Find(id);
            if(oldProduct == null)
            {
                return NotFound();
            }
            if (User.IsInRole("Admin") || (User.IsInRole("Colaborator") && _userManager.GetUserId(User) == oldProduct.UserID))
            {
                oldProduct.Name = newProduct.Name;
                oldProduct.Price = newProduct.Price;
                oldProduct.Description = newProduct.Description;
                oldProduct.CategoryID = newProduct.CategoryID;
                oldProduct.Stock = newProduct.Stock;

                ModelState.ClearValidationState(nameof(oldProduct));
                if (!TryValidateModel(oldProduct, nameof(oldProduct)))
                {
                    try
                    {
                        if(oldProduct.Description != null)
                        {
                            var sanitizer = new HtmlSanitizer();
                            oldProduct.Description = sanitizer.Sanitize(oldProduct.Description);
                        }
                        db.SaveChanges();
                        TempData["Success"] = "Produs modificat cu succes!";
                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        TempData["Error"] = "A aparut o eroare, va rugam reincercati.";
                        ViewBag.Categories = db.Categories;
                        return View(oldProduct);
                    }
                }
                else
                {
                    ViewBag.Categories = db.Categories;
                    return View(oldProduct);
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

        [Authorize(Roles = "Colaborator,Admin")]
        [HttpPost]
        public IActionResult ChangeImage(string productID, IFormFile image)
        {
            var product = db.Products.FirstOrDefault(p => productID == p.ProductID);
            if (product != null)
            {
                if (image != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        image.CopyTo(memoryStream);
                        product.Image = memoryStream.ToArray();
                        db.SaveChanges();
                    }
                    TempData["Success"] = "Imagine incarcata.";
                }
                else
                {
                    TempData["Error"] = "Imaginea nu a putut fi incarcata.";
                }
                return RedirectToAction("Edit", new { id = productID });
            }
            return NotFound();
        }
    }
}
