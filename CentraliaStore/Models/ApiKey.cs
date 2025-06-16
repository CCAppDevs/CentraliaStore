using CentraliaStore.Areas.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CentraliaStore.Models
{
    public class ApiKey
    {
        public int ApiKeyId { get; set; }
        public string ApiSecret { get; set; }
        public string AppUserId { get; set; }
        [ValidateNever]
        public virtual AppUser AppUser { get; set; }
    }
}
