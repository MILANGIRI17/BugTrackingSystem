using BugTracker.Application.Services;
using BugTracker.Domain.Entities;
using BugTracker.Infrastructure.Identity;
using BugTracker.Shared.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<User,Role,string>
    {
        private ICurrentUserService _currentUserService;
        public AppDbContext(DbContextOptions options, ICurrentUserService currentUserService) : base(options)
        {
            _currentUserService = currentUserService;
        }

        DbSet<Bug> Bug { get; set; }
        DbSet<Attachment> Attachment { get; set; }
        DbSet<SystemExceptionLog> SystemExceptions { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Bug>().HasMany(x=>x.Attachments).WithOne(x=>x.Bug).HasForeignKey(x=>x.BugId);
            builder.Entity<User>(entity => {entity.ToTable("User"); });
            builder.Entity<Role>(entity => { entity.ToTable("Role"); });
            builder.Entity<IdentityUserLogin<string>>(entity => { entity.ToTable("UserLogin"); });
            builder.Entity<IdentityUserClaim<string>>(entity => { entity.ToTable("UserClaim"); });
            builder.Entity<IdentityUserToken<string>>(entity => { entity.ToTable("UserToken"); });
            builder.Entity<IdentityRoleClaim<string>>(entity => { entity.ToTable("RoleClaim"); });
        }

        public override int SaveChanges()
        {
            AuditChanges();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            AuditChanges();
            return await base.SaveChangesAsync(cancellationToken);
        }

        public void AuditChanges()
        {
            var currentUserId = _currentUserService.GetUserId();
            if (currentUserId == null) return;
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is AuditableBaseEntity auditEntity)
                {
                    if (entry.State == EntityState.Added)
                        auditEntity.SetAudit(currentUserId, true);
                    if (entry.State == EntityState.Modified)
                        auditEntity.SetAudit(currentUserId, false);
                }
            }
        }
    }
}
