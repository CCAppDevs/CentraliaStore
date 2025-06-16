using CentraliaStore.Areas.Identity;
using CentraliaStore.Data;
using CentraliaStore.Models;
using CentraliaStore.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CentraliaStore.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class RoleManagerController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly StoreContext _context;

        public RoleManagerController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, StoreContext ctx)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = ctx;
        }

        public async Task<IActionResult> Index()
        {
            var model = new UserRoleViewModel
            {
                Users = await _context.Users.ToListAsync(),
                Roles = await _context.Roles.ToListAsync(),
                Additions = new List<RoleModificationViewModel>(),
                Subtractions = new List<RoleModificationViewModel>()
            };
            
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("userId,roleId")] UserRoleViewModel additions)
        {
            // use the role manager service to add a new user role with the userid and the roleid

            // return to the begining
            return RedirectToAction("Index");
        }
    }
}
