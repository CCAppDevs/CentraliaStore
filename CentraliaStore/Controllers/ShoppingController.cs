using CentraliaStore.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CentraliaStore.Controllers
{
    public class ShoppingController : Controller
    {

        private readonly StoreContext _context;

        public ShoppingController(StoreContext context)
        {
            _context = context;
        }



       
        //Get list of products that are available if error return to home page.
       [Authorize]
      //  [Authorize(Roles = "Customer,Admin")] add this once roles are implemented
        public async Task<IActionResult> Index()
        {
            try
            {
                var products = await _context.Products
                    .Include(p => p.Category)
                    .ToListAsync();

                return View("ShoppingView", products);
            }
            catch (Exception ex)
            {
               
                return RedirectToAction("Error", "Home"); 
            }
        }

    }
}
