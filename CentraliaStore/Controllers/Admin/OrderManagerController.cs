using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CentraliaStore.Data;
using CentraliaStore.Models;

namespace CentraliaStore.Controllers.Admin
{
    // URL pattern: /Admin/Orders/Index, /Admin/Orders/Edit/5, etc.
    [Authorize(Roles = "Admin")]
    [Route("Admin/Orders/{action=Index}/{id?}")]
    public class OrderManagerController : Controller
    {
        private readonly StoreContext _context;

        public OrderManagerController(StoreContext context)
        {
            _context = context;
        }

        // GET: Admin/Orders?filter=incomplete|complete|all
        public async Task<IActionResult> Index(string filter = "incomplete")
        {
            IQueryable<Order> q = _context.Orders.AsQueryable();

            q = filter switch
            {
                "complete" => q.Where(o => o.Completed),
                "all" => q,
                _ => q.Where(o => !o.Completed)      // default = incomplete
            };

            ViewBag.Filter = filter;
            var orders = await q.OrderByDescending(o => o.OrderedOn).ToListAsync();
            return View(orders);
        }

        // GET: Admin/Orders/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == id);
            return order == null ? NotFound() : View(order);
        }

        // GET: Admin/Orders/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            return order == null ? NotFound() : View(order);
        }

        // POST: Admin/Orders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId,Completed")] Order patch)
        {
            if (id != patch.OrderId) return NotFound();

            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();

            order.Completed = patch.Completed;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/Orders/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == id);
            return order == null ? NotFound() : View(order);
        }

        // POST: Admin/Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}