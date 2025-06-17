using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CentraliaStore.Areas.Identity;
using CentraliaStore.Authorization;
using CentraliaStore.Data;
using CentraliaStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using static CentraliaStore.Infrastructure.ApiKeyAuthorizationCrudHandler;

namespace CentraliaStore.Controllers
{
    [Authorize]
    public class ApiKeysController : Controller
    {
        private readonly StoreContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IAuthorizationService _authorizationService;

        public ApiKeysController(
            StoreContext context,
            UserManager<AppUser> usr,
            IAuthorizationService authorizationService
        )
        {
            _context = context;
            _userManager = usr;
            _authorizationService = authorizationService;
        }

        // GET: ApiKeys
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Administrator");

            IQueryable<ApiKey> keysQuery;

            if (isAdmin)
            {
                keysQuery = _context.ApiKeys.Include(k => k.AppUser);
            }
            else
            {
                keysQuery = _context.ApiKeys
                    .Where(k => k.AppUserId == userId)
                    .Include(k => k.AppUser);
            }

            var keys = await keysQuery.ToListAsync();
            return View(keys);
        }

        // GET: ApiKeys/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var apiKey = await _context.ApiKeys
                .Include(a => a.AppUser)
                .FirstOrDefaultAsync(m => m.ApiKeyId == id);

            if (apiKey == null)
            {
                return NotFound();
            }

            var authorizationResult = await _authorizationService
                .AuthorizeAsync(User, apiKey, Operations.Read);

            if (authorizationResult.Succeeded)
            {
                return View(apiKey);
            }
            else if (User.Identity.IsAuthenticated)
            {
                return new ForbidResult();
            }
            else
            {
                return new ChallengeResult();
            }
        }

        // GET: ApiKeys/Create
        public IActionResult Create()
        {
            if (!User.IsInRole("Administrator"))
            {
                ViewData["AppUserId"] = new SelectList(_context.Users.Where(u => u.UserName == User.Identity.Name), "Id", "Id");
            }
            else
            {
                ViewData["AppUserId"] = new SelectList(_context.Users, "Id", "Id");
            }

            return View();
        }

        // POST: ApiKeys/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AppUserId")] string appUserId)
        {
            ApiKey apiKey;

            if (!User.IsInRole("Administrator"))
            {
                apiKey = new ApiKey
                {
                    ApiKeyId = 0,
                    ApiSecret = Guid.NewGuid().ToString(),
                    AppUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                };
            }
            else
            {
                apiKey = new ApiKey
                {
                    ApiKeyId = 0,
                    ApiSecret = Guid.NewGuid().ToString(),
                    AppUserId = appUserId
                };
            }

            _context.Add(apiKey);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: ApiKeys/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var apiKey = await _context.ApiKeys.FindAsync(id);
            if (apiKey == null)
            {
                return NotFound();
            }

            var authorizationResult = await _authorizationService
                .AuthorizeAsync(User, apiKey, new ApiKeyOwnerRequirement());

            if (authorizationResult.Succeeded)
            {
                ViewData["AppUserId"] = new SelectList(_context.Users, "Id", "Id", apiKey.AppUserId);
                return View(apiKey);
            }
            else if (User.Identity.IsAuthenticated)
            {
                return new ForbidResult();
            }
            else
            {
                return new ChallengeResult();
            }
        }

        // POST: ApiKeys/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ApiKeyId,ApiSecret,AppUserId")] ApiKey apiKey)
        {
            if (id != apiKey.ApiKeyId)
            {
                return NotFound();
            }

            var authorizationResult = await _authorizationService
                .AuthorizeAsync(User, apiKey, new ApiKeyOwnerRequirement());

            if (authorizationResult.Succeeded)
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(apiKey);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!ApiKeyExists(apiKey.ApiKeyId))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return RedirectToAction(nameof(Index));
                }

                ViewData["AppUserId"] = new SelectList(_context.Users, "Id", "Id", apiKey.AppUserId);
                return View(apiKey);
            }
            else if (User.Identity.IsAuthenticated)
            {
                return new ForbidResult();
            }
            else
            {
                return new ChallengeResult();
            }
        }

        // GET: ApiKeys/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var apiKey = await _context.ApiKeys
                .Include(a => a.AppUser)
                .FirstOrDefaultAsync(m => m.ApiKeyId == id);

            if (apiKey == null)
            {
                return NotFound();
            }

            var authorizationResult = await _authorizationService
                .AuthorizeAsync(User, apiKey, new ApiKeyOwnerRequirement());

            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }

            return View(apiKey);
        }

        // POST: ApiKeys/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var apiKey = await _context.ApiKeys.FindAsync(id);

            if (apiKey == null)
            {
                return NotFound();
            }

            var authorizationResult = await _authorizationService
                .AuthorizeAsync(User, apiKey, new ApiKeyOwnerRequirement());

            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }

            _context.ApiKeys.Remove(apiKey);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ApiKeyExists(int id)
        {
            return _context.ApiKeys.Any(e => e.ApiKeyId == id);
        }
    }
}
