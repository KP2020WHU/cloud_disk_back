using Microsoft.EntityFrameworkCore;

namespace CloudService.Models
{
    public class UserContext: DbContext
    {
        public UserContext(DbContextOptions<UserContext> options)
            : base(options)
        {
            Database.EnsureCreated(); // 自动建库建表
        }

        public DbSet<User> users { get; set; }
    }
}
