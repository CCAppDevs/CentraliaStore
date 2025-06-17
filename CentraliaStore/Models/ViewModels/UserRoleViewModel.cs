using CentraliaStore.Areas.Identity;
using Microsoft.AspNetCore.Identity;

namespace CentraliaStore.Models.ViewModels
{
    public class UserRoleViewModel
    {
        public ICollection<AppUser> Users { get; set; }
        public ICollection<IdentityRole> Roles { get; set; }
        public List<RoleModificationViewModel> Additions { get; set; }
        public List<RoleModificationViewModel> Subtractions { get; set; }
    }
}
