using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Verifier.Domain.Entities.User;

namespace Verifier.Infrastructure.EntityConfiguration.User
{
    public class UserConfiguration : IEntityTypeConfiguration<VerifierUser>
    {
        public void Configure(EntityTypeBuilder<VerifierUser> builder)
        {
            builder.ToTable("Users");

            builder.Property(u => u.LoginScheme).HasMaxLength(50);

            builder.Property(u => u.Email).IsRequired();

            builder.HasIndex(u => u.Email).IsUnique();

            builder.Property(u => u.UserName).IsRequired();

            builder.HasIndex(u => u.UserName).IsUnique();

            builder.Property(u => u.FirstName).IsRequired();

            builder.Property(u => u.LastName).IsRequired();

            builder.Property(u => u.UserSchemeId).HasMaxLength(200);
        }
    }
}
