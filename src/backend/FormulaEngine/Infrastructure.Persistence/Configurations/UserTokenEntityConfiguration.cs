using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class UserTokenEntityConfiguration : IEntityTypeConfiguration<IdentityUserToken<Guid>>
{

    public void Configure(EntityTypeBuilder<IdentityUserToken<Guid>> builder)
    {
        builder.HasKey(x => new { x.UserId, x.LoginProvider, x.Name });
    }
}
