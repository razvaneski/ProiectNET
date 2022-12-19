using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        [HttpPost]
        public IActionResult Add(string id, Review review)
        {
            review.UserID = _userManager.GetUserId(User);
            review.ReviewID = Guid.NewGuid().ToString();
            review.ProductID = id;
            try
            {
                db.Reviews.Add(review);
                db.SaveChanges();
                TempData["Success"] = "Recenzia a fost adaugata cu succes!";
                return RedirectToAction("Show", "Products", new { id = id });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "A aparut o eroare, va rugam reincercati.";
                return RedirectToAction("Show", "Products", new { id = id });
            }
        }

        [Authorize(Roles = "User,Colaborator,Admin")]
        [HttpPost]
        public IActionResult Delete(string id)
        {
            string userID = _userManager.GetUserId(User);
            Review review = db.Reviews.Find(id);
            if (review == null)
            {
                return NotFound();
            }
            else if (User.IsInRole("Admin") || userID == review.UserID)
            {
                try
                {
                    db.Remove(review);
                    db.SaveChanges();
                    TempData["Success"] = "Recenzie stearsa cu succes!";
                    return RedirectToAction("Index", "Products");
                }
                catch
                {
                    TempData["Error"] = "A aparut o eroare, va rugam reincercati.";
                    return RedirectToAction("Index", "Products");
                }
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}
