using Microsoft.AspNetCore.Identity;

namespace Modelos.Identity
{
    public class ApplicationUserRole : IdentityUserRole<string>

    {
        public ApplicationRole Role { get; set; }
        public ApplicationUser User { get; set; }
    }
}