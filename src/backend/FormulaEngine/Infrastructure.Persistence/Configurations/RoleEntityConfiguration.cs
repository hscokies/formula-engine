using Infrastructure.Persistence.Seeders;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class RoleEntityConfiguration : IEntityTypeConfiguration<IdentityRole<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityRole<Guid>> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasMany<IdentityUserRole<Guid>>()
            .WithOne()
            .HasForeignKey(ur => ur.RoleId);

        builder.Seed();
    }
}
