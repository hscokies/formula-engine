using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class UserRoleEntityConfiguration : IEntityTypeConfiguration<IdentityUserRole<Guid>>
{

    public void Configure(EntityTypeBuilder<IdentityUserRole<Guid>> builder)
    {
        builder.HasKey(ur => new { ur.UserId, ur.RoleId });
    }
}
