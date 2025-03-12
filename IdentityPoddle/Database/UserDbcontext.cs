using IdentityPoddle.Entity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityPoddle.Database
{
    public class UserDbcontext : IdentityDbContext<User>
    {
        public UserDbcontext(DbContextOptions<UserDbcontext> options) : base(options)
        {
        }

        public DbSet<OtpRecord> OtpRecords { get; set; }
    }
}
