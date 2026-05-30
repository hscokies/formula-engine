using Domain.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Seeders;

internal static class RoleSeeder
{
    internal static void Seed(this EntityTypeBuilder<IdentityRole<Guid>> builder)
    {
        builder.HasData(
            Create("0131895b-65b6-4b20-88f6-2ba133c3d054", RoleName.User, "1549794e-2db9-4e5b-bc8d-798627fc4d37"),
            Create("7ceb50b3-2d36-4a76-a8c7-ae2d5a924d4c", RoleName.Admin, "1549794e-2db9-4e5b-bc8d-798627fc4d37")
        );
    }

    private static IdentityRole<Guid> Create(string id, string name, string concurrencyStamp)
    {
        return new IdentityRole<Guid>()
        {
            Id = new Guid(id),
            Name = name,
            NormalizedName = name.ToUpper(),
            ConcurrencyStamp = new Guid(concurrencyStamp).ToString(),
        };
    }
}
