using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CentraliaStore.Data;
using CentraliaStore.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using CentraliaStore.Areas.Identity;
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
            var keys = _context.ApiKeys.Include(a => a.AppUser).Where(k => k.AppUser.UserName == User.Identity.Name || User.IsInRole("Administrator"));
            return View(await keys.ToListAsync());
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
            } else
            {
                ViewData["AppUserId"] = new SelectList(_context.Users, "Id", "Id");
            }

            return View();
        }

        // POST: ApiKeys/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
                // Is admin, let do it all
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
            .AuthorizeAsync(User, apiKey, "EditPolicy");

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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ApiKeyId,ApiSecret,AppUserId")] ApiKey apiKey)
        {
            if (id != apiKey.ApiKeyId)
            {
                return NotFound();
            }

            var authorizationResult = await _authorizationService
            .AuthorizeAsync(User, apiKey, "EditPolicy");

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
                .AuthorizeAsync(User, apiKey, Operations.Delete);

            if (authorizationResult.Succeeded)
            {
                return View(apiKey);
            }else if (User.Identity.IsAuthenticated)
            {
                return new ForbidResult();
            }
            else
            {
                return new ChallengeResult();
            }

            return View(apiKey);
        }

        // POST: ApiKeys/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // TODO: Add delete functionality for only admins
            var apiKey = await _context.ApiKeys.FindAsync(id);
            if (apiKey == null)
            {
                return NotFound();
            }
            var authorizationResult = await _authorizationService
                .AuthorizeAsync(User, apiKey, Operations.Delete);
            //if (apiKey != null)
            //{
            //    _context.ApiKeys.Remove(apiKey);
            //}

            if (authorizationResult.Succeeded)
            {
                _context.ApiKeys.Remove(apiKey);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
               
            }
            else if (User.Identity.IsAuthenticated)
            {
                return new ForbidResult();
            }
            else
            {
                return new ChallengeResult();
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ApiKeyExists(int id)
        {
            return _context.ApiKeys.Any(e => e.ApiKeyId == id);
        }
    }
}
