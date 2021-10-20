using Microsoft.EntityFrameworkCore;

namespace CloudService.Models
{
    public class CloudFileContext: DbContext
    {
        public CloudFileContext(DbContextOptions<CloudFileContext> options)
            : base(options)
        {
            Database.EnsureCreated(); // 自动建库建表
        }

        public DbSet<CloudFile> cloudFiles { get; set; }
    }
}
