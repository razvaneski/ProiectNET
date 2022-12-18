using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StoreProject.Data;
using StoreProject.Models;

namespace StoreProject.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ReviewsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
        )
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [Authorize(Roles = "User,Colaborator,Admin")]
        public IActionResult Add(string productID, Review review)
        {
            review.UserID = _userManager.GetUserId(User);
            review.ReviewID = Guid.NewGuid().ToString();
            try
            {
                db.Reviews.Add(review);
                db.SaveChanges();
                TempData["Success"] = "Recenzia a fost adaugata cu succes!";
                return RedirectToAction("Show", "Products", new { id = productID });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "A aparut o eroare, va rugam reincercati.";
                return RedirectToAction("Show", "Products", new { id = productID });
            }
        }
    }
}
