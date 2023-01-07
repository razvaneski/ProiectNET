using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreProject.Data;
using StoreProject.Models;

namespace StoreProject.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public CartController(
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
        public IActionResult Index()
        {
            var cart = db.Carts.FirstOrDefault(c => c.UserID == _userManager.GetUserId(User));
            if(cart != null)
            {
                var cartItems = db.CartItems.Include("Product").Where(ci => ci.CartID == cart.CartID);
                return View(cartItems.ToList());
            }
            else
            {
                cart = new Cart();
                cart.CartID = Guid.NewGuid().ToString();
                cart.UserID = _userManager.GetUserId(User);
                db.Add(cart);
                db.SaveChanges();
                return View(null);
            }
        }

        [Authorize(Roles = "User,Colaborator,Admin")]
        public IActionResult RemoveFromCart(string id)
        {
            var cart = db.Carts.FirstOrDefault(c => c.UserID == _userManager.GetUserId(User));
            if(cart != null)
            {
                var cartItem = db.CartItems.FirstOrDefault(ci => ci.CartItemID == id);
                if(cartItem != null)
                {
                    db.Remove(cartItem);
                    db.SaveChanges();
                    TempData["Success"] = "Produs sters cu succes.";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["Error"] = "Eroare la stergerea produsului!";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                TempData["Error"] = "Eroare la stergerea produsului!";
                return RedirectToAction("Index");
            }
        }

        [Authorize(Roles = "User,Colaborator,Admin")]
        public IActionResult IncreaseQuantity(string id)
        {
            var cart = db.Carts.FirstOrDefault(c => c.UserID == _userManager.GetUserId(User));
            if(cart != null)
            {
                var cartItem = db.CartItems.FirstOrDefault(ci => ci.ProductID == id && ci.CartID == cart.CartID);
                if (cartItem != null)
                {
                    var product = db.Products.FirstOrDefault(p => p.ProductID == cartItem.ProductID);
                    if(product != null && product.Stock > cartItem.Quantity)
                    {
                        cartItem.Quantity += 1;
                        db.SaveChanges();
                        TempData["Success"] = "Produs adaugat cu succes.";
                    }
                    else
                    {
                        TempData["Error"] = "Stoc insuficient!";
                    }
                }
                else
                {
                    TempData["Error"] = "Eroare!";
                }
            }
            else
            {
                TempData["Error"] = "Eroare!";
            }
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "User,Colaborator,Admin")]
        public IActionResult DecreaseQuantity(string id)
        {
            var cart = db.Carts.FirstOrDefault(c => c.UserID == _userManager.GetUserId(User));
            if(cart != null)
            {
                var cartItem = db.CartItems.FirstOrDefault(ci => ci.ProductID == id && ci.CartID == cart.CartID);
                if (cartItem != null)
                {
                    if (cartItem.Quantity > 1)
                    {
                        cartItem.Quantity -= 1;
                        db.SaveChanges();
                        TempData["Success"] = "Produs adaugat cu succes.";
                    }
                    else
                    {
                        return RedirectToAction("Remove", new { id = id });
                    }
                }
                else
                {
                    TempData["Error"] = "Eroare!";
                }
            }
            else
            {
                TempData["Error"] = "Eroare!";
            }
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "User,Colaborator,Admin")]
        public IActionResult Remove(string id)
        {
            var cart = db.Carts.FirstOrDefault(c => c.UserID == _userManager.GetUserId(User));
            if(cart != null)
            {
                var cartItem = db.CartItems.FirstOrDefault(ci => ci.CartID == cart.CartID && ci.ProductID == id);
                if(cartItem != null)
                {
                    db.CartItems.Remove(cartItem);
                    db.SaveChanges();
                    TempData["Success"] = "Produs sters cu succes!";
                }
                else
                {
                   TempData["Error"] = "Eroare!";
                }
            }
            else
            {
                TempData["Error"] = "Eroare!";
            }
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "User,Colaborator,Admin")]
        public IActionResult RemoveUnavailableProducts()
        {
            var cart = db.Carts.Include("CartItems").FirstOrDefault(c => c.UserID == _userManager.GetUserId(User));
            if(cart != null)
            {
                if(cart.CartItems != null)
                {
                    var cartItems = db.CartItems.Include("Product").Where(ci => ci.Product.Stock < ci.Quantity);
                    if(cartItems != null)
                    {
                        db.CartItems.RemoveRange(cartItems);
                        db.SaveChanges();
                        TempData["Success"] = "Produse eliminate cu succes.";
                    }
                }
            }
            return RedirectToAction("Index");
        }
    }
}
