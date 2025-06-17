using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CentraliaStore.Data;
using CentraliaStore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using CentraliaStore.Areas.Identity;

namespace CentraliaStore.Controllers
{
    public class OrdersController : Controller
    {
        private readonly StoreContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IAuthorizationService _authorizationService;

        public OrdersController(StoreContext context, UserManager<AppUser> userManager, IAuthorizationService authorizationService)
        {
            _context = context;
            _userManager = userManager;
            _authorizationService = authorizationService;
        }

        public async Task<IActionResult> Index()
        {
            var currentUserId = _userManager.GetUserId(User);
            var isAdmin = User.IsInRole("Administrator");

            var orders = await _context.Orders
                .Where(o => isAdmin || o.UserId == currentUserId)
                .ToListAsync();

            return View(orders);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var order = await _context.Orders.FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
                return NotFound();

            var authResult = await _authorizationService.AuthorizeAsync(User, order, "IsOrderOwner");
            if (!authResult.Succeeded)
                return Forbid();

            return View(order);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,ProductId,Quantity,OrderedOn")] Order order)
        {
            if (ModelState.IsValid)
            {
                order.UserId = _userManager.GetUserId(User);
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return NotFound();

            var authResult = await _authorizationService.AuthorizeAsync(User, order, "IsOrderOwner");
            if (!authResult.Succeeded)
                return Forbid();

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId,ProductId,Quantity,UserId,OrderedOn")] Order order)
        {
            if (id != order.OrderId)
                return NotFound();

            var existingOrder = await _context.Orders.AsNoTracking().FirstOrDefaultAsync(o => o.OrderId == id);
            if (existingOrder == null)
                return NotFound();

            var authResult = await _authorizationService.AuthorizeAsync(User, existingOrder, "IsOrderOwner");
            if (!authResult.Succeeded)
                return Forbid();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var order = await _context.Orders.FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
                return NotFound();

            var authResult = await _authorizationService.AuthorizeAsync(User, order, "IsOrderOwner");
            if (!authResult.Succeeded)
                return Forbid();

            return View(order);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return NotFound();

            var authResult = await _authorizationService.AuthorizeAsync(User, order, "IsOrderOwner");
            if (!authResult.Succeeded)
                return Forbid();

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }
    }
}
