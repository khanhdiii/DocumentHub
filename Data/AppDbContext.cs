using DocumentHub.Model;

using Microsoft.EntityFrameworkCore;

namespace DocumentHub.Data
{
    internal class AppDbContext : DbContext
    {
        public DbSet<IncomingDocument> IncomingDocuments
        {
            get; set;
        }
        public DbSet<UserCredential> UserCredentials
        {
            get; set;
        }
        public DbSet<OutgoingDocument> OutgoingDocuments
        {
            get; set;
        }
        public DbSet<ReceivingOfficer> ReceivingOfficers
        {
            get; set;
        }
        public DbSet<ConstructionStaff> ConstructionStaff
        {
            get; set;
        }
        public DbSet<Signer> Signers
        {
            get; set;
        }
        public DbSet<Recipient> Recipients
        {
            get; set;
        }


        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=D:\\DocumentHub\\app.db");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserCredential>().Property(u => u.PIN).HasMaxLength(6);

            modelBuilder.Entity<OutgoingDocument>()
                .HasOne(o => o.ReceivingOfficer)
                .WithMany()
                .HasForeignKey("ReceivingOfficerId");

            modelBuilder.Entity<OutgoingDocument>()
                .HasOne(o => o.ConstructionStaff)
                .WithMany()
                .HasForeignKey("ConstructionStaffId");

            modelBuilder.Entity<OutgoingDocument>()
                .HasOne(o => o.Signer)
                .WithMany()
                .HasForeignKey("SignerId");

            modelBuilder.Entity<OutgoingDocument>()
                .HasOne(o => o.Recipient)
                .WithMany()
                .HasForeignKey("RecipientId");

            base.OnModelCreating(modelBuilder);

          
        }
    }
}
