using CentraliaStore.Areas.Identity;
using Microsoft.AspNetCore.Identity;

namespace CentraliaStore.Models.ViewModels
{
    public class RoleModificationViewModel
    {
        public AppUser User { get; set; }
        public IdentityRole Role { get; set; }
    }
}