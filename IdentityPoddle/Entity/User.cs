using Microsoft.AspNetCore.Identity;

namespace IdentityPoddle.Entity
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Address { get; set; }
        public string Role { get; set; }
    }
}
