using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;


internal sealed class UserPasskeyEntityConfiguration : IEntityTypeConfiguration<IdentityUserPasskey<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityUserPasskey<Guid>> builder)
    {
        builder.HasKey(x => x.UserId);
        builder.Property(x => x.Data).HasColumnType("jsonb");
    }
}