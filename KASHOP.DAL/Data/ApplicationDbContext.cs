using KASHOP.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.DAL.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,
            IHttpContextAccessor httpContextAccessor) : base(options)
        {
            this.httpContextAccessor = httpContextAccessor;
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CategoryTranslarion> CategoryTranslarions { get; set; }

        public DbSet<Product> Products { get; set; }
        public DbSet<ProductTranslation> productTranslations { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<BrandTranslation> BrandTranslations { get; set; }



        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<ApplicationUser>().ToTable("Users");
            builder.Entity<IdentityRole>().ToTable("RoleS");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");

            builder.Entity<Category>()
                .HasOne(p => p.CreatedBy)
                .WithMany()
                .HasForeignKey(p => p.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Category>()
                .HasOne(p => p.UpdatedBy)
                .WithMany()
                .HasForeignKey(p => p.UpdatedById)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Product>()
                .HasOne(p => p.CreatedBy)
                .WithMany()
                .HasForeignKey(p => p.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Product>()
                .HasOne(p => p.UpdatedBy)
                .WithMany()
                .HasForeignKey(p => p.UpdatedById)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Brand>()
                .HasOne(b => b.CreatedBy)
                .WithMany()
                .HasForeignKey(b => b.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Brand>()
               .HasOne(b => b.UpdatedBy)
               .WithMany()
               .HasForeignKey(b => b.UpdatedById)
               .OnDelete(DeleteBehavior.Restrict);



        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {

            if(httpContextAccessor.HttpContext != null)
            {
                var entries = ChangeTracker.Entries<AuditableEntity>();
                var currentUserId = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                foreach (var entry in entries)
                {
                    if (entry.State == EntityState.Added)
                    {
                        entry.Property(e => e.CreatedById).CurrentValue = currentUserId;
                        entry.Property(e => e.CreatedOn).CurrentValue = DateTime.UtcNow;
                    }
                    if (entry.State == EntityState.Modified)
                    {
                        entry.Property(e => e.UpdatedById).CurrentValue = currentUserId;
                        entry.Property(e => e.UpdatedOn).CurrentValue = DateTime.UtcNow;
                    }
                }
            }
            
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
