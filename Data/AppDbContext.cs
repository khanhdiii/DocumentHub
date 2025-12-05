using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentHub.Data
{
    internal class AppDbContext : DbContext
    {
        public DbSet<PinCode> PinCodes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=app.db");
    }

    public class PinCode
    {
        public int Id { get; set; }
        public required string Code { get; set; }
    }
}
