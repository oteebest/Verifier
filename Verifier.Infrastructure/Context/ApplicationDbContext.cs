using Application.Interfaces.Services;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Verifier.Application.Interfaces.Db;
using Verifier.Application.Interfaces.Services.User;
using Verifier.Domain.Contracts;
using Verifier.Domain.Entities.User;
using Verifier.Infrastructure.EntityConfiguration.User;
using Verifier.Shared.Constants.Application;

namespace Infrastructure.Context
{
    public class ApplicationDbContext : AuditableContext, IDatabaseService
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IDateTimeService _dateTimeService;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options
            , ICurrentUserService currentUserService
            , IDateTimeService dateTimeService) : base(options)
        {
            _currentUserService = currentUserService;
            _dateTimeService = dateTimeService;
        }
        
        public DbSet<VerifierUser> TheTerminalUsers { get; set; }
        public Task<int> SaveAsync()
        {
            return SaveChangesAsync(new CancellationToken());
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
        {
            foreach (var entry in ChangeTracker.Entries<IAuditableEntity>().ToList())
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedOn = _dateTimeService.NowUtc;
                        entry.Entity.CreatedBy = _currentUserService.UserId;
                        break;

                    case EntityState.Modified:
                        entry.Entity.LastModifiedOn = _dateTimeService.NowUtc;
                        entry.Entity.LastModifiedBy = _currentUserService.UserId;
                        break;
                }

            if (_currentUserService.UserId == null)
                return await base.SaveChangesAsync(cancellationToken);
            else
                return await base.SaveChangesAsync(_currentUserService.UserId, cancellationToken);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            base.OnModelCreating(builder);

            builder.HasCollation(ApplicationConstants.DatabaseConfiguration.CaseInsensitiveCollation, locale: ApplicationConstants.DatabaseConfiguration.Locale, provider: ApplicationConstants.DatabaseConfiguration.Provider, deterministic: false);
            builder.UseDefaultColumnCollation(ApplicationConstants.DatabaseConfiguration.CaseInsensitiveCollation);

            builder.ApplyConfiguration(new UserConfiguration());
            builder.ApplyConfiguration(new RoleConfiguration());
            builder.ApplyConfiguration(new RoleClaimConfiguration());
            builder.ApplyConfiguration(new UserLoginConfiguration());
            builder.ApplyConfiguration(new UserRoleConfiguration());
            builder.ApplyConfiguration(new UserTokenConfiguration());
            builder.ApplyConfiguration(new UserClaimConfiguration());
            
        }
    }
}
