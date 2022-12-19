using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StoreProject.Data;
using StoreProject.Models;

namespace StoreProject.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(
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
            var users = db.Users.ToArray();
            return View(users);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Show(string id)
        {
            ApplicationUser user = db.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            if (User.IsInRole("Admin") || _userManager.GetUserId(User) == id)
            {
                return View(user);
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
            ApplicationUser user = db.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            else if (_userManager.GetUserId(User) == id)
            {
                TempData["Error"] = "Nu iti poti sterge propriul cont.";
                return RedirectToAction("Index");
            }
            try
            {
                db.Reviews.RemoveRange(db.Reviews.Where(r => r.UserID == id));
                db.Products.RemoveRange(db.Products.Where(p => p.UserID == id));
                db.Users.Remove(user);
                db.SaveChanges();
                TempData["Success"] = "Utilizator sters cu succes!";
                return RedirectToAction("Index");
            }
            catch
            {
                TempData["Error"] = "A aparut o eroare, te rugam sa reincerci.";
                return RedirectToAction("Index");
            }
        }

        [Authorize(Roles = "User,Colaborator,Admin")]
        [HttpPost]
        public IActionResult Edit(string id, ApplicationUser user)
        {
            ApplicationUser oldUser = db.Users.FirstOrDefault(u => u.Id == id);
            if (oldUser == null)
            {
                return NotFound();
            }
            if (User.IsInRole("Admin") || _userManager.GetUserId(User) == id)
            {
                try
                {
                    oldUser.Email = user.Email;
                    oldUser.UserName = user.UserName;
                    oldUser.PhoneNumber = user.PhoneNumber;
                    db.SaveChanges();
                    TempData["Success"] = "Utilizator editat cu succes!";
                    return RedirectToAction("Index");
                }
                catch
                {
                    TempData["Error"] = "A aparut o eroare, te rugam sa reincerci.";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}
