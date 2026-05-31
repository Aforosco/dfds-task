using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DFDSVisitManagementAPI.Domain.src.Entities;
using DFDSVisitManagementAPI.Domain.src.Entities.Visits;
using DFDSVisitManagementAPI.Domain.src.Entities.Drivers;
using DFDSVisitManagementAPI.Domain.src.Entities.Activities;

namespace DFDSVisitManagementAPI.Domain.src.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Visit> Visits { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Activity> Activities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Visit>(entity =>
            {
                entity.HasKey(v => v.Id);

                entity.HasOne(v => v.Driver)
                    .WithMany()
                    .HasForeignKey(v => v.DriverId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(v => v.Activities)
                    .WithOne(a => a.Visit)
                    .HasForeignKey(a => a.VisitId).IsRequired(false)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}