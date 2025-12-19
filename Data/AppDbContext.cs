using DocumentHub.Model;
using Microsoft.EntityFrameworkCore;

namespace DocumentHub.Data
{
    internal class AppDbContext : DbContext
    {
        public DbSet<IncomingDocument> IncomingDocuments { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=app.db");
    }
}
