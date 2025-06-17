using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CentraliaStore.Data;

namespace CentraliaStore.Controllers
{
    public class CartController : Controller
    {
        private readonly StoreContext _context;
        private readonly ILogger<CartController> _logger;

        public CartController(StoreContext context, ILogger<CartController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var cart = GetCart();
            var products = _context.Products.Where(p => cart.ContainsKey(p.Id)).ToList();
            ViewBag.Cart = cart;
            return View(products);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(int id, int qty)
        {
            if (!_context.Products.Any(p => p.Id == id))
            {
                return RedirectToAction("Index");
            }

            qty = qty < 1 || qty > 100 ? 1 : qty;
            var cart = GetCart();
            cart[id] = cart.ContainsKey(id) ? cart[id] + qty : qty;
            HttpContext.Session.SetString("Cart", JsonSerializer.Serialize(cart));
            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Remove(int id)
        {
            var cart = GetCart();
            cart.Remove(id);
            HttpContext.Session.SetString("Cart", JsonSerializer.Serialize(cart));
            return RedirectToAction("Index");
        }

        private Dictionary<int, int> GetCart()
        {
            var json = HttpContext.Session.GetString("Cart");
            return json == null ? new Dictionary<int, int>() : JsonSerializer.Deserialize<Dictionary<int, int>>(json);
        }
    }
}