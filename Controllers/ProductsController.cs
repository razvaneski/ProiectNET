using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreProject.Data;
using StoreProject.Models;

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
            var products = db.Products.Include("Category");
            ViewBag.Products = products;
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
            try
            {
                product.ProductID = Guid.NewGuid().ToString();
                product.UserID = _userManager.GetUserId(User);
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                return RedirectToAction("Add");
            }
        }
    }
}
