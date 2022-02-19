using Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Verifier.Infrastructure.EntityConfiguration.User
{
    public class RoleClaimConfiguration : IEntityTypeConfiguration<VerifierRoleClaim>
    {
        public void Configure(EntityTypeBuilder<VerifierRoleClaim> builder)
        {
            builder.ToTable("RoleClaims");
        }
    }
}
