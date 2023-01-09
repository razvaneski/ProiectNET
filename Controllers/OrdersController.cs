using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging.Signing;
using NuGet.Versioning;
using StoreProject.Data;
using StoreProject.Models;
using System.Text.RegularExpressions;

namespace StoreProject.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public OrdersController(
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
            var orders = db.Orders.Include("Address").Include("OrderItems").Where(o => o.UserID == _userManager.GetUserId(User));
            return View(orders.ToList());
        }

        [Authorize(Roles = "User,Colaborator,Admin")]
        public IActionResult Show(string id)
        {
            var order = db.Orders.FirstOrDefault(o => o.OrderID == id);
            if(order != null)
            {
                var orderItems = from orders in db.Orders
                                 select orders.OrderItems;
                return View(orderItems);
            }
            return NotFound();
        }

        [Authorize(Roles = "User,Colaborator,Admin")]
        [HttpPost]
        public IActionResult New(Address address)
        {

            var cart = db.Carts.FirstOrDefault(c => c.UserID == _userManager.GetUserId(User));
            if (cart != null)
            {
                address.AddressID = Guid.NewGuid().ToString();
                db.Addresses.Add(address);
                db.SaveChanges();
                
                var newOrder = new Order();
                newOrder.OrderID = Guid.NewGuid().ToString();
                newOrder.UserID = _userManager.GetUserId(User);
                newOrder.OrderDate = DateTime.Now;
                newOrder.AddressID = address.AddressID;

                var cartItems = db.CartItems.Include("Product").Where(ci => ci.CartID == cart.CartID);
                if (cartItems != null)
                {
                    newOrder.OrderItems = new List<OrderItem>();
                    foreach (var cartItem in cartItems)
                    {
                        var orderItem = new OrderItem();
                        orderItem.OrderItemID = Guid.NewGuid().ToString();
                        orderItem.OrderID = newOrder.OrderID;
                        orderItem.Quantity = cartItem.Quantity;
                        orderItem.ProductPrice = cartItem.Product.Price;
                        orderItem.ProductName = cartItem.Product.Name;
                        newOrder.OrderItems.Add(orderItem);

                        cartItem.Product.Stock -= cartItem.Quantity;
                        db.CartItems.Remove(cartItem);
                    }
                    db.Orders.Add(newOrder);
                    db.SaveChanges();
                    TempData["Success"] = "Comanda plasata.";
                }
                else
                {
                    TempData["Error"] = "Cosul este gol.";
                }
            }
            else
            {
                TempData["Error"] = "Cosul este gol.";
            }
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "User,Colaborator,Admin")]
        public IActionResult Checkout()
        {
            var cart = db.Carts.Include("CartItems").Include("CartItems.Product").FirstOrDefault(c => c.UserID == _userManager.GetUserId(User));
            if(cart != null)
            {
                return View(cart);
            }

            return RedirectToAction("Index", "Cart");
        }
    }
}
