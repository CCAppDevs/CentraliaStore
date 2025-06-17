using CentraliaStore.Models;
using Microsoft.AspNetCore.Identity;

namespace CentraliaStore.Areas.Identity
{
    public class AppUser : IdentityUser
    {
        public virtual ICollection<Phone> PhoneNumbers { get; set; }
        public virtual ICollection<Address> Addresses { get; set; }
        
    }
}