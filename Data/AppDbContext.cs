using DocumentHub.Model;
using DocumentHub.ViewModel;
using System.IO;
using System.Windows;

using Microsoft.EntityFrameworkCore;

namespace DocumentHub.Data
{
    public class AppDbContext : DbContext
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
        public DbSet<WorkProgress> WorkProgresses
        {
            get; set;
        }
        public DbSet<Person> People
        {
            get; set;
        }
        public DbSet<WorkProgressMonth> WorkProgressMonths
        {
            get; set;
        }
        public DbSet<WorkProgressQuater> WorkProgressQuaters
        {
            get; set;
        }
        public DbSet<WorkProgressYear> WorkProgressYears
        {
            get; set;
        }


        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
           var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "app.db");
            options.UseSqlite($"Data Source={dbPath}");

        }

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

            modelBuilder.Entity<WorkProgressQuater>()
                .HasOne(q => q.WorkProgress)
                .WithMany(w => w.Quarters)
                .HasForeignKey(q => q.WorkProgressId);

            modelBuilder.Entity<WorkProgress>().HasOne(w => w.Assigner).WithMany().HasForeignKey(w => w.AssignerId);
            modelBuilder.Entity<WorkProgress>().HasOne(w => w.PersonInCharge).WithMany().HasForeignKey(w => w.PersonInChargeId);

            base.OnModelCreating(modelBuilder);

        }
    }
}
