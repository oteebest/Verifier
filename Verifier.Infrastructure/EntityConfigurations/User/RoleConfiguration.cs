using Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Verifier.Infrastructure.EntityConfiguration.User
{
    public class RoleConfiguration : IEntityTypeConfiguration<VerifierRole>
    {
        public void Configure(EntityTypeBuilder<VerifierRole> builder)
        {
            builder.ToTable("Roles");
        }
    }
}
